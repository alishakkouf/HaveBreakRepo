using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Domain.Accounts;
using HaveBreak.Shared.Exceptions;
using HaveBreak.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace HaveBreak.Manager.Accounts
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountProvider _accountProvider;
        private readonly IStringLocalizer _localizer;
        private IConfiguration Configuration { get; set; }

        public AccountManager(IAccountProvider accountProvider, IConfiguration Configuration,
             IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(CommonResource));
            _accountProvider = accountProvider;
            this.Configuration = Configuration;
        }

        public async Task<TokenDomain> LoginAsync(string username, string password)
        {
            var user = await _accountProvider.LoginAsync(username, password);

            var token = JWTGenerator.GenerateJWTToken(new CreateTokenRequest
            {
                Email = user.Email,
                Roles = new List<string> { user.Role },
                IsActive = true,
                UserId = user.Id
            }, Configuration);

            return token;
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterInputCommand command)
        {
            var existedUser = await _accountProvider.FindUserAsync(command.Email);

            if (existedUser != null) throw new BusinessException("Existed Email");

            var result = await _accountProvider.RegisterAsync(command);

            if (!result.Succeeded)
                throw new BusinessException("Error registering new user");

            return result;
        }

        
        public Task<UserAccountDomain> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
