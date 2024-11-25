using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HaveBreak.Data;

namespace HaveBreak.Common
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public long? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }

        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserName();
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
        }

    }
}
