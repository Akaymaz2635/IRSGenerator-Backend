using IRSGenerator.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureDatabaseContext(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IRSGeneratorDbContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }

    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services)
    {
        services.AddAuthentication("AppCookie")
            .AddCookie("AppCookie", options =>
            {
                options.Cookie.Name     = "qc_auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                // API — yönlendirme yerine HTTP durum kodları dön
                options.Events.OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
        return services;
    }

    public static IServiceCollection ConfigureAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Mühendis dışındaki her kimliğe doğrulanmış kullanıcı yazabilir
            options.AddPolicy("CanWrite", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireAssertion(ctx =>
                          ctx.User.FindFirst("role")?.Value != "engineer"));

            // Yalnızca admin
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireAssertion(ctx =>
                          ctx.User.FindFirst("role")?.Value == "admin"));

            // NCM sayfası — engineer + admin yazabilir (inspector okur)
            options.AddPolicy("CanWriteNcm", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireAssertion(ctx =>
                      {
                          var role = ctx.User.FindFirst("role")?.Value;
                          return role == "engineer" || role == "admin";
                      }));
        });
        return services;
    }

    public static IServiceCollection ConfigureMapper(
        this IServiceCollection services)
    {
        // AutoMapper veya başka bir mapper gerekirse buraya ekle
        return services;
    }

    public static IServiceCollection ConfigureServices(
        this IServiceCollection services)
    {
        // Uygulama servisleri buraya eklenebilir
        return services;
    }
}
