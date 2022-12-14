using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category? Category { get; set; }
        public string Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
