using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Account;

namespace financing_api.Dtos.Transaction
{
    public class GetTransactionsDto
    {
        public int Id { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        public List<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
        public List<TransactionDto> RecentBills { get; set; } = new List<TransactionDto>();
        public List<TransactionDto> Expenses { get; set; } = new List<TransactionDto>();
        public List<TransactionDto> Incomes { get; set; } = new List<TransactionDto>();
        public Dictionary<string, decimal> Categories { get; set; }
        public HashSet<string> CategoryLabels { get; set; }
        public HashSet<decimal> CategoryAmounts { get; set; }
        public decimal CurrentSpendAmount { get; set; }
        public decimal TodaySpendAmount { get; set; }
    }
}
