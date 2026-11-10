using college_events_admin_API.Models;
using college_events_admin_API.Models.Data_transfer_objects;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace college_events_admin_API.Controllers
{
    [ApiController]
    [Route("/college/admin/auth")]
    public class ControllerAuthorization(SutrEventsDbContext db, AuthorizationService service, HashService hashService) : Controller
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly AuthorizationService _service = service;
        private readonly HashService _hashService = hashService;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest user)
        {
            try
            {
                var db_response = _db.UserUsertypes
                    .Include(u => u.AuthorizedUser.User)
                    .FirstOrDefault(u =>
                        (u.UserTypeId == 1 || u.UserTypeId == 4)
                        && EF.Functions.Collate(u.AuthorizedUser.Login, "Latin1_General_CS_AS") == user.login
                    );

                if (db_response == null)
                {
                    return Unauthorized(new { message = "Неверный логин или пароль" });
                }


                if (!_hashService.VerifyPassword(user.password, db_response.AuthorizedUser.PasswordHash))
                {
                    return Unauthorized(new { message = "Неверный логин или пароль" });
                }

                var access = _service.GenerateJwtToken(user);
                var refresh = _service.GenerateRefreshToken(user.login);

                return Ok(new
                {
                    //НЕ МЕНЯТЬ ИМЕНА ПОЛЕЙ. ЧЕРЕВАТО НЕИЗВЕСТНЫМИ ОШИБКАМИ
                    //Во всей системе, на стороне настольного приложения
                    //firstname - имя
                    //surname - фамилия
                    //lastname - отчество
                    FirstName = db_response.AuthorizedUser.User.FirstName,
                    SurName = db_response.AuthorizedUser.User.LastName,
                    LastName = db_response.AuthorizedUser.User.MiddleName,
                    db_response.UserTypeId,

                    AccessToken = access.SerializedJwtToken,
                    RefreshToken = refresh.SerializedJwtToken,
                    AccessTokenExpiresInUTC = access.ExpiresIn,
                    RefreshTokenExpiresInUTC = refresh.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                var principal = _service.ValidateRefreshToken(request.RefreshToken);
                if (principal == null) return Unauthorized("Invalid refresh token");

                var login = principal.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(login)) return Unauthorized("Invalid refresh token");

                // опционально: можно перепроверить, что пользователь ещё существует/активен
                var exists = _db.UserUsertypes.Any(u =>
                    (u.UserTypeId == 1 || u.UserTypeId == 4)
                    && EF.Functions.Collate(u.AuthorizedUser.Login, "Latin1_General_CS_AS") == login);

                if (!exists) return Unauthorized("User not found");

                var access = _service.GenerateJwtToken(new LoginRequest { login = login, password = "" });
                var refresh = _service.GenerateRefreshToken(login);

                return Ok(new AuthResponse
                {
                    AccessToken = access.SerializedJwtToken,
                    RefreshToken = refresh.SerializedJwtToken,
                    AccessTokenExpiresInUTC = access.ExpiresIn,
                    RefreshTokenExpiresInUTC = refresh.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
