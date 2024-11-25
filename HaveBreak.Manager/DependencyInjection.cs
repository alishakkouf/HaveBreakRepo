using HaveBreak.Domain.Accounts;
using HaveBreak.Domain.Posts;
using HaveBreak.Manager.Accounts;
using HaveBreak.Manager.Posts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HaveBreak.Manager
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureManagerModule(this IServiceCollection services,
                                                                     IConfiguration configuration)
        {
            services.AddScoped<IPostManager, PostManager>();
            services.AddScoped<IAccountManager, AccountManager>();

            return services;
        }
    }
}
