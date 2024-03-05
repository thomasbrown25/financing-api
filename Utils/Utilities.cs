using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using financing_api.Data;

namespace financing_api.Utils
{
    public static class Utilities
    {
        public static User GetCurrentUser(DataContext _context, IHttpContextAccessor _httpContextAccessor)
        {
            try
            {
                string email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (email == null)
                    return null;

                // Get current user from sql db
                User user = _context.Users.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()));

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static int GetUserId(IHttpContextAccessor _httpContextAccessor)
        {
            try
            {
                return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public static string[] GetAccountIds(IReadOnlyList<Going.Plaid.Entity.Account> accounts)
        {
            string[] accountIds = new string[accounts.Count()];
            int i = 0;

            foreach (var account in accounts)
            {
                accountIds[i] = account.AccountId;
                i++;
            }

            return accountIds;
        }


        // public static T IsValid<T>(ServiceResponse<T> serviceResponse)
        // {
        //     if (plaidResponse.Result.Error is not null)
        //     {
        //         Console.WriteLine(plaidResponse.Result.Error.ErrorMessage);
        //         response.Success = false;
        //         response.Error = new Error();
        //         response.Error.ErrorCode = plaidResponse.Result.Error.ErrorCode.ToString();
        //         response.Error.ErrorMessage = plaidResponse.Result.Error.ErrorMessage;
        //         return response;
        //     }
        // }

    }
}
