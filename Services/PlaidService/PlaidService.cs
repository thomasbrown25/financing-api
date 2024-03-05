using financing_api.Data;
using financing_api.Utils;
using financing_api.PlaidInterface;
using financing_api.DbLogger;

namespace financing_api.Services.PlaidService
{
    public class PlaidService : IPlaidService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlaidApi _plaidApi;
        private readonly ILogging _logging;

        public PlaidService(
            DataContext context,
            IHttpContextAccessor httpContextAccessor,
            IPlaidApi plaidApi,
            ILogging logging
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _plaidApi = plaidApi;
            _logging = logging;
        }

        public async Task<ServiceResponse<string>> CreateLinkToken()
        {
            ServiceResponse<string> response = new ServiceResponse<string>();

            try
            {
                // Get current user from sql db
                User user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var linkResponse = _plaidApi.CreateLinkTokenRequest(user);

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
            ServiceResponse<string> response = new ServiceResponse<string>();

            try
            {
                // Get current user from sql db
                User user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var linkResponse = _plaidApi.UpdateLinkTokenRequest(user);

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

        public async Task<ServiceResponse<string>> PublicTokenExchange(string publicToken)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();

            try
            {
                // Exchange publicToken for accessToken
                var exchangeResponse = _plaidApi.PublicTokenExchangeRequest(publicToken);

                // Save accessToken to SQL DB
                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);
                user.AccessToken = exchangeResponse.Result.AccessToken;

                await _context.SaveChangesAsync();

                response.Data = exchangeResponse.Result.AccessToken;
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
