

using AutoMapper;
using financing_api.Data;
using financing_api.Dtos.Account;
using financing_api.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace financing_api.DataAccess.AccountDA
{
    public class AccountDataAccess(DataContext context, IMapper mapper) : IAccountDataAccess
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;


        public void AddAccount(Going.Plaid.Entity.Account? account, LoadUserDto user)
        {
            var accountDb = new Account
            {
                UserId = user.Id,
                AccountId = account.AccountId,
                Name = account.Name,
                Mask = account.Mask,
                OfficialName = account.OfficialName,
                Type = account.Subtype?.ToString(),
                BalanceCurrent = account.Balances.Current,
                BalanceAvailable = account.Balances.Available,
                BalanceLimit = account.Balances.Limit
            };

            _context.Accounts.Add(accountDb);
        }

        public async Task<AccountDto> GetAccount(LoadUserDto user, string accountId)
        {
            var accountDb = await _context.Accounts
                                .Where(a => a.UserId == user.Id)
                                .Where(a => a.AccountId == accountId)
                                .FirstOrDefaultAsync();

            return _mapper.Map<AccountDto>(accountDb);
        }

        public async Task<List<AccountDto>> GetAccounts(LoadUserDto user)
        {
            var dbAccounts = await _context.Accounts
                            .Where(a => a.UserId == user.Id)
                            .ToListAsync();

            return dbAccounts.Select(_mapper.Map<AccountDto>).ToList();
        }

        public async Task<AccountDto> GetAccountById(string accountId)
        {
            var accountDb = await _context.Accounts
                       .FirstOrDefaultAsync(a => a.AccountId == accountId);

            return _mapper.Map<AccountDto>(accountDb);
        }

        public void DeleteUserAccounts(User user)
        {
            _context.Accounts.RemoveRange(_context.Accounts.Where(x => x.UserId == user.Id));
        }

        public async Task<List<AccountDto>> DeleteAccount(LoadUserDto user, string accountId)
        {
            var dbAccount = await _context.Accounts
                                .Where(a => a.UserId == user.Id)
                                .Where(a => a.AccountId == accountId)
                                .FirstOrDefaultAsync();

            _context.Accounts.Remove(dbAccount);

            await _context.SaveChangesAsync();

            var dbAccounts = await _context.Accounts
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            return dbAccounts.Select(_mapper.Map<AccountDto>).ToList();
        }

        public async Task<bool> SaveContextAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}