using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Managed heap memory region
/// </summary>
public partial class ManagedHeapMemoryRegion : SafeHandleMinusOneIsInvalid
{

    #region Properties
    /// <summary>
    /// Managed heap memory region buffer
    /// </summary>
    public int[] Buffer { get; }

    /// <summary>
    /// Managed heap memory region size
    /// </summary>
    public nuint Size { get; }

    /// <summary>
    /// Managed heap memory region handle
    /// </summary>
    public GCHandle Handle { get; }

    /// <summary>
    /// Managed heap memory region pointer
    /// </summary>
    public nint Pointer { get; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="size"></param>
    public ManagedHeapMemoryRegion(int size) : base(true)
    {
        Buffer = new int[size];
        Size = (nuint)Buffer.Length * sizeof(int);
        Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
        Pointer = Handle.AddrOfPinnedObject();
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
    public bool CompareWith(ManagedHeapMemoryRegion region)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Size == region.Size && CompareRegionsLinux(Pointer, region.Pointer, Size) == 0;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Size == region.Size && CompareRegionsWindows(Pointer, region.Pointer, Size) == 0;
        else
            throw new("The current OS platform is not supported.");
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        Handle.Free();
        return true;
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
    private static partial IntPtr FillRegionLinux(IntPtr region, int fill, nuint size);

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