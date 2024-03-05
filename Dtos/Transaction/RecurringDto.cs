using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Transaction
{
    public class RecurringDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StreamId { get; set; }
        public string AccountId { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public IReadOnlyList<string>? Categories { get; set; }
        public string Description { get; set; }
        public string? MerchantName { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Frequency { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public bool InternalTransfer { get; set; } = false;
    }

    public enum EType
    {
        Income,
        Expense
    }
}