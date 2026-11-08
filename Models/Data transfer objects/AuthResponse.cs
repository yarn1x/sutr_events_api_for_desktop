namespace college_events_admin_API.Models.Data_transfer_objects
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime AccessTokenExpiresInUTC { get; set; }
        public DateTime RefreshTokenExpiresInUTC { get; set; }
    }
}
