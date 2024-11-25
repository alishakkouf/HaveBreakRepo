using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HaveBreak.Data.Models
{
    public class UserRole : IdentityRole<long>, IAuditedEntity
    {
        UserRole() : base() { }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public UserRole(string roleName) : base(roleName)
        {
        }

        internal virtual ICollection<UserAccount> UserAccounts { get; set; } = [];

        public virtual ICollection<IdentityRoleClaim<long>> Claims { get; set; } = [];
    }
}
