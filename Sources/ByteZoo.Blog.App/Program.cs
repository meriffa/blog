using ByteZoo.Blog.Common.Exceptions;
using ByteZoo.Blog.Common.Extensions;
using ByteZoo.Blog.Common.Services;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ByteZoo.Blog.App;

/// <summary>
/// Application class
/// </summary>
class Program
{

    #region Initialization
    /// <summary>
    /// Application entry
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static int Main(string[] args)
    {
        var host = CreateApplicationHost(args);
        var displayService = host.Services.GetRequiredService<DisplayService>();
        try
        {
            DisplayService.WriteTitle("ByteZoo.Blog Application");
            displayService.WriteInformation($"ByteZoo.Blog application started.");
            Parser.Default
                .ParseArguments(args, [.. Assembly.GetExecutingAssembly().GetTypes().Where(i => i.GetCustomAttributes(typeof(VerbAttribute), true).Length > 0)])
                .WithParsed<Controllers.Controller>(i => i.Execute(host.Services));
            displayService.WriteInformation($"ByteZoo.Blog application completed.");
            return 0;
        }
        catch (Exception ex) when (ex is not UnhandledException && ex is not ThreadInterruptedException)
        {
            displayService.WriteError(ex);
            return -1;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create application host
    /// </summary>
    /// <param name="args"></param>
    private static IHost CreateApplicationHost(string[] args) => Host
        .CreateDefaultBuilder()
        .ConfigureHost(args)
        .ConfigureServices((context, services) => services.AddServices())
        .Build();
    #endregion

}