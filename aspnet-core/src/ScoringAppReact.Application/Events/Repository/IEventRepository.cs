using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Events.Repository
{
    public interface IEventRepository
    {
        Task<Event> Get(long id);
    }
}
