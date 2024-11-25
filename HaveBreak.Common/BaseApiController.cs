using System.Runtime.InteropServices;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HaveBreak.Common
{
    /// <summary>
    /// Base api controller annotated with attribute [ApiController] and default
    /// base route("api/[controller]").
    /// Needs the service IStringLocalizerFactory to be injected for localization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private readonly IStringLocalizer _localizer;

        protected BaseApiController(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(CommonResource));
        }

        /// <summary>
        /// Localize a message from <see cref="CommonResource"/>.
        /// </summary>
        protected string Localize(string message)
        {
            return _localizer[message];
        }

        /// <summary>
        /// Localize a message from <see cref="CommonResource"/> with arguments.
        /// </summary>
        protected string Localize(string message, params object[] arguments)
        {
            return _localizer[message, arguments];
        }
    }
}
