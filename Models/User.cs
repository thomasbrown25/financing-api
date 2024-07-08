

namespace financing_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public int SettingId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
        public string? AccessToken { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}