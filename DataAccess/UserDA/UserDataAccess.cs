
using System.Security.Claims;
using AutoMapper;
using financing_api.Data;
using financing_api.DbLogger;
using financing_api.Dtos.User;
using financing_api.Dtos.UserSetting;
using Microsoft.EntityFrameworkCore;

namespace financing_api.DataAccess.UserDA
{
    public class UserDataAccess(DataContext context, IHttpContextAccessor httpContextAccessor, ILogging logging, IMapper mapper) : IUserDataAccess
    {
        private readonly DataContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogging _logging = logging;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> UserExists(string email)
        {
            try
            {
                if (await _context.Users.AnyAsync(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
            }

            return false;
        }

        public async Task<LoadUserDto?> SaveUser(RegisterUserDto user)
        {
            try
            {
                var dbUser = _mapper.Map<User>(user);

                _context.Users.Add(dbUser);

                await _context.SaveChangesAsync();

                return _mapper.Map<LoadUserDto>(user);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public async Task<LoadUserDto?> UpdateUser(LoadUserDto user)
        {
            try
            {
                var dbUser = _context.Users.FirstOrDefault(x => x.Email == user.Email);

                _context.Entry(dbUser).CurrentValues.SetValues(user);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public async Task<LoadUserDto> GetCurrentUser()
        {
            try
            {
                string email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (email == null)
                    return null;

                // Get current user from sql db
                var user = await _context.Users.FirstOrDefaultAsync(
                    u => u.Email.ToLower().Equals(email.ToLower())
                );

                if (user is null)
                    return null;

                return _mapper.Map<LoadUserDto>(user);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<RegisterUserDto> GetUser(string email)
        {
            try
            {
                var dbUser = await _context.Users.FirstOrDefaultAsync(
                    u => u.Email.ToLower().Equals(email.ToLower())
                );

                return _mapper.Map<RegisterUserDto>(dbUser);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public async Task<User> GetUser(int id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public async Task<LoadUserDto> ValidateUser(RegisterUserDto user, string password)
        {
            try
            {
                if (user is null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;

                var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

                return _mapper.Map<LoadUserDto>(dbUser);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                return null;
            }
        }

        public async void DeleteUser(User user)
        {
            try
            {
                _context.Remove(user);
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
            }
        }

        public async void SaveContextAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<SettingsDto> AddUserSettings(int userId)
        {
            var dbSettings = new UserSettings
            {
                UserId = userId
            };

            _context.UserSettings.Add(dbSettings);
            await _context.SaveChangesAsync();

            dbSettings = await _context.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);

            return _mapper.Map<SettingsDto>(dbSettings);
        }

        public async Task<SettingsDto> UpdateUserSettings(SettingsDto settingsDto)
        {
            var dbSettings = await _context.UserSettings.FirstOrDefaultAsync(x => x.UserId == settingsDto.UserId);

            _context.Entry(dbSettings).CurrentValues.SetValues(settingsDto);

            await _context.SaveChangesAsync();

            return settingsDto;
        }

        public async Task<SettingsDto> GetUserSettings(int userId)
        {
            var dbSettings = await _context.UserSettings
                   .FirstOrDefaultAsync(s => s.UserId == userId);

            return _mapper.Map<SettingsDto>(dbSettings);
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

    }
}