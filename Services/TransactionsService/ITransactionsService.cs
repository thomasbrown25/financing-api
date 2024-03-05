using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Transaction;

namespace financing_api.Services.TransactionsService
{
    public interface ITransactionsService
    {
        Task<ServiceResponse<GetTransactionsDto>> GetTransactions();
        Task<ServiceResponse<GetRecurringDto>> GetRecurringTransactions();
        Task<ServiceResponse<GetTransactionsDto>> GetRecentBills();
        Task<ServiceResponse<GetTransactionsDto>> GetAccountTransactions(string accountId);
    }
}
