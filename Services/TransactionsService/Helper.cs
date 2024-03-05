using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using financing_api.Data;
using financing_api.Dtos.Transaction;
using Going.Plaid.Entity;
using Microsoft.EntityFrameworkCore;

namespace financing_api.Services.TransactionsService
{
    public static class Helper
    {
        public static RecurringDto MapPlaidStream(RecurringDto recurring, TransactionStream stream, User user, EType type)
        {
            try
            {
                recurring.UserId = user.Id;
                recurring.StreamId = stream.StreamId;
                recurring.AccountId = stream.AccountId;
                recurring.Type = Enum.GetName(type);
                recurring.Category = stream.Category.Count > 1 ? stream.Category[1] : stream.Category[0];
                recurring.Description = stream.Description;
                recurring.MerchantName = stream.MerchantName;
                recurring.FirstDate = stream.FirstDate.ToDateTime(TimeOnly.Parse("00:00:00"));
                recurring.LastDate = stream.LastDate.ToDateTime(TimeOnly.Parse("00:00:00"));
                recurring.Frequency = stream.Frequency.ToString();
                recurring.Amount = type == EType.Income ? stream.LastAmount.Amount * -1 : stream.LastAmount.Amount;
                recurring.IsActive = stream.IsActive;
                recurring.Status = stream.Status.ToString();

                if (stream.Category.Count > 1 && stream.Category[1].ToLower() == "internal account transfer")
                {
                    recurring.InternalTransfer = true;
                }

                // Add days to the last date to have an accurate due date... :refactor later
                if (stream.Frequency.ToString().ToLower() == "monthly")
                {
                    recurring.DueDate = stream.LastDate.ToDateTime(TimeOnly.Parse("00:00:00")).AddMonths(1);
                }
                else if (stream.Frequency.ToString().ToLower() == "biweekly" || stream.Frequency.ToString().ToLower() == "semimonthly")
                {
                    recurring.DueDate = stream.LastDate.ToDateTime(TimeOnly.Parse("00:00:00")).AddDays(14);
                }
                else if (stream.Frequency.ToString().ToLower() == "weekly")
                {
                    recurring.DueDate = stream.LastDate.ToDateTime(TimeOnly.Parse("00:00:00")).AddDays(7);
                }
                else
                {
                    recurring.DueDate = stream.LastDate.ToDateTime(TimeOnly.Parse("00:00:00")).AddMonths(1);
                }

                return recurring;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static TransactionDto MapPlaidStream(TransactionDto transactionDto, Going.Plaid.Entity.Transaction transaction, User user)
        {
            try
            {
                transactionDto.UserId = user.Id;
                transactionDto.TransactionId = transaction.TransactionId;
                transactionDto.AccountId = transaction.AccountId;
                transactionDto.Name = transaction.Name;
                transactionDto.MerchantName = transaction.MerchantName;
                transactionDto.Amount = transaction.Amount;
                transactionDto.Pending = transaction.Pending;
                transactionDto.Date = transaction.Date.ToString();
                transactionDto.Category = transaction.Category.Count > 1 ? transaction.Category[1] : transaction.Category[0];

                return transactionDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async void AddStreams(IReadOnlyList<TransactionStream> inflowStreams, DataContext context, IMapper mapper, User user, EType expense, List<RecurringDto> dbExpenses)
        {
            foreach (var inflowStream in inflowStreams)
            {
                if (!inflowStream.Category.Contains("Internal Account Transfer"))
                {
                    var dbRecurring = await context.Recurrings
                        .FirstOrDefaultAsync(r => r.StreamId == inflowStream.StreamId);

                    if (dbRecurring is null)
                    {
                        var recurring = Helper.MapPlaidStream(new RecurringDto(), inflowStream, user, EType.Income);

                        // Map recurring with recurringDto db
                        Recurring recurringDb = mapper.Map<Recurring>(recurring);
                        context.Recurrings.Add(recurringDb);
                    }
                }
            }
        }

        public static decimal GetTithes(List<RecurringDto> incomes)
        {
            decimal tithes = 0;
            decimal totalIncome = 0;

            foreach (var income in incomes)
            {
                totalIncome = totalIncome + income.Amount;
            }

            tithes = totalIncome / 10;

            return tithes;
        }

        public static decimal GetTotalIncome(List<RecurringDto> incomes)
        {
            decimal totalIncome = 0;
            git
            foreach (var income in incomes)
            {
                totalIncome = totalIncome + income.Amount;
            }

            return totalIncome;
        }

        public static void GetMockRecurringData(ref List<RecurringDto> recurringsDto)
        {
            var count = 1;

            foreach (var transaction in recurringsDto)
            {
                transaction.DueDate = DateTime.Now.AddDays(count);

                if (count >= 60)
                    count = 1;

                if (count >= 40)
                    count = 2;

                if (count >= 24)
                    count = 7;

                if (count >= 20)
                    count = 6;

                if (count >= 12)
                    count = 5;

                if (count >= 8)
                    count = 3;

                count *= 2;
            }
        }
    }
}