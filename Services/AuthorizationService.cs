using college_events_admin_API.Controllers;
using college_events_admin_API.Models.Data_transfer_objects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace college_events_admin_API.Services
{
    public class AuthorizationService(IOptions<JwtOptions> options)
    {
        private readonly JwtOptions _jwt = options.Value;

        public (string SerializedJwtToken, DateTime ExpiresIn) GenerateJwtToken(LoginRequest user)
        {
            var expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenLifetimeMinutes);

            var token = BuildToken(
                secret: _jwt.Secret,
                claims: new[]
                {
                new Claim(ClaimTypes.Name, user.login),
                new Claim("type", "access")
                },
                expires: expires);

            return (token, expires);
        }

        public (string SerializedJwtToken, DateTime ExpiresIn) GenerateRefreshToken(string login)
        {
            var expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenLifetimeDays);

            var token = BuildToken(
                secret: _jwt.RefreshSecret,
                claims: new[]
                {
                new Claim(ClaimTypes.Name, login),
                new Claim("type", "refresh")
                },
                expires: expires);

            return (token, expires);
        }

        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.RefreshSecret));

            try
            {
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = _jwt.Audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ClockSkew = TimeSpan.Zero
                }, out _);

                if (principal.FindFirst("type")?.Value != "refresh") return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private string BuildToken(string secret, IEnumerable<Claim> claims, DateTime expires)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
