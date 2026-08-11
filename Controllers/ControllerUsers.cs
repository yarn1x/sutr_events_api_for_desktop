using college_events_admin_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace college_events_admin_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("college/admin/users")]
    public class ControllerUsers(SutrEventsDbContext db) : Controller
    {
        SutrEventsDbContext _db = db;

        [HttpPost("roles")]
        public ActionResult GETRolesList()
        {
            var arr = _db.UserTypes.Select(t => new
            {
                roleId = t.UserTypeId,
                roleName = t.TypeName
            }).ToList();
            return Ok(arr);
        }
    }
}
