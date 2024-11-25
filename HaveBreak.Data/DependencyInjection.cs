using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Data.Models;
using HaveBreak.Data.Providers;
using HaveBreak.Domain.Accounts;
using HaveBreak.Domain.Posts;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HaveBreak.Data
{
    public static class DependencyInjection
    {
        const string ConnectionStringName = "DefaultConnection";
        const bool SeedData = true;

        public static IServiceCollection ConfigureDataModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HaveBreakDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(ConnectionStringName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddProviders();

            return services;
        }

        public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder builder)
        {
            var scope = builder.ApplicationServices.CreateAsyncScope();

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<HaveBreakDbContext>();

                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }

                if (SeedData)
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserAccount>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
                    await HaveBreakSeed.SeedSuperAdminAsync(context, roleManager, userManager);

                    await HaveBreakSeed.SeedStaticRolesAsync(roleManager);
                    await HaveBreakSeed.SeedDefaultUserAsync(userManager, roleManager, Constants.DefaultPassword);


                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while migrating or seeding the database.");

                throw;
            }
        }


        private static void AddProviders(this IServiceCollection services)
        {
            services.AddScoped<IPostProvider, PostProvider>();
            services.AddScoped<IAccountProvider, AccountProvider>();

        }
    }
}
