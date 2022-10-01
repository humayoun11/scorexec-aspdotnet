using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using ScoringAppReact.Players.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Players.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IRepository<Player, long> _repository;
        public PlayerRepository(IRepository<Player, long> repository)
        {
            _repository = repository;
        }


        public async Task<List<Player>> GetAll(int? tenantId, long? teamId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                (!tenantId.HasValue || i.TenantId == tenantId) &&
                (!teamId.HasValue || i.Teams.Any(j => j.TeamId == teamId))
                )
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Player>> GetAllPaginated(PagedPlayerResultRequestDto input, int? tenantId)
        {
            return _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId))
                .WhereIf(input.TeamId.HasValue, i => i.Teams.Any(j => j.TeamId == input.TeamId))
                .WhereIf(input.PlayingRole.HasValue, i => i.PlayerRoleId == input.PlayingRole)
                .WhereIf(input.BattingStyle.HasValue, i => i.BattingStyleId == input.BattingStyle)
                .WhereIf(input.BowlingStyle.HasValue, i => i.BowlingStyleId == input.BowlingStyle)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.ToLower().Contains(input.Name.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Contact),
                    x => x.Contact.ToLower().Contains(input.Contact.ToLower()));
        }


        public async Task<Player> Get(long id)
        {
            var result = await _repository.GetAll().Where(i => i.Id == id && i.IsDeleted == false).Include(i => i.Pictures).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Player>> GetAllByTeamIds(List<long> teamIds, int? tenantId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                (!tenantId.HasValue || i.TenantId == tenantId) && i.Teams.Any(j => teamIds.Contains(j.TeamId))).ToListAsync();
            return result;
        }

        public void InsertOrUpdateRange(List<Player> models)
        {
            _repository.GetDbContext().UpdateRange(models);
        }

        public async Task<Player> Update(Player model)
        {
            return await _repository.UpdateAsync(model);

        }

        public async Task<Player> Insert(Player model)
        {
            return await _repository.InsertAsync(model);

        }
    }
}
