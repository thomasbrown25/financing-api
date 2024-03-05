using Going.Plaid;
using financing_api.Shared;
using Microsoft.Extensions.Options;
using Going.Plaid.Entity;
using financing_api.Utils;
using financing_api.DbLogger;

namespace financing_api.PlaidInterface
{
    public class PlaidApi : IPlaidApi
    {
        private readonly IConfiguration _configuration;
        private readonly PlaidCredentials _credentials;
        private readonly PlaidClient _client;
        private readonly ILogging _logging;

        public PlaidApi(
            IConfiguration configuration,
            IOptions<PlaidCredentials> credentials,
            PlaidClient client,
            ILogging logging
        )
        {
            _configuration = configuration;
            _credentials = credentials.Value;
            _client = client;
            _client.AccessToken = _credentials.AccessToken;
            _logging = logging;
        }

        public async Task<Going.Plaid.Link.LinkTokenCreateResponse> CreateLinkTokenRequest(User user)
        {
            var plaidUser = new LinkTokenCreateRequestUser()
            {
                ClientUserId = user.Id.ToString()
            };

            var request = new Going.Plaid.Link.LinkTokenCreateRequest()
            {
                ClientId = _configuration["PlaidClientId"],
                Secret = _configuration["PlaidSecret"],
                ClientName = "Financing Api",
                Language = Language.English,
                CountryCodes = new CountryCode[] { CountryCode.Us },
                User = plaidUser,
                Products = new Products[] { Products.Transactions, Products.Liabilities }

            };

            _logging.LogDataExchange("FinanceApp", "Plaid", "CreateLinkToken", Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var response = await _client.LinkTokenCreateAsync(request);

            _logging.LogDataExchange("Plaid", "FinanceApp", "CreateLinkToken", Newtonsoft.Json.JsonConvert.SerializeObject(response));

            return response;
        }

        public async Task<Going.Plaid.Link.LinkTokenCreateResponse> UpdateLinkTokenRequest(User user)
        {
            var plaidUser = new LinkTokenCreateRequestUser()
            {
                ClientUserId = user.Id.ToString()
            };

            var request = new Going.Plaid.Link.LinkTokenCreateRequest()
            {
                ClientId = _configuration["PlaidClientId"],
                Secret = _configuration["PlaidSecret"],
                AccessToken = user.AccessToken,
                ClientName = "Financing Api",
                Language = Language.English,
                CountryCodes = new CountryCode[] { CountryCode.Us },
                User = plaidUser,
            };

            _logging.LogDataExchange("FinanceApp", "Plaid", "UpdateLinkToken", Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var response = await _client.LinkTokenCreateAsync(request);

            _logging.LogDataExchange("Plaid", "FinanceApp", "UpdateLinkToken", Newtonsoft.Json.JsonConvert.SerializeObject(response));

            return response;
        }

        public async Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> PublicTokenExchangeRequest(string publicToken)
        {
            var response = await _client.ItemPublicTokenExchangeAsync(
                new()
                {
                    ClientId = _configuration["PlaidClientId"],
                    Secret = _configuration["PlaidSecret"],
                    PublicToken = publicToken
                }
            );

            return response;
        }

        public async Task<Going.Plaid.Accounts.AccountsGetResponse> GetAccountsRequest(User user)
        {
            var request = new Going.Plaid.Accounts.AccountsBalanceGetRequest()
            {
                ClientId = _configuration["PlaidClientId"],
                Secret = _configuration["PlaidSecret"],
                AccessToken = user.AccessToken,
            };

            _logging.LogDataExchange("FinanceApp", "Plaid", "GetAccounts", Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var response = await _client.AccountsBalanceGetAsync(request);

            _logging.LogDataExchange("Plaid", "FinanceApp", "GetAccounts", Newtonsoft.Json.JsonConvert.SerializeObject(response));

            return response;
        }

        public async Task<Going.Plaid.Transactions.TransactionsGetResponse> GetTransactionsRequest(User user)
        {
            var startDate = DateTime.Today.AddMonths(-4);

            var request = new Going.Plaid.Transactions.TransactionsGetRequest()
            {
                Options = new TransactionsGetRequestOptions()
                {
                    Count = 500
                },
                ClientId = _configuration["PlaidClientId"],
                Secret = _configuration["PlaidSecret"],
                AccessToken = user.AccessToken,
                StartDate = new DateOnly(startDate.Year, startDate.Month, startDate.Day),
                EndDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)
            };

            _logging.LogDataExchange("FinanceApp", "Plaid", "GetTransactions", Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var response = await _client.TransactionsGetAsync(request);

            _logging.LogDataExchange("Plaid", "FinanceApp", "GetTransactions", Newtonsoft.Json.JsonConvert.SerializeObject(response));


            return response;
        }

        public async Task<Going.Plaid.Transactions.TransactionsRecurringGetResponse> GetRecurringTransactionsRequest(User user, Going.Plaid.Accounts.AccountsGetResponse accountResponse)
        {
            var startDate = DateTime.Today.AddMonths(-4);

            var request = new Going.Plaid.Transactions.TransactionsRecurringGetRequest()
            {
                ClientId = _configuration["PlaidClientId"],
                Secret = _configuration["PlaidSecret"],
                AccessToken = user.AccessToken,
                AccountIds = Utilities.GetAccountIds(accountResponse.Accounts),
            };

            _logging.LogDataExchange("FinanceApp", "Plaid", "GetRecurringTransactions", Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var response = await _client.TransactionsRecurringGetAsync(request);

            _logging.LogDataExchange("Plaid", "FinanceApp", "GetRecurringTransactions", Newtonsoft.Json.JsonConvert.SerializeObject(response));

            return response;
        }
    }
}