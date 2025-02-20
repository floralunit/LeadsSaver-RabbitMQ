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
using Microsoft.Extensions.Configuration;
using LeadsSaver_RabbitMQ.Models;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsDataMartConsumer : IConsumer<RabbitMQLeadMessage_DataMart>
{
    private readonly IConfiguration _configuration;
    private readonly BrandConfigurationSettings _brandSettings;
    private readonly ILogger<LeadsLMSConsumer> _logger;

    private readonly IBrandDbContextFactory _dbContextFactory;

    private readonly IPublishEndpoint _publishEndpoint;
    public LeadsDataMartConsumer(IConfiguration configuration,
                            IOptions<BrandConfigurationSettings> brandSettings,
                            ILogger<LeadsDataMartConsumer> logger,
                            IPublishEndpoint publishEndpoint,
                            IBrandDbContextFactory dbContextFactory)
    {
        _configuration = configuration;
        _brandSettings = brandSettings.Value;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_DataMart> context)
    {
        //using var _dbContext = _dbContextFactory.CreateDbContext(context.Message.BrandName);
        _logger.LogInformation("NEW DATAMART MESSAGE Received: LMS Message ({Message}))", context.Message.Message_ID);
        //var entityMessage = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        //if (entityMessage == null)
        //{
        //    _logger.LogError($"Message ({context.Message.Message_ID}) was not found in OuterMessage table", DateTimeOffset.Now);
        //}
        //else
        //{
        //    if (entityMessage.ProcessingStatus == 1 || entityMessage.ProcessingStatus == 2)
        //        return;

        //    var jsonString = entityMessage.MessageText;
        //    try
        //    {
        //        _dbContext.Database.ExecuteSqlRaw("DISABLE TRIGGER [stella].[TR_OuterMessage_AU_101] on [stella].[OuterMessage]");

        //        var centerId = context.Message.Center_ID;
        //        var projectId = context.Message.Project_ID;

        //        DatamartRequestModel lead = JsonConvert.DeserializeObject<DatamartRequestModel>(jsonString);
        //        var request_id = lead.crmLeadId;
        //        var request_type_id = null//lead.Request_Type_Id;
        //        var client_id = lead.Client_Id;

        //        Guid? employeeId = null;
        //        var assigned_id = lead.Assigned_Id;
        //        if (assigned_id != null)
        //        {
        //            var query = "SELECT roi.Row_ID FROM autocrm.RowOuterID roi " +
        //                        "WHERE roi.Outer_ID = {0} AND roi.SendLogRowType_ID = 4 AND roi.Center_ID = '{1}'";

        //            employeeId = _dbContext.EmployeeIdResults
        //                    .FromSqlRaw(query, assigned_id, centerId.ToString())
        //                    .AsEnumerable()
        //                    .FirstOrDefault()?.Row_ID;
        //        }

        //        var comment = lead.crmLeadText;
        //        var last_name = lead.crmLastName;
        //        var first_name = lead.crmFirstName;
        //        var middle_name = lead.crmMiddleName;
        //        var business_name = lead.crmClientCompanyName;
        //        var email = lead.crmEmail;
        //        var phone = lead.crmPhone;
        //        var address = lead.crmDealerNameWithAddress;
        //        var communication_method = lead.comm;
        //        var client_confirm_communication = lead.ClientConfirmCommunication;
        //        var vin = lead.Vin;
        //        var created_at = lead.crmDateTimeMoskvichReceive;//?
        //        var request_type_name = lead.r;
        //        var model_name = lead.crmModelOfInterest;

        //        Guid.TryParse("5F67890F-D8ED-46BA-99C8-0C35EF6A0E51", out var updUser);

        //        Guid? visitAimId = GetVisitAimId(request_type_id);

        //        Guid? eMessageSubjectId = GetEMessageSubjectId(request_type_id);

        //        string eMessageComment = $"{request_type_id}\r\n" +
        //                                  $"LMS лид {client_id}\r\n" +
        //                                  $"{last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
        //                                  $"{business_name ?? ""}\r\n" +
        //                                  //{phone ?? ""}\r\n" +
        //                                  $"{model_name ?? ""}\r\n" +
        //                                  $"{vin ?? ""}\r\n" +
        //                                  $"{comment ?? ""}\r\n" +
        //                                  $"Предпочтительный способ связи = {communication_method}";

        //        var connectionString = _configuration.GetConnectionString(brand.DataBase);

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            SqlCommand command = new SqlCommand("dbo.PR_EMessage_Set", connection);
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandTimeout = 60;

        //            command.Parameters.Add("@DocumentBaseDate", SqlDbType.DateTime).Value = created_at;
        //            command.Parameters.Add("@Department_ID", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
        //            command.Parameters.Add("@Project_ID", SqlDbType.UniqueIdentifier).Value = projectId;
        //            command.Parameters.Add("@Center_ID", SqlDbType.UniqueIdentifier).Value = centerId;
        //            command.Parameters.Add("@InsApplicationUser_ID", SqlDbType.UniqueIdentifier).Value = updUser;
        //            command.Parameters.Add("@VisitAim_ID", SqlDbType.UniqueIdentifier).Value = visitAimId ?? (object)DBNull.Value;
        //            command.Parameters.Add("@EMessageSubject_ID", SqlDbType.UniqueIdentifier).Value = eMessageSubjectId ?? (object)DBNull.Value;
        //            command.Parameters.Add("@EmployeePlan_ID", SqlDbType.UniqueIdentifier).Value = employeeId ?? (object)DBNull.Value;
        //            command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = eMessageComment;
        //            command.Parameters.Add("@ProActivity", SqlDbType.TinyInt).Value = 39;
        //            command.Parameters.Add("@LeadOrderNumber", SqlDbType.VarChar, 255).Value = request_id.ToString();
        //            command.Parameters.Add("@LeadID", SqlDbType.VarChar, 255).Value = client_id;
        //            command.Parameters.Add("@LeadPhoneNumber", SqlDbType.VarChar, 255).Value = phone;
        //            command.Parameters.Add("@LeadEMail", SqlDbType.VarChar, 255).Value = email;
        //            command.Parameters.Add("@LeadLastName", SqlDbType.VarChar, 255).Value = last_name;
        //            command.Parameters.Add("@LeadFirstName", SqlDbType.VarChar, 255).Value = first_name;
        //            command.Parameters.Add("@LeadMiddleName", SqlDbType.VarChar, 255).Value = middle_name;
        //            command.Parameters.Add("@LeadBusinessName", SqlDbType.VarChar, 255).Value = business_name;
        //            command.Parameters.Add("@LeadAddress", SqlDbType.VarChar, 255).Value = address;
        //            command.Parameters.Add("@LeadCommunicationMethod", SqlDbType.TinyInt).Value = communication_method == '1' ? 1 : (communication_method == '0' ? 0 : DBNull.Value);
        //            command.Parameters.Add("@ForceTransitionToAccept", SqlDbType.Bit).Value = 1;

        //            command.Parameters.Add("@ErrMes", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;

        //            connection.Open();
        //            command.ExecuteNonQuery();

        //            var errorMes = command.Parameters["@ErrMes"].Value;
        //            Guid.TryParse(errorMes.ToString(), out var guid);

        //            entityMessage.UpdDate = DateTime.Now;
        //            if (guid == Guid.Empty)
        //            {
        //                entityMessage.ErrorCode = 6;
        //                entityMessage.ErrorMessage = errorMes.ToString();
        //                entityMessage.ProcessingStatus = 3; //ошибка
        //                _logger.LogError($"Ошибка создания электронного обращения для id={request_id}, brand={context.Message.BrandName}, request_type={request_type_id} ({entityMessage.OuterMessage_ID}): {errorMes.ToString()}", DateTimeOffset.Now);
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                var stages = lead.stages;
        //                var stage = stages.First(x => x.stage_type_id == 3);

        //                entityMessage.ProcessingStatus = 1; //обработали и создали обращение
        //                var message = new RabbitMQStatusMessage_LMS
        //                {
        //                    Message_ID = entityMessage.OuterMessage_ID,
        //                    StageWorkId = stage.Id,
        //                    ResultId = stage.result_id,
        //                    Center_ID = centerId,
        //                    BrandName = context.Message.BrandName
        //                };
        //                await _dbContext.SaveChangesAsync();
        //                _dbContext.Database.ExecuteSqlRaw("ENABLE TRIGGER [stella].[TR_OuterMessage_AU_101] on [stella].[OuterMessage]");
        //                _logger.LogInformation($"Успешно создано электронное обращение для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID})", DateTimeOffset.Now);

        //                await _publishEndpoint.Publish(message);
        //                _logger.LogInformation($"Собщение для изменения статуса {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}) успешно добавлено в очередь RabbitMQ LMS status queue", DateTimeOffset.Now);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"InnerException: {ex.Message}))", DateTimeOffset.Now);

        //        if (ex.InnerException != null)
        //        {
        //            _logger.LogError($"Error Info: {ex.InnerException.Message}");
        //        }
        //    }
        //}
    }

}