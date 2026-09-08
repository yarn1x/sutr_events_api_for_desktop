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


        [HttpGet]
        public ActionResult GETUserList()
        {
            var list = _db.AuthorizedUsers.Select(au => new {

                au.AuthorizedUserId,

                firstname = au.User.FirstName,
                surname = au.User.LastName,
                lastname = au.User.MiddleName,

                au.Email,
                au.Phone,
                
                roles = au.UserUsertypes
                .Where(ut => ut.AuthorizedUserId == au.AuthorizedUserId)
                .Select(ut => new {
                    ut.UserUsertypeId,
                    ut.UserTypeId,
                    ut.UserType.TypeName,
                }),

            
            }).ToList();

            return Ok(list);
        }

        [HttpGet("{userId}")]
        public ActionResult GetUser(int userId)
        {
            var user = _db.AuthorizedUsers
            .FirstOrDefault(u => u.AuthorizedUserId == userId);
            return Ok(user);
        }


        [HttpGet("roles")]
        public ActionResult GETRolesList()
        {
            var arr = _db.UserTypes.Select(t => new
            {
                t.UserTypeId,
                t.TypeName
            }).ToList();
            return Ok(arr);
        }


        [HttpPut("{userId}/roles")]
        public ActionResult? PUTGrantRoles([FromBody] List<UserUsertype> body)
        {
            //var existingRolesIds = _db.UserUsertypes.Where(UserUsertype => UserUsertype.AuthorizedUserId == body.);
            return null;
        }
    }
}
