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
using LeadsSaver_RabbitMQ.Models;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsLMPConsumer : IConsumer<RabbitMQLeadMessage_LMP>
{
    //private readonly AstraContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LeadsLMSConsumer> _logger;

    private readonly IBrandDbContextFactory _dbContextFactory;

    private readonly IPublishEndpoint _publishEndpoint;

    public LeadsLMPConsumer(IConfiguration configuration,
                            ILogger<LeadsLMSConsumer> logger,
                            IPublishEndpoint publishEndpoint,
                            IBrandDbContextFactory dbContextFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_LMP> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        using var _dbContext = _dbContextFactory.CreateDbContext("ASTRA_AUDI_TEST");
        _logger.LogInformation("NEW LMP MESSAGE Received: LMP Message ({Message}))", context.Message.Message_ID);
        var entityMessage = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        if (entityMessage == null)
        {
            _logger.LogError($"LMP Message ({context.Message.Message_ID}) was not found in OuterMessage table", DateTimeOffset.Now);
        }
        else
        {
            if ((new List<byte> { 1, 2, 4 }).Contains(entityMessage.ProcessingStatus))
                return;

            var jsonString = entityMessage.MessageText;
            try
            {

                LMPRequestModel lead = JsonConvert.DeserializeObject<LMPRequestModel>(jsonString);
                var request_id = lead.public_id;
                var request_type_id = lead.type_id;

                var centerId = EMessageHelper.GetBWMCenterByOutletCode(context.Message.OutletCode);
                Guid.TryParse("2d673fec-bf76-4258-8c20-33dc74fe8206", out var projectId);

                var last_name = lead.contact.last_name;
                var first_name = lead.contact.first_name;
                var middle_name = lead.contact.middle_name;
               // var business_name = lead.contact.;
                var email = lead.contact.email;
                var phone = lead.contact.contact_phone;
                var address = lead.contact.address;
                var vin = lead.contact.vin;
                var created_at = lead.dealer_receive_date;
                var model_name = lead.contact.car_model;

                Guid.TryParse("1E835730-9CB3-4C47-8397-B7BF7CF0231F", out var updUser);

                Guid? visitAimId = EMessageHelper.GetVisitAimId_BMW(request_type_id);

                Guid? eMessageSubjectId = EMessageHelper.GetEMessageSubjectId_BMW(request_type_id);
                string? eMessageSubjectName = EMessageHelper.GetEMessageSubjectName_BMW(request_type_id);

                string eMessageComment = $"RequestTypeId: {request_type_id}\r\n" +
                                          $"LMS лид {request_id}\r\n" +
                                          $"Тема обращения: {eMessageSubjectName}\r\n" +
                                          $"{last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
                                          $"Телефон клиента: {phone ?? ""}\r\n" +
                                          $"EMail клиента: {email ?? ""}\r\n" +
                                          $"Модель: {model_name ?? ""}\r\n" +
                                          $"VIN: {vin ?? ""}\r\n" +
                                          $"Дата и время передачи лида дилеру {lead.dealer_receive_date}\r\n" +
                                          $"Дата и время -дедлайн на взятие заявки в работу (в часовом поясе UTC) {lead.target_processing_date}\r\n" +
                                          $"Дата и время -дедлайн на обработку заявки (в часовом поясе UTC) {lead.target_first_contact_date}\r\n";

                var connectionString = _configuration.GetConnectionString("ASTRA_AUDI_TEST");

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
                    //command.Parameters.Add("@EmployeePlan_ID", SqlDbType.UniqueIdentifier).Value = employeeId ?? (object)DBNull.Value;
                    command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = eMessageComment;
                    command.Parameters.Add("@ProActivity", SqlDbType.TinyInt).Value = 39;
                    command.Parameters.Add("@LeadOrderNumber", SqlDbType.VarChar, 255).Value = request_id.ToString();
                    command.Parameters.Add("@LeadID", SqlDbType.VarChar, 255).Value = lead.lead_id.ToString();
                    command.Parameters.Add("@LeadPhoneNumber", SqlDbType.VarChar, 255).Value = phone;
                    command.Parameters.Add("@LeadEMail", SqlDbType.VarChar, 255).Value = email;
                    command.Parameters.Add("@LeadLastName", SqlDbType.VarChar, 255).Value = last_name;
                    command.Parameters.Add("@LeadFirstName", SqlDbType.VarChar, 255).Value = first_name;
                    command.Parameters.Add("@LeadMiddleName", SqlDbType.VarChar, 255).Value = middle_name;
                    //command.Parameters.Add("@LeadBusinessName", SqlDbType.VarChar, 255).Value = business_name;
                    command.Parameters.Add("@LeadAddress", SqlDbType.VarChar, 255).Value = address;
                    //command.Parameters.Add("@LeadCommunicationMethod", SqlDbType.TinyInt).Value = communication_method == '1' ? 1 : (communication_method == '0' ? 0 : DBNull.Value);
                    command.Parameters.Add("@ForceTransitionToAccept", SqlDbType.Bit).Value = 1;

                    command.Parameters.Add("@ErrMes", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    var errorMes = command.Parameters["@ErrMes"].Value;
                    Guid.TryParse(errorMes.ToString(), out var guid);

                    entityMessage.UpdDate = DateTime.Now;
                    if (guid == Guid.Empty)
                    {
                        var mes = $"[LMP] Ошибка создания электронного обращения для id={request_id}, request_type={request_type_id}: {{errorMes.ToString()}}";
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
                        var message = new RabbitMQStatusMessage_LMP
                        {
                            Message_ID = entityMessage.OuterMessage_ID,
                            Outlet_Code = context.Message.OutletCode
                        };
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Успешно создано LMP электронное обращение для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID})", DateTimeOffset.Now);

                        try
                        {
                            await _publishEndpoint.Publish(message);
                            _logger.LogInformation($"Сообщение для изменения статуса {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}) успешно добавлено в очередь RabbitMQ LMP status queue", DateTimeOffset.Now);
                        }
                        catch (Exception ex)
                        {
                            entityMessage.ErrorCode = 1;
                            entityMessage.ErrorMessage = ex.Message;
                            entityMessage.ProcessingStatus = 4;
                            await _dbContext.SaveChangesAsync();
                            _logger.LogError($"Ошибка отправки LMP сообщения в RabbitMQ для {entityMessage.OuterMessage_ID}: {ex.Message}", DateTimeOffset.Now);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                entityMessage.ErrorCode = 1;
                entityMessage.ErrorMessage = ex.Message;
                entityMessage.ProcessingStatus = 3; //ошибка создания обращения
                var centerId = EMessageHelper.GetBWMCenterByOutletCode(context.Message.OutletCode);
                _logger.LogError($"Внутренняя ошибка создания LMP электронного обращения для {entityMessage.OuterMessage_ID}, ({entityMessage.MessageOuter_ID}), outer code = {context.Message.OutletCode}, centerid = {centerId}: {ex.Message}", DateTimeOffset.Now);
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