using HaveBreak.Common;
using HaveBreak.Common.Filters;
using HaveBreak.Common.Middlewares;
using HaveBreak.Data;
using HaveBreak.Data.Models;
using HaveBreak.Domain.Settings;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Text;

namespace HaveBreak.API.Configuration
{
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Add Controllers, AutoMapper, Cors
        /// </summary>
        internal static IServiceCollection ConfigureApiControllers(this IServiceCollection services, IConfiguration configuration, string corsPolicyName)
        {
            services.AddControllers(config =>
            {
                config.Filters.Add(new ValidationFilterAttribute());
                config.Filters.Add(new NormalizeFilterAttribute());
            })
                .AddDataAnnotationsLocalization(o =>
                {
                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(CommonResource));
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            services.AddMemoryCache();

            services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Resources";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = SupportedLanguages.ListAll.Select(x => new CultureInfo(x)).ToList();

                options.DefaultRequestCulture = new RequestCulture(SupportedLanguages.English);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.FallBackToParentUICultures = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();

            return services;
        }


        /// <summary>
        /// Add Identity specific services, and Api Bearer Authentication
        /// </summary>
        internal static IServiceCollection ConfigureApiIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            //configure password
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.AddIdentityCore<UserAccount>()
                .AddRoles<UserRole>()
                .AddEntityFrameworkStores<HaveBreakDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))

                    };
                });

            services.AddSingleton<IAuthorizationMiddlewareResultHandler, FailedAuthorizationWrapperHandler>();

            return services;
        }

        /// <summary>
        /// Add Swagger Documentation
        /// </summary>
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "HaveBreak", Version = "v1" });

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Application.xml");
                var sharedXmlPath = Path.Combine(basePath, "Shared.xml");

                //x.IncludeXmlComments(xmlPath);
                //x.IncludeXmlComments(sharedXmlPath);

                //Adding API http header
                //x.OperationFilter<SwaggerHttpHeaderFilter>();

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Id="Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                        }, new List<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Register Swagger, Swagger UI middlewares
        /// </summary>
        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseSwagger();

            var uri = new Uri(config["App:ServerRootAddress"]);
            var basePath = uri.LocalPath.TrimEnd('/');

            app.UseSwaggerUI(c =>
            {
                c.DisplayRequestDuration();
                c.SwaggerEndpoint($"{basePath}/swagger/v1/swagger.json", "HaveBreak");
            });

            return app;
        }
    }
}
