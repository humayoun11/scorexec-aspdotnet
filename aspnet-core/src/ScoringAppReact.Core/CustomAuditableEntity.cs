using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact
{
    public class CustomAuditableEntity : FullAuditedEntity<long>
    {
        public long? CreatingTime { get; set; }
        public long? UpdatingTime { get; set; }
        public long? DeletingTime { get; set; }
    }
}
