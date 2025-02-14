using MassTransit;
using LeadsSaverRabbitMQ.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using LeadsSaverRabbitMQ.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using LeadsSaverRabbitMQ.MessageModels;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsLMSConsumer : IConsumer<RabbitMQLeadMessage_LMS>
{
    private readonly AstraContext _dbContext;
    private readonly BrandConfigurationSettings _brandSettings;
    private readonly ConnectionDBSettings _connectionDBSettings;
    private readonly ILogger<LeadsLMSConsumer> _logger;

    private readonly IPublishEndpoint _publishEndpoint;

    public LeadsLMSConsumer(AstraContext dbContext, 
                            IOptions<BrandConfigurationSettings> brandSettings, 
                            IOptions<ConnectionDBSettings> connectionDBSettings,
                            ILogger<LeadsLMSConsumer> logger,
                            IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _brandSettings = brandSettings.Value;
        _connectionDBSettings = connectionDBSettings.Value;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_LMS> context)
    {
        _logger.LogInformation("NEW LMS MESSAGE Received: LMS Message ({Message}))", context.Message.Message_ID);
        var entityMessage = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        if (entityMessage == null)
        {
            _logger.LogError($"Message ({context.Message.Message_ID}) was not found in OuterMessage table", DateTimeOffset.Now);
        }
        else
        {
            var jsonString = entityMessage.MessageText;
            try
            {
                var centerId = context.Message.Center_ID;
                var brand = _brandSettings.Brands.FirstOrDefault(x => x.BrandName == context.Message.BrandName);
                if (brand == null)
                {
                    _logger.LogError($"Brand ({context.Message.BrandName}) was not found", DateTimeOffset.Now);
                    return;
                }
                var projectId = brand.ProjectGuid;

                Request lead = JsonConvert.DeserializeObject<Request>(jsonString);
                var request_id = lead.Id;
                var request_type_id = lead.Request_Type_Id;
                var client_id = lead.Client_Id;

                Guid? employeeId = null;
                var assigned_id = lead.Assigned_Id;
                if (assigned_id != null)
                {
                    var query = "SELECT roi.Row_ID FROM autocrm.RowOuterID roi " +
                                "WHERE roi.Outer_ID = {0} AND roi.SendLogRowType_ID = 4 AND roi.Center_ID = '{1}'";

                    employeeId = _dbContext.EmployeeIdResults
                            .FromSqlRaw(query, assigned_id, centerId.ToString())
                            .AsEnumerable()
                            .FirstOrDefault()?.Row_ID;
                }

                var comment = lead.Comment;
                var last_name = lead.Last_Name;
                var first_name = lead.First_Name;
                var middle_name = lead.Middle_Name;
                var business_name = lead.Business_Name;
                var email = lead.Email;
                var phone = lead.Phones?.FirstOrDefault()?.Number;
                var address = lead.Address;
                var communication_method = lead.communication_method;
                var client_confirm_communication = lead.ClientConfirmCommunication;
                var vin = lead.Vin;
                var created_at = lead.created_at;
                var request_type_name = lead.requestType?.Name;
                var model_name = lead.vehicleModel?.name;

                Guid.TryParse("B5C8A3E3-CF6F-44B3-83BF-68ACA010B473", out var updUser);

                Guid? visitAimId = GetVisitAimId(request_type_id);

                Guid? eMessageSubjectId = GetEMessageSubjectId(request_type_id);

                string eMessageComment = $"{request_type_id}\r\n" +
                                          $"LMS лид {client_id}\r\n" +
                                          $"{last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
                                          $"{business_name ?? ""}\r\n" +
                                          //{phone ?? ""}\r\n" +
                                          $"{model_name ?? ""}\r\n" +
                                          $"{vin ?? ""}\r\n" +
                                          $"{comment ?? ""}\r\n" +
                                          $"Предпочтительный способ связи = {communication_method}";

                string connectionString = _connectionDBSettings.DefaultConnection;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("dbo.PR_EMessage_Set", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@DocumentBaseDate", SqlDbType.DateTime).Value = created_at;
                    command.Parameters.Add("@Department_ID", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
                    command.Parameters.Add("@Project_ID", SqlDbType.UniqueIdentifier).Value = projectId;
                    command.Parameters.Add("@Center_ID", SqlDbType.UniqueIdentifier).Value = centerId;
                    command.Parameters.Add("@InsApplicationUser_ID", SqlDbType.UniqueIdentifier).Value = updUser;
                    command.Parameters.Add("@VisitAim_ID", SqlDbType.UniqueIdentifier).Value = visitAimId ?? (object)DBNull.Value;
                    command.Parameters.Add("@EMessageSubject_ID", SqlDbType.UniqueIdentifier).Value = eMessageSubjectId ?? (object)DBNull.Value;
                    command.Parameters.Add("@EmployeePlan_ID", SqlDbType.UniqueIdentifier).Value = employeeId ?? (object)DBNull.Value;
                    command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = eMessageComment;
                    command.Parameters.Add("@ProActivity", SqlDbType.TinyInt).Value = 39;
                    command.Parameters.Add("@LeadOrderNumber", SqlDbType.VarChar, 255).Value = request_id.ToString();
                    command.Parameters.Add("@LeadID", SqlDbType.VarChar, 255).Value = client_id;
                    command.Parameters.Add("@LeadPhoneNumber", SqlDbType.VarChar, 255).Value = phone;
                    command.Parameters.Add("@LeadEMail", SqlDbType.VarChar, 255).Value = email;
                    command.Parameters.Add("@LeadLastName", SqlDbType.VarChar, 255).Value = last_name;
                    command.Parameters.Add("@LeadFirstName", SqlDbType.VarChar, 255).Value = first_name;
                    command.Parameters.Add("@LeadMiddleName", SqlDbType.VarChar, 255).Value = middle_name;
                    command.Parameters.Add("@LeadBusinessName", SqlDbType.VarChar, 255).Value = business_name;
                    command.Parameters.Add("@LeadAddress", SqlDbType.VarChar, 255).Value = address;
                    command.Parameters.Add("@LeadCommunicationMethod", SqlDbType.TinyInt).Value = communication_method == '1' ? 1 : (communication_method == '0' ? 0 : DBNull.Value);
                    command.Parameters.Add("@ForceTransitionToAccept", SqlDbType.Bit).Value = 1;

                    command.Parameters.Add("@ErrMes", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    var errorMes = command.Parameters["@ErrMes"].Value;

                    entityMessage.UpdDate = DateTime.Now;
                    if (errorMes != null)
                    {
                        entityMessage.ErrorCode = 6;
                        entityMessage.ErrorMessage = errorMes.ToString();
                        entityMessage.ProcessingStatus = 3; //ошибка
                        _logger.LogError($"Ошибка создания электронного обращения для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}): {errorMes.ToString()}", DateTimeOffset.Now);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var stages = lead.stages;
                        var stage = stages.First(x => x.stage_type_id == 3);

                        entityMessage.ProcessingStatus = 1; //обработали и создали обращение
                        var message = new RabbitMQStatusMessage_LMS
                        {
                            Message_ID = entityMessage.OuterMessage_ID,
                            StageWorkId = stage.Id,
                            ResultId = stage.result_id,
                            Center_ID = centerId,
                            BrandName = context.Message.BrandName
                        };
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Успешно создано электронное обращение для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID})", DateTimeOffset.Now);

                        await _publishEndpoint.Publish(message);
                        _logger.LogInformation($"Собщение для изменения статуса {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}) успешно добавлено в очередь RabbitMQ LMS status queue", DateTimeOffset.Now);
                    }
         
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"InnerException: {ex.Message}))", DateTimeOffset.Now);

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Error Info: {ex.InnerException.Message}");
                }
            }
        }
    }

    private Guid? GetVisitAimId(int requestTypeId)
    {
        return requestTypeId switch
        {
            3 or 6 or 12 or 15 or 27 or 31 or 39 or 42 or 45 => Guid.Parse("52CBAC59-E526-4BCE-9252-CF0CF7305363"), // Отдел розницы
            9 or 30 or 48 or 56 => Guid.Parse("83AFA901-636B-4B19-BFED-3BCB92C9F3B8"), // Сервис-бюро (слесарный ремонт)
            18 or 36 => Guid.Parse("51925334-249F-4072-90E0-91CEAE1F24D9"), // Оценка (продажа) ТС с пробегом
            21 or 29 or 51 or 54 => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
            24 => Guid.Parse("1E64A761-306D-46DD-98C0-5696395DF71A"), // Корпоративный отдел
            _ => null
        };
    }

    private Guid? GetEMessageSubjectId(int requestTypeId)
    {
        return requestTypeId switch
        {
            3 => Guid.Parse("B92FCB08-5642-485C-A0A8-9DAA233214C0"), // Заявка на тест-драйв
            6 or 12 or 21 or 24 or 29 or 30 or 39 or 48 or 54 or 56 => Guid.Parse("92472552-F566-4795-8553-5052466B968C"), // Вопрос
            9 => Guid.Parse("30994BF3-BF73-4D9F-A6FA-ED98FB9B9411"), // On-line запись на сервис
            15 or 31 or 42 or 45 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль
            18 or 36 => Guid.Parse("EC2354C6-F88F-4798-B2FB-8475C6C1DB3B"), // Оценка а/м с пробегом
            27 => Guid.Parse("22994476-3A70-4F81-A856-A3EBD0E6E707"), // Заказ обратного звонка
            51 => Guid.Parse("719F0EBC-C512-4190-BC53-085D9ED74EAA"), // Контакт импортёра
            _ => null
        };
    }
}