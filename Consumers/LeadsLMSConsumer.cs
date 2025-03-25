using MassTransit;
using LeadsSaverRabbitMQ.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using LeadsSaverRabbitMQ.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using LeadsSaverRabbitMQ.MessageModels;
using Microsoft.Extensions.Configuration;
using LeadsSaver_RabbitMQ.Services;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsLMSConsumer : IConsumer<RabbitMQLeadMessage_LMS>
{
    //private readonly AstraContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LeadsLMSConsumer> _logger;

    private readonly IBrandDbContextFactory _dbContextFactory;

    private readonly IPublishEndpoint _publishEndpoint;

    public LeadsLMSConsumer(IConfiguration configuration,
                            ILogger<LeadsLMSConsumer> logger,
                            IPublishEndpoint publishEndpoint,
                            IBrandDbContextFactory dbContextFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_LMS> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        using var _dbContext = _dbContextFactory.CreateDbContext(context.Message.Database);
        _logger.LogInformation("NEW LMS MESSAGE Received: LMS Message ({Message}))", context.Message.Message_ID);
        var entityMessage = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        if (entityMessage == null)
        {
            _logger.LogError($"LMS Message ({context.Message.Message_ID}) was not found in OuterMessage table", DateTimeOffset.Now);
        }
        else
        {
            if ((new List<byte> { 1, 2, 4 }).Contains(entityMessage.ProcessingStatus))
                return;

            var jsonString = entityMessage.MessageText;
            try
            {

                var centerId = context.Message.Center_ID;
                var projectId = context.Message.Project_ID;
                var brand = context.Message.BrandCenterName;

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

                Guid.TryParse("1E835730-9CB3-4C47-8397-B7BF7CF0231F", out var updUser);

                Guid? visitAimId = EMessageHelper.GetVisitAimId(brand, request_type_id);

                Guid? eMessageSubjectId = EMessageHelper.GetEMessageSubjectId(brand,request_type_id);
                string? eMessageSubjectName = EMessageHelper.GetEMessageSubjectName(brand,request_type_id);

                string eMessageComment = $"RequestTypeId: {request_type_id}\r\n" +
                                          $"LMS лид {request_id}\r\n" +
                                          $"Тема обращения: {eMessageSubjectName}\r\n" +
                                          $"{last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
                                          $"{business_name ?? ""}\r\n" +
                                          $"Телефон клиента: {phone ?? ""}\r\n" +
                                          $"EMail клиента: {email ?? ""}\r\n" +
                                          $"Модель: {model_name ?? ""}\r\n" +
                                          $"VIN: {vin ?? ""}\r\n" +
                                          $"Комментарий клиента: {comment ?? ""}\r\n" +
                                          $"Предпочтительный способ связи: {communication_method}";

                var connectionString = _configuration.GetConnectionString(context.Message.Database);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("dbo.PR_EMessage_Set", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120;

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
                    Guid.TryParse(errorMes.ToString(), out var guid);

                    entityMessage.UpdDate = DateTime.Now;
                    if (guid == Guid.Empty)
                    {
                        var mes = $"[LMS] Ошибка создания электронного обращения для id={request_id}, brand={context.Message.BrandCenterName}, request_type={request_type_id}: {errorMes.ToString()}";
                        entityMessage.ErrorCode = 1;
                        entityMessage.ErrorMessage = mes;
                        entityMessage.ProcessingStatus = 3; //ошибка создания обращения
                        _logger.LogError(mes, DateTimeOffset.Now);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {

                        entityMessage.ProcessingStatus = 1; //обработали и создали обращение
                        entityMessage.ErrorCode = 0;
                        entityMessage.ErrorMessage = "";
                        var message = new RabbitMQStatusMessage_LMS
                        {
                            Message_ID = entityMessage.OuterMessage_ID,
                            Center_ID = centerId,
                            BrandCenterName = context.Message.BrandCenterName
                        };
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Успешно создано LMS электронное обращение для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID})", DateTimeOffset.Now);

                        try
                        {
                            await _publishEndpoint.Publish(message);
                            _logger.LogInformation($"Сообщение для изменения статуса {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}) успешно добавлено в очередь RabbitMQ LMS status queue", DateTimeOffset.Now);
                        }
                        catch (Exception ex)
                        {
                            entityMessage.ErrorCode = 1;
                            entityMessage.ErrorMessage = ex.Message;
                            entityMessage.ProcessingStatus = 4;
                            await _dbContext.SaveChangesAsync();
                            _logger.LogError($"Ошибка отправки LMS сообщения в RabbitMQ для {entityMessage.OuterMessage_ID}: {ex.Message}", DateTimeOffset.Now);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                entityMessage.ErrorCode = 1;
                entityMessage.ErrorMessage = ex.Message;
                entityMessage.ProcessingStatus = 3; //ошибка создания обращения
                _logger.LogError($"Внутренняя ошибка создания LMS электронного обращения для {entityMessage.OuterMessage_ID}, brand={context.Message.BrandCenterName}, ({entityMessage.OuterMessage_ID}): {ex.Message}", DateTimeOffset.Now);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"InnerException: {ex.Message}))", DateTimeOffset.Now);

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Error Info: {ex.InnerException.Message}");
                }

            }
        }
    }

}