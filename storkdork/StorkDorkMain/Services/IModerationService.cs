using StorkDorkMain.Models;
using System.Security.Claims;

namespace StorkDorkMain.Services
{
    public interface IModerationService
    {
        Task<IEnumerable<ModeratedContent>> GetPendingContentAsync();
        Task<ModeratedContent> GetContentDetailsAsync(int id);
        Task<bool> ApproveContentAsync(int id, ClaimsPrincipal moderatorUser, string notes);
        Task<bool> RejectContentAsync(int id, ClaimsPrincipal moderatorUser, string reason);
        Task<IEnumerable<ModeratedContent>> GetContentByStatusAsync(string status);
        Task<IEnumerable<ModeratedContent>> GetModerationHistoryAsync();
        Task<bool> ApproveRangeSubmission(int id, ClaimsPrincipal moderatorUser, string notes);
    }
}