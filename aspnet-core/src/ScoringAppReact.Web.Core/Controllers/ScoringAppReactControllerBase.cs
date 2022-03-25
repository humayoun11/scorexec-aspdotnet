using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace ScoringAppReact.Controllers
{
    public abstract class ScoringAppReactControllerBase: AbpController
    {
        protected ScoringAppReactControllerBase()
        {
            LocalizationSourceName = ScoringAppReactConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
