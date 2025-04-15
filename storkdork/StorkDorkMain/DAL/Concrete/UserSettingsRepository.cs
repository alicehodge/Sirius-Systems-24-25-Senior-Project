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