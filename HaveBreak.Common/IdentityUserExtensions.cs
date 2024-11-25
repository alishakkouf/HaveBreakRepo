using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace HaveBreak.Common
{
    public static class IdentityUserExtensions
    {
        public static long? GetUserId(this ClaimsPrincipal user)
        {
            var id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(id) ? (long?)null : long.Parse(id);
        }

        public static string? GetUserName(this ClaimsPrincipal user)
            => user?.FindFirst(ClaimTypes.Name)?.Value;

    }
}
