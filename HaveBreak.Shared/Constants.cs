using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Shared
{
    public static class Constants
    {
        //public const string EmailRegularExpression = @"^([\w\.\-]+)@([\w\-]+\.)+(\w){2,}$";
        public const string EmailRegularExpression = @"^([\w\.\-]+)@([\w\-]+\.?)+(\w){2,}$";
        public const string PhoneCountryCodeExpression = @"^(\+)[1-9]\d{0,3}$";
        public const string PhoneRegularExpression = @"^(0|00|\+)[1-9]\d{5,14}$";
        public const string UserNameRegularExpression = @"^([\w\.\-]+)@([\w\-]+\.)*[\w\-]{2,}$";

        public const string AdministratorUserName = "Administrator";
        public const string DefaultPassword = "P@ssw0rd";
        public const string DefaultPhoneNumber = "0";

        public const string SuperAdminRoleName = "SuperAdmin";
        public const string SuperAdminUserName = "SuperAdmin";
        public const string SuperAdminEmail = "superadmin@yolo.clinic";

        public const int DefaultPageIndex = 1;
        public const int DefaultPageSize = 10;
        public const int DropdownPageSize = 100;

        /// <summary>
        /// The limit count to be considered as getting all items
        /// </summary>
        public const int AllItemsPageSize = 1000;

        /// <summary>
        /// The custom claim type for the role permissions
        /// </summary>
        public const string PermissionsClaimType = "Permissions";

    }
}
