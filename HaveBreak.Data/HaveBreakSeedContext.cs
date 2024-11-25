
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Data.Models;
using HaveBreak.Domain.Authorization;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HaveBreak.Data
{
    internal static class HaveBreakSeed
    {
        /// <summary>
        /// Seed super admin user.
        /// </summary>
        internal static async Task SeedSuperAdminAsync(HaveBreakDbContext context, RoleManager<UserRole> roleManager, UserManager<UserAccount> userManager)
        {
            var role = await context.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Name == Constants.SuperAdminRoleName);

            if (role == null)
            {
                role = new UserRole(Constants.SuperAdminRoleName) { IsActive = true, Description = string.Empty };
                await roleManager.CreateAsync(role);
            }

            var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserName.StartsWith(Constants.SuperAdminUserName));

            if (user == null)
            {
                user = new UserAccount
                {
                    UserName = Constants.SuperAdminEmail,
                    Email = Constants.SuperAdminEmail,
                    FirstName = "Super Admin",
                    LastName = "Super Admin",
                    PhoneNumber = "935479586",
                    IsActive = true,
                    Gender = Shared.Enums.Gender.Male
                };

                var result = await userManager.CreateAsync(user, Constants.DefaultPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Log.Error($"Error: {error.Code} - {error.Description}");
                        Console.WriteLine($"Error: {error.Code} - {error.Description}");
                    }
                }
                await userManager.AddToRolesAsync(user, new[] { Constants.SuperAdminRoleName });
            }
        }

        /// <summary>
        /// Seed static roles and add permissions claims to them.
        /// </summary>
        internal static async Task SeedStaticRolesAsync(RoleManager<UserRole> roleManager)
        {
            foreach (var rolePermission in StaticRolePermissions.Roles)
            {
                var role = await roleManager.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                    x.NormalizedName == rolePermission.Key.ToUpper());

                if (role == null)
                {
                    role = new UserRole(rolePermission.Key)
                    {
                        IsActive = true,
                        Description = string.Empty,
                    };

                    await roleManager.CreateAsync(role);

                    //Add static role permissions to db
                    foreach (var permission in rolePermission.Value)
                    {
                        await roleManager.AddClaimAsync(role,
                            new Claim(Constants.PermissionsClaimType, permission));
                    }

                    continue;
                }

                if (rolePermission.Key == StaticRoleNames.Administrator)
                {
                    var dbRoleClaims = await roleManager.GetClaimsAsync(role);

                    //Remove any claim in db and not in static role permissions.
                    foreach (var dbPermission in dbRoleClaims.Where(x => x.Type == Constants.PermissionsClaimType &&
                                                                         !rolePermission.Value.Contains(x.Value)))
                    {
                        await roleManager.RemoveClaimAsync(role, dbPermission);
                    }

                    //Add static role permissions to db if they don't already exist.
                    foreach (var permission in rolePermission.Value)
                    {
                        if (!dbRoleClaims.Any(x => x.Type == Constants.PermissionsClaimType && x.Value == permission))
                        {
                            await roleManager.AddClaimAsync(role,
                                new Claim(Constants.PermissionsClaimType, permission));
                        }
                    }
                }
            }
        }

        internal static async Task SeedDefaultUserAsync(UserManager<UserAccount> userManager,
            RoleManager<UserRole> roleManager, string password)
        {
            var adminRole = await roleManager.Roles.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Name == StaticRoleNames.Administrator );

            var adminUser = await userManager.Users.FirstOrDefaultAsync();

            if (adminUser == null && adminRole != null)
            {
                adminUser = new UserAccount
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "0123456789",
                    IsActive = true
                };

                adminUser.UserRoles.Add(adminRole);
                await userManager.CreateAsync(adminUser, password);
            }
        }

    }
}
