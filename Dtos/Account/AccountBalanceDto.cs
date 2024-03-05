using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Account
{
    public class AccountBalanceDto
    {
        public int Id { get; set; }
        public decimal? Current { get; set; }
        public decimal? Available { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal? Limit { get; set; }
        public string? IsoCurrencyCode { get; set; }
    }
}
