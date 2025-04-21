using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Memory region
/// </summary>
public abstract partial class MemoryRegion : SafeHandle
{

    #region Properties
    /// <summary>
    /// Memory region pointer
    /// </summary>
    public nint Pointer { get; init; }

    /// <summary>
    /// Memory region size
    /// </summary>
    public nuint Size { get; init; }

    /// <inheritdoc/>
    public override bool IsInvalid => Pointer == 0;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public MemoryRegion() : base(0, true)
    {
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Fill memory region with specified fill value
    /// </summary>
    /// <param name="fill"></param>
    public void Fill(byte fill)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            FillRegionLinux(Pointer, fill, Size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            FillRegionWindows(Pointer, Size, fill);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Fill memory region with zeros
    /// </summary>
    public void Clear()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ClearRegionLinux(Pointer, Size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ClearRegionWindows(Pointer, Size);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Compare memory regions
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public bool CompareWith(MemoryRegion region)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Size == region.Size && CompareRegionsLinux(Pointer, region.Pointer, Size) == 0;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Size == region.Size && CompareRegionsWindows(Pointer, region.Pointer, Size) == 0;
        else
            throw new("The current OS platform is not supported.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Fill memory region with specified fill value (Linux)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="fill"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [LibraryImport("libc", EntryPoint = "memset")]
    private static partial IntPtr FillRegionLinux(IntPtr region, byte fill, nuint size);

    /// <summary>
    /// Fill memory region with specified fill value (Windows)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    /// <param name="fill"></param>
    [LibraryImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
    private static partial void FillRegionWindows(IntPtr region, nuint size, byte fill);

    /// <summary>
    /// Fill memory region with zeros (Linux)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("libc", EntryPoint = "bzero")]
    private static partial void ClearRegionLinux(IntPtr region, nuint size);

    /// <summary>
    /// Fill memory region with zeros (Windows)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
    private static partial void ClearRegionWindows(IntPtr region, nuint size);

    /// <summary>
    /// Compare memory regions (Linux)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [LibraryImport("libc", EntryPoint = "memcmp")]
    private static partial int CompareRegionsLinux(IntPtr region1, IntPtr region2, nuint size);

    /// <summary>
    /// Compare memory regions (Windows)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [LibraryImport("msvcrt.dll", EntryPoint = "memcmp"), UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int CompareRegionsWindows(IntPtr region1, IntPtr region2, nuint size);
    #endregion

}