namespace college_events_admin_API.Models.Data_transfer_objects
{
    public class LoginRequest
    {
        public required string login { get; set; }
        public required string passwordHash { get; set; }
    }
}
