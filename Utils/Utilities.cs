
using financing_api.Data;
using financing_api.DataAccess.UserDA;

namespace financing_api.Utils
{
    public static class Utilities
    {

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
