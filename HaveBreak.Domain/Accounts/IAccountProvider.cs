using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Domain.Accounts
{
    public interface IAccountProvider
    {
        /// <summary>
        /// Get user by email with related entities. Throws <see cref="EntityNotFoundException"/> if not found.
        /// </summary>
        Task<UserAccountDomain> FindUserAsync(string email);

        /// <summary>
        /// Login user and return token.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserAccountDomain> LoginAsync(string username, string password);

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<RegistrationResult> RegisterAsync(RegisterInputCommand command);
    }
}
