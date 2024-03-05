using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.User;
using financing_api.Dtos.UserSetting;

namespace financing_api.Data
{
    public interface IUserService
    {
        Task<ServiceResponse<LoadUserDto>> Register(User user, string password);
        Task<ServiceResponse<LoadUserDto>> Login(string email, string password);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<LoadUserDto>> LoadUser();
        Task<ServiceResponse<string>> DeleteUser(int userId);
        Task<ServiceResponse<SettingsDto>> GetSettings();
        Task<ServiceResponse<SettingsDto>> SaveSettings(SettingsDto newSettings);
    }
}