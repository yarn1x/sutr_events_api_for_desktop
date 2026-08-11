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
                u.AuthorizedUser.User.FirstName,
                u.AuthorizedUser.User.LastName,
                u.AuthorizedUser.User.MiddleName,
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
    }
}
