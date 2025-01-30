namespace ByteZoo.Blog.Web.Extensions;

/// <summary>
/// IApplicationBuilder extension methods
/// </summary>
public static class IApplicationBuilderExtensions
{

    #region Public Methods
    /// <summary>
    /// Initialize Swagger services
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseServicesSwagger(this IApplicationBuilder application) => application
        .UseSwagger()
        .UseSwaggerUI();
    #endregion

}