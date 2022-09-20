using Abp;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using ScoringAppReact.EntityAdmins.Dto;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.EntityAdmins
{
    public class EntityAdminAppService : AbpServiceBase, IEntityAdminAppService
    {
        private readonly IRepository<EntityAdmin, long> _repository;
        private readonly IAbpSession _abpSession;
        public EntityAdminAppService(IRepository<EntityAdmin, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
        }

        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrEditEntityAdmin model)
        {
            ResponseMessageDto result;
            if (model.Id == 0)
            {
                result = await CreateEntityAdminAsync(model);
            }
            else
            {
                result = await UpdateEntityAdminAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateEntityAdminAsync(CreateOrEditEntityAdmin model)
        {
            var entityAdmins = new List<EntityAdmin>();
            if (model.UserIds.Length > 0)
            {
                foreach (var item in model.UserIds)
                {
                    var entityAdmin = new EntityAdmin
                    {
                        UserId = item,
                        MatchId = model.MatchId.HasValue ? model.MatchId : null,
                        PlayerId = model.PlayerId.HasValue ? model.PlayerId : null,
                        GroundId = model.GroundId.HasValue ? model.GroundId : null,
                        TeamId = model.TeamId.HasValue ? model.TeamId : null,
                        EventId = model.EventId.HasValue ? model.EventId : null,
                        TenantId = _abpSession.TenantId
                    };
                    entityAdmins.Add(entityAdmin);
                }
            }

            await _repository.GetDbContext().AddRangeAsync(entityAdmins);
            await UnitOfWorkManager.Current.SaveChangesAsync();


            if (getEnitityId(model) != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = getEnitityId(model),
                    SuccessMessage = AppConsts.SuccessfullyInserted,
                    Success = true,
                    Error = false,
                };
            }
            return new ResponseMessageDto()
            {
                Id = 0,
                ErrorMessage = AppConsts.InsertFailure,
                Success = false,
                Error = true,
            };
        }

        private async Task<ResponseMessageDto> UpdateEntityAdminAsync(CreateOrEditEntityAdmin model)
        {
            var allData = _repository.GetAll()
                .Where(i =>
                (!model.EventId.HasValue || i.EventId == model.EventId) &&
                (!model.PlayerId.HasValue || i.PlayerId == model.PlayerId) &&
                (!model.TeamId.HasValue || i.TeamId == model.TeamId) &&
                (!model.MatchId.HasValue || i.MatchId == model.MatchId)
                && i.IsDeleted == false).ToList();

            var prev = allData.Select(i => i.UserId);
            var toDelete = prev.Except(model.UserIds);
            var toAddNew = model.UserIds.Except(prev);

            if (toDelete.Any())
            {
                var deleteEntityAdmin = new List<EntityAdmin>();
                foreach (var id in toDelete)
                {
                    var data = allData.Where(j =>
                        (!model.EventId.HasValue || j.EventId == model.EventId) &&
                        (!model.PlayerId.HasValue || j.PlayerId == model.PlayerId) &&
                        (!model.TeamId.HasValue || j.TeamId == model.TeamId) &&
                        (!model.MatchId.HasValue || j.MatchId == model.MatchId)).FirstOrDefault();
                    data.IsDeleted = true;
                    deleteEntityAdmin.Add(data);
                }
                _repository.GetDbContext().UpdateRange(deleteEntityAdmin);
            }

            if (toAddNew.Any())
            {
                var addNewTeams = new List<EntityAdmin>();
                foreach (var id in toAddNew)
                {
                    var team = new EntityAdmin()
                    {
                        MatchId = model.MatchId.HasValue ? model.MatchId : null,
                        PlayerId = model.PlayerId.HasValue ? model.PlayerId : null,
                        GroundId = model.GroundId.HasValue ? model.GroundId : null,
                        TeamId = model.TeamId.HasValue ? model.TeamId : null,
                        EventId = model.EventId.HasValue ? model.EventId : null,
                        TenantId = _abpSession.TenantId
                    };
                    addNewTeams.Add(team);
                }
                _repository.GetDbContext().AddRange(addNewTeams);
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (getEnitityId(model) != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = getEnitityId(model),
                    SuccessMessage = AppConsts.SuccessfullyUpdated,
                    Success = true,
                    Error = false,
                };
            }
            return new ResponseMessageDto()
            {
                Id = 0,
                ErrorMessage = AppConsts.UpdateFailure,
                Success = false,
                Error = true,
            };
        }


        private long getEnitityId(CreateOrEditEntityAdmin model)
        {

            if (model.MatchId.HasValue)
                return model.MatchId.Value;
            if (model.TeamId.HasValue)
                return model.TeamId.Value;
            if (model.EventId.HasValue)
                return model.EventId.Value;
            if (model.PlayerId.HasValue)
                return model.PlayerId.Value;
            if (model.GroundId.HasValue)
                return model.GroundId.Value;

            return 0;

        }

    }
}
