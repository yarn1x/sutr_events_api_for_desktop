using Azure;
using college_events_admin_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace college_events_admin_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("/college/admin/groups")]
    public class ControllerGroups(SutrEventsDbContext db) : Controller
    {
        SutrEventsDbContext _db = db;

        [HttpGet]
        public ActionResult GETGroupList()
        {
            var arr = _db.Groups.Select(g => new
            {
                g.GroupId,
                GroupName = g.Name,
                //НЕ МЕНЯТЬ ИМЕНА ПОЛЕЙ. ЧЕРЕВАТО НЕИЗВЕСТНЫМИ ОШИБКАМИ
                //Во всей системе, на стороне настольного приложения
                //firstname - имя
                //surname - фамилия
                //lastname - отчество
                SupervisorFirstname = g.AuthorizedUser.User.FirstName,
                SupervisorSurname = g.AuthorizedUser.User.LastName,
                SupervisorLastname = g.AuthorizedUser.User.MiddleName,
                SupervisorEmail = g.AuthorizedUser.Email,
                SupervisorPhone = g.AuthorizedUser.Phone,
                g.creationDate,
                g.StudentsCount,
                eventsCount = _db.ActualAttendances.Select(a => new
                {
                    a.EventGroup.GroupId,
                }).Count(a => a.GroupId == g.GroupId),

                categoriesCount = _db.ActualAttendances.Select(c => new
                {
                    c.EventGroup.GroupId,
                    c.EventGroup.Event.CategoryId,
                }).Where(c => c.GroupId == g.GroupId).GroupBy(co => co.CategoryId).Count(),
                
                totalScore = _db.ActualAttendances.Where(c => c.EventGroup.GroupId == g.GroupId).Sum(ts => ts.TotalScore)
            })
            .OrderByDescending(g => g.GroupName);

            return Ok(arr);
        }

        [HttpGet("{GroupID}/statistic")]
        public async Task<ActionResult> GETGroupEventsOnlyStatistic(int GroupID)
        {
            var response = await _db.ActualAttendances
                .Where(a => a.EventGroup.GroupId == GroupID)
                .GroupBy(g => new
                {
                    g.EventGroup.GroupId,
                    g.EventGroup.Event.CategoryId,
                    CategoryName = g.EventGroup.Event.Category.Name,
                    g.EventGroup.Event.Title,
                    g.ActualListenersCount,
                    g.ActualParticipantsCount,
                    g.ActualSuperParticipantsCount,
                    g.TotalScore,
                })
                .Select(eg => new
                {
                    eg.Key.Title,
                    eg.Key.CategoryId,
                    eg.Key.CategoryName,
                    eg.Key.ActualListenersCount,
                    eg.Key.ActualParticipantsCount,
                    eg.Key.ActualSuperParticipantsCount,
                    eg.Key.TotalScore,
                })
                .ToListAsync();

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
                events = response
            });
        }
    }
}
