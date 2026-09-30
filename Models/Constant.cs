namespace college_events_admin_API.Models
{
    public static class EventConstant
    {
        /// <summary>
        /// Уникальный идентификатор для статуса предложенного
        /// </summary>
        public const int status_suggested = 1;


        /// <summary>
        /// Уникальный идентификатор для статуса запланированного
        /// </summary>
        public const int status_applied = 2;


        /// <summary>
        /// Уникальный идентификатор для статуса прошедшего, требуется составление отчёта
        /// </summary>
        public const int status_done_report_needed = 3;


        /// <summary>
        /// Уникальный идентификатор для статуса прошедшего
        /// </summary>
        public const int status_done = 4;


        /// <summary>
        /// Уникальный идентификатор для статуса перенесённого
        /// </summary>
        public const int status_rescheduled = 5;


        /// <summary>
        /// Уникальный идентификатор для статуса отменённого
        /// </summary>
        public const int status_rejected = 6;
    }
    public static class UserConstant
    {
        /// <summary>
        /// Уникальный идентификатор для роли администратора
        /// </summary>
        public const int administratorTypeId = 1;


        /// <summary>
        /// Уникальный идентификатор для роли куратора
        /// </summary>
        public const int supervisorTypeId = 2;


        /// <summary>
        /// Уникальный идентификатор для роли организатора
        /// </summary>
        public const int organizerTypeId = 3;
    }
}
