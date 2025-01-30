using ByteZoo.Blog.Web.Extensions;

namespace ByteZoo.Blog.Web;

/// <summary>
/// Application class
/// </summary>
public class Program
{

    #region Initialization
    /// <summary>
    /// Application entry
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args) => CreateApplication(CreateBuilder(args)).Run();
    #endregion

    #region Private Methods
    /// <summary>
    /// Create web application builder
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static WebApplicationBuilder CreateBuilder(string[] args) => ConfigureServices(WebApplication.CreateBuilder(args));

    /// <summary>
    /// Configure services
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static WebApplicationBuilder ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        services.AddServices(configuration);
        services.AddControllers();
        services.AddServicesSwagger();
        services.AddRazorPages();
        return builder;
    }

    /// <summary>
    /// Create web application
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static WebApplication CreateApplication(WebApplicationBuilder builder)
    {
        var application = builder.Build();
        if (!application.Environment.IsDevelopment())
        {
            application.UseExceptionHandler("/Error");
            application.UseHsts();
        }
        application.UseHttpsRedirection();
        application.MapStaticAssets();
        application.UseRouting();
        application.UseServicesSwagger();
        application.MapControllers();
        application.MapRazorPages().WithStaticAssets();
        return application;
    }
    #endregion

}