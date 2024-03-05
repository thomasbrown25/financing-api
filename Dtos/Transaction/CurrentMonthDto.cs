using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Transaction
{
    public class CurrentMonthDto
    {
        public decimal Income { get; set; } = 0;
        public decimal Expense { get; set; } = 0;
    }
}