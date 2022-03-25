using Abp.Authorization;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Authorization.Users;

namespace ScoringAppReact.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
