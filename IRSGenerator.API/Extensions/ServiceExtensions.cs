using IRSGenerator.Data;
using Microsoft.AspNetCore.Authentication.Negotiate;
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
        services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();
        return services;
    }

    public static IServiceCollection ConfigureAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization();
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
