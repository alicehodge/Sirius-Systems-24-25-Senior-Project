using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDorkMain.Services
{
    public class ModerationService : IModerationService
    {
        private readonly ISDUserRepository _sdUserRepository;
        private readonly IModeratedContentRepository _moderatedContentRepository;
        private readonly IBirdRepository _birdRepository;

        public ModerationService(
            ISDUserRepository sdUserRepository,
            IModeratedContentRepository moderatedContentRepository,
            IBirdRepository birdRepository)
        {
            _sdUserRepository = sdUserRepository;
            _moderatedContentRepository = moderatedContentRepository;
            _birdRepository = birdRepository;
        }

        public async Task<IEnumerable<ModeratedContent>> GetPendingContentAsync()
        {
            return _moderatedContentRepository.GetAll()
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate);
        }

        public async Task<ModeratedContent> GetContentDetailsAsync(int id)
        {
            return _moderatedContentRepository.FindById(id);
        }

        public async Task<bool> ApproveContentAsync(int id, ClaimsPrincipal moderatorUser, string notes)
        {
            var content = _moderatedContentRepository.FindById(id);
            if (content == null) return false;

            var moderator = await _sdUserRepository.GetSDUserByIdentity(moderatorUser);
            if (moderator == null) return false;

            content.Status = "Approved";
            content.ModeratorId = moderator.Id; 
            content.ModeratedDate = DateTime.UtcNow;
            content.ModeratorNotes = notes;

            _moderatedContentRepository.AddOrUpdate(content);
            await _moderatedContentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectContentAsync(int id, ClaimsPrincipal moderatorUser, string notes)
        {
            var content = _moderatedContentRepository.FindById(id);
            if (content == null) return false;

            var moderator = await _sdUserRepository.GetSDUserByIdentity(moderatorUser);
            if (moderator == null) return false;

            content.Status = "Rejected";
            content.ModeratorId = moderator.Id;
            content.ModeratedDate = DateTime.UtcNow;
            content.ModeratorNotes = notes;

            _moderatedContentRepository.AddOrUpdate(content);
            await _moderatedContentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ModeratedContent>> GetContentByStatusAsync(string status)
        {
            return _moderatedContentRepository.GetAll()
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.ModeratedDate);
        }

        public async Task<IEnumerable<ModeratedContent>> GetModerationHistoryAsync()
        {
            return _moderatedContentRepository.GetAll()
                .Where(c => c.Status != "Pending")
                .OrderByDescending(c => c.ModeratedDate);
        }

        public async Task<bool> ApproveRangeSubmission(int id, ClaimsPrincipal moderatorUser, string notes)
        {
            var submission = await _moderatedContentRepository.GetContentWithDetailsAsync(id) as RangeSubmission;
            if (submission == null) return false;

            var bird = _birdRepository.FindById(submission.BirdId);
            if (bird == null) return false;

            var moderator = await _sdUserRepository.GetSDUserByIdentity(moderatorUser);
            if (moderator == null) return false;

            try
            {
                // Update the bird's range information
                bird.Range = submission.RangeDescription;
                _birdRepository.AddOrUpdate(bird);

                // Update submission status
                submission.Status = "Approved";
                submission.ModeratorId = moderator.Id;
                submission.ModeratedDate = DateTime.UtcNow;
                submission.ModeratorNotes = notes;
                _moderatedContentRepository.AddOrUpdate(submission);
                
                await _moderatedContentRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ApprovePhotoSubmission(int id, ClaimsPrincipal moderatorUser, string notes)
        {
            var submission = await _moderatedContentRepository.GetContentWithDetailsAsync(id) as BirdPhotoSubmission;
            if (submission == null) return false;

            var bird = _birdRepository.FindById(submission.BirdId);
            if (bird == null) return false;

            var moderator = await _sdUserRepository.GetSDUserByIdentity(moderatorUser);
            if (moderator == null) return false;

            try
            {
                var birdPhoto = new BirdPhoto
                {
                    BirdId = bird.Id,
                    PhotoData = submission.PhotoData,
                    PhotoContentType = submission.PhotoContentType,
                    Caption = submission.Caption,
                    DateAdded = DateTime.UtcNow
                };
                // If Bird has a Photos collection:
                bird.Photos.Add(birdPhoto);
                _birdRepository.AddOrUpdate(bird);

                // 2) Mark the submission as approved
                submission.Status = "Approved";
                submission.ModeratorId = moderator.Id;
                submission.ModeratedDate = DateTime.UtcNow;
                submission.ModeratorNotes = notes;
                _moderatedContentRepository.AddOrUpdate(submission);

                // 3) Save changes
                await _moderatedContentRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        
    }
}