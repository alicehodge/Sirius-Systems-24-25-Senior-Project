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

        public async Task<(IEnumerable<Bird> Birds, int TotalCount)> GetRelatedBirds(
            string speciesCode, 
            string? reportAs, 
            string? categoryFilter = null,
            int page = 1, 
            int pageSize = 10)
        {
            var relatedBirds = new List<Bird>();

            // Find subspecies, forms, and domestic variants that report as the same species
            if (!string.IsNullOrEmpty(reportAs))
            {
                var sameSpecies = await _birds.Where(b => 
                    b.ReportAs == reportAs && 
                    b.SpeciesCode != speciesCode && 
                    (b.Category == "species" || 
                     b.Category == "issf" || 
                     b.Category == "domestic" ||
                     b.Category == "form" ||
                     b.Category == "intergrade"))
                .ToListAsync();
                relatedBirds.AddRange(sameSpecies);
            }

            // Get the current bird's common name to use in hybrid search
            var currentBird = await _birds.FirstOrDefaultAsync(b => b.SpeciesCode == speciesCode);
            if (currentBird != null)
            {
                var nameToSearch = currentBird.CommonName.Split(' ')[0];

                // Find hybrids and intergrades
                var hybrids = await _birds.Where(b => 
                    (b.Category == "hybrid" || b.Category == "intergrade") && 
                    (EF.Functions.Like(b.CommonName, $"%{nameToSearch}%") || 
                     EF.Functions.Like(b.ScientificName, $"%{nameToSearch}%")))
                .ToListAsync();
                relatedBirds.AddRange(hybrids);

                // Find slash and spuh combinations
                var uncertainSpecies = await _birds.Where(b =>
                    (b.Category == "slash" || b.Category == "spuh") &&
                    (EF.Functions.Like(b.CommonName, $"%{nameToSearch}%") ||
                     EF.Functions.Like(b.ScientificName, $"%{nameToSearch}%")))
                .ToListAsync();
                relatedBirds.AddRange(uncertainSpecies);
            }

            // Apply category filter if provided
            var filteredBirds = categoryFilter != null 
                ? relatedBirds.Where(b => b.Category == categoryFilter)
                : relatedBirds;

            // Get total count before pagination
            var totalCount = filteredBirds.Count();

            // Apply pagination
            var paginatedBirds = filteredBirds
                .Distinct()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return (paginatedBirds, totalCount);
        }
    }
}