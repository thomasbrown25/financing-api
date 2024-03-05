using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using financing_api.Data;
using financing_api.Dtos.Account;
using financing_api.Utils;
using Going.Plaid;
using financing_api.PlaidInterface;
using financing_api.DbLogger;

namespace financing_api.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IPlaidApi _plaidApi;
        private readonly ILogging _logging;

        public AccountService(
            DataContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPlaidApi plaidApi,
            ILogging logging
        )
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _plaidApi = plaidApi;
            _logging = logging;
        }

        public async Task<ServiceResponse<GetAccountsDto>> GetAccountsBalance()
        {
            var response = new ServiceResponse<GetAccountsDto>();
            try
            {
                response.Data = new GetAccountsDto();

                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var result = await _plaidApi.GetAccountsRequest(user);

                decimal? cashAmount = 0;
                decimal? creditAmount = 0;
                decimal? loanAmount = 0;

                foreach (var account in result.Accounts)
                {
                    var accountDto = Helper.MapPlaidStream(new AccountDto(), account, user);

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
                response.Data = new GetAccountsDto();
                response.Data.Account = new AccountDto();

                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var dbAccount = await _context.Accounts
                                .Where(a => a.UserId == user.Id)
                                .Where(a => a.AccountId == accountId)
                                .FirstOrDefaultAsync();

                response.Data.Account = _mapper.Map<AccountDto>(dbAccount);
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
                response.Data = new GetAccountsDto();
                response.Data.Accounts = new List<AccountDto>();

                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var accountResponse = _plaidApi.GetAccountsRequest(user);

                foreach (var account in accountResponse.Result.Accounts)
                {
                    var dbAccount = await _context.Accounts
                       .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

                    if (dbAccount is null)
                    {
                        var accountDto = Helper.MapPlaidStream(new AccountDto(), account, user);

                        Account accountDb = _mapper.Map<Account>(accountDto);
                        _context.Accounts.Add(accountDb);
                    }
                    else
                    {
                        dbAccount.BalanceAvailable = account.Balances.Available;
                        dbAccount.BalanceCurrent = account.Balances.Current;
                        dbAccount.BalanceLimit = account.Balances.Limit;
                    }
                }

                await _context.SaveChangesAsync();

                var dbAccounts = await _context.Accounts
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

                response.Data.Accounts = dbAccounts.Select(a => _mapper.Map<AccountDto>(a)).ToList();

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
                response.Data = new GetAccountsDto();
                response.Data.Accounts = new List<AccountDto>();

                // Get user for accessToken
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var dbAccount = await _context.Accounts
                                .Where(a => a.UserId == user.Id)
                                .Where(a => a.AccountId == accountId)
                                .FirstOrDefaultAsync();

                _context.Accounts.Remove(dbAccount);

                await _context.SaveChangesAsync();

                var dbAccounts = await _context.Accounts
                    .Where(r => r.UserId == user.Id)
                    .ToListAsync();

                response.Data.Accounts = dbAccounts.Select(a => _mapper.Map<AccountDto>(a)).ToList();

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
