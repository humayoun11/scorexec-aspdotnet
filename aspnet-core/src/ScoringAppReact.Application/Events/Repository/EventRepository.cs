using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Events.Dto;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Events.Repository
{
    public class EventRepository : IEventRepository
    {

        private readonly IRepository<Event, long> _repository;
        public EventRepository(IRepository<Event, long> repository)
        {
            _repository = repository;
        }

        public async Task<Event> Get(long id, int? tenantId)
        {
            var result = await _repository.GetAll()
                .FirstOrDefaultAsync(i => i.Id == id && i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId));
            return result;
        }

        public async Task<List<Event>> GetAllPaginated(PagedEventResultRequestDto input, int? tenantId)
        {
            var model = _repository.GetAll()
               .Where(i => i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId)
               && (!input.Type.HasValue || i.EventType == input.Type) && (!input.StartDate.HasValue || i.StartDate >= input.StartDate)
               && (!input.EndDate.HasValue || i.EndDate <= input.EndDate))
               .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                   x => x.Name.ToLower().Contains(input.Name.ToLower()));
            return model.ToList();
        }


        public async Task<List<Event>> GetAll(int? tenantId, long? teamId, int? type)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false
                && (!tenantId.HasValue || i.TenantId == tenantId)
                && (!teamId.HasValue || i.EventTeams.Any(j => j.TeamId == teamId))
                && (!type.HasValue || i.EventType == type)
                )
                .ToListAsync();
            return result;
        }

        public void InsertOrUpdateRange(List<Event> models)
        {
            _repository.GetDbContext().UpdateRange(models);
        }

        public async Task<Event> Update(Event model)
        {
            return await _repository.UpdateAsync(model);

        }

        public async Task<Event> Insert(Event model)
        {
            return await _repository.InsertAsync(model);

        }
    }
}
