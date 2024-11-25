using AutoMapper;
using HaveBreak.API.Controllers.Accounts.Dtos;
using HaveBreak.Common;
using HaveBreak.Domain.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Serilog;

namespace HaveBreak.API.Controllers.Accounts
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountManager _accountManager;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public AccountController(IAccountManager accountManager, IMapper mapper,
             IMemoryCache cache,
            IStringLocalizerFactory factory) : base(factory)
        {
            _accountManager = accountManager;
            _cache = cache;
            _mapper = mapper;
        }

        /// <summary>
        /// Get access token via username and password
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResultDto>> LoginAsync([FromBody] LoginInputDto input)
        {
            Log.Information("start login");

            var result = await _accountManager.LoginAsync(input.UserName, input.Password);

            Log.Information("start mapping");

            var toBeReturned = new LoginResultDto
            {
                AccessToken = result.AccessToken,
                ExpiresIn = result.ExpiresIn
            };

            return Ok(toBeReturned);
        }

        /// <summary>
        /// Get access token via username and password
        /// </summary>

        [HttpPost("Register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterInputDto input)
        {
            Log.Information("start registeration");

            await _accountManager.RegisterAsync(_mapper.Map<RegisterInputCommand>(input));

            Log.Information("start mapping");

            return Ok();
        }
    }
}
