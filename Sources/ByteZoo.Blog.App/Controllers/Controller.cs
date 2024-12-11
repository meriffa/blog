using ByteZoo.Blog.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ByteZoo.Blog.App.Controllers;

/// <summary>
/// Base controller
/// </summary>
public abstract class Controller
{

    #region Protected Fields
    protected IServiceProvider services = null!;
    protected DisplayService displayService = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    /// <param name="services"></param>
    public void Execute(IServiceProvider services)
    {
        this.services = services;
        InitializeServices();
        Execute();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Initialize services
    /// </summary>
    protected virtual void InitializeServices() => displayService = services.GetRequiredService<DisplayService>();

    /// <summary>
    /// Execute controller
    /// </summary>
    protected abstract void Execute();
    #endregion

}