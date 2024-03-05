using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Transaction
{
    public class AddRecurringDto
    {
        public int UserId { get; set; }
        public string StreamId { get; set; }
        public string AccountId { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string? MerchantName { get; set; }
        public string FirstDate { get; set; }
        public string LastDate { get; set; }
        public string Frequency { get; set; }
        public decimal LastAmount { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }
}