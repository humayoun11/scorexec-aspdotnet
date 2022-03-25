using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ScoringAppReact.Configuration;
using ScoringAppReact.Web;

namespace ScoringAppReact.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ScoringAppReactDbContextFactory : IDesignTimeDbContextFactory<ScoringAppReactDbContext>
    {
        public ScoringAppReactDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ScoringAppReactDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ScoringAppReactDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ScoringAppReactConsts.ConnectionStringName));

            return new ScoringAppReactDbContext(builder.Options);
        }
    }
}
