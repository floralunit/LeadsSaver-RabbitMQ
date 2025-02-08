using MassTransit;
using LeadsSaverRabbitMQ.Models;
using LMSWebService.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using LeadsSaverRabbitMQ.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsLMSConsumer : IConsumer<RabbitMQLeadMessage>
{
    private readonly AstraContext _dbContext;
    private readonly BrandConfigurationSettings _brandSettings;

    public LeadsLMSConsumer(AstraContext dbContext, IOptions<BrandConfigurationSettings> brandSettings)
    {
        _dbContext = dbContext;
        _brandSettings = brandSettings.Value;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage> context)
    {
        var value = $"Received: LMS Message ({context.Message.Message_ID}))";
        Console.WriteLine(value);
        var entity = await _dbContext.OuterMessage.FirstOrDefaultAsync(e => e.OuterMessage_ID.ToString().ToLower() == context.Message.Message_ID.ToString().ToLower());
        if (entity != null)
        {
            var jsonString = entity.MessageText;
            try
            {
                Request l = JsonConvert.DeserializeObject<Request>(jsonString);
                var request_id = l.Id;
                var request_type_id = l.RequestTypeId;
                var client_id = l.ClientId;
                var assigned_id = l.AssignedId;
                var comment = l.Comment;
                var last_name = l.LastName;
                var first_name = l.FirstName;
                var middle_name = l.MiddleName;
                var business_name = l.BusinessName;
                var email = l.Email;
                var phone = l.Phones?.First().Number;
                var address = l.Address;
                var communication_method = l.CommunicationMethod;
                var client_confirm_communication = l.ClientConfirmCommunication;
                var vin = l.Vin;
                var created_at = l.СreatedAt;
                var request_type_name = l.RequestType.Name;
                var model_name = l.VehicleModel.Name;
                var stage_work_id = l.Stages.FirstOrDefault(x => x.StageTypeId == 3).Id;
                var centerId = context.Message.Center_ID;
                var projectId = _brandSettings.Brands.FirstOrDefault(x => x.BrandName == context.Message.BrandName);
                Guid.TryParse("B5C8A3E3-CF6F-44B3-83BF-68ACA010B473", out var updUser);

                Guid? visitAimId = GetVisitAimId(request_type_id);

                Guid? eMessageSubjectId = GetEMessageSubjectId(request_type_id);

                string eMessageComment = $"{request_type_id}\r\n" +
                                          $"LMS лид {client_id}\r\n" +
                                          $"{last_name ?? ""} {first_name ?? ""} {middle_name ?? ""}\r\n" +
                                          $"{business_name ?? ""}\r\n" +
                                          $"{phone ?? ""}\r\n" +
                                          $"{model_name ?? ""}\r\n" +
                                          $"{vin ?? ""}\r\n" +
                                          $"{comment ?? ""}\r\n" +
                                          $"Предпочтительный способ связи = {communication_method}";

                var query = "SELECT roi.Row_ID FROM autocrm.RowOuterID roi " +
                    "WHERE roi.Outer_ID = {0} AND roi.SendLogRowType_ID = 4 AND roi.Center_ID = {1}";

                var employeeId = _dbContext.EmployeeIdResults
                        .FromSqlRaw(query, assigned_id, centerId)
                        .AsEnumerable()
                        .FirstOrDefault();

                var createdAtParam = new SqlParameter("@DocumentBaseDate", created_at);
                var departmentIdParam = new SqlParameter("@Department_ID", DBNull.Value);
                var projectIdParam = new SqlParameter("@Project_ID", projectId);
                var centerIdParam = new SqlParameter("@Center_ID", centerId);
                var userIdParam = new SqlParameter("@InsApplicationUser_ID", updUser);
                var visitAimIdParam = new SqlParameter("@VisitAim_ID", visitAimId);
                var eMessageSubjectIdParam = new SqlParameter("@EMessageSubject_ID", eMessageSubjectId);
                var employeeIdParam = new SqlParameter("@EmployeePlan_ID", employeeId);
                var eMessageCommentParam = new SqlParameter("@Comment", eMessageComment ?? (object)DBNull.Value);

                var errorParam = new SqlParameter("@ErrMes", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

                var proActivityParam = new SqlParameter("@ProActivity", 39);
                var leadOrderNumberParam = new SqlParameter("@LeadOrderNumber", request_id);
                var leadIdParam = new SqlParameter("@LeadID", client_id);
                var leadPhoneNumberParam = new SqlParameter("@LeadPhoneNumber", phone);
                var leadEmailParam = new SqlParameter("@LeadEMail", email);
                var leadLastNameParam = new SqlParameter("@LeadLastName", last_name);
                var leadFirstNameParam = new SqlParameter("@LeadFirstName", first_name);
                var leadMiddleNameParam = new SqlParameter("@LeadMiddleName", middle_name);
                var leadBusinessNameParam = new SqlParameter("@LeadBusinessName", business_name);
                var leadAddressParam = new SqlParameter("@LeadAddress", address);
                var leadCommunicationMethodParam = new SqlParameter("@LeadCommunicationMethod", communication_method);
                var leadForceTransitionToAccept = new SqlParameter("@ForceTransitionToAccept ", 1);

                _dbContext.Database.ExecuteSqlRaw("EXEC dbo.PR_EMessage_Set @DocumentBaseDate, @Department_ID, @Project_ID, @Center_ID, @InsApplicationUser_ID, " +
                                                 "@VisitAim_ID, @EMessageSubject_ID, @EmployeePlan_ID, @Comment, @ErrMes OUTPUT, @ProActivity, " +
                                                 "@LeadOrderNumber, @LeadID, @LeadPhoneNumber, @LeadEMail, @LeadLastName, @LeadFirstName, " +
                                                 "@LeadMiddleName, @LeadBusinessName, @LeadAddress, @LeadCommunicationMethod, @ForceTransitionToAccept",
                                                 createdAtParam,
                                                 departmentIdParam,
                                                 projectIdParam,
                                                 centerIdParam,
                                                 userIdParam,
                                                 visitAimIdParam,
                                                 eMessageSubjectIdParam,
                                                 employeeIdParam,
                                                 eMessageCommentParam,
                                                 errorParam,
                                                 proActivityParam,
                                                 leadOrderNumberParam,
                                                 leadIdParam,
                                                 leadPhoneNumberParam,
                                                 leadEmailParam,
                                                 leadLastNameParam,
                                                 leadFirstNameParam,
                                                 leadMiddleNameParam,
                                                 leadBusinessNameParam,
                                                 leadAddressParam,
                                                 leadCommunicationMethodParam,
                                                 leadForceTransitionToAccept);

                var eMessageError = (string)errorParam.Value;

            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка десериализации: {ex.Message}");

                // Попробуйте извлечь информацию о проблемных полях
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Дополнительная информация: {ex.InnerException.Message}");
                }
            }
        }
        else
        {
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
public class EmployeeIdResult
{
    public Guid Row_ID { get; set; }
}
