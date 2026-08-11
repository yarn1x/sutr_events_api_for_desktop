using college_events_admin_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;

namespace college_events_admin_API.Services
{
    public class EventsService(SutrEventsDbContext db, ILogger<EventsService> logger)
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly ILogger<EventsService> _logger = logger;


        public List<Event> GetFullList() => [.. _db.Events];

        public List<Event> GetListByStatus(int Id) => [.. _db.Events.Where(s => s.StatusId == Id)];


        public int UpdateEventsStatuses()
        {
            DateTime realTime = DateTime.Now;
            int savedCount = 0;

            try {
                //список мероприятий, которые нужно изменить
                var expired_events = _db.Events
                    .Where(e => e.StatusId == 2 && e.EndDatetime <= realTime)
                    .ToList();
                if (expired_events.Any())
                {
                    //по каждому мероприятию проходим в цикле и меняем статус на 3 (пройдено, требуется отчёт)
                    foreach (Event expired in expired_events)
                    {
                        _logger?.LogInformation($"~~ 🔄️ Обновление мероприятия: {expired.Title} (ID: {expired.EventId}) ~~");
                        expired.StatusId = 3;
                    }
                    savedCount += _db.SaveChanges();
                }
            } catch (Exception ex) {
                _logger?.LogError($"~~! Ошибка изменения статусов мероприятий !~~\nСообщение ошибки {ex} !~~");
            }

            //изменение ложно изменённых статусов если дата проведения мероприятия не пройдена
            try {

                //список мероприятий, которые нужно изменить
                var fake_expired_events = _db.Events
                    .Where(e => e.StatusId == 3 && e.EndDatetime >= realTime)
                    .ToList();
                if (fake_expired_events.Any())
                {
                    //по каждому мероприятию проходим в цикле и меняем статус на 3 (пройдено, требуется отчёт)
                    foreach (Event fake_expired_event in fake_expired_events)
                    {
                        _logger?.LogInformation($"~~ Обновление мероприятия: {fake_expired_event.Title} (ID: {fake_expired_event.EventId}) ~~");
                        fake_expired_event.StatusId = 2;
                    }
                    savedCount += _db.SaveChanges();
                }
            } catch (Exception ex) {
                _logger?.LogError($"~~! Ошибка изменения ложно изменённых статусов !~~\nСообщение ошибки {ex} !~~");
            }
            return savedCount;
        }
    }
}
