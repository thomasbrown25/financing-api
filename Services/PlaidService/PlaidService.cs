using financing_api.DbLogger;
using financing_api.DataAccess.UserDA;
using financing_api.DataAccess.PlaidDA;

namespace financing_api.Services.PlaidService
{
    public class PlaidService(
        IUserDataAccess userDataAccess,
        IPlaidDataAccess plaidDataAccess,
        ILogging logging
        ) : IPlaidService
    {
        private readonly IUserDataAccess _userDataAccess = userDataAccess;
        private readonly IPlaidDataAccess _plaidDataAccess = plaidDataAccess;
        private readonly ILogging _logging = logging;

        public async Task<ServiceResponse<string>> CreateLinkToken()
        {
            ServiceResponse<string> response = new();

            try
            {
                // Get current user from sql db
                var user = await _userDataAccess.GetCurrentUser();

                var linkResponse = _plaidDataAccess.CreateLinkTokenRequest(user);

                if (linkResponse.Result.Error is not null)
                {
                    Console.WriteLine(linkResponse.Result.Error.ErrorMessage);
                    response.Success = false;
                    response.Error = new Error();
                    response.Error.ErrorCode = linkResponse.Result.Error.ErrorCode.ToString();
                    response.Error.ErrorMessage = linkResponse.Result.Error.ErrorMessage;
                    return response;
                }

                response.Data = linkResponse.Result.LinkToken;

                return response;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<ServiceResponse<string>> UpdateLinkToken()
        {
            ServiceResponse<string> response = new();

            try
            {
                // Get current user from sql db
                var user = await _userDataAccess.GetCurrentUser();

                var linkResponse = _plaidDataAccess.UpdateLinkTokenRequest(user);

                if (linkResponse.Result.Error is not null)
                {
                    Console.WriteLine(linkResponse.Result.Error.ErrorMessage);
                    response.Success = false;
                    response.Error = new Error
                    {
                        ErrorCode = linkResponse.Result.Error.ErrorCode.ToString(),
                        ErrorMessage = linkResponse.Result.Error.ErrorMessage
                    };
                    return response;
                }

                response.Data = linkResponse.Result.LinkToken;

                return response;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Exchange publicToken for accessToken
        public async Task<ServiceResponse<string>> PublicTokenExchange(string publicToken)
        {
            ServiceResponse<string> response = new();

            try
            {
                var exchangeResponse = _plaidDataAccess.PublicTokenExchangeRequest(publicToken);

                var user = await _userDataAccess.GetCurrentUser();

                user.AccessToken = exchangeResponse.Result.AccessToken;

                var loadedUser = await _userDataAccess.UpdateUser(user);

                response.Data = loadedUser.AccessToken;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }


            return response;
        }
    }
}
