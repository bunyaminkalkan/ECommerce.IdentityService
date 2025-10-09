using ECommerce.IdentityService.API.Data.Context;
using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.Middlewares;
using ECommerce.IdentityService.API.Options;
using ECommerce.IdentityService.API.Services;
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
        services.AddIdentityCore<User>(options =>
        {
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<UserManager<User>>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        services.AddScoped<IUserValidator<User>, UserValidator<User>>();
        services.AddScoped<IPasswordValidator<User>, PasswordValidator<User>>();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(5);
        });
        #endregion


        #region Interfaces
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITokenService, TokenService>();
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

        services.AddAuthentication()
            .AddJwtBearer();

        services.AddAuthorization();
        #endregion

        return services;
    }
}
