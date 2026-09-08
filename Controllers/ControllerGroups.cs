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
                SupervisorName = g.AuthorizedUser.User.FirstName,
                SupervisorSurname = g.AuthorizedUser.User.LastName,
                SupervisorLastname = g.AuthorizedUser.User.MiddleName,
                SupervisorEmail = g.AuthorizedUser.Email,
                SupervisorPhone = g.AuthorizedUser.Phone,
                g.creationDate,
                eventsCount = _db.ActualAttendances.Select(a => new
                {
                    a.EventGroup.GroupId,
                }).Count(a => a.GroupId == g.GroupId),
                categoriesCount = _db.ActualAttendances.Select(c => new
                {
                    c.EventGroup.GroupId,
                    c.EventGroup.Event.CategoryId,
                }).Where(c => c.GroupId == g.GroupId).GroupBy(co => co.CategoryId).Count()
            })
            .OrderByDescending(g => g.GroupName);

            return Ok(arr);
        }

        [HttpGet("{GroupID}/statistic")]
        public ActionResult GETGroupEventsOnlyStatistic(int GroupID)
        {
            var arr = _db.ActualAttendances
                .Where(a => a.EventGroup.GroupId == GroupID)
                .Select(eg => new
                {
                    eg.EventGroup.EventId,
                    eg.EventGroup.Event.Title,
                    eg.EventGroup.Group.Name,
                    eg.ActualListenersCount,
                    eg.ActualParticipantsCount,
                    eg.ActualSuperParticipantsCount,
                    SupervisorFirstname = eg.EventGroup.Group.AuthorizedUser.User.FirstName,
                    SupervisorSurname = eg.EventGroup.Group.AuthorizedUser.User.LastName,
                    SupervisorLastname = eg.EventGroup.Group.AuthorizedUser.User.MiddleName,
                });


            return Ok(arr);
        }
    }
}
