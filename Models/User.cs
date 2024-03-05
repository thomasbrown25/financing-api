using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public int SettingId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? AccessToken { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}