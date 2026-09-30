using college_events_admin_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace college_events_admin_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("/college/admin/supervisors")]
    public class ControllerSupervisors(SutrEventsDbContext db) : Controller
    {
        SutrEventsDbContext _db = db;

        [HttpGet]
        public ActionResult GETSupervisorList()
        {
            var arr = _db.UserUsertypes.Select(u => new
            {
                u.UserTypeId,
                u.AuthorizedUser.UserId,
                //НЕ МЕНЯТЬ ИМЕНА ПОЛЕЙ. ЧЕРЕВАТО НЕИЗВЕСТНЫМИ ОШИБКАМИ
                u.AuthorizedUser.User.FirstName,
                SurName = u.AuthorizedUser.User.LastName,
                LastName = u.AuthorizedUser.User.MiddleName,
                u.AuthorizedUser.Email,
                u.AuthorizedUser.Phone,
                groups = _db.Groups.Select(g => new
                {
                    g.AuthorizedUser.UserId,
                    g.Name,
                }).Where(g => g.UserId == u.AuthorizedUser.UserId).ToList()
            })
            .Where(uid => uid.UserTypeId == 2)
            .ToList();

            return Ok(arr);
        }

        [HttpGet("{supervisorId}/statistic")]
        public async Task<ActionResult> GETSupervisorStatistic(int supervisorId)
        {
            //находим записи о фактически присутствовавших группах на мероприятиях
            //с указанием ID куратора
            var baseQuery = _db.ActualAttendances
                .Where(aa => aa.EventGroup.Group.AuthorizedUserId == supervisorId);

            //собираем инфу
            var response = await baseQuery
                .GroupBy(events => new
                {
                    events.EventGroup.EventId,
                    events.EventGroup.Event.Title,
                    events.EventGroup.Event.CategoryId,
                    categoryName = events.EventGroup.Event.Category.Name,
                    groupName = events.EventGroup.Group.Name,
                    events.ActualListenersCount,
                    events.ActualParticipantsCount,
                    events.ActualSuperParticipantsCount,
                    events.TotalScore,
                })
                .Select(k => new
                {
                    k.Key.EventId,
                    k.Key.Title,
                    k.Key.CategoryId,
                    k.Key.categoryName,
                    k.Key.groupName,
                    k.Key.ActualListenersCount,
                    k.Key.ActualParticipantsCount,
                    k.Key.ActualSuperParticipantsCount,
                    k.Key.TotalScore,
                }).ToListAsync();

            /*дополнительно всякие статистические циферки*/
            //количество мероприятий, на которое был записан данный куратор (а точнее, сколько раз его группы присутствовали на мероприятии)
            int eventsCount = response.Count;

            //количество направлений мероприятий (сколько различных направлений посетили группы куратора)
            int categoriesCount = response.Select(c => c.CategoryId).Distinct().Count();

            //тоталити сколько баллов за все мероприятия
            int totalScore = response.Sum(ts => ts.TotalScore);

            return Ok(new
            {
                eventsCount,
                categoriesCount,
                totalScore,

                events = response.Select(e => new
                {
                    e.groupName,
                    e.Title,
                    e.categoryName,
                    e.ActualListenersCount,
                    e.ActualParticipantsCount,
                    e.ActualSuperParticipantsCount,
                    e.TotalScore,
                })
            });
        }
    }
}
