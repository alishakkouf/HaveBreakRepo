using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Data
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Returns User Id of current logged user
        /// </summary>
        long? GetUserId();

        /// <summary>
        /// Returns current User Name from Claims
        /// </summary>
        string? GetUserName();

        /// <summary>
        /// Checks if current user has a specific role
        /// </summary>
        bool IsInRole(string role);

    }
}
