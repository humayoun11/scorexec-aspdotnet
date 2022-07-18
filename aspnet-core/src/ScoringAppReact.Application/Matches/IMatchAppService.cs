using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Matches.Dto;
using ScoringAppReact.Models;

namespace ScoringAppReact.Matches
{
    public interface IMatchAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateMatchDto typeDto);

        Task<MatchDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<MatchDto>> GetAll();

        Task<PagedResultDto<MatchDto>> GetPaginatedAllAsync(PagedMatchResultRequestDto input);

        Task<List<MatchDto>> GetAllMatchesBYEventId(long eventId);

        BracketStages GetAllStagedMatchesByEventId(long eventId);

        Task<EventMatch> EditEventMatch(long id);

        Task<List<ViewMatch>> GetMatchesByPlayerId(long id, int matchResultFilter);

        List<ViewMatch> GetMOMByPlayerId(long id);

        List<ViewMatch> GetMatchesByTeamId(long id, int matchResultFilter);

        List<ViewMatch> GetMatchesByEventId(long id);

        void InsertDbRange(List<Match> matches);

        void CreateBracketMatch(List<EventTeam> teams, long? eventId);
    }
}
