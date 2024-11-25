using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HaveBreak.Data.Models
{
    public class Post : IAuditedEntity
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Like> Likes { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual UserAccount User { get; set; }
    }
}
