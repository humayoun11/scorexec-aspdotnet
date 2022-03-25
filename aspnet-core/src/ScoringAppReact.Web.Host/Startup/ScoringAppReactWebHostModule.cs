using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ScoringAppReact.Configuration;

namespace ScoringAppReact.Web.Host.Startup
{
    [DependsOn(
       typeof(ScoringAppReactWebCoreModule))]
    public class ScoringAppReactWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ScoringAppReactWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ScoringAppReactWebHostModule).GetAssembly());
        }
    }
}
