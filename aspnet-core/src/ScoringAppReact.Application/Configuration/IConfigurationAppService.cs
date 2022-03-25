using System.Threading.Tasks;
using ScoringAppReact.Configuration.Dto;

namespace ScoringAppReact.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
