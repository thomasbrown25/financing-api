using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public int UserId { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string MerchantName { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public bool Pending { get; set; }
        public string Date { get; set; }
    }
}
