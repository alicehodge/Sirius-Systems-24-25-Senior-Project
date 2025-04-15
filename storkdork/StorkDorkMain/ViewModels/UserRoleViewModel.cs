using System.Collections.Generic;

namespace StorkDorkMain.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsAdmin => Roles.Contains("Admin");
        public bool IsModerator => Roles.Contains("Moderator");
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleManagementViewModel
    {
        public IEnumerable<UserRoleViewModel> Users { get; set; }
        public IEnumerable<string> AvailableRoles { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public Dictionary<string, int> RoleCounts { get; set; }
    }
}