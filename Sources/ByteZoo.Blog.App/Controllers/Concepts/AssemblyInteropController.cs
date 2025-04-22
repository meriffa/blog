using ByteZoo.Blog.Common.Interop;
using CommandLine;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Assembly Interop controller
/// </summary>
[Verb("Concepts-AssemblyInterop", HelpText = "Assembly Interop operation.")]
public partial class AssemblyInteropController : Controller
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

    #region Delegates
    /// <summary>
    /// Assembly function 5 callback
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private delegate long AssemblyFunction5Callback(long a, long b);
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), AssemblyLibraryResolver.Resolve);
        var size = 256;
        nuint byteCount = (nuint)size * sizeof(int);
        using var region = new MemoryRegionManaged(size);
        region.Fill(0x03);
        displayService.WriteInformation($"Function 1: Result = {AssemblyFunction1(true, 2, 3, '\u0004', 5, 6, 7, 8, 9, 10)}");
        displayService.WriteInformation($"Function 2: Result = {AssemblyFunction2(1.01f, 2.02d, 3.03f, 4.04d, 5.05f, 6.06d, 7.07f, 8.08d, 9.09f, 10.10d)}");
        var result3 = AssemblyFunction3(region.Pointer, 3.03f, 4.04d, byteCount);
        displayService.WriteInformation($"Function 3: Result = [{result3.Field1}, {result3.Field2}]");
        var p3 = new Structure1 { Field1 = 0x55555555, Field2 = 0x66666666 };
        var p5 = 7.89m;
        var p7 = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        var result4 = AssemblyFunction4(new Structure1 { Field1 = 0x11111111, Field2 = 0x22222222 }, new Structure2 { Field1 = 0x33, Field2 = 0x44444444 }, ref p3, out var p4, ref p5, "Input text.", p7, p7.Length);
        displayService.WriteInformation($"Function 4: Result = [{result4.Field1:X16}, {result4.Field2}, {result4.Field3}], P3 = [{p3.Field1:X8}, {p3.Field2:X8}], P4 = [{p4.Field1:X8}, {p4.Field2:X8}]");
        displayService.WriteInformation($"Function 5: Result = {AssemblyFunction5(111, 222, (a, b) => a + b)}");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
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
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "Assembly_Function1", SetLastError = false), SuppressGCTransition]
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
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "Assembly_Function2", SetLastError = false), SuppressGCTransition]
    private static partial double AssemblyFunction2(float p1, double p2, float p3, double p4, float p5, double p6, float p7, double p8, float p9, double p10);

    /// <summary>
    /// Assembly function 3 (mixed parameters)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "Assembly_Function3", SetLastError = false)]
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
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "Assembly_Function4", SetLastError = false, StringMarshalling = StringMarshalling.Utf8)]
    private static partial Structure4 AssemblyFunction4(Structure1 p1, Structure2 p2, ref Structure1 p3, out Structure1 p4, [MarshalUsing(typeof(DecimalMarshaller))] ref decimal p5, string p6, in int[] p7, int p8);

    /// <summary>
    /// Assembly function 5 (managed callback function)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "Assembly_Function5", SetLastError = false)]
    private static partial long AssemblyFunction5(long p1, long p2, AssemblyFunction5Callback p3);
    #endregion

}