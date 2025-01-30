using ByteZoo.Blog.Web.Interfaces;
using ByteZoo.Blog.Web.Options;
using ByteZoo.Blog.Web.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;

namespace ByteZoo.Blog.Web.Extensions;

/// <summary>
/// IServiceCollection extension methods
/// </summary>
public static class IServiceCollectionExtensions
{

    #region Public Methods
    /// <summary>
    /// Register services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration) => services
        .Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"))
        .Configure<WebServerOptions>(configuration.GetRequiredSection(Assembly.GetExecutingAssembly().GetName().Name!))
        .AddScoped<IWeatherService, WeatherService>();

    /// <summary>
    /// REgister Swagger services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddServicesSwagger(this IServiceCollection services) => services.AddSwaggerGen();
    #endregion

}