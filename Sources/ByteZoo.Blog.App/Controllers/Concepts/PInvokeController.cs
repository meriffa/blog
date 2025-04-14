using ByteZoo.Blog.Common.InteropServices;
using CommandLine;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// P/Invoke controller
/// </summary>
[Verb("Concepts-PInvoke", HelpText = "P/Invoke operation.")]
public partial class PInvokeController : Controller
{

    #region Structures
    /// <summary>
    /// Structure 1 (Size = 1, 2, 4, 8)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct Structure1
    {

        #region Public Members
        public int Field1;
        public int Field2;
        #endregion

    }

    /// <summary>
    /// Structure 2 (Size = 3, 5, 6, 7, >= 9)
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct Structure2
    {

        #region Public Members
        [FieldOffset(0)]
        public byte Field1;
        [FieldOffset(1)]
        public int Field2;
        #endregion

    }

    /// <summary>
    /// Structure 3 (Size = 16 bytes)
    /// </summary>
    [NativeMarshalling(typeof(Structure3Marshaller))]
    public struct Structure3
    {

        #region Public Members
        public long Field1;
        public long Field2;
        #endregion

    }

    /// <summary>
    /// Structure 4 (Size = 24 bytes)
    /// </summary>
    [NativeMarshalling(typeof(Structure4Marshaller))]
    public struct Structure4
    {

        #region Public Members
        public long Field1;
        public long Field2;
        public long Field3;
        #endregion

    }
    #endregion

    #region Marshallers
    /// <summary>
    /// Decimal marshaller
    /// </summary>
    [CustomMarshaller(typeof(decimal), MarshalMode.Default, typeof(DecimalMarshaller))]
    private static class DecimalMarshaller
    {

        #region Structures
        /// <summary>
        /// Decimal (unmanaged)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DecimalUnmanaged
        {

            #region Public Members
            public ulong _lo64;
            public uint _hi32;
            public int _flags;
            #endregion

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Convert managed to unmanaged
        /// </summary>
        /// <param name="managed"></param>
        /// <returns></returns>
        public static DecimalUnmanaged ConvertToUnmanaged(decimal managed)
        {
            var bits = decimal.GetBits(managed);
            return new DecimalUnmanaged { _lo64 = (((ulong)bits[1]) << 32) + (ulong)bits[0], _hi32 = (uint)bits[2], _flags = bits[3] };
        }

        /// <summary>
        /// Convert unmanaged to managed
        /// </summary>
        /// <param name="unmanaged"></param>
        /// <returns></returns>
        public static decimal ConvertToManaged(DecimalUnmanaged unmanaged) => new([(int)(unmanaged._lo64 & 0xFFFFFFFF), (int)(unmanaged._lo64 >> 32), (int)unmanaged._hi32, unmanaged._flags]);
        #endregion

    }

    /// <summary>
    /// Structure 3 marshaller
    /// </summary>
    [CustomMarshaller(typeof(Structure3), MarshalMode.Default, typeof(Structure3Marshaller))]
    internal static class Structure3Marshaller
    {

        #region Structures
        /// <summary>
        /// Structure 3 (unmanaged)
        /// </summary>
        internal struct Structure3Unmanaged
        {

            #region Public Members
            public long _Field1;
            public long _Field2;
            #endregion

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Convert managed to unmanaged
        /// </summary>
        /// <param name="managed"></param>
        /// <returns></returns>
        public static Structure3Unmanaged ConvertToUnmanaged(Structure3 managed) => new() { _Field1 = managed.Field1, _Field2 = managed.Field2 };

        /// <summary>
        /// Convert unmanaged to managed
        /// </summary>
        /// <param name="unmanaged"></param>
        /// <returns></returns>
        public static Structure3 ConvertToManaged(Structure3Unmanaged unmanaged) => new() { Field1 = unmanaged._Field1, Field2 = unmanaged._Field2 };
        #endregion

    }

    /// <summary>
    /// Structure 4 marshaller
    /// </summary>
    [CustomMarshaller(typeof(Structure4), MarshalMode.Default, typeof(Structure4Marshaller))]
    internal static class Structure4Marshaller
    {

        #region Structures
        /// <summary>
        /// Structure 4 (unmanaged)
        /// </summary>
        internal struct Structure4Unmanaged
        {

            #region Public Members
            public long _Field1;
            public long _Field2;
            public long _Field3;
            #endregion

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Convert managed to unmanaged
        /// </summary>
        /// <param name="managed"></param>
        /// <returns></returns>
        public static Structure4Unmanaged ConvertToUnmanaged(Structure4 managed) => new() { _Field1 = managed.Field1, _Field2 = managed.Field2, _Field3 = managed.Field3 };

        /// <summary>
        /// Convert unmanaged to managed
        /// </summary>
        /// <param name="unmanaged"></param>
        /// <returns></returns>
        public static Structure4 ConvertToManaged(Structure4Unmanaged unmanaged) => new() { Field1 = unmanaged._Field1, Field2 = unmanaged._Field2, Field3 = unmanaged._Field3 };
        #endregion

    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), AssemblyLibraryResolver.Resolve);
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
            displayService.WriteInformation($"_Function1(): Result = {AssemblyFunction1(true, 2, 3, '\u0004', 5, 6, 7, 8, 9, 10)}");
            displayService.WriteInformation($"_Function2(): Result = {AssemblyFunction2(1.01f, 2.02d, 3.03f, 4.04d, 5.05f, 6.06d, 7.07f, 8.08d, 9.09f, 10.10d)}");
            var result3 = AssemblyFunction3(pointer1, 3.03f, 4.04d, byteCount);
            displayService.WriteInformation($"_Function3(): Result = [{result3.Field1}, {result3.Field2:X16}]");
            var p3 = new Structure1 { Field1 = 0x55555555, Field2 = 0x66666666 };
            var p5 = 7.89m;
            var p7 = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            var result4 = AssemblyFunction4(new Structure1 { Field1 = 0x11111111, Field2 = 0x22222222 }, new Structure2 { Field1 = 0x33, Field2 = 0x44444444 }, ref p3, out var p4, ref p5, "Input text.", p7, p7.Length);
            displayService.WriteInformation($"_Function4(): Result = [{result4.Field1:X16}, {result4.Field2:X16}, {result4.Field3:X16}], P3 = [{p3.Field1:X8}, {p3.Field2:X8}], P4 = [{p4.Field1:X8}, {p4.Field2:X8}]");
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
    /// Fill memory region with specified fill value
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    /// <param name="fill"></param>
    private static void FillRegion(IntPtr region, nuint size, byte fill)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            FillRegionLinux(region, fill, size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            FillRegionWindows(region, size, fill);
        else
            throw new("The current OS platform is not supported.");
    }

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
    /// Return array sum (unchecked)
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetSum(int[] buffer) => buffer.Aggregate((sum, i) => unchecked(sum + i));

    /// <summary>
    /// Compare memory regions
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static int CompareRegions(IntPtr region1, IntPtr region2, nuint size)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return CompareRegionsLinux(region1, region2, size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return CompareRegionsWindows(region1, region2, size);
        else
            throw new("The current OS platform is not supported.");
    }

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

    /// <summary>
    /// Fill memory region with zeros
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    private static void ZeroRegion(IntPtr region, nuint size)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ZeroRegionLinux(region, size);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ZeroRegionWindows(region, size);
        else
            throw new("The current OS platform is not supported.");
    }

    /// <summary>
    /// Fill memory region with zeros (Linux)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("libc", EntryPoint = "bzero")]
    private static partial void ZeroRegionLinux(IntPtr region, nuint size);

    /// <summary>
    /// Fill memory region with zeros (Windows)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="size"></param>
    [LibraryImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
    private static partial void ZeroRegionWindows(IntPtr region, nuint size);

    /// <summary>
    /// Assembly function 1 (integral parameters)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <param name="p9"></param>
    /// <param name="p10"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "_Function1", SetLastError = false), SuppressGCTransition]
    private static partial long AssemblyFunction1([MarshalAs(UnmanagedType.U1)] bool p1, byte p2, sbyte p3, [MarshalAs(UnmanagedType.U2)] char p4, short p5, ushort p6, int p7, uint p8, long p9, ulong p10);

    /// <summary>
    /// Assembly function 2 (float parameters)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <param name="p9"></param>
    /// <param name="p10"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "_Function2", SetLastError = false), SuppressGCTransition]
    private static partial double AssemblyFunction2(float p1, double p2, float p3, double p4, float p5, double p6, float p7, double p8, float p9, double p10);

    /// <summary>
    /// Assembly function 3 (mixed parameters)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "_Function3", SetLastError = false)]
    private static partial Structure3 AssemblyFunction3(IntPtr p1, float p2, double p3, UIntPtr p4);

    /// <summary>
    /// Assembly function 4 (composite parameters)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "_Function4", SetLastError = false, StringMarshalling = StringMarshalling.Utf8)]
    private static partial Structure4 AssemblyFunction4(Structure1 p1, Structure2 p2, ref Structure1 p3, out Structure1 p4, [MarshalUsing(typeof(DecimalMarshaller))] ref decimal p5, string p6, in int[] p7, int p8);
    #endregion

}