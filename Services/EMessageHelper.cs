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
                1 or 2 or 3 or 6 or 11 or 12 or 15 or 27 or 31 or 39 or 42 or 45 or 70 => Guid.Parse("52CBAC59-E526-4BCE-9252-CF0CF7305363"), // Отдел розницы
                9 or 30 or 48 or 56 or 94 => Guid.Parse("83AFA901-636B-4B19-BFED-3BCB92C9F3B8"), // Сервис-бюро (слесарный ремонт)
                10 or 18 or 36 => Guid.Parse("51925334-249F-4072-90E0-91CEAE1F24D9"), // Оценка (продажа) ТС с пробегом
                21 or 29 or 51 or 54 or 79 => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
                24 => Guid.Parse("1E64A761-306D-46DD-98C0-5696395DF71A"), // Корпоративный отдел
                5 or 7 => Guid.Parse("5ceaabbe-5f28-4f46-93a6-61fdadbfa905"), // ОФУ
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
                1 or 3 or 70 => Guid.Parse("B92FCB08-5642-485C-A0A8-9DAA233214C0"), // Заявка на тест-драйв
                // 3 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль для гак
                5 or 7 => Guid.Parse("1db39b6c-59aa-4222-814e-e15928beb775"), // Заявка на кредит
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
                5 or 7 => "Заявка на кредит", // Заявка на кредит
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
                "жалоба" => Guid.Parse("a0d02d92-ff61-46fa-92db-3af8102c7aec"), // жалоба
                _ => null
            };
        }

        public static Guid? GetBWMCenterByOutletCode(string outlet_code)
        {
            return outlet_code switch
            {
                "31983" => Guid.Parse("24774de6-fdf3-44e1-af56-d41ae1add566"), // БМВ-Вернадский
                "32330" => Guid.Parse("c84196e4-9890-405e-a6c8-888de9289d4d"), // БМВ-МКАД 51 км
                "32869" => Guid.Parse("53e571c6-8b39-479e-b130-4852fd47a74e"), // БМВ-Зорге
                "33173" => Guid.Parse("61568afe-81fe-4b55-b2bd-453bb6e51fd9"), // БМВ-Стартовая
                "45551" => Guid.Parse("a1dfedf2-ab79-460c-886e-1d0fd604f058"), // БМВ-Шмитовский
                "49608" => Guid.Parse("a6c88432-e250-4d4f-aefb-0ba8ce1fe4fb"), // БМВ-Внуково
                _ => null
            };
        }

        public static Guid? GetVisitAimId_BMW(int type_id)
        {
            return type_id switch
            {
                1 or 4 or 8 => Guid.Parse("52CBAC59-E526-4BCE-9252-CF0CF7305363"), // Отдел розницы
                5 => Guid.Parse("83AFA901-636B-4B19-BFED-3BCB92C9F3B8"), // Сервис-бюро (слесарный ремонт)
                18 => Guid.Parse("2A20D8B0-C7F8-43BD-B085-C0281816CF13"), // Клиентская служба
                3 => Guid.Parse("b688a9c7-cbf8-43c2-9b15-76e2f3e98bb2"), // Покупка автомобиля с пробегом
                9 or 16 => Guid.Parse("5ceaabbe-5f28-4f46-93a6-61fdadbfa905"), // ОФУ
                11 => Guid.Parse("7834a5c9-dd15-456d-95fd-d33af57fce6a"), // Запчасти
                _ => null
            };
        }

        public static Guid? GetEMessageSubjectId_BMW(int type_id)
        {
            return type_id switch
            {
                1 => Guid.Parse("B92FCB08-5642-485C-A0A8-9DAA233214C0"), // Заявка на тест-драйв
                8 or 9 or 18 => Guid.Parse("92472552-F566-4795-8553-5052466B968C"), // Вопрос
                4 => Guid.Parse("2600F15D-8DF5-42F5-9098-215357DAF5B4"), // Заявка на новый автомобиль
                5 => Guid.Parse("30994BF3-BF73-4D9F-A6FA-ED98FB9B9411"), // On-line запись на сервис
                11 => Guid.Parse("22994476-3A70-4F81-A856-A3EBD0E6E707"), // Заказ обратного звонка
                3 => Guid.Parse("d0c08e77-bd0e-4354-872f-3ba3c4661b07"), // Покупка автомобиля с пробегом
                16 => Guid.Parse("1db39b6c-59aa-4222-814e-e15928beb775"), // Заявка на кредит
                _ => null
            };
        }

        public static string? GetEMessageSubjectName_BMW(int type_id)
        {
            return type_id switch
            {
                1 => "Заявка на тест-драйв", // Заявка на тест-драйв
                8 or 9 or 18 => "Вопрос", // Вопрос
                4 => "Заявка на новый автомобиль", // Заявка на новый автомобиль
                5 => "On-line запись на сервис", // On-line запись на сервис
                11 => "Заказ обратного звонка", // Заказ обратного звонка
                3 => "Покупка автомобиля с пробегом", // Покупка автомобиля с пробегом
                16 => "Заявка на кредит", // Заявка на кредит
                _ => null
            };
        }

    }
}
