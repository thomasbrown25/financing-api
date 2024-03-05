using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Account;

namespace financing_api.Services.AccountService
{
    public static class Helper
    {
        public static AccountDto MapPlaidStream(AccountDto accountDto, Going.Plaid.Entity.Account account, User user)
        {
            accountDto.UserId = user.Id;
            accountDto.AccountId = account.AccountId;
            accountDto.Name = account.Name;
            accountDto.Mask = account.Mask;
            accountDto.OfficialName = account.OfficialName;
            accountDto.Type = account.Subtype?.ToString();
            accountDto.Subtype = account.Subtype?.ToString();
            accountDto.BalanceCurrent = account.Balances.Current;
            accountDto.BalanceAvailable = account.Balances.Available;
            accountDto.BalanceLimit = account.Balances.Limit;

            return accountDto;
        }

        public static void SetAccountTotals(ref ServiceResponse<GetAccountsDto>? response)
        {
            decimal? cashAmount = 0;
            decimal? creditAmount = 0;
            decimal? loanAmount = 0;
            response.Data.CreditAccounts = new List<AccountDto>();
            response.Data.CashAccounts = new List<AccountDto>();
            response.Data.LoanAccounts = new List<AccountDto>();

            foreach (var account in response.Data.Accounts)
            {
                if (account.Subtype.ToLower().Contains("credit"))
                {
                    creditAmount = creditAmount + account.BalanceCurrent;
                    response.Data.CreditAccounts.Add(account);
                }
                else if (account.Type.ToLower().Contains("loan"))
                {
                    loanAmount = loanAmount + account.BalanceCurrent;
                    response.Data.LoanAccounts.Add(account);
                }
                else
                {
                    cashAmount = cashAmount + account.BalanceAvailable;
                    response.Data.CashAccounts.Add(account);
                }
            }
            response.Data.CashAmount = cashAmount;
            response.Data.CreditAmount = creditAmount;
            response.Data.LoanAmount = loanAmount;
        }
    }
}