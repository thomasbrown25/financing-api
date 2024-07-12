using financing_api.Dtos.User;
using financing_api.Dtos.UserSetting;

namespace financing_api.DataAccess.UserDA
{
    public interface IUserDataAccess
    {
        Task<bool> UserExists(string email);
        Task<LoadUserDto?> SaveUser(RegisterUserDto user);
        Task<LoadUserDto?> UpdateUser(LoadUserDto user);
        Task<LoadUserDto> GetCurrentUser();
        Task<RegisterUserDto> GetUser(string email);
        Task<User> GetUser(int id);
        Task<LoadUserDto> ValidateUser(RegisterUserDto user, string password);
        void DeleteUser(User user);
        void SaveContextAsync();
        Task<SettingsDto> AddUserSettings(int userId);
        Task<SettingsDto> UpdateUserSettings(SettingsDto settingsDto);
        Task<SettingsDto> GetUserSettings(int userId);
    }
}