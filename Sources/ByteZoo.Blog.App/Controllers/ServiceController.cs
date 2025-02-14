using CommandLine;

namespace ByteZoo.Blog.App.Controllers;

/// <summary>
/// Base controller
/// </summary>
public abstract class ServiceController : Controller
{

    #region Private Members
    private static readonly HttpClientHandler handler = new() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
    #endregion

    #region Properties
    /// <summary>
    /// Host URL
    /// </summary>
    [Option('h', "url", Default = "https://localhost:7543", HelpText = "Host URL.")]
    public string HostUrl { get; set; } = null!;

    /// <summary>
    /// Host timeout
    /// </summary>
    [Option('t', "timeout", Default = 300, HelpText = "Host timeout.")]
    public int HostTimeout { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return HttpClient instance
    /// </summary>
    /// <returns></returns>
    protected HttpClient GetHttpClient() => new(handler) { BaseAddress = new Uri(HostUrl), Timeout = TimeSpan.FromSeconds(HostTimeout) };
    #endregion

}