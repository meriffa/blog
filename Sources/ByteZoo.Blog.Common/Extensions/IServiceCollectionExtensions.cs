using Microsoft.Extensions.DependencyInjection;

namespace ByteZoo.Blog.Common.Extensions;

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
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services) => services.AddSingleton<Services.DisplayService>();
    #endregion

}