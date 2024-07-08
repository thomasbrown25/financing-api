

using financing_api.Dtos.Account;
using financing_api.Dtos.User;

namespace financing_api.DataAccess.AccountDA
{

    public interface IAccountDataAccess
    {
        void AddAccount(Going.Plaid.Entity.Account? account, LoadUserDto user);
        Task<AccountDto> GetAccount(LoadUserDto user, string accountId);
        Task<AccountDto> GetAccountById(string accountId);
        Task<List<AccountDto>> GetAccounts(LoadUserDto user);
        void DeleteUserAccounts(User user);
        Task<List<AccountDto>> DeleteAccount(LoadUserDto user, string accountId);
        Task<bool> SaveContextAsync();
    }
}