using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HaveBreak.Data.Models;
using HaveBreak.Domain.Accounts;
using HaveBreak.Shared.Exceptions;
using HaveBreak.Shared.Enums;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography;
using HaveBreak.Domain.Authorization;

namespace HaveBreak.Data.Providers
{
    internal class AccountProvider : GenericProvider<UserAccount>, IAccountProvider
    {

        private readonly UserManager<UserAccount> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _localizer;

        public AccountProvider(HaveBreakDbContext dbContext,
            UserManager<UserAccount> userManager,
            RoleManager<UserRole> roleManager,
            IMapper mapper,
            IStringLocalizerFactory factory)
        {
            DbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;

            _localizer = factory.Create(typeof(CommonResource));
        }

        public async Task<Domain.Accounts.UserAccountDomain> LoginAsync(string username, string password)
        {
            // Fetch the user from the database with associated roles
            var userEntity = await ActiveDbSet.Include(u => u.UserRoles)
                                              .FirstOrDefaultAsync(u => u.UserName == username) 
                                              ?? throw new EntityNotFoundException(nameof(UserAccount), username);

            // Validate the password and check if rehashing is needed
            var verificationResult = ValidatePassword(password, userEntity.PasswordHash, out string newPasswordHash);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid username or password.");

            // Rehash and update the password if needed
            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                userEntity.PasswordHash = newPasswordHash;
                await DbContext.SaveChangesAsync();
            }

            // Return the user domain object
            return new UserAccountDomain
            {
                Id = userEntity.Id,
                Email = userEntity.Email.ToLower(),
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                UserName = userEntity.UserName,
                Role = userEntity.UserRoles.FirstOrDefault()?.Name
            };
        }

        private static PasswordVerificationResult ValidatePassword(string password, string storedHash, out string updatedHash)
        {
            var passwordHasher = new PasswordHasher<object>();

            // Verify the password against the stored hash
            var result = passwordHasher.VerifyHashedPassword(null, storedHash, password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                updatedHash = HashPassword(password); // Rehash the password
                return result;
            }

            if (result == PasswordVerificationResult.Success)
            {
                updatedHash = storedHash; // No rehash needed
                return result;
            }

            // Check against legacy hashing algorithm
            if (HashPasswordV1(password) == storedHash)
            {
                updatedHash = HashPassword(password); // Rehash to the new standard
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            updatedHash = null; // Password verification failed
            return PasswordVerificationResult.Failed;
        }

        private static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<object>();
            return passwordHasher.HashPassword(null, password);
        }

        private static string HashPasswordV1(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterInputCommand command)
        {
            var userRole = await _roleManager.Roles.FirstAsync(x => x.Name == StaticRoleNames.User);

            var userAccount = new UserAccount
            {
                UserName = command.Email,
                Gender = command.Gender.HasValue ? (Gender)command.Gender.Value : null,
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.PhoneNumber,
                PhoneNumberConfirmed = true,
                IsActive = true,                
                IsCodeConfirmed = false,
                UserRoles = []
            };

            userAccount.UserRoles.Add(userRole);

            var identityResult = await _userManager.CreateAsync(userAccount, command.Password);

            var result = new RegistrationResult(identityResult.Succeeded);

            if (!result.Succeeded)
            {
                result.Errors.AddRange(identityResult.Errors.Select(x => x.Description));
            }

            result.Id = userAccount.Id;

            return result;
        }

        public async Task<UserAccountDomain> FindUserAsync(string email)
        {
            var user = await ActiveDbSet.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null) 
                return null;

            return _mapper.Map<UserAccountDomain>(user);
        }

    }
}
