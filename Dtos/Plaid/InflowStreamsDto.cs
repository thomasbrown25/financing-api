using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Plaid
{
    public class InflowStreamsDto
    {
        public string AccountId { get; set; }
        public Going.Plaid.Entity.TransactionStreamAmount AverageAmount { get; set; }
        public IReadOnlyList<string>? Categories { get; set; }
        public string Description { get; set; }
        public DateTime FirstDate { get; set; }
        public string Frequency { get; set; }
        public bool IsActive { get; set; }
        public Going.Plaid.Entity.TransactionStreamAmount LastAmount { get; set; }
        public DateTime LastDate { get; set; }
        public string? MerchantName { get; set; }
        public Going.Plaid.Entity.TransactionStreamStatus Status { get; set; }
        public string StreamId { get; set; }
    }
}