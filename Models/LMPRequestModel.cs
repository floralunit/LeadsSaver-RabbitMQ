using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadsSaver_RabbitMQ.Models
{

    /// <summary>
    /// Основная модель, представляющая лид.
    /// </summary>
    public class LMPRequestModel
    {
        /// <summary>
        /// Идентификатор лида в С360 (guid).
        /// </summary>
        public Guid lead_id { get; set; }

        /// <summary>
        /// Код ДЦ лида.
        /// </summary>
        public string outlet_code { get; set; }

        /// <summary>
        /// Идентификатор типа лида.
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// Название типа лида.
        /// </summary>
        public string type_name { get; set; }

        /// <summary>
        /// Идентификатор температуры лида (0 - не маркирован, 1 - холодный, 2 - горячий).
        /// </summary>
        public int? temperature_id { get; set; }

        /// <summary>
        /// Название температуры лида.
        /// </summary>
        public string temperature_name { get; set; }

        /// <summary>
        /// Идентификатор источника лида.
        /// </summary>
        public int source_id { get; set; }

        /// <summary>
        /// Название источника лида.
        /// </summary>
        public string source_name { get; set; }

        /// <summary>
        /// Список групп дополнительных свойств лида.
        /// </summary>
        public List<LeadCustomPropertyGroupDto> custom_properties_groups { get; set; }

        /// <summary>
        /// Контактная информация по лиду.
        /// </summary>
        public LeadContactDto contact { get; set; }

        /// <summary>
        /// Идентификатор статуса лида.
        /// </summary>
        public int status_id { get; set; }

        /// <summary>
        /// Название статуса лида.
        /// </summary>
        public string status_name { get; set; }

        /// <summary>
        /// Идентификатор статуса лида со стороны дилера.
        /// </summary>
        public int dealer_status_id { get; set; }

        /// <summary>
        /// Название статуса лида со стороны дилера.
        /// </summary>
        public string dealer_status_name { get; set; }

        /// <summary>
        /// Название статуса лида со стороны дилера, сконфигурированное в настройке точки интеграции (при наличии).
        /// </summary>
        public string dealer_status_crm_name { get; set; }

        /// <summary>
        /// Публичный идентификатор лида.
        /// </summary>
        public int public_id { get; set; }

        /// <summary>
        /// Дата и время экспирации лида (если предусмотрено) в часовом поясе UTC.
        /// </summary>
        public DateTime? expiration_datetime { get; set; }

        /// <summary>
        /// Дата и время - дедлайн на взятие заявки в работу (в часовом поясе UTC).
        /// </summary>
        public DateTime? target_processing_date { get; set; }

        /// <summary>
        /// Дата и время - дедлайн на обработку заявки (в часовом поясе UTC).
        /// </summary>
        public DateTime? target_first_contact_date { get; set; }

        /// <summary>
        /// Дата последнего события на стороне дилера (дата простановки статуса дилером).
        /// </summary>
        public DateTime? dealer_event_date { get; set; }

        /// <summary>
        /// Дата и время передачи лида дилеру.
        /// </summary>
        public DateTime? dealer_receive_date { get; set; }

        /// <summary>
        /// Комментарий дилера к причине отказа.
        /// </summary>
        public string dealer_refuse_comment { get; set; }

        /// <summary>
        /// Номер рабочего листа.
        /// </summary>
        public string client_dms_id { get; set; }

        /// <summary>
        /// Идентификатор причины отказа.
        /// </summary>
        public int? dealer_refuse_reason_id { get; set; }

        /// <summary>
        /// Планируемая дилером дата визита в ДЦ.
        /// </summary>
        public DateTime? dealer_visit_planned_date { get; set; }

        /// <summary>
        /// Планируемая дилером дата перезвона.
        /// </summary>
        public DateTime? dealer_recall_date { get; set; }

        /// <summary>
        /// Планируемая дата визита в ДЦ, полученная от клиента при создании лида.
        /// </summary>
        public DateTime? visit_planned_date { get; set; }

        /// <summary>
        /// Идентификатор группы связанных лидов.
        /// </summary>
        public Guid lead_group_id { get; set; }

        /// <summary>
        /// Название причины отказа.
        /// </summary>
        public string dealer_refuse_reason_name { get; set; }

        /// <summary>
        /// Признак, требуется ли для типа и источника данного лида квалификация или не требуется.
        /// </summary>
        public bool qulification_required { get; set; }

        /// <summary>
        /// Имя пользователя, в данный момент ответственного за обработку лида.
        /// </summary>
        public string responsible_user { get; set; }

        /// <summary>
        /// SourceChannelDetail.
        /// </summary>
        public string source_channel_detail { get; set; }

        /// <summary>
        /// Подразделение (Продажа новых а/м, Послепродажное обслуживание и т.д.).
        /// </summary>
        public string communication_target { get; set; }

        /// <summary>
        /// Источник кампании.
        /// </summary>
        public string source_campaign { get; set; }
    }

    /// <summary>
    /// Контактная информация по лиду.
    /// </summary>
    public class LeadContactDto
    {
        /// <summary>
        /// Имя.
        /// </summary>
        public string first_name { get; set; }

        /// <summary>
        /// Фамилия.
        /// </summary>
        public string last_name { get; set; }

        /// <summary>
        /// Отчество.
        /// </summary>
        public string middle_name { get; set; }

        /// <summary>
        /// Контактный номер телефона.
        /// </summary>
        public string contect_phone { get; set; }

        /// <summary>
        /// Адрес электронной почты.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Адрес.
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// Бренд.
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// Модель автомобиля.
        /// </summary>
        public string car_model { get; set; }

        /// <summary>
        /// Год выпуска модели автомобиля.
        /// </summary>
        public int? car_model_year { get; set; }

        /// <summary>
        /// Пол.
        /// </summary>
        public int? gender { get; set; }

        /// <summary>
        /// Дата рождения.
        /// </summary>
        public DateTime? date_of_birth { get; set; }

        /// <summary>
        /// VIN автомобиля.
        /// </summary>
        public string vin { get; set; }
    }

    /// <summary>
    /// Группа дополнительных свойств лида.
    /// </summary>
    public class LeadCustomPropertyGroupDto
    {
        /// <summary>
        /// Название группы.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Последовательность отображения групп.
        /// </summary>
        public int? sequence { get; set; }

        /// <summary>
        /// Список дополнительных свойств данной группы.
        /// </summary>
        public List<LeadCustomPropertyDto> custom_properties { get; set; }
    }

    /// <summary>
    /// Дополнительное свойство лида.
    /// </summary>
    public class LeadCustomPropertyDto
    {
        /// <summary>
        /// Код.
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Тип дополнительного свойства.
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// Название типа дополнительного свойства.
        /// </summary>
        public string type_name { get; set; }

        /// <summary>
        /// Значение дополнительного свойства.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Последовательность отображения дополнительных свойств.
        /// </summary>
        public int? sequence { get; set; }
    }
}
