using college_events_admin_API.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace college_events_admin_API.Services
{
    public class AuthorizationService
    {
        public string GenerateJwtToken(LoginRequest user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.login) };
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
    public class AuthOptions
    {
        public const string ISSUER = "ApiServer";
        public const string AUDIENCE = "AuthClient";
        const string KEY = "supersupersupersupersecretkey!123";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
