using college_events_admin_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace college_events_admin_API.Services
{
    public class EventsService(SutrEventsDbContext db, ILogger<EventsService> logger)
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly ILogger<EventsService> _logger = logger;


        public int UpdateEventsStatuses()
        {
            //настоящее время для сверивания
            DateTime realTime = DateTime.Now;
            
            //количество обновленных мероприятий
            int savedCount = 0;

            //статус, на который будет обновлено мероприятие
            byte newStatus = EventConstant.status_suggested;


            //со статуса #2 на статус #3
            try
            {
                newStatus = EventConstant.status_done_report_needed;

                //список мероприятий, которые нужно изменить
                var expired_events = _db.Events
                    .Where(e => e.StatusId == EventConstant.status_applied && e.EndDatetime <= realTime)
                    .ToList();

                if (expired_events.Any())
                {
                    //по каждому мероприятию проходим в цикле и меняем статус на 3 (пройдено, требуется отчёт)
                    foreach (Event expired in expired_events)
                    {
                        _logger?.LogInformation($"~~ Обновление мероприятия: {expired.Title} (ID: {expired.EventId}). StatusId {expired.StatusId} -> {newStatus} ~~");
                        expired.StatusId = newStatus;
                    }
                    savedCount += _db.SaveChanges();
                }
            } 
            catch (Exception ex) 
            {
                _logger?.LogError($"### Ошибка изменения статусов мероприятий ###\n### Сообщение ошибки {ex} ###");
            }



            //изменение ложно изменённых статусов если дата проведения мероприятия не пройдена
            //(со статуса #3 на статус #2)
            try {

                newStatus = EventConstant.status_applied;

                //список мероприятий, которые нужно изменить
                var fake_expired_events = _db.Events
                    .Where(e => e.StatusId == EventConstant.status_done_report_needed && e.EndDatetime >= realTime)
                    .ToList();

                if (fake_expired_events.Any())
                {
                    //по каждому мероприятию проходим в цикле и меняем статус на 2 (запланировано)
                    foreach (Event fake_expired_event in fake_expired_events)
                    {
                        _logger?.LogInformation($"~~ Обновление мероприятия: {fake_expired_event.Title} (ID: {fake_expired_event.EventId}). StatusId {fake_expired_event.StatusId} -> {newStatus} ~~");
                        fake_expired_event.StatusId = newStatus;
                    }
                    savedCount += _db.SaveChanges();
                }

            } 
            catch (Exception ex) 
            {
                _logger?.LogError($"### Ошибка изменения ложно изменённых статусов ###\n### Сообщение ошибки {ex} ###");
            }
            


            return savedCount;
        }

        
    }
}
