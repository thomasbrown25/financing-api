using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public AccountType Type { get; set; }
    }
}
