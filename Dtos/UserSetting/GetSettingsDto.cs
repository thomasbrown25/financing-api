using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.UserSetting
{
    public class GetSettingsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long? FontSize { get; set; }
        public string? Language { get; set; }
        public string? Messages { get; set; }
        public bool DarkMode { get; set; }
        public bool SidenavMini { get; set; }
        public bool NavbarFixed { get; set; }
        public string? SidenavType { get; set; }
    }
}