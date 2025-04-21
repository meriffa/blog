using System.Reflection;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Assembly library resolver
/// </summary>
public static class AssemblyLibraryResolver
{

    #region Public Methods
    /// <summary>
    /// Resolve assembly library
    /// </summary>
    /// <param name="libraryName"></param>
    /// <param name="assembly"></param>
    /// <param name="searchPath"></param>
    /// <returns></returns>
    public static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../../../ByteZoo.Blog.Asm/bin", libraryName.EndsWith(".so") ? libraryName : libraryName + ".so");
        if (File.Exists(path))
            return NativeLibrary.Load(path, assembly, searchPath);
        return IntPtr.Zero;
    }
    #endregion

}