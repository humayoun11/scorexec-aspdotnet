using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Players.Dto
{
    public class CreateOrUpdatePlayerDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public int? BattingStyleId { get; set; }
        public int? BowlingStyleId { get; set; }
        public int? PlayerRoleId { get; set; }
        public long? DOB { get; set; }
        public string IsGuestorRegistered { get; set; }
        public bool IsDeactivated { get; set; }
        public List<long> TeamIds { get; set; }
        public string ProfileUrl { get; set; }
        public PictureDto Profile { get; set; }
        public List<PictureDto> Gallery { get; set; }
        public long? CreationTime { get; set; }
        public int? TenantId { get; set; }
    }
}
