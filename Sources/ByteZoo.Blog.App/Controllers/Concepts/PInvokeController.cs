using CommandLine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// P/Invoke controller
/// </summary>
[Verb("Concepts-PInvoke", HelpText = "P/Invoke operation.")]
public partial class PInvokeController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var size = 256;
        byte fillValue = 0x03;
        nuint byteCount = (nuint)size * sizeof(int);
        var (buffer1, handle1, pointer1) = AllocateRegion(size);
        var (buffer2, handle2, pointer2) = AllocateRegion(size);
        try
        {
            displayService.WriteInformation($"Memory regions allocated (Buffer 1 = {buffer1.Sum()}, Buffer 2 = {buffer2.Sum()}).");
            FillRegion(pointer1, byteCount, fillValue);
            FillRegion(pointer2, byteCount, fillValue);
            displayService.WriteInformation($"Memory regions filled (Buffer 1 = {GetSum(buffer1)}, Buffer 2 = {GetSum(buffer2)}).");
            var match = CompareRegions(pointer1, pointer2, (nuint)size) == 0;
            displayService.WriteInformation($"Memory regions compared (Match = {match}).");
            ZeroRegion(pointer1, byteCount);
            ZeroRegion(pointer2, byteCount);
            displayService.WriteInformation($"Memory regions cleared (Buffer 1 = {buffer1.Sum()}, Buffer 2 = {buffer2.Sum()}).");
            displayService.Wait();
        }
        finally
        {
            handle1.Free();
            handle2.Free();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return array sum (unchecked)
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetSum(int[] buffer) => buffer.Aggregate((sum, i) => unchecked(sum + i));

    /// <summary>
    /// Allocate memory region
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private static (int[] buffer, GCHandle handle, IntPtr pointer) AllocateRegion(int size)
    {
        int[] buffer = new int[size];
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var pointer = handle.AddrOfPinnedObject();
        return (buffer, handle, pointer);
    }

    /// <summary>
    /// Fill memory region with zeros
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    private static void ZeroRegion(IntPtr region, nuint size)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ZeroRegionWindows(region, size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ZeroRegionLinux(region, size);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Fill memory region with specified fill value
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    /// <param name="fill"></param>
    private static void FillRegion(IntPtr region, nuint size, byte fill)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            FillRegionWindows(region, size, fill);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            FillRegionLinux(region, fill, size);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Compare memory regions
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static int CompareRegions(IntPtr region1, IntPtr region2, nuint size)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return CompareRegionsWindows(region1, region2, size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return CompareRegionsLinux(region1, region2, size);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Fill memory region with zeros (Windows)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
    private static partial void ZeroRegionWindows(IntPtr region, nuint size);

    /// <summary>
    /// Fill memory region with specified fill value (Windows)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    /// <param name="fill"></param>
    [LibraryImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
    private static partial void FillRegionWindows(IntPtr region, nuint size, byte fill);

    /// <summary>
    /// Compare memory regions (Windows)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [LibraryImport("msvcrt.dll", EntryPoint = "memcmp"), UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int CompareRegionsWindows(IntPtr region1, IntPtr region2, nuint size);

    /// <summary>
    /// Fill memory region with zeros (Linux)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("libc", EntryPoint = "bzero")]
    private static partial void ZeroRegionLinux(IntPtr region, nuint size);

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
    /// Compare memory regions (Linux)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [LibraryImport("libc", EntryPoint = "memcmp")]
    private static partial int CompareRegionsLinux(IntPtr region1, IntPtr region2, nuint size);
    #endregion

}