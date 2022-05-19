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
using Abp.Domain.Uow;

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

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
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


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostCenturiesdto>> MostCenturies(BattingInput input)
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
                var result = await connection.QueryAsync<MostCenturiesdto>("usp_GetMostCenturies",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramPosition, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostCenturiesdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostFiftiesdto>> MostFifties(BattingInput input)
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
                var result = await connection.QueryAsync<MostFiftiesdto>("usp_GetMostFifties",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramPosition, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostFiftiesdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostFoursdto>> MostFours(BattingInput input)
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
                var result = await connection.QueryAsync<MostFoursdto>("usp_GetMostFours",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramPosition, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostFoursdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostSixesdto>> MostSixes(BattingInput input)
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
                var result = await connection.QueryAsync<MostSixesdto>("usp_GetMostSixes",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramPosition, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostSixesdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
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
                var result = await connection.QueryAsync<MostWicketsdto>("usp_GetMostWickets",
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


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostCatchesdto>> MostCatches(BowlingInput input)
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
                var result = await connection.QueryAsync<MostCatchesdto>("usp_GetMostCatches",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostCatchesdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostStumpsdto>> MostStumps(BowlingInput input)
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
                var result = await connection.QueryAsync<MostStumpsdto>("usp_GetMostStumps",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostStumpsdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostMaidensdto>> MostMaidens(BowlingInput input)
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
                var result = await connection.QueryAsync<MostMaidensdto>("usp_GetMostMaidens",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostMaidensdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<MostRunoutsdto>> MostRunouts(BowlingInput input)
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
                var result = await connection.QueryAsync<MostRunoutsdto>("usp_GetMostRunouts",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<MostRunoutsdto>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<HighestWicket>> HighestWicketsInAnInning(BowlingInput input)
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
                var result = await connection.QueryAsync<HighestWicket>("usp_GetHighestWicketsIndividual",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<HighestWicket>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<List<BestScore>> HighestRunsInAnInning(BowlingInput input)
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
                var result = await connection.QueryAsync<BestScore>("usp_GetHighestRunsIndividual",
                    new
                    { paramTeamId, paramSeason, paramOvers, paramMatchType, paramTournamentId, paramPlayerRoleId },

                    commandType: CommandType.StoredProcedure) ?? new List<BestScore>();
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}

