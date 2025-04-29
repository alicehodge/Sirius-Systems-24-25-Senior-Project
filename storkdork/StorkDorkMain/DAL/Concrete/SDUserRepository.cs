using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete
{
    public class SDUserRepository : ISDUserRepository
    {
        private readonly StorkDorkDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SDUserRepository(StorkDorkDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<SdUser>> GetSdUsers()
        {
            return await _context.SdUsers
                .ToListAsync();
        }

        public async Task<SdUser?> GetSDUserByIdentity(ClaimsPrincipal user)
        {
            var identityId = _userManager.GetUserId(user);

            if (identityId == null || identityId == "")
                return null;

            return await _context.SdUsers
                .FirstOrDefaultAsync(s => s.AspNetIdentityId == identityId);
        }
        
        public async Task<SdUser?> GetSDUserById(int id)
        {
            return await _context.SdUsers
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<SdUser?> GetSDUserByIdentityId(string identityId)
        {
            if (string.IsNullOrEmpty(identityId))
                return null;
                
            return await _context.SdUsers
                .FirstOrDefaultAsync(s => s.AspNetIdentityId == identityId);
        }

        public async Task UpdateAsync(SdUser user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}