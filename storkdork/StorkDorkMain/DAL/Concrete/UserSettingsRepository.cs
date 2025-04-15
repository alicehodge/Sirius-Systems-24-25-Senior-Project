using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V133.SystemInfo;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly StorkDorkDbContext _storkDorkDbContext;

    public UserSettingsRepository(StorkDorkDbContext storkDorkDbContext)
    {
        _storkDorkDbContext = storkDorkDbContext;
    }

    public async Task<UserSettings?> UpdateAsync(UserSettings updatedSettings)
    {
        var existingSettings = await _storkDorkDbContext.UserSettings
            .FirstOrDefaultAsync(us => us.Id == updatedSettings.Id);

        if (existingSettings == null)
            return null;

        // Update only the necessary fields
        existingSettings.AnonymousSightings = updatedSettings.AnonymousSightings;
        // Add more settings here if needed later

        await _storkDorkDbContext.SaveChangesAsync();
        return existingSettings;
    }

    public async Task<UserSettings?> CreateAsync(UserSettings settings)
    {
        _storkDorkDbContext.UserSettings.Add(settings);
        await _storkDorkDbContext.SaveChangesAsync();
        return settings;
    }
    public async Task<UserSettings?> GetSettingsByUserIdAsync(int sdUserId)
    {
        return await _storkDorkDbContext.UserSettings
            .FirstOrDefaultAsync(us => us.SdUserId == sdUserId);
    }
}