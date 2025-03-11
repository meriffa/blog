using CommandLine;
using Microsoft.Diagnostics.Runtime;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Dump controller
/// </summary>
public abstract class DumpController : Controller
{

    #region Properties
    /// <summary>
    /// Dump file
    /// </summary>
    [Option('d', "dumpFile", Required = true, HelpText = "Dump file.")]
    public string DumpFile { get; set; } = null!;

    /// <summary>
    /// DAC library file
    /// </summary>
    [Option("dacFile", HelpText = "DAC library file.")]
    public string? DacFile { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return DataTarget instance
    /// </summary>
    /// <returns></returns>
    protected DataTarget GetDataTarget() => DataTarget.LoadDump(DumpFile);

    /// <summary>
    /// Return DataTarget instance
    /// </summary>
    /// <param name="dumpFile"></param>
    /// <returns></returns>
    protected static DataTarget GetDataTarget(string dumpFile) => DataTarget.LoadDump(dumpFile);

    /// <summary>
    /// Return ClrRuntime instance
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected ClrRuntime GetClrRuntime(DataTarget target)
    {
        if (target.DataReader.Architecture != RuntimeInformation.ProcessArchitecture || target.DataReader.PointerSize != IntPtr.Size)
            throw new($"The core dump architecture '{target.DataReader.Architecture}' does not match the current process architecture.");
        if (!RuntimeInformation.IsOSPlatform(target.DataReader.TargetPlatform))
            throw new($"The core dump platform '{target.DataReader.TargetPlatform}' does not match the current process platform.");
        return string.IsNullOrEmpty(DacFile) ? target.ClrVersions[0].CreateRuntime() : target.ClrVersions[0].CreateRuntime(DacFile);
    }

    /// <summary>
    /// Return formatted address value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetAddress(ulong? value) => value != null ? $"0x{value:X16}" : "N/A";

    /// <summary>
    /// Return formatted size
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static string GetSize(ulong? value) => GetSize(value ?? 0);

    /// <summary>
    /// Return formatted size
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static string GetSize(ulong value) => $"{value:n0}";

    /// <summary>
    /// Return formatted size
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static string GetSize(int value) => $"{value:n0}";

    /// <summary>
    /// Return formatted metadata token
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static string GetToken(int value) => $"{value:X16}";

    /// <summary>
    /// Parse address value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static ulong ParseAddress(string value) => Convert.ToUInt64(value, 16);

    /// <summary>
    /// Return module by name
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    protected static ClrModule GetModule(ClrRuntime runtime, string moduleName)
    {
        var name = $"{Path.DirectorySeparatorChar}{moduleName}";
        return runtime.EnumerateModules().FirstOrDefault(i => i.Name != null && i.Name.EndsWith(name)) ?? throw new($"Module '{moduleName}' is not found.");
    }
    #endregion

}