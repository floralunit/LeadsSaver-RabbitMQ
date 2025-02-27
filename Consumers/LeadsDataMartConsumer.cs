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
using LeadsSaver_RabbitMQ.Services;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsDataMartConsumer : IConsumer<RabbitMQLeadMessage_DataMart>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LeadsDataMartConsumer> _logger;

    private readonly IBrandDbContextFactory _dbContextFactory;

    private readonly IPublishEndpoint _publishEndpoint;
    public LeadsDataMartConsumer(IConfiguration configuration,
                            ILogger<LeadsDataMartConsumer> logger,
                            IPublishEndpoint publishEndpoint,
                            IBrandDbContextFactory dbContextFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_DataMart> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        using var _dbContext = _dbContextFactory.CreateDbContext("CASH_FLOW_SKODA_O_TEST");
        _logger.LogInformation("NEW DATAMART MESSAGE Received: DATAMART Message ({Message}))", context.Message.Message_ID);
        var entityMessage = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        if (entityMessage == null)
        {
            _logger.LogError($"DATAMART Message ({context.Message.Message_ID}) was not found in OuterMessage table", DateTimeOffset.Now);
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

                DatamartRequestModel lead = JsonConvert.DeserializeObject<DatamartRequestModel>(jsonString);
                var request_id = lead.crmLeadId;
                var request_type = lead.crmLeadType;

                var comment = lead.crmLeadText;
                var last_name = lead.crmLastName;
                var first_name = lead.crmFirstName;
                var middle_name = lead.crmMiddleName;
                var business_name = lead.crmClientCompanyName;
                var email = lead.crmEmail;
                var phone = lead.crmPhone;
                var address = lead.crmDealerNameWithAddress;
                var created_at = lead.crmDateTimeMoskvichReceive;//?
                var model_name = lead.crmModelOfInterest;

                Guid.TryParse("1E835730-9CB3-4C47-8397-B7BF7CF0231F", out var updUser);

                Guid? visitAimId = EMessageHelper.GetVisitAimId(request_type);

                Guid? eMessageSubjectId = EMessageHelper.GetEMessageSubjectId(request_type);

                string eMessageComment =
                    $"DataMart лид {request_id}\r\n" +
                    $"ФИО клиента: {last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
                    $"Телефон: {phone}\r\n" +
                    $"Почта: {email}\r\n" +
                    "\n" +
                    $"Дата и время получения лида {lead.crmDateTimeMoskvichReceive}\r\n" +
                    $"Дата и время до наступления которых ответ должен быть загружен в систему {lead.crmProcessingDeadline}\r\n" +
                    $"Дата, когда клиент дал согласие на сайте {lead.crmPersonalDataConsentDate}\r\n" +
                    $"Дата согласия на маркетинговую и рекламную коммуникацию {lead.crmMarketingCommAgreementDate}\r\n" +
                    "\n" +
                    $"Тип лида: {request_type}\r\n" +
                    $"Подтип лида: {lead.crmLeadSubtype}\r\n" +
                    $"Источник лида {lead.crmLeadSource}\r\n" +
                    $"Суть запроса клиента {lead.crmLeadText}\r\n" +
                    $"Ссылка на ресурс с доп. информацией {lead.crmLeadLink}\r\n" +
                    "\n" +
                    $"Компания: {lead.crmClientCompanyName}\r\n" +
                    $"ИНН: {lead.crmClientTIN}\r\n" +
                    $"Тип компании для корп. клиента: {lead.crmClientCompanyType}\r\n" +
                    $"Сфера деятельности компании: {lead.crmClientCompanyActivitySphere}\r\n" +
                    "\n" +
                    $"Бренд интереса: {lead.crmBrand}\r\n" +
                    $"Модель: {lead.crmModelOfInterest}\r\n" +
                    $"VIN: {lead.crmRequestedVIN}\r\n" +
                    "\n" +
                    $"Бренд а/м во владении {lead.crmOwnedBrand}\r\n" +
                    $"Модель а/м во владении {lead.crmOwnedModel}\r\n" +
                    $"Год производства а/м во владении {lead.crmOwnedYear}\r\n" +
                    $"Двигатель а/м во владении {lead.crmOwnedEngine}\r\n" +
                    $"Привод а/м во владении {lead.crmOwnedDriveType}\r\n" +
                    $"Тип коробки переключения передач а/м во владении {lead.crmOwnedTransmission}\r\n" +
                    $"Пробег а/м во владении в км {lead.crmOwnedMileage}\r\n" +
                    $"VIN а/м во владении {lead.crmOwnedVIN}\r\n" +
                    $"Регистрационный номер а/м во владении {lead.crmOwnedLicenceNumber}\r\n";

                    
                var connectionString = _configuration.GetConnectionString("CASH_FLOW_SKODA_O_TEST");

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
                    command.Parameters.Add("@LeadID", SqlDbType.VarChar, 255).Value = request_id;
                    command.Parameters.Add("@LeadPhoneNumber", SqlDbType.VarChar, 255).Value = phone;
                    command.Parameters.Add("@LeadEMail", SqlDbType.VarChar, 255).Value = email;
                    command.Parameters.Add("@LeadLastName", SqlDbType.VarChar, 255).Value = last_name;
                    command.Parameters.Add("@LeadFirstName", SqlDbType.VarChar, 255).Value = first_name;
                    command.Parameters.Add("@LeadMiddleName", SqlDbType.VarChar, 255).Value = middle_name;
                    command.Parameters.Add("@LeadBusinessName", SqlDbType.VarChar, 255).Value = business_name;
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
                        entityMessage.ErrorCode = 1;
                        entityMessage.ErrorMessage = errorMes.ToString();
                        entityMessage.ProcessingStatus = 3; //ошибка создания обращения
                        _logger.LogError($"Ошибка создания DATAMART электронного обращения для id={request_id}, request_type={request_type} ({entityMessage.OuterMessage_ID}): {errorMes.ToString()}", DateTimeOffset.Now);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {

                        entityMessage.ProcessingStatus = 1; //обработали и создали обращение
                        entityMessage.ErrorCode = 0;
                        entityMessage.ErrorMessage = "";
                        var message = new RabbitMQStatusMessage_DataMart
                        {
                            Message_ID = entityMessage.OuterMessage_ID
                        };
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Успешно создано DATAMART электронное обращение для id {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID})", DateTimeOffset.Now);

                        try
                        {
                            await _publishEndpoint.Publish(message);
                            _logger.LogInformation($"Сообщение для изменения статуса {entityMessage.MessageOuter_ID} ({entityMessage.OuterMessage_ID}) успешно добавлено в очередь RabbitMQ DATAMART status queue", DateTimeOffset.Now);
                        }
                        catch (Exception ex)
                        {
                            entityMessage.ErrorCode = 1;
                            entityMessage.ErrorMessage = ex.Message;
                            entityMessage.ProcessingStatus = 4;
                            await _dbContext.SaveChangesAsync();
                            _logger.LogError($"Ошибка отправки сообщения DATAMART в RabbitMQ для {entityMessage.OuterMessage_ID}: {ex.Message}", DateTimeOffset.Now);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                entityMessage.ErrorCode = 1;
                entityMessage.ErrorMessage = ex.Message;
                entityMessage.ProcessingStatus = 3; //ошибка создания обращения
                _logger.LogError($"Внутренняя ошибка создания электронного обращения DATAMART для {entityMessage.OuterMessage_ID}, ({entityMessage.OuterMessage_ID}): {ex.Message}", DateTimeOffset.Now);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"InnerException DATAMART: {ex.Message}))", DateTimeOffset.Now);

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Error Info: {ex.InnerException.Message}");
                }

            }
        }
    }

}