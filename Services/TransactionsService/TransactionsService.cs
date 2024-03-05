using Microsoft.EntityFrameworkCore;
using financing_api.Data;
using financing_api.Dtos.Transaction;
using AutoMapper;
using financing_api.Utils;
using financing_api.PlaidInterface;
using financing_api.DAL;
using financing_api.DbLogger;
using Going.Plaid.Errors;

namespace financing_api.Services.TransactionsService
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IPlaidApi _plaidApi;
        private readonly TransactionDAL _transactionDal;
        private readonly ILogging _logging;

        public TransactionsService(
            DataContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPlaidApi plaidApi,
            TransactionDAL transactionDAL,
            ILogging logging
        )
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _plaidApi = plaidApi;
            _transactionDal = transactionDAL;
            _logging = logging;
        }

        public async Task<ServiceResponse<GetTransactionsDto>> GetTransactions()
        {
            var response = new ServiceResponse<GetTransactionsDto>
            {
                Data = new GetTransactionsDto()
            };

            try
            {
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var result = await _plaidApi.GetTransactionsRequest(user);

                if (result is null || result.Error is not null)
                {
                    throw new Exception(result?.Error.ErrorMessage);
                }

                _context.Transactions.RemoveRange(_context.Transactions.Where(x => x.UserId == user.Id));

                foreach (var transaction in result.Transactions)
                {
                    var transactionDto = Helper.MapPlaidStream(new TransactionDto(), transaction, user);

                    Transaction transactionDb = _mapper.Map<Transaction>(transactionDto);

                    _context.Transactions.Add(transactionDb);

                    if (transaction.Category?[0] == "Payment")
                    {
                        response.Data.RecentBills.Add(transactionDto);
                    }
                }

                await _context.SaveChangesAsync();

                var dbTransactions = _transactionDal.GetDbTransactions(user).Result;

                response.Data.Transactions = _transactionDal.GetTransactions(dbTransactions);

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
                // Get user for accessToken
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                // Recurring Transactions
                var getAccountRequest = new Going.Plaid.Accounts.AccountsGetRequest()
                {
                    ClientId = _configuration["PlaidClientId"],
                    Secret = _configuration["PlaidSecret"],
                    AccessToken = user.AccessToken
                };

                var accountResponse = await _plaidApi.GetAccountsRequest(user);

                if (accountResponse.Error is not null)
                {
                    Console.WriteLine(accountResponse.Error.ErrorMessage);
                    response.Success = false;
                    response.Message = accountResponse.Error.ErrorMessage;
                    return response;
                }

                var recurringResponse = _plaidApi.GetRecurringTransactionsRequest(user, accountResponse);

                if (recurringResponse.Result.Error is not null)
                {
                    if (recurringResponse.Result.Error.ErrorCode == Going.Plaid.Entity.ErrorCode.InvalidProduct)
                    {
                        var mockUser = _context.Users.FirstOrDefault(x => x.Id == 14);
                        var transactions = _transactionDal.GetRecurringTransactions(mockUser).Result;

                        Helper.GetMockRecurringData(ref transactions);

                        response.Data.Expenses = transactions.OrderBy(x => x.DueDate).ToList();
                        return response;
                    }

                    Console.WriteLine(recurringResponse.Result.Error.ErrorMessage);
                    response.Success = false;
                    response.Message = recurringResponse.Result.Error.ErrorMessage;
                    return response;
                }

                _context.Recurrings.RemoveRange(_context.Recurrings.Where(x => x.UserId == user.Id));

                foreach (var outflowStream in recurringResponse.Result.OutflowStreams)
                {
                    if (!outflowStream.Category.Contains("Internal Account Transfer"))
                    {
                        var recurring = Helper.MapPlaidStream(new RecurringDto(), outflowStream, user, EType.Expense);

                        // Map recurring with recurringDto db
                        Recurring recurringDb = _mapper.Map<Recurring>(recurring);
                        _context.Recurrings.Add(recurringDb);
                        await _context.SaveChangesAsync();
                    }
                }

                response.Data.Expenses = _transactionDal.GetExpenses(user).Result;
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
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                // var dateCompare = DateTime.Compare(user.LastUpdated, DateTime.Now.AddMinutes(-30));

                //if (dateCompare < 0)
                // {
                var result = await _plaidApi.GetTransactionsRequest(user);

                _context.RecentBills.RemoveRange(_context.RecentBills.Where(x => x.UserId == user.Id));

                foreach (var transaction in result.Transactions)
                {
                    var transactionDto = Helper.MapPlaidStream(new TransactionDto(), transaction, user);

                    RecentBill recentBillDb = _mapper.Map<RecentBill>(transactionDto);

                    _context.RecentBills.Add(recentBillDb);
                }

                var dbExpenses = _transactionDal.GetExpenses(user, true, true).Result;
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
                    Transactions = new List<TransactionDto>()
                };

                // Get user for accessToken
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var dbTransactions = _transactionDal.GetDbTransactions(user).Result;

                response.Data.Transactions = _transactionDal.GetAccountTransactions(dbTransactions, accountId);

                var todayTransactions = _transactionDal.GetTodaysTransactions(dbTransactions, accountId);

                decimal totalAmount = 0;
                foreach (var transaction in todayTransactions)
                {
                    if (transaction.Amount > 0)
                    {
                        totalAmount = totalAmount + transaction.Amount;
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
