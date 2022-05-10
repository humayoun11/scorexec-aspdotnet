using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using ScoringAppReact.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp;
using System.Data;
using Abp.EntityFrameworkCore;
using ScoringAppReact.EntityFrameworkCore;
using System;
using Dapper;
using ScoringAppReact.Statistics.Dto;

namespace ScoringAppReact.Statistics
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class StatisticsAppService : AbpServiceBase, IStatisticsAppService
    {
        private readonly IDbContextProvider<ScoringAppReactDbContext> _context;
        public StatisticsAppService(
            IDbContextProvider<ScoringAppReactDbContext> context)
        {
            _context = context;
        }
        public async Task<List<MostRunsdto>> MostRuns(BattingInput input)
        {
            try
            {
                var dbContext = _context.GetDbContext();

                var connection = dbContext.Database.GetDbConnection();
                var paramTeamId = input.TeamId;
                var paramSeason = input.Season;
                var paramOvers = input.Overs;
                var paramPosition = input.Position;
                var paramMatchType = input.MatchType;
                var paramTournamentId = input.EventId;
                var paramPlayerRoleId = input.PlayerRoleId;
                var result = await connection.QueryAsync<MostRunsdto>("usp_GetMostRuns",
                    new 
                    { paramTeamId, paramSeason, paramOvers, paramPosition, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostRunsdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<MostWicketsdto>> MostWickets(BowlingInput input)
        {
            try
            {
                var dbContext = _context.GetDbContext();

                var connection = dbContext.Database.GetDbConnection();
                var paramTeamId = input.TeamId;
                var paramSeason = input.Season;
                var paramOvers = input.Overs;
                var paramMatchType = input.MatchType;
                var paramTournamentId = input.EventId;
                var paramPlayerRoleId = input.PlayerRoleId;
                var result = await connection.QueryAsync<MostWicketsdto>("usp_GetMostRuns",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostWicketsdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}

