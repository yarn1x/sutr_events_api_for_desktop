using college_events_admin_API.Models;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace college_events_admin_API.Controllers
{
    [ApiController]
    [Route("/college/admin/auth")]
    public class ControllerAuthorization(SutrEventsDbContext db, AuthorizationService service, ILogger<ControllerAuthorization> logger) : Controller
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly AuthorizationService _service = service;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest user)
        {
            try
            {
                //Учётная запись должна иметь тип 1 или 4 (админ или супер-админ), логин и пароль должны совпадать с полученными данными из запроса
                var db_response = _db.UserUsertypes.FirstOrDefault(u =>
                    (u.UserTypeId == 1 || u.UserTypeId == 4) &&
                    EF.Functions.Collate(u.AuthorizedUser.Login, "Latin1_General_CS_AS") == user.login &&
                    EF.Functions.Collate(u.AuthorizedUser.PasswordHash, "Latin1_General_CS_AS") == user.passwordHash
                );

                //если учетка не найдена, возврат неверной авторизации
                if (db_response == null) return Unauthorized("Invalid credentials");
                var jwt = _service.GenerateJwtToken(user);
                return Ok(new { Token = jwt, ExpiresIn = 120 });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
    public class LoginRequest
    {
        public required string login { get; set; }
        public required string passwordHash { get; set; }
    }
}
