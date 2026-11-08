namespace college_events_admin_API.Models.Data_transfer_objects
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Secret { get; set; } = null!;
        public string RefreshSecret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenLifetimeMinutes { get; set; } = 15;
        public int RefreshTokenLifetimeDays { get; set; } = 30;
    }
}
