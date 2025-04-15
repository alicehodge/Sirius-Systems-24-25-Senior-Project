using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete;

public class MilestoneRepository : Repository<Milestone>, IMilestoneRepository
{
    private DbSet<Milestone> _milestones;
    private DbSet<Sighting> _sightings;
    private DbSet<Bird> _birds;
    private readonly StorkDorkContext _context;

    public const int GoldTier = 1;
    public const int SilverTier = 2;
    public const int BronzeTier = 3;
    public const int NoTier = 0;
    public DateTime sqlMinDate = new DateTime(1753, 1, 1);
    public MilestoneRepository(StorkDorkContext context) : base(context)
    {
        _context = context;
        _milestones = context.Milestone;
        _sightings = context.Sightings;
        _birds = context.Birds;
    }

    public async Task<MostSpottedBirdDTO?> GetMostSpottedBirdAsync(int userId)
    {
        var result = await _sightings
            .Where(s => s.SdUserId == userId)
            .GroupBy(s => s.BirdId)
            .OrderByDescending(g => g.Count())
            .Select(g => new
            {
                BirdId = g.Key,
                Count = g.Count()
            })
            .Join(_birds,
                s => s.BirdId,
                b => b.Id,
                (s, b) => new MostSpottedBirdDTO
                {
                    BirdID = b.Id,
                    SpeciesCode = b.SpeciesCode,
                    CommonName = b.CommonName,
                    SightingsCount = s.Count
                })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<SightingsInADayDTO?> GetDailySightingRecord(int userId)
    {
        var result = await _sightings
            .Where(s => s.SdUserId == userId)
            .GroupBy(s => EF.Functions.DateDiffDay(sqlMinDate, s.Date))
            .OrderByDescending(g => g.Count())
            .Select(g => new SightingsInADayDTO
            {
                DayOfSightings = sqlMinDate.AddDays(g.Key ?? 0),
                NumberOfSightings = g.Count()
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<int> GetSightingsMade(int userId)
    {
        return await _milestones
                    .Where(s => s.SDUserId == userId)
                    .Select(s => s.SightingsMade)
                    .FirstOrDefaultAsync();
    }

    public async Task<int> GetPhotosContributed(int userId)
    {
        return await _milestones
                    .Where(s => s.SDUserId == userId)
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

    public void IncrementSightingsMade(int userId)
    {
        var milestone = _milestones
                        .FirstOrDefault(s => s.SDUserId == userId);

        if (milestone != null)
        {
            // Increment the SightingsMade field
            milestone.SightingsMade++;

            // Save the changes to the database
            _context.SaveChanges();
        }
    }
}