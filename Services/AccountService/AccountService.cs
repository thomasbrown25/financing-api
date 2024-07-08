using financing_api.Dtos.Account;
using financing_api.DbLogger;
using financing_api.DataAccess.UserDA;
using financing_api.DataAccess.PlaidDA;
using financing_api.DataAccess.AccountDA;

namespace financing_api.Services.AccountService
{
    public class AccountService(
        IUserDataAccess userDataAccess,
        IPlaidDataAccess plaidDataAccess,
        IAccountDataAccess accountDataAccess,
        ILogging logging
        ) : IAccountService
    {
        private readonly IUserDataAccess _userDataAccess = userDataAccess;
        private readonly IPlaidDataAccess _plaidDataAccess = plaidDataAccess;
        private readonly IAccountDataAccess _accountDataAccess = accountDataAccess;
        private readonly ILogging _logging = logging;

        public async Task<ServiceResponse<GetAccountsDto>> GetAccountsBalance()
        {
            var response = new ServiceResponse<GetAccountsDto>();
            try
            {
                response.Data = new GetAccountsDto();

                var user = await _userDataAccess.GetCurrentUser();

                var result = await _plaidDataAccess.GetAccountsRequest(user);

                decimal? cashAmount = 0;
                decimal? creditAmount = 0;
                decimal? loanAmount = 0;

                foreach (var account in result.Accounts)
                {
                    var accountDto = Helper.MapPlaidStream(new AccountDto(), account, user.Id);

                    switch (accountDto.Type)
                    {
                        case "Checking":
                        case "Savings":
                        case "MoneyMarket":
                            cashAmount += accountDto.BalanceAvailable;
                            response.Data.CashAccounts.Add(accountDto);
                            break;
                        case "CreditCard":
                            creditAmount += accountDto.BalanceCurrent;
                            response.Data.CreditAccounts.Add(accountDto);
                            break;

                        default:
                            loanAmount += accountDto.BalanceCurrent;
                            response.Data.LoanAccounts.Add(accountDto);
                            break;
                    }

                    response.Data.Accounts.Add(accountDto);
                }

                response.Data.CashAmount = cashAmount;
                response.Data.CreditAmount = creditAmount;
                response.Data.LoanAmount = loanAmount;

                return response;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ServiceResponse<GetAccountsDto>> GetAccountBalance(string accountId)
        {
            var response = new ServiceResponse<GetAccountsDto>();
            try
            {
                response.Data = new GetAccountsDto
                {
                    Account = new AccountDto()
                };

                var user = await _userDataAccess.GetCurrentUser();

                response.Data.Account = await _accountDataAccess.GetAccount(user, accountId);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

        public async Task<ServiceResponse<GetAccountsDto>> RefreshAccountsBalance()
        {
            var response = new ServiceResponse<GetAccountsDto>();
            try
            {
                response.Data = new GetAccountsDto
                {
                    Accounts = []
                };

                var user = await _userDataAccess.GetCurrentUser();

                var accountResponse = _plaidDataAccess.GetAccountsRequest(user);

                foreach (var account in accountResponse.Result.Accounts)
                {
                    var dbAccount = await _accountDataAccess.GetAccountById(account.AccountId);

                    if (dbAccount is null)
                    {
                        _accountDataAccess.AddAccount(account, user);
                    }
                    else
                    {
                        dbAccount.BalanceAvailable = account.Balances.Available;
                        dbAccount.BalanceCurrent = account.Balances.Current;
                        dbAccount.BalanceLimit = account.Balances.Limit;
                    }
                }

                await _accountDataAccess.SaveContextAsync();

                response.Data.Accounts = await _accountDataAccess.GetAccounts(user);

                Helper.SetAccountTotals(ref response);

            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                response.InnerException = ex.InnerException.Message;
                return response;
            }

            return response;
        }

        public async Task<ServiceResponse<GetAccountsDto>> DeleteAccount(string accountId)
        {
            var response = new ServiceResponse<GetAccountsDto>();

            try
            {
                response.Data = new GetAccountsDto
                {
                    Accounts = []
                };

                // Get user for accessToken
                var user = await _userDataAccess.GetCurrentUser();

                response.Data.Accounts = await _accountDataAccess.DeleteAccount(user, accountId);

                Helper.SetAccountTotals(ref response);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

    }
}
