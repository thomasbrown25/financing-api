using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class LoggingTrace
    {
        public int Id { get; set; }
        public DateTime LogDate { get; set; } = DateTime.Now.ToLocalTime();
        public string? Message { get; set; }
    }
}