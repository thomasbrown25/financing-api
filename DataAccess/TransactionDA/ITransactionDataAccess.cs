

using financing_api.Dtos.Transaction;
using financing_api.Dtos.User;

namespace financing_api.DataAccess.TransactionDA
{
    public interface ITransactionDataAccess
    {
        Task<List<TransactionDto>> GetTransactions(LoadUserDto user);
        void DeleteUserTransactions(int userId);
        void DeleteUserRecurringTransactions(int userId);
        Task<TransactionDto> AddTransaction(Going.Plaid.Entity.Transaction? transaction, LoadUserDto user);
        void AddRecurringTransaction(Going.Plaid.Entity.TransactionStream? transaction, LoadUserDto user);
        Task<bool> SaveContextAsync();
        Task<List<RecurringDto>> GetRecurrings(LoadUserDto user, bool isActive = true, bool ignoreActive = false);
        void AddRecentBills(Going.Plaid.Entity.Transaction? transaction, LoadUserDto user);
        void DeleteRecentBills(LoadUserDto user);
        Task<List<TransactionDto>> GetAccountTransactions(List<TransactionDto> dbTransactions, string accountId);
        Task<List<TransactionDto>> GetTodaysTransactions(List<TransactionDto> dbTransactions, string accountId);
    }
}