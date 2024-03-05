using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Account;

namespace financing_api.Services.AccountService
{
    public interface IAccountService
    {
        Task<ServiceResponse<GetAccountsDto>> GetAccountsBalance();
        Task<ServiceResponse<GetAccountsDto>> GetAccountBalance(string accountId);
        Task<ServiceResponse<GetAccountsDto>> RefreshAccountsBalance();
        Task<ServiceResponse<GetAccountsDto>> DeleteAccount(string accountId);
    }
}