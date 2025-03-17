using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete;

public class MilestoneRepository : Repository<Milestone>, IMilestoneRepository
{
    private DbSet<Milestone> _milestones;
    private readonly StorkDorkContext _context;

    public const int GoldTier = 1;
    public const int SilverTier = 2;
    public const int BronzeTier = 3;
    public const int NoTier = 0;
    public MilestoneRepository(StorkDorkContext context) : base(context)
    {
        _milestones = context.Milestone;
        _context = context;
    }

    public async Task<int> GetSightingsMade(int SDUserID)
    {
        return await _milestones
                    .Where(s => s.SDUserId == SDUserID)
                    .Select(s => s.SightingsMade)
                    .FirstOrDefaultAsync();
    }

    public async Task<int> GetPhotosContributed(int SDUserID)
    {
        return await _milestones
                    .Where(s => s.SDUserId == SDUserID)
                    .Select(s => s.PhotosContributed)
                    .FirstOrDefaultAsync();
    }

    public int GetMilestoneTier(int Achievement)
    {
        if (Achievement >= 100)
            return GoldTier;

        else if (Achievement >= 50)
            return SilverTier;

        else if (Achievement >= 25)
            return BronzeTier;

        else
            return NoTier;
    }

    public void IncrementSightingsMade(int SDUserID)
    {
        var milestone = _milestones
                        .FirstOrDefault(s => s.SDUserId == SDUserID);

        if (milestone != null)
        {
            // Increment the SightingsMade field
            milestone.SightingsMade++;

            // Save the changes to the database
            _context.SaveChanges();
        }
    }
}