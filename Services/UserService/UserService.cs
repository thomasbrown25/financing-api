using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using financing_api.Data;
using financing_api.DataAccess.AccountDA;
using financing_api.DataAccess.TransactionDA;
using financing_api.DataAccess.UserDA;
using financing_api.DbLogger;
using financing_api.Dtos.User;
using financing_api.Dtos.UserSetting;
using Microsoft.IdentityModel.Tokens;

namespace financing_api.Services.UserService
{
    public class UserService(
        IUserDataAccess userDataAccess,
        IConfiguration configuration,
        ILogging logging,
        ITransactionDataAccess transactionDataAccess,
        IAccountDataAccess accountDataAccess) : IUserService
    {
        private readonly IUserDataAccess _userDataAccess = userDataAccess;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogging _logging = logging;
        private readonly ITransactionDataAccess _transactionDataAccess = transactionDataAccess;
        private readonly IAccountDataAccess _accountDataAccess = accountDataAccess;

        public async Task<ServiceResponse<LoadUserDto>> Register(RegisterUserDto user)
        {
            ServiceResponse<LoadUserDto> response = new();

            try
            {
                if (await _userDataAccess.UserExists(user.Email))
                {
                    response.Message = "A user with that email already exists.";
                    response.Success = false;
                    _logging.LogTrace("Register user failed: A user with that email already exists.");
                    return response;
                }

                CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                var token = CreateToken(user);

                var loadedUser = await _userDataAccess.SaveUser(user);

                var settings = await _userDataAccess.AddUserSettings(loadedUser.Id);

                response.Data = new LoadUserDto();
                response.Data = loadedUser;
                response.Data.JWTToken = token;

            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<LoadUserDto>> Login(string email, string password)
        {
            ServiceResponse<LoadUserDto> response = new();

            try
            {
                var user = await _userDataAccess.GetUser(email);

                var validUser = await _userDataAccess.ValidateUser(user, password);

                if (validUser is null)
                {
                    response.Success = false;
                    response.Message = "Invalid email or password";
                    return response;
                }

                validUser.JWTToken = CreateToken(user);

                response.Data = validUser;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = "Sorry we ran into an issue. Please contact support for assistance.";
            }
            return response;
        }

        public async Task<ServiceResponse<LoadUserDto>> LoadUser()
        {
            ServiceResponse<LoadUserDto> response = new();

            try
            {
                LoadUserDto user = await _userDataAccess.GetCurrentUser();

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                response.Data = user;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteUser(int userId)
        {
            ServiceResponse<string> response = new();

            try
            {
                User user = await _userDataAccess.GetUser(userId);

                _transactionDataAccess.DeleteUserTransactions(user.Id);
                _transactionDataAccess.DeleteUserRecurringTransactions(user.Id);
                _accountDataAccess.DeleteUserAccounts(user);
                _userDataAccess.DeleteUser(user);
                _userDataAccess.SaveContextAsync();

                response.Data = "User Deleted: " + user.FirstName;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<SettingsDto>> GetSettings()
        {
            var response = new ServiceResponse<SettingsDto>
            {
                Data = new SettingsDto() { DarkMode = true, SidenavMini = false }
            };

            try
            {
                var user = await _userDataAccess.GetCurrentUser();

                SettingsDto? userSettings = new();

                if (user is not null)
                {
                    userSettings = await _userDataAccess.GetUserSettings(user.Id);

                    userSettings ??= await _userDataAccess.AddUserSettings(user.Id);

                    response.Data = userSettings;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        public async Task<ServiceResponse<SettingsDto>> SaveSettings(SettingsDto newSettings)
        {
            var response = new ServiceResponse<SettingsDto>();

            try
            {
                response.Data = new SettingsDto();

                var user = await _userDataAccess.GetCurrentUser();

                var settings = await _userDataAccess.UpdateUserSettings(newSettings);

                response.Data = settings;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        private void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt
        )
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(RegisterUserDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration["Key"]
                )
            );

            SigningCredentials creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha512Signature
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(_configuration["JWTTokenExpiration"])
                ),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
