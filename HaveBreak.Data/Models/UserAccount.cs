using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Shared.Enums;
using Microsoft.AspNetCore.Identity;

namespace HaveBreak.Data.Models
{
    public class UserAccount : IdentityUser<long>, IAuditedEntity
    {

        [StringLength(100)]
        public required string FirstName { get; set; }

        [StringLength(100)]
        public required string LastName { get; set; }

        [StringLength(20)]
        public override required string PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginTime { get; set; }

        public bool IsCodeConfirmed { get; set; } = false;

        internal virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    }
}
