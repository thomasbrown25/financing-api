using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Plaid;

namespace financing_api.Services.PlaidService
{
    public interface IPlaidService
    {
        // Creates a link token in Plaid and returns the link token
        Task<ServiceResponse<string>> CreateLinkToken();

        // Update a link token in Plaid and returns the link token
        Task<ServiceResponse<string>> UpdateLinkToken();

        // Exchanges the plaid public token for the access token
        Task<ServiceResponse<string>> PublicTokenExchange(string linkToken);

        // Get Recurring Transactions
        //Task<ServiceResponse<GetRecurringDto>> GetRecurringTransactions();
    }
}
