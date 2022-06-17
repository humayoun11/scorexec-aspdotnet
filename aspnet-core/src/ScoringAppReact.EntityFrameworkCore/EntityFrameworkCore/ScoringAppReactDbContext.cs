using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Authorization.Users;
using ScoringAppReact.MultiTenancy;
using ScoringAppReact.Models;

namespace ScoringAppReact.EntityFrameworkCore
{
    public class ScoringAppReactDbContext : AbpZeroDbContext<Tenant, Role, User, ScoringAppReactDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<FallOfWicket> FallOfWickets { get; set; }
        public DbSet<Ground> Grounds { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchSchedule> MatchSchedules { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerScore> PlayerScores { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamScore> TeamScores { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventTeam> EventTeams { get; set; }
        public DbSet<EventBracket> EventBrackets { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<PlayerPastRecord> PlayerPastRecords { get; set; }
        public ScoringAppReactDbContext(DbContextOptions<ScoringAppReactDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasOne(s => s.OppponentTeam)
                .WithMany(s => s.OpponentTeamMatches)
                .HasForeignKey(s => s.OppponentTeamId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

                entity.HasOne(s => s.HomeTeam)
                .WithMany(s => s.HomeTeamMatches)
                .HasForeignKey(s => s.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            });


            //It might not have been needed
            //modelBuilder.Entity<Team>(entity =>
            //{
            //    entity.HasMany(i => i.HomeTeamMatches)
            //    .WithOne(i => i.HomeTeam)
            //    .HasForeignKey(i => i.HomeTeamId)
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired();

            //    entity.HasMany(i => i.OpponentTeamMatches)
            //    .WithOne(i => i.OppponentTeam)
            //    .HasForeignKey(i => i.OppponentTeamId)
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired();
            //});
        }
    }
}
