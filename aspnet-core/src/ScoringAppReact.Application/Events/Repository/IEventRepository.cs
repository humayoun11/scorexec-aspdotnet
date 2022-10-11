using ScoringAppReact.Events.Dto;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Events.Repository
{
    public interface IEventRepository
    {
        Task<Event> Get(long id, int? tenantId);

        Task<List<Event>> GetAll(int? tenantId, long? teamId, int? type);

        Task<List<Event>> GetAllPaginated(PagedEventResultRequestDto input, int? tenantId);

        void InsertOrUpdateRange(List<Event> models);

        Task<Event> Update(Event model);

        Task<Event> Insert(Event model);
    }
}
