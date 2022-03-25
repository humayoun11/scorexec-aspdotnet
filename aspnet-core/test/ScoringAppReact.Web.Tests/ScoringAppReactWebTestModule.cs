using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ScoringAppReact.EntityFrameworkCore;
using ScoringAppReact.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ScoringAppReact.Web.Tests
{
    [DependsOn(
        typeof(ScoringAppReactWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ScoringAppReactWebTestModule : AbpModule
    {
        public ScoringAppReactWebTestModule(ScoringAppReactEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ScoringAppReactWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ScoringAppReactWebMvcModule).Assembly);
        }
    }
}