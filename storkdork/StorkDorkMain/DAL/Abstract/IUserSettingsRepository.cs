using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Abstract;

public interface IUserSettingsRepository
{
    Task<UserSettings?> CreateAsync(UserSettings settings);
    Task<UserSettings?> UpdateAsync(UserSettings updatedSettings);
    Task<UserSettings?> GetSettingsByUserIdAsync(int sdUserId);
}