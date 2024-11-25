using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Shared;

namespace HaveBreak.Domain.Authorization
{
    /// <summary>
    /// Static permissions of the system are defined here, and they will be added as claims (of type <see cref="Constants.PermissionsClaimType"/>)
    /// to the corresponding roles. Static role permissions are defined in <see cref="StaticRolePermissions"/>.
    /// </summary>
    public static class Permissions
    {
        private const string PermissionsPrefix = Constants.PermissionsClaimType + ".";

        public static readonly string[] ListAll =
        {

            Post.Create,
            Post.Delete,
            Post.Update,
            Post.View
        };


        #region Post Permissions
        public static class Post
        {
            public const string View = PermissionsPrefix + "Post.View";
            public const string Create = PermissionsPrefix + "Post.Create";
            public const string Update = PermissionsPrefix + "Post.Update";
            public const string Delete = PermissionsPrefix + "Post.Delete";
        }
        #endregion


    }
}
