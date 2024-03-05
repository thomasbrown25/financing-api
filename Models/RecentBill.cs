using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class RecentBill
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public int UserId { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string? MerchantName { get; set; }

        public string? Category { get; set; }
        public string Amount { get; set; }
        public bool Pending { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}