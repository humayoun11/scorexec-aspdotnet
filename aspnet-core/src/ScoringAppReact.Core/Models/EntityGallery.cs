using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class EntityGallery : FullAuditedEntity<long>
    {

        public long GalleryId { get; set; }
        public long EntityId { get; set; }
        [ForeignKey("GalleryId")]
        public Gallery Gallery { get; set; }
    }
}