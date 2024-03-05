using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.PlaidInterface
{
    public interface IPlaidApi
    {
        Task<Going.Plaid.Accounts.AccountsGetResponse> GetAccountsRequest(User user);
        Task<Going.Plaid.Link.LinkTokenCreateResponse> CreateLinkTokenRequest(User user);
        Task<Going.Plaid.Link.LinkTokenCreateResponse> UpdateLinkTokenRequest(User user);
        Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> PublicTokenExchangeRequest(string publicToken);
        Task<Going.Plaid.Transactions.TransactionsGetResponse> GetTransactionsRequest(User user);
        Task<Going.Plaid.Transactions.TransactionsRecurringGetResponse> GetRecurringTransactionsRequest(User user, Going.Plaid.Accounts.AccountsGetResponse accountResponse);
    }
}