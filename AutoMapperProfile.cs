using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using financing_api.Dtos.User;
using financing_api.Dtos.Transaction;
using financing_api.Dtos.Account;
using financing_api.Dtos.Category;
using financing_api.Dtos.UserSetting;

namespace financing_api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, LoadUserDto>();
            CreateMap<AddRecurringDto, Recurring>();
            CreateMap<Recurring, RecurringDto>();
            CreateMap<RecurringDto, Recurring>();
            CreateMap<UpdateRecurringDto, Recurring>();
            CreateMap<Recurring, GetRecurringDto>();
            CreateMap<TransactionDto, Transaction>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionDto, RecentBill>();
            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<AddCategoryDto, Category>();
            CreateMap<Category, AddCategoryDto>();
            CreateMap<SettingsDto, UserSettings>();
            CreateMap<UserSettings, SettingsDto>();
        }
    }
}
