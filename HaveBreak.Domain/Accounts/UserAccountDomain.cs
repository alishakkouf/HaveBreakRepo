using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Shared.Enums;

namespace HaveBreak.Domain.Accounts
{
    public class UserAccountDomain
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        /// <summary>
        /// Role of the user which should be only one currently
        /// </summary>
        public string Role { get; set; }

        public string FullName => $"{FirstName} {LastName}";

    }
}
