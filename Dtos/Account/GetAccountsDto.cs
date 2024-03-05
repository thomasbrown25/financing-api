using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Account
{
    public class GetAccountsDto
    {
        public int Id { get; set; }
        public List<AccountDto> Accounts { get; set; } = new List<AccountDto>();
        public AccountDto Account { get; set; } = new AccountDto();
        public List<AccountDto> CashAccounts { get; set; } = new List<AccountDto>();
        public List<AccountDto> CreditAccounts { get; set; } = new List<AccountDto>();
        public List<AccountDto> LoanAccounts { get; set; } = new List<AccountDto>();
        public decimal? CashAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? LoanAmount { get; set; }

    }
}
