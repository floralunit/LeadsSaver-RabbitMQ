namespace LeadsSaverRabbitMQ.Models
{
    public class Request
    {
        /// <summary>
        /// Идентификационный номер лида в LMS.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Идентификационный номер рекламной кампании в LMS.
        /// </summary>
        public int? Campaign_Id { get; set; }

        /// <summary>
        /// Идентификационный номер департамента (отдела) в LMS.
        /// </summary>
        public int? Department_Id { get; set; }

        /// <summary>
        /// Идентификационный номер типа запроса в LMS.
        /// </summary>
        public int Request_Type_Id { get; set; }
        /// <summary>
        /// Идентификационный клиента в LMS.
        /// </summary>
        public int? Client_Id { get; set; }
        public int? Assigned_Id { get; set; }

        /// <summary>
        /// Идентификационный номер площадки (рекламного канала) в LMS.
        /// </summary>
        public int? Channel_Id { get; set; }

        /// <summary>
        /// Идентификационный номер источника возникновения лида в LMS.
        /// </summary>
        public int? Source_Id { get; set; }

        /// <summary>
        /// Идентификационный номер дилера в LMS.
        /// </summary>
        public int? Dealer_Id { get; set; }

        /// <summary>
        /// Комментарий к обращению клиента (лиду). В теле комментария может быть указана комплектация и желаемый цвет автомобиля-потребности клиента.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Статус лида: 1 - Новый, 2 - В работе, 3 - Закрыт.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Признак клиента-юридического лица: 0 – физ. лицо, 1 – юр. Лицо.
        /// </summary>
        public int? Is_Business { get; set; }

        /// <summary>
        /// Наименование юр. лица.
        /// </summary>
        public string Business_Name { get; set; }

        /// <summary>
        /// Имя клиента - физ. лица.
        /// </summary>
        public string First_Name { get; set; }

        /// <summary>
        /// Отчество клиента - физ. лица.
        /// </summary>
        public string Middle_Name { get; set; }

        /// <summary>
        /// Фамилия клиента - физ. лица.
        /// </summary>
        public string Last_Name { get; set; }

        /// <summary>
        /// Адрес электронной почты, указанный клиентом в обращении.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Адрес проживания клиента, указанный в обращении.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Предпочтительный способ связи: 1 – телефон, 0 – электронная почта.
        /// </summary>
        public char? communication_method { get; set; }

        /// <summary>
        /// Идентификатор бренда в LMS, для автомобиля, указанного в обращении клиента.
        /// </summary>
        public int? Brand_Id { get; set; }

        /// <summary>
        /// Идентификатор модели в LMS, для автомобиля, указанного в обращении клиента.
        /// </summary>
        public int? Model_Id { get; set; }

        /// <summary>
        /// VIN автомобиля.
        /// </summary>
        public string Vin { get; set; }

        /// <summary>
        /// ID обращения, по которому был создан данный лид.
        /// </summary>
        public int? InterestId { get; set; }

        /// <summary>
        /// Список номеров телефонов, указанных клиентом в обращении.
        /// </summary>
        public List<Phone>? Phones { get; set; }
        
        /// <summary>
        /// Согласие на отправку смс и других уведомлений: 0 - нет, 1 - да.
        /// </summary>
        public bool? ClientConfirmCommunication { get; set; }

        /// <summary>
        /// Признак сделки.
        /// </summary>
        public bool? IsDeal { get; set; }

        /// <summary>
        /// Признак VIP обращения.
        /// </summary>
        public bool? IsVip { get; set; }

        /// <summary>
        /// Желаемая дата контакта в формате yyyy-MM-dd hh:mm:ss.
        /// </summary>
        public string Contact_At { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? Updated_At { get; set; }

        /// <summary>
        /// Параметры источника
        /// </summary>
        public Source source { get; set; }

        /// <summary>
        /// Описание бренда
        /// </summary>
        public VehicleBrand vehicleBrand { get; set; }

        /// <summary>
        /// Описание модели
        /// </summary>
        public VehicleModel vehicleModel { get; set; }

        /// <summary>
        /// Описание типа запроса
        /// </summary>
        public RequestType requestType { get; set; }

        /// <summary>
        /// Массив стадий запроса
        /// </summary>
        public Stage[] stages { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
    }

}
