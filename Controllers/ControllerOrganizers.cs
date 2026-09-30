using college_events_admin_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace college_events_admin_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("/college/admin/organizers")]
    public class ControllerOrganizers(SutrEventsDbContext db) : Controller
    {
        SutrEventsDbContext _db = db;

        [HttpGet]
        public ActionResult GETOrganizersList()
        {
            var arr = _db.UserUsertypes.Select(u => new
            {
                u.AuthorizedUser.UserId,
                //НЕ МЕНЯТЬ ИМЕНА ПОЛЕЙ. ЧЕРЕВАТО НЕИЗВЕСТНЫМИ ОШИБКАМИ
                //Во всей системе, на стороне настольного приложения
                //firstname - имя
                //surname - фамилия
                //lastname - отчество
                u.AuthorizedUser.User.FirstName,
                SurName = u.AuthorizedUser.User.LastName,
                LastName = u.AuthorizedUser.User.MiddleName,
                u.UserTypeId,
                u.UserType.TypeName,
                u.AuthorizedUser.Email,
                u.AuthorizedUser.Phone,
                eventsCount = _db.Events.Where(uid => uid.OrganizerId == u.AuthorizedUser.UserId && uid.StatusId == 4).Count()
            })
            .Where(t => t.UserTypeId == 3)
            .ToList();

            return Ok(arr);
        }



        [HttpGet("{organizerId}/statistic")]
        public async Task<ActionResult> GETOrganizerStatistic(int organizerId)
        {
            //находим составленные отчёты с конкретным организатором
            var baseQuery = _db.ActualAttendances
                .Where(e => e.EventGroup.Event.OrganizerId == organizerId);

            //из найденных отчётов извлекаем нужную информацию
            var groupedEvents = await baseQuery
                .GroupBy(a => new
                {
                    a.EventGroup.Event.EventId,
                    a.EventGroup.Event.Title,
                    a.EventGroup.Event.StartDatetime,
                    a.EventGroup.Event.EndDatetime,
                    a.EventGroup.Event.FullDescription,
                    a.EventGroup.Event.Category.Name,
                })
                .Select(g => new
                {
                    g.Key.EventId,
                    g.Key.Title,
                    g.Key.StartDatetime,
                    g.Key.EndDatetime,
                    g.Key.FullDescription,
                    CategoryName = g.Key.Name,

                    ActualListenersCount = g.Sum(a => a.ActualListenersCount),
                    ActualParticipantsCount = g.Sum(a => a.ActualParticipantsCount),
                    ActualSuperParticipantsCount = g.Sum(a => a.ActualSuperParticipantsCount),

                    SupervisorIds = g.Select(a => a.EventGroup.Group.AuthorizedUserId).Distinct()
                })
                .ToListAsync();

            //считаем сколько суммарно слушателей на всех мероприятиях
            int totalListeners = groupedEvents.Sum(e => e.ActualListenersCount);
            //сколько суммарно участников
            int totalParticipants = groupedEvents.Sum(e => e.ActualParticipantsCount);
            //сколько супер-участников
            int totalSuperParticipants = groupedEvents.Sum(e => e.ActualSuperParticipantsCount);
            
            //считаем сколько организатор провёл мероприятий (именно провёл, а не всего мероприятий с указанием этого организатора)
            int eventsCount = groupedEvents.Count;

            //считаем сколько кураторов было (косвенно) суммарно на всех мероприятиях конкретного организатора
            int supervisorsCount = groupedEvents
                .SelectMany(e => e.SupervisorIds)
                .Distinct()
                .Count();

            //формируем финальный результат
            return Ok(new
            {
                studentsCount = totalListeners + totalParticipants + totalSuperParticipants,
                eventsCount,
                supervisorsCount,
                events = groupedEvents.Select(e => new
                {
                    e.EventId,
                    e.Title,
                    e.StartDatetime,
                    e.EndDatetime,
                    e.FullDescription,
                    e.CategoryName,
                    e.ActualListenersCount,
                    e.ActualParticipantsCount,
                    e.ActualSuperParticipantsCount
                }).ToList()
            });
        }

    }
}
