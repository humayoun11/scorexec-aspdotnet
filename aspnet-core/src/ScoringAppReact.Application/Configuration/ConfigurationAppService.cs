using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using ScoringAppReact.Configuration.Dto;

namespace ScoringAppReact.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ScoringAppReactAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
