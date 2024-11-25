using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Data
{
    public interface IAuditedEntity
    {
        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
