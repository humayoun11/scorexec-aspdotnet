using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.FallOfWickets.Dto;
using ScoringAppReact.Players.Dto;
using ScoringAppReact.TeamScores.Dto;

namespace ScoringAppReact.FallOfWickets
{
    public interface IFallofWicketAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateFallofWicketDto model);

        Task<FallofWicketsDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<FallofWicketsDto>> GetByTeamIdAndMatchId(long teamId, long matchId);
    }
}

