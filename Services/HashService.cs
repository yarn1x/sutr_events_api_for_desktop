using BCrypt.Net;

namespace college_events_admin_API.Services
{
    public class HashService
    {
        private const int WorkFactor = 11;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            catch (SaltParseException)
            {
                return false;
            }
        }
    }
}
