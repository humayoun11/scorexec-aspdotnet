using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ScoringAppReact.Authorization;

namespace ScoringAppReact
{
    [DependsOn(
        typeof(ScoringAppReactCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ScoringAppReactApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ScoringAppReactAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ScoringAppReactApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
