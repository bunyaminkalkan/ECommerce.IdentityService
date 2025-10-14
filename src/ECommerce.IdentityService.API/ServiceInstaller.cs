using ECommerce.BuildingBlocks.Shared.Kernel.Auth.Options;
using ECommerce.IdentityService.API.Data.Context;
using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.Middlewares;
using ECommerce.IdentityService.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Space.DependencyInjection;

namespace ECommerce.IdentityService.API;

public static class ServiceInstaller
{
    private const string SectionName = "PostgreSQL";

    public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region OpenApi
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    }
                };

                document.SecurityRequirements = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    }
                };

                return Task.CompletedTask;
            });
        });
        #endregion

        #region DB
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString(SectionName));
        });

        //identity
        services.AddIdentity<User, Role>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Token settings
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Token lifespan ayarı
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(5);
        });
        #endregion


        #region Interfaces
        services.AddScoped<IJwtService, JwtService>();
        #endregion

        #region Exceptions
        services.AddScoped<ExceptionMiddleware>();
        #endregion

        #region Space
        services.AddSpace(configuration =>
        {
            configuration.ServiceLifetime = ServiceLifetime.Scoped;
        });
        #endregion

        #region Auth
        services.AddHttpContextAccessor();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<JwtOptions>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();

        services.AddAuthorization();
        #endregion

        return services;
    }
}
