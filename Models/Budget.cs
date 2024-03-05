using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Spend { get; set; }
        public string Percent { get; set; }
    }
}
