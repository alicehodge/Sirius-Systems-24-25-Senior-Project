using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete;

public class MilestoneRepository : Repository<Milestone>, IMilestoneRepository
{
    private DbSet<Milestone> _milestones;
    public MilestoneRepository(StorkDorkContext context) : base(context)
    {
        _milestones = context.Milestone;
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
}