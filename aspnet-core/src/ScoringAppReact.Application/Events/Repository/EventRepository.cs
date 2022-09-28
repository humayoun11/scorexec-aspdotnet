using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
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

        public async Task<Event> Get(long id)
        {
            var result = await _repository.GetAll()
                .FirstOrDefaultAsync(i => i.Id == id);
            return result;
        }
    }
}
