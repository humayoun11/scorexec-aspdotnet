using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ScoringAppReact.EntityFrameworkCore
{
    public static class ScoringAppReactDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ScoringAppReactDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ScoringAppReactDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
