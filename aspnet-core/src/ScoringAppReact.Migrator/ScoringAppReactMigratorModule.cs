using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ScoringAppReact.Configuration;
using ScoringAppReact.EntityFrameworkCore;
using ScoringAppReact.Migrator.DependencyInjection;

namespace ScoringAppReact.Migrator
{
    [DependsOn(typeof(ScoringAppReactEntityFrameworkModule))]
    public class ScoringAppReactMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ScoringAppReactMigratorModule(ScoringAppReactEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(ScoringAppReactMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                ScoringAppReactConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ScoringAppReactMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
