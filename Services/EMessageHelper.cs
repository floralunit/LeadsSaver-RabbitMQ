using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadsSaver_RabbitMQ.Services
{
    public class EMessageHelper
    {
        public static Guid? GetVisitAimId(int requestTypeId)
        {
            return requestTypeId switch
            {
                1 or 2 or 3 or 5 or 6 or 11 or 12 or 15 or 27 or 31 or 39 or 42 or 45 => Guid.Parse("52CBAC59-E526-4BCE-9252-CF0CF7305363"), // Отдел розницы
                9 or 30 or 48 or 56 or 94 => Guid.Parse("83AFA901-636B-4B19-BFED-3BCB92C9F3B8"), // Сервис-бюро (слесарный ремонт)
                10 or 18 or 36 => Guid.Parse("51925334-249F-4072-90E0-91CEAE1F24D9"), // Оценка (продажа) ТС с пробегом
                21 or 29 or 51 or 54 or 79 => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
                24 => Guid.Parse("1E64A761-306D-46DD-98C0-5696395DF71A"), // Корпоративный отдел
                _ => null
            };
        }
        public static Guid? GetVisitAimId(string crmLeadType)
        {
            return crmLeadType.ToLower() switch
            {
                "продажи" => Guid.Parse("52CBAC59-E526-4BCE-9252-CF0CF7305363"), // Отдел розницы
                "сервис" => Guid.Parse("83AFA901-636B-4B19-BFED-3BCB92C9F3B8"), // Сервис-бюро (слесарный ремонт)
                "а/м с пробегом" => Guid.Parse("51925334-249F-4072-90E0-91CEAE1F24D9"), // Оценка (продажа) ТС с пробегом
                "жалоба" => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
                _ => null
            };
        }

        public static Guid? GetEMessageSubjectId(int requestTypeId)
        {
            return requestTypeId switch
            {
                1 or 3 => Guid.Parse("B92FCB08-5642-485C-A0A8-9DAA233214C0"), // Заявка на тест-драйв
                // 3 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль для гак
                // 5 => Guid.Parse(""), // Заявка на кредит
                6 or 12 or 21 or 24 or 29 or 30 or 39 or 48 or 54 or 56 or 79 or 94 => Guid.Parse("92472552-F566-4795-8553-5052466B968C"), // Вопрос
                9 => Guid.Parse("30994BF3-BF73-4D9F-A6FA-ED98FB9B9411"), // On-line запись на сервис
                11 or 15 or 31 or 42 or 45 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль
                10 or 18 or 36 => Guid.Parse("EC2354C6-F88F-4798-B2FB-8475C6C1DB3B"), // Оценка а/м с пробегом
                2 or 27 => Guid.Parse("22994476-3A70-4F81-A856-A3EBD0E6E707"), // Заказ обратного звонка
                51 => Guid.Parse("719F0EBC-C512-4190-BC53-085D9ED74EAA"), // Контакт импортёра
                _ => null
            };
        }
        public static string? GetEMessageSubjectName(int requestTypeId)
        {
            return requestTypeId switch
            {
                1 or 3 => "Заявка на тест-драйв", // Заявка на тест-драйв
                // 3 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль для гак
                // 5 => "Заявка на кредит", // Заявка на кредит
                6 or 12 or 21 or 24 or 29 or 30 or 39 or 48 or 54 or 56 or 79 or 94 => "Вопрос", // Вопрос
                9 => "On-line запись на сервис", // On-line запись на сервис
                11 or 15 or 31 or 42 or 45 => "Заявка на новый автомобиль", // Заявка на новый автомобиль
                10 or 18 or 36 => "Оценка а/м с пробегом", // Оценка а/м с пробегом
                2 or 27 => "Заказ обратного звонка", // Заказ обратного звонка
                51 => "Контакт импортёра", // Контакт импортёра
                _ => null
            };
        }
        public static Guid? GetEMessageSubjectId(string crmLeadType)
        {
            return crmLeadType.ToLower() switch
            {
                "продажи" => Guid.Parse("92472552-F566-4795-8553-5052466B968C"), // Вопрос
                "сервис" => Guid.Parse("30994BF3-BF73-4D9F-A6FA-ED98FB9B9411"), // On-line запись на сервис
                "а/м с пробегом" => Guid.Parse("EC2354C6-F88F-4798-B2FB-8475C6C1DB3B"), // Оценка а/м с пробегом
                //"жалоба" => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
                "жалоба" => Guid.Parse("92472552-F566-4795-8553-5052466B968C"), // Клиентская служба
                _ => null
            };
        }

    }
}
