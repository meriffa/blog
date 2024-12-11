using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ByteZoo.Blog.Common.Extensions;

/// <summary>
/// IHostBuilder extension methods
/// </summary>
public static class IHostBuilderExtensions
{

    #region Public Methods
    /// <summary>
    /// Configure host
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureHost(this IHostBuilder host, string[] args) => host
        .ConfigureHostConfiguration((configuration) =>
        {
            configuration.AddEnvironmentVariables();
        })
        .ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            configuration.AddEnvironmentVariables();
            configuration.AddCommandLine(args);
        });
    #endregion

}