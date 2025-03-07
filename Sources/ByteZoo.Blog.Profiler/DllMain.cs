using ByteZoo.Blog.Profiler.Services;
using Silhouette;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Profiler;

/// <summary>
/// Profiler class
/// </summary>
public class DllMain
{

    #region Constants
    private static readonly Guid PROFILER_CLSID = new("ECB669ED-DDD3-4BCD-85C8-A023EC310FE2");
    #endregion

    #region Public Methods
    /// <summary>
    /// Register profiler interface
    /// </summary>
    /// <param name="rclsid"></param>
    /// <param name="riid"></param>
    /// <param name="ppv"></param>
    /// <returns></returns>
    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, IntPtr* ppv)
    {
        DisplayService.WriteInformation("Profiler registration started.");
        var registrationClassId = *rclsid;
        if (registrationClassId != PROFILER_CLSID)
        {
            DisplayService.WriteError($"Invalid registration class specified (CLSID = '{registrationClassId}').");
            return HResult.E_NOINTERFACE;
        }
        *ppv = new ClassFactory(new CorProfilerCallback()).IClassFactory;
        DisplayService.WriteInformation("Profiler registration completed.");
        return HResult.S_OK;
    }
    #endregion

}