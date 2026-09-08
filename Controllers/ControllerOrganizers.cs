using college_events_admin_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                u.AuthorizedUser.User.FirstName,
                SurName = u.AuthorizedUser.User.LastName,
                LastName = u.AuthorizedUser.User.MiddleName,
                u.UserTypeId,
                u.UserType.TypeName,
                u.AuthorizedUser.Email,
                u.AuthorizedUser.Phone,
                eventsCount = _db.Events.Where(uid => uid.OrganizerId == u.AuthorizedUser.UserId).Count()
            })
            .Where(t => t.UserTypeId == 3)
            .ToList();

            return Ok(arr);
        }

        [HttpGet("{organizerId}/statistic")]
        public ActionResult GETOrganizerStatistic(int organizerId)
        {
            var arr = _db.ActualAttendances.Where(e => e.EventGroup.Event.OrganizerId == organizerId).Select(a => new
            {
                a.EventGroup.Event.Title,
                a.EventGroup.Event.StartDatetime,
                a.EventGroup.Event.EndDatetime,
                a.EventGroup.Event.FullDescription,
                a.ActualListenersCount,
                a.ActualParticipantsCount,
                a.ActualSuperParticipantsCount,
            });
            return Ok(arr);
        }
    }
}
