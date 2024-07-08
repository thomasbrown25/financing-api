
using financing_api.Dtos.User;

namespace financing_api.DataAccess.PlaidDA
{
    public interface IPlaidDataAccess
    {
        Task<Going.Plaid.Accounts.AccountsGetResponse> GetAccountsRequest(LoadUserDto user);
        Task<Going.Plaid.Link.LinkTokenCreateResponse> CreateLinkTokenRequest(LoadUserDto user);
        Task<Going.Plaid.Link.LinkTokenCreateResponse> UpdateLinkTokenRequest(LoadUserDto user);
        Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> PublicTokenExchangeRequest(string publicToken);
        Task<Going.Plaid.Transactions.TransactionsGetResponse> GetTransactionsRequest(LoadUserDto user);
        Task<Going.Plaid.Transactions.TransactionsRecurringGetResponse> GetRecurringTransactionsRequest(LoadUserDto user, Going.Plaid.Accounts.AccountsGetResponse accountResponse);

    }
}