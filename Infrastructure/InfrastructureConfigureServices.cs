using System.Text;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.SettingsModels;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class InfrastructureConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddOptions<ImageKitSettings>()
            .Bind(configuration.GetSection("ImageKitSettings"))
            .ValidateDataAnnotations();

        services.AddOptions<JWTSettings>()
            .Bind(configuration.GetSection("JWTSettings"))
            .ValidateDataAnnotations();
        
        services.AddScoped<AuditableEntitySaveInterceptor>();

        services.AddDbContext<MoodBoardDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(MoodBoardDbContext).Assembly.FullName)));

        services.AddScoped<IMoodBoardDbContext, MoodBoardDbContext>();

        services.AddIdentity<MoodBoardUser, IdentityRole>()
            .AddEntityFrameworkStores<MoodBoardDbContext>();
        var jwtSettings = configuration.GetSection("JwtSettings");

        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IImageProcessingService, ImageProcessingService>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
            };
            options.Events = new JwtBearerEvents();
            options.Events.OnMessageReceived += context =>
            {
                if (context.Request.Cookies.ContainsKey("mood-session"))
                    context.Token = context.Request.Cookies["mood-session"];
                return Task.CompletedTask;
            };
        });

        services.AddAuthorization();
        return services;
    }
}