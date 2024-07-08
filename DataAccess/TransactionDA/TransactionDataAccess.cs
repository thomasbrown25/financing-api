
using AutoMapper;
using financing_api.Data;
using financing_api.DbLogger;
using financing_api.Dtos.Transaction;
using financing_api.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace financing_api.DataAccess.TransactionDA
{
    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly DataContext _context;
        private readonly ILogging _logging;
        private readonly IMapper _mapper;

        public TransactionDataAccess(DataContext context, ILogging logging, IMapper mapper)
        {
            _context = context;
            _logging = logging;
            _mapper = mapper;
        }

        public async Task<List<TransactionDto>> GetTransactions(LoadUserDto user)
        {
            try
            {
                var transactionsDb = await _context.Transactions
                        .Where(r => r.UserId == user.Id)
                        .OrderByDescending(r => r.Date)
                        .ToListAsync();

                return transactionsDb.Select(c => _mapper.Map<TransactionDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public void DeleteUserTransactions(int userId)
        {
            _context.Transactions.RemoveRange(_context.Transactions.Where(x => x.UserId == userId));
        }

        public void DeleteUserRecurringTransactions(int userId)
        {
            _context.Recurrings.RemoveRange(_context.Recurrings.Where(x => x.UserId == userId));
        }

        public async Task<TransactionDto> AddTransaction(Going.Plaid.Entity.Transaction? transaction, LoadUserDto user)
        {
            var transactionDb = new Transaction
            {
                UserId = user.Id,
                TransactionId = transaction.TransactionId,
                AccountId = transaction.AccountId,
                Name = transaction.Name,
                MerchantName = transaction.MerchantName,
                Amount = transaction.Amount > 0 ? transaction.Amount.ToString() : "0",
                Pending = transaction.Pending,
                Date = transaction.Date,
                Category = transaction.Category?.Count > 1 ? transaction.Category[1] : transaction.Category?[0]
            };

            _context.Transactions.Add(transactionDb);

            return _mapper.Map<TransactionDto>(transactionDb);
        }

        public async void AddRecurringTransaction(Going.Plaid.Entity.TransactionStream? transaction, LoadUserDto user)
        {
            var transactionDb = new Recurring
            {

                UserId = user.Id,
                StreamId = transaction.StreamId,
                AccountId = transaction.AccountId,
                Type = Enum.GetName(EType.Expense),
                Category = transaction.Category.Count > 1 ? transaction.Category[1] : transaction.Category[0],
                Description = transaction.Description,
                MerchantName = transaction.MerchantName,
                FirstDate = transaction.FirstDate.ToDateTime(TimeOnly.Parse("00:00:00")),
                LastDate = transaction.LastDate.ToDateTime(TimeOnly.Parse("00:00:00")),
                Amount = transaction.LastAmount.Amount,
                IsActive = transaction.IsActive,
                Status = transaction.Status.ToString()
            };


            _context.Recurrings.Add(transactionDb);
        }

        public async Task<bool> SaveContextAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RecurringDto>> GetRecurrings(LoadUserDto user, bool isActive = true, bool ignoreActive = false)
        {
            var dbRecurrings = new List<Recurring>();

            if (ignoreActive)
            {
                dbRecurrings = await _context.Recurrings
                                    .Where(x => x.UserId == user.Id)
                                    .ToListAsync();
            }
            else
            {
                dbRecurrings = await _context.Recurrings
                                    .Where(x => x.UserId == user.Id)
                                    .Where(x => x.IsActive == isActive)
                                    .ToListAsync();
            }

            return dbRecurrings
                    .Where(x => x.Type == Enum.GetName<EType>(EType.Expense))
                    .Where(x => x.IsActive == true)
                    .Select(_mapper.Map<RecurringDto>)
                    .OrderBy(x => x.DueDate)
                    .ToList();
        }

        public async void AddRecentBills(Going.Plaid.Entity.Transaction? transaction, LoadUserDto user)
        {
            var transactionDb = new RecentBill
            {
                UserId = user.Id,
                TransactionId = transaction.TransactionId,
                AccountId = transaction.AccountId,
                Name = transaction.Name,
                MerchantName = transaction.MerchantName,
                Amount = transaction.Amount > 0 ? transaction.Amount.ToString() : "0",
                Pending = transaction.Pending,
                Date = transaction.Date,
                Category = transaction.Category?.Count > 1 ? transaction.Category[1] : transaction.Category?[0]
            };


            _context.RecentBills.Add(transactionDb);
        }

        public async void DeleteRecentBills(LoadUserDto user)
        {
            _context.RecentBills.RemoveRange(_context.RecentBills.Where(x => x.UserId == user.Id));
        }

        public async Task<List<TransactionDto>> GetAccountTransactions(List<TransactionDto> dbTransactions, string accountId)
        {
            return dbTransactions
                    .Where(t => t.AccountId == accountId)
                    .Select(_mapper.Map<TransactionDto>)
                    .ToList();
        }

        public async Task<List<TransactionDto>> GetTodaysTransactions(List<TransactionDto> dbTransactions, string accountId)
        {
            return dbTransactions
                    .Where(t => t.AccountId == accountId)
                    .Where(t => t.Date.Equals(DateTime.Today))
                    .Select(_mapper.Map<TransactionDto>)
                    .ToList();
        }

    }
}