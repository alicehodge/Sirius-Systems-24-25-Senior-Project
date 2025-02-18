using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete
{
    public class BirdRepository : Repository<Bird>, IBirdRepository
    {
        private DbSet<Bird> _birds;
        public BirdRepository(StorkDorkContext context) : base(context)
        {
            _birds = context.Birds;
        }

        public async Task<IEnumerable<Bird>> GetBirdsByName(string name)
        {
            return await _birds.Where(b => 
                EF.Functions.Like(b.CommonName, $"%{name}%") || 
                EF.Functions.Like(b.ScientificName, $"%{name}%")
            ).ToListAsync();
        }

        public async Task<IEnumerable<Bird>> GetBirdsByTaxonomy(string searchTerm)
        {
            return await _birds.Where(b => 
                EF.Functions.Like(b.Order, $"%{searchTerm}%") || 
                EF.Functions.Like(b.FamilyCommonName, $"%{searchTerm}%") || 
                EF.Functions.Like(b.FamilyScientificName, $"%{searchTerm}%")
            ).ToListAsync();
        }
    }
}