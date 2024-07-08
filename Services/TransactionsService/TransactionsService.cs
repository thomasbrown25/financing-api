using Microsoft.EntityFrameworkCore;
using financing_api.Dtos.Transaction;
using financing_api.DbLogger;
using financing_api.DataAccess.TransactionDA;
using financing_api.DataAccess.UserDA;
using financing_api.DataAccess.PlaidDA;

namespace financing_api.Services.TransactionsService
{
    public class TransactionsService(
        ITransactionDataAccess transactionDataAccess,
        IUserDataAccess userDataAccess,
        IConfiguration configuration,
        IPlaidDataAccess plaidDataAccess,
        ILogging logging) : ITransactionsService
    {
        private readonly ITransactionDataAccess _transactionDataAccess = transactionDataAccess;
        private readonly IUserDataAccess _userDataAccess = userDataAccess;
        private readonly IPlaidDataAccess _plaidDataAccess = plaidDataAccess;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogging _logging = logging;

        public async Task<ServiceResponse<GetTransactionsDto>> GetTransactions()
        {
            var response = new ServiceResponse<GetTransactionsDto>
            {
                Data = new GetTransactionsDto()
            };

            try
            {
                var user = await _userDataAccess.GetCurrentUser();

                var result = await _plaidDataAccess.GetTransactionsRequest(user);

                if (result is null || result.Error is not null)
                {
                    throw new Exception(result?.Error?.ErrorMessage);
                }

                _transactionDataAccess.DeleteUserTransactions(user.Id);

                foreach (var transaction in result.Transactions)
                {
                    var transactionDto = await _transactionDataAccess.AddTransaction(transaction, user);

                    if (transaction.Category?[0] == "Payment")
                    {
                        response.Data.RecentBills.Add(transactionDto);
                    }
                }

                await _transactionDataAccess.SaveContextAsync();

                response.Data.Transactions = await _transactionDataAccess.GetTransactions(user);

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

        public async Task<ServiceResponse<GetRecurringDto>> GetRecurringTransactions()
        {
            var response = new ServiceResponse<GetRecurringDto>
            {
                Data = new GetRecurringDto()
            };

            try
            {
                var user = await _userDataAccess.GetCurrentUser();

                // The account info is needed in order to get the recurring transactions from Plaid
                var getAccountRequest = new Going.Plaid.Accounts.AccountsGetRequest()
                {
                    ClientId = _configuration["PlaidClientId"],
                    Secret = _configuration["PlaidSecret"],
                    AccessToken = user.AccessToken
                };

                var accountResponse = await _plaidDataAccess.GetAccountsRequest(user);

                if (accountResponse.Error is not null)
                {
                    Console.WriteLine(accountResponse.Error.ErrorMessage);
                    response.Success = false;
                    response.Message = accountResponse.Error.ErrorMessage;
                    return response;
                }

                var recurringResponse = _plaidDataAccess.GetRecurringTransactionsRequest(user, accountResponse);

                if (recurringResponse.Result.Error is not null)
                {
                    Console.WriteLine(recurringResponse.Result.Error.ErrorMessage);
                    response.Success = false;
                    response.Message = recurringResponse.Result.Error.ErrorMessage;
                    return response;
                }

                _transactionDataAccess.DeleteUserRecurringTransactions(user.Id);

                foreach (var transaction in recurringResponse.Result.OutflowStreams)
                {
                    if (!transaction.Category.Contains("Internal Account Transfer"))
                    {
                        _transactionDataAccess.AddRecurringTransaction(transaction, user);
                    }
                }
                await _transactionDataAccess.SaveContextAsync();

                response.Data.Expenses = await _transactionDataAccess.GetRecurrings(user);
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

        public async Task<ServiceResponse<GetTransactionsDto>> GetRecentBills()
        {
            var response = new ServiceResponse<GetTransactionsDto>
            {
                Data = new GetTransactionsDto()
            };

            try
            {
                // Get user for accessToken
                var user = await _userDataAccess.GetCurrentUser();

                var result = await _plaidDataAccess.GetTransactionsRequest(user);

                _transactionDataAccess.DeleteRecentBills(user);

                foreach (var transaction in result.Transactions)
                {
                    _transactionDataAccess.AddRecentBills(transaction, user);
                }

                await _transactionDataAccess.SaveContextAsync();

                var recurrings = await _transactionDataAccess.GetRecurrings(user, true, true);

                //response.Data.RecentBills = recurrings;
                //Helper.AddStreams(recurringResponse.Result.OutflowStreams, _context, _mapper, user, EType.Expense, dbExpenses);
                // }

                //response.Data.Transactions = _transactionDal.GetRecurringTransactions(user).Result;

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

        public async Task<ServiceResponse<GetTransactionsDto>> GetAccountTransactions(string accountId)
        {
            var response = new ServiceResponse<GetTransactionsDto>();

            try
            {
                response.Data = new GetTransactionsDto
                {
                    Transactions = []
                };

                // Get user for accessToken
                var user = await _userDataAccess.GetCurrentUser();

                var dbTransactions = await _transactionDataAccess.GetTransactions(user);

                response.Data.Transactions = await _transactionDataAccess.GetAccountTransactions(dbTransactions, accountId);

                var todayTransactions = await _transactionDataAccess.GetTodaysTransactions(dbTransactions, accountId);

                decimal totalAmount = 0;
                foreach (var transaction in todayTransactions)
                {
                    if (transaction.Amount > 0)
                    {
                        totalAmount += transaction.Amount;
                    }
                }
                response.Data.TodaySpendAmount = totalAmount;

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
