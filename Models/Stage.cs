namespace LeadsSaverRabbitMQ.Models
{
    /// <summary>
    /// Представляет информацию об этапе запроса.
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// Идентификатор этапа в LMS.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор связанного запроса.
        /// </summary>
        public int? request_Id { get; set; }

        /// <summary>
        /// Идентификатор типа этапа.
        /// </summary>
        public int? stage_type_id { get; set; }

        /// <summary>
        /// Дата и время последнего обновления данных по этапу.
        /// </summary>
        public DateTime LastTimeAt { get; set; }

        /// <summary>
        /// Дата и время закрытия этапа.
        /// </summary>
        public DateTime? FinishedAt { get; set; } // Nullable, так как этап может быть открытым

        /// <summary>
        /// Идентификатор типа результата закрытия этапа.
        /// </summary>
        public int? result_id { get; set; }

        /// <summary>
        /// Признак закрытия этапа.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Код (статус) текущего состояния обращения.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Время, оставшееся на обработку текущего этапа.
        /// </summary>
        public TimeSpan TimeLeft { get; set; }
        public StageType stageType { get; set; }
    }

    public class StageType
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int priority { get; set; }
        public int status { get; set; }
        public int user_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
