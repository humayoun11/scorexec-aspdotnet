using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ScoringAppReact.Models
{
    public class Player : CustomAuditableEntity, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        public string Contact { get; set; }
        [Required]
        public int Gender { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public int? BattingStyleId { get; set; }
        public int? BowlingStyleId { get; set; }
        public int? PlayerRoleId { get; set; }
        public long? DOB { get; set; }
        public string IsGuestorRegistered { get; set; }
        public bool IsDeactivated { get; set; }
        public string FileName { get; set; }
        public List<TeamPlayer> Teams { get; set; }
        public bool? IsVerified { get; set; }
        public int? TenantId { get; set; }
    }
}
