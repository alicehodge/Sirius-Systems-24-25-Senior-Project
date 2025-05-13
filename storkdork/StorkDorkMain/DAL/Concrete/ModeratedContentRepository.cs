using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using StorkDorkMain.Data;

namespace StorkDorkMain.DAL.Concrete
{
    public class ModeratedContentRepository : Repository<ModeratedContent>, IModeratedContentRepository
    {
        private readonly StorkDorkDbContext _context;

        public ModeratedContentRepository(StorkDorkDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModeratedContent>> GetContentWithSubmitterAsync(string status)
        {
            return await _context.ModeratedContent
                .Include(c => c.SubmitterId)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task<ModeratedContent> GetContentWithDetailsAsync(int id)
        {
            // First try to get it as a RangeSubmission
            ModeratedContent content = await _context.ModeratedContent
                .Include(c => c.Submitter)
                .Include(c => c.Moderator)
                .OfType<RangeSubmission>()
                .Include(rs => rs.Bird)
                .FirstOrDefaultAsync(c => c.Id == id);

            // If not found as RangeSubmission, try BirdPhotoSubmission
            if (content == null)
            {
                content = await _context.ModeratedContent
                    .Include(c => c.Submitter)
                    .Include(c => c.Moderator)
                    .OfType<BirdPhotoSubmission>()
                    .Include(ps => ps.Bird)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }

            // If still not found, return the base type
            if (content == null)
            {
                content = await _context.ModeratedContent
                    .Include(c => c.Submitter)
                    .Include(c => c.Moderator)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }

            return content;
        }

        public override ModeratedContent FindById(int id)
        {
            return _context.ModeratedContent
                .FirstOrDefault(c => c.Id == id);
        }

        public override ModeratedContent Update(ModeratedContent entity)
        {
            _context.ModeratedContent.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public void AddOrUpdate(ModeratedContent content)
        {
            if (content.Id == 0)
            {
                _context.ModeratedContent.Add(content);
            }
            else
            {
                _context.ModeratedContent.Update(content);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}