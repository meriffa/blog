using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ByteZoo.Blog.Common.Interop;
using ByteZoo.Blog.Common.Services;
using CommandLine;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Intrinsics controller
/// </summary>
[Verb("Concepts-Intrinsics", HelpText = "Intrinsics operation."), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
public partial class IntrinsicsController : Controller
{

    #region Constants
    private const int REGION_SIZE = 1_000_003;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), AssemblyLibraryResolver.Resolve);
        Min();
        Max();
        Sum();
        Count();
        Compare();
        displayService.Wait();
    }
    #endregion

    #region Private Methods

    #region Min
    /// <summary>
    /// Min functions
    /// </summary>
    private unsafe void Min()
    {
        var region = MemoryService.GenerateRegionInt(REGION_SIZE);
        displayService.WriteInformation($"Min = {MinLoop(region):n0} (Loop)");
        displayService.WriteInformation($"Min = {MinLinq(region):n0} (LINQ)");
        displayService.WriteInformation($"Min = {MinVector(region):n0} (Vector)");
        fixed (int* pRegion = region)
        {
            if (Avx2.IsSupported)
            {
                displayService.WriteInformation($"Min = {MinAvx2(region):n0} (AVX2)");
                displayService.WriteInformation($"Min = {MinAvx2Interop(pRegion, region.Length):n0} (AVX2 Interop)");

            }
            if (Avx512F.IsSupported)
            {
                displayService.WriteInformation($"Min = {MinAvx512(region):n0} (AVX-512)");
                displayService.WriteInformation($"Min = {MinAvx512Interop(pRegion, region.Length):n0} (AVX-512 Interop)");
            }
        }
    }

    /// <summary>
    /// Return memory region minimum (loop)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MinLoop(ReadOnlySpan<int> region)
    {
        var result = region[0];
        for (var i = 1; i < region.Length; i++)
            if (result > region[i])
                result = region[i];
        return result;
    }

    /// <summary>
    /// Return memory region minimum (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MinLinq(int[] region) => region.Min();

    /// <summary>
    /// Return memory region minimum (Variable SIMD)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MinVector(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector<int>.Count)
            return MinLoop(region);
        var vector = new Vector<int>(region);
        for (int i = Vector<int>.Count, count = region.Length - Vector<int>.Count; i <= count; i += Vector<int>.Count)
            vector = Vector.Min(vector, new Vector<int>(region[i..]));
        var result = vector[0];
        for (var i = 1; i < Vector<int>.Count; i++)
            if (result > vector[i])
                result = vector[i];
        for (int i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            if (result > region[i])
                result = region[i];
        return result;
    }

    /// <summary>
    /// Return memory region minimum (AVX2)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int MinAvx2(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector256<int>.Count)
            return MinLoop(region);
        fixed (int* pRegion = region)
        {
            var vector = Avx.LoadVector256(pRegion);
            for (int i = Vector256<int>.Count, count = region.Length - Vector256<int>.Count; i <= count; i += Vector256<int>.Count)
                vector = Avx2.Min(vector, Avx.LoadVector256(pRegion + i));
            int* resultVector = stackalloc int[Vector256<int>.Count];
            Avx.Store(resultVector, vector);
            var result = resultVector[0];
            for (int i = 1; i < Vector256<int>.Count; i++)
                if (result > resultVector[i])
                    result = resultVector[i];
            for (int i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
                if (result > region[i])
                    result = region[i];
            return result;
        }
    }

    /// <summary>
    /// Return memory region minimum (AVX2)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "MinAvx2_Interop", SetLastError = false)]
    private static unsafe partial int MinAvx2Interop(int* pRegion, int itemCount);

    /// <summary>
    /// Return memory region minimum (AVX-512)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int MinAvx512(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector512<int>.Count)
            return MinLoop(region);
        fixed (int* pRegion = region)
        {
            var vector = Avx512F.LoadVector512(pRegion);
            for (int i = Vector512<int>.Count, count = region.Length - Vector512<int>.Count; i <= count; i += Vector512<int>.Count)
                vector = Avx512F.Min(vector, Avx512F.LoadVector512(pRegion + i));
            int* resultVector = stackalloc int[Vector512<int>.Count];
            Avx512F.Store(resultVector, vector);
            var result = resultVector[0];
            for (int i = 1; i < Vector512<int>.Count; i++)
                if (result > resultVector[i])
                    result = resultVector[i];
            for (int i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
                if (result > region[i])
                    result = region[i];
            return result;
        }
    }

    /// <summary>
    /// Return memory region minimum (AVX-512)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "MinAvx512_Interop", SetLastError = false)]
    private static unsafe partial int MinAvx512Interop(int* pRegion, int itemCount);
    #endregion

    #region Max
    /// <summary>
    /// Max functions
    /// </summary>
    private unsafe void Max()
    {
        var region = MemoryService.GenerateRegionInt(REGION_SIZE);
        displayService.WriteInformation($"Max = {MaxLoop(region):n0} (Loop)");
        displayService.WriteInformation($"Max = {MaxLinq(region):n0} (LINQ)");
        displayService.WriteInformation($"Max = {MaxVector(region):n0} (Vector)");
        fixed (int* pRegion = region)
        {
            if (Avx2.IsSupported)
            {
                displayService.WriteInformation($"Max = {MaxAvx2(region):n0} (AVX2)");
                displayService.WriteInformation($"Max = {MaxAvx2Interop(pRegion, region.Length):n0} (AVX2 Interop)");
            }
            if (Avx512F.IsSupported)
            {
                displayService.WriteInformation($"Max = {MaxAvx512(region):n0} (AVX-512)");
                displayService.WriteInformation($"Max = {MaxAvx2Interop(pRegion, region.Length):n0} (AVX2 Interop)");
            }
        }
    }

    /// <summary>
    /// Return memory region maximum (loop)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MaxLoop(ReadOnlySpan<int> region)
    {
        var result = region[0];
        for (var i = 1; i < region.Length; i++)
            if (result < region[i])
                result = region[i];
        return result;
    }

    /// <summary>
    /// Return memory region maximum (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MaxLinq(int[] region) => region.Max();

    /// <summary>
    /// Return memory region maximum (Variable SIMD)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int MaxVector(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector<int>.Count)
            return MaxLoop(region);
        var vector = new Vector<int>(region);
        for (int i = Vector<int>.Count, count = region.Length - Vector<int>.Count; i <= count; i += Vector<int>.Count)
            vector = Vector.Max(vector, new Vector<int>(region[i..]));
        var result = vector[0];
        for (var i = 1; i < Vector<int>.Count; i++)
            if (result < vector[i])
                result = vector[i];
        for (int i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            if (result < region[i])
                result = region[i];
        return result;
    }

    /// <summary>
    /// Return memory region maximum (AVX2)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int MaxAvx2(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector256<int>.Count)
            return MaxLoop(region);
        fixed (int* pRegion = region)
        {
            var vector = Avx.LoadVector256(pRegion);
            for (int i = Vector256<int>.Count, count = region.Length - Vector256<int>.Count; i <= count; i += Vector256<int>.Count)
                vector = Avx2.Max(vector, Avx.LoadVector256(pRegion + i));
            int* resultVector = stackalloc int[Vector256<int>.Count];
            Avx.Store(resultVector, vector);
            var result = resultVector[0];
            for (int i = 1; i < Vector256<int>.Count; i++)
                if (result < resultVector[i])
                    result = resultVector[i];
            for (int i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
                if (result < region[i])
                    result = region[i];
            return result;
        }
    }

    /// <summary>
    /// Return memory region maximum (AVX2)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "MaxAvx2_Interop", SetLastError = false)]
    private static unsafe partial int MaxAvx2Interop(int* pRegion, int itemCount);

    /// <summary>
    /// Return memory region maximum (AVX-512)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int MaxAvx512(ReadOnlySpan<int> region)
    {
        if (region.Length < Vector512<int>.Count)
            return MaxLoop(region);
        fixed (int* pRegion = region)
        {
            var vector = Avx512F.LoadVector512(pRegion);
            for (int i = Vector512<int>.Count, count = region.Length - Vector512<int>.Count; i <= count; i += Vector512<int>.Count)
                vector = Avx512F.Max(vector, Avx512F.LoadVector512(pRegion + i));
            int* resultVector = stackalloc int[Vector512<int>.Count];
            Avx512F.Store(resultVector, vector);
            var result = resultVector[0];
            for (int i = 1; i < Vector512<int>.Count; i++)
                if (result < resultVector[i])
                    result = resultVector[i];
            for (int i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
                if (result < region[i])
                    result = region[i];
            return result;
        }
    }

    /// <summary>
    /// Return memory region maximum (AVX-512)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "MaxAvx512_Interop", SetLastError = false)]
    private static unsafe partial int MaxAvx512Interop(int* pRegion, int itemCount);
    #endregion

    #region Sum
    /// <summary>
    /// Sum functions
    /// </summary>
    private unsafe void Sum()
    {
        var region = MemoryService.GenerateRegionInt(REGION_SIZE);
        displayService.WriteInformation($"Sum = {SumLoop(region):n0} (Loop)");
        displayService.WriteInformation($"Sum = {SumLinq(region):n0} (LINQ)");
        displayService.WriteInformation($"Sum = {SumUnrolled(region):n0} (Unrolled)");
        displayService.WriteInformation($"Sum = {SumVector(region):n0} (Vector)");
        fixed (int* pRegion = region)
        {
            if (Avx2.IsSupported)
            {
                displayService.WriteInformation($"Sum = {SumAvx2(region):n0} (AVX2)");
                displayService.WriteInformation($"Sum = {SumAvx2Interop(pRegion, region.Length):n0} (AVX2 Interop)");
            }
            if (Avx512F.IsSupported)
            {
                displayService.WriteInformation($"Sum = {SumAvx512(region):n0} (AVX-512)");
                displayService.WriteInformation($"Sum = {SumAvx512Interop(pRegion, region.Length):n0} (AVX-512 Interop)");
            }
        }
    }

    /// <summary>
    /// Return memory region sum (loop)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int SumLoop(ReadOnlySpan<int> region)
    {
        var result = 0;
        for (var i = 0; i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int SumLinq(int[] region) => region.Aggregate(0, (current, i) => current + i);

    /// <summary>
    /// Return memory region sum (unrolled loop)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumUnrolled(ReadOnlySpan<int> region)
    {
        var result = 0;
        var i = 0;
        var lastBlockIndex = region.Length - (region.Length % 4);
        fixed (int* pRegion = region)
        {
            while (i < lastBlockIndex)
            {
                result += pRegion[i + 0];
                result += pRegion[i + 1];
                result += pRegion[i + 2];
                result += pRegion[i + 3];
                i += 4;
            }
            while (i < region.Length)
                result += pRegion[i++];
        }
        return result;
    }

    /// <summary>
    /// Return memory region sum (Variable SIMD)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int SumVector(ReadOnlySpan<int> region)
    {
        var vector = Vector<int>.Zero;
        for (int i = 0, count = region.Length - Vector<int>.Count; i <= count; i += Vector<int>.Count)
            vector += new Vector<int>(region[i..]);
        var result = Vector.Dot(vector, Vector<int>.One);
        for (var i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (AVX2)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumAvx2(ReadOnlySpan<int> region)
    {
        var vector = Vector256<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0, count = region.Length - Vector256<int>.Count; i <= count; i += Vector256<int>.Count)
                vector = Avx2.Add(vector, Avx.LoadVector256(pRegion + i));
        var result = Vector256.Dot(vector, Vector256<int>.One);
        for (var i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (AVX2)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "SumAvx2_Interop", SetLastError = false)]
    private static unsafe partial int SumAvx2Interop(int* pRegion, int itemCount);

    /// <summary>
    /// Return memory region sum (AVX-512)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumAvx512(ReadOnlySpan<int> region)
    {
        var vector = Vector512<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0, count = region.Length - Vector512<int>.Count; i <= count; i += Vector512<int>.Count)
                vector = Avx512F.Add(vector, Avx512F.LoadVector512(pRegion + i));
        var result = Vector512.Dot(vector, Vector512<int>.One);
        for (var i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (AVX-512)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "SumAvx512_Interop", SetLastError = false)]
    private static unsafe partial int SumAvx512Interop(int* pRegion, int itemCount);
    #endregion

    #region Count
    /// <summary>
    /// Count functions
    /// </summary>
    private unsafe void Count()
    {
        var region = MemoryService.GenerateRegionInt(REGION_SIZE);
        var item = region[^1];
        displayService.WriteInformation($"Count = {CountLoop(region, item):n0} (Loop)");
        displayService.WriteInformation($"Count = {CountLinq(region, item):n0} (LINQ)");
        displayService.WriteInformation($"Count = {CountVector(region, item):n0} (Vector)");
        fixed (int* pRegion = region)
        {
            if (Avx2.IsSupported)
            {
                displayService.WriteInformation($"Count = {CountAvx2(region, item):n0} (AVX2)");
                displayService.WriteInformation($"Count = {CountAvx2Interop(pRegion, region.Length, item):n0} (AVX2 Interop)");
            }
            if (Avx512F.IsSupported)
            {
                displayService.WriteInformation($"Count = {CountAvx512(region, item):n0} (AVX-512)");
                displayService.WriteInformation($"Count = {CountAvx512Interop(pRegion, region.Length, item):n0} (AVX-512 Interop)");
            }
        }
    }

    /// <summary>
    /// Return item count in memory region (loop)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int CountLoop(ReadOnlySpan<int> region, int item)
    {
        var result = 0;
        foreach (int i in region)
            if (i == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return item count in memory region (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int CountLinq(int[] region, int item) => region.Count(i => i == item);

    /// <summary>
    /// Return item count in memory region (Variable SIMD)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int CountVector(ReadOnlySpan<int> region, int item)
    {
        var mask = new Vector<int>(item);
        var vector = Vector<int>.Zero;
        for (int i = 0, count = region.Length - Vector<int>.Count; i <= count; i += Vector<int>.Count)
            vector -= Vector.Equals(new Vector<int>(region[i..]), mask);
        var result = Vector.Dot(vector, Vector<int>.One);
        for (var i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return item count in memory region (AVX2)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private unsafe int CountAvx2(ReadOnlySpan<int> region, int item)
    {
        int* pItem = stackalloc int[1];
        pItem[0] = item;
        var mask = Avx2.BroadcastScalarToVector256(pItem);
        var vector = Vector256<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0, count = region.Length - Vector256<int>.Count; i <= count; i += Vector256<int>.Count)
                vector = Avx2.Subtract(vector, Avx2.CompareEqual(Avx.LoadVector256(pRegion + i), mask));
        var result = Vector256.Dot(vector, Vector256<int>.One);
        for (var i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return item count in memory region (AVX2)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "CountAvx2_Interop", SetLastError = false)]
    private static unsafe partial int CountAvx2Interop(int* pRegion, int itemCount, int item);

    /// <summary>
    /// Return item count in memory region (AVX-512)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private unsafe int CountAvx512(ReadOnlySpan<int> region, int item)
    {
        int* pItem = stackalloc int[1];
        pItem[0] = item;
        var mask = Avx512F.BroadcastScalarToVector512(Avx2.BroadcastScalarToVector128(pItem));
        var vector = Vector512<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0, count = region.Length - Vector512<int>.Count; i <= count; i += Vector512<int>.Count)
                vector = Avx512F.Subtract(vector, Avx512F.CompareEqual(Avx512F.LoadVector512(pRegion + i), mask));
        var result = Vector512.Dot(vector, Vector512<int>.One);
        for (var i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return item count in memory region (AVX-512)
    /// </summary>
    /// <param name="pRegion"></param>
    /// <param name="itemCount"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "CountAvx512_Interop", SetLastError = false)]
    private static unsafe partial int CountAvx512Interop(int* pRegion, int itemCount, int item);
    #endregion

    #region Compare
    /// <summary>
    /// Compare functions
    /// </summary>
    private unsafe void Compare()
    {
        var region1 = MemoryService.GenerateRegionByte(REGION_SIZE);
        var region2 = MemoryService.CopyRegion(region1);
        displayService.WriteInformation($"Match = {CompareLoop(region1, region2)} (Loop)");
        displayService.WriteInformation($"Match = {CompareLinq(region1, region2)} (LINQ)");
        displayService.WriteInformation($"Match = {CompareVector(region1, region2)} (Vector)");
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
        {
            if (Avx2.IsSupported)
            {
                displayService.WriteInformation($"Match = {CompareAvx2(region1, region2)} (AVX2)");
                displayService.WriteInformation($"Match = {CompareAvx2Interop(pRegion1, pRegion2, region1.Length):n0} (AVX2 Interop)");
            }
            if (Avx512F.IsSupported)
            {
                displayService.WriteInformation($"Match = {CompareAvx512(region1, region2)} (AVX-512)");
                displayService.WriteInformation($"Match = {CompareAvx512Interop(pRegion1, pRegion2, region1.Length):n0} (AVX-512 Interop)");
            }
        }
    }

    /// <summary>
    /// Compare memory regions (loop)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private static bool CompareLoop(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        for (var i = 0; i < region1.Length; i++)
            if (region1[i] != region2[i])
                return false;
        return true;
    }

    /// <summary>
    /// Compare memory regions (LINQ)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private static bool CompareLinq(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2) => region1.SequenceEqual(region2);

    /// <summary>
    /// Compare memory regions (Variable SIMD)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private static bool CompareVector(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        for (int i = 0, count = region1.Length - Vector<byte>.Count; i <= count; i += Vector<byte>.Count)
            if (!Vector.EqualsAll(new Vector<byte>(region1[i..]), new Vector<byte>(region2[i..])))
                return false;
        for (var i = region1.Length - (region1.Length % Vector<byte>.Count); i < region1.Length; i++)
            if (region1[i] != region2[i])
                return false;
        return true;
    }

    /// <summary>
    /// Compare memory regions (AVX2)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private unsafe bool CompareAvx2(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        const int mask = unchecked((int)0xFFFF_FFFF);
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
        {
            for (int i = 0, count = region1.Length - Vector256<byte>.Count; i <= count; i += Vector256<byte>.Count)
                if (Avx2.MoveMask(Avx2.CompareEqual(Avx.LoadVector256(pRegion1 + i), Avx.LoadVector256(pRegion2 + i))) != mask)
                    return false;
            for (var i = region1.Length - (region1.Length % Vector256<byte>.Count); i < region1.Length; i++)
                if (region1[i] != region2[i])
                    return false;
            return true;
        }
    }

    /// <summary>
    /// Compare memory regions (AVX2)
    /// </summary>
    /// <param name="pRegion1"></param>
    /// <param name="pRegion2"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "CompareAvx2_Interop", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static unsafe partial bool CompareAvx2Interop(byte* pRegion1, byte* pRegion2, int itemCount);

    /// <summary>
    /// Compare memory regions (AVX-512)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private unsafe bool CompareAvx512(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        const byte mask = 0xFF;
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
        {
            for (int i = 0, count = region1.Length - Vector512<byte>.Count; i <= count; i += Vector512<byte>.Count)
            {
                var resultVector = Avx512BW.CompareEqual(Avx512F.LoadVector512(pRegion1 + i), Avx512F.LoadVector512(pRegion2 + i));
                for (int j = 0; j < Vector512<byte>.Count; j++)
                    if (resultVector[j] != mask)
                        return false;
            }
            for (var i = region1.Length - (region1.Length % Vector512<byte>.Count); i < region1.Length; i++)
                if (region1[i] != region2[i])
                    return false;
            return true;
        }
    }

    /// <summary>
    /// Compare memory regions (AVX-512)
    /// </summary>
    /// <param name="pRegion1"></param>
    /// <param name="pRegion2"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    [LibraryImport("ByteZoo.Blog.Asm.Library", EntryPoint = "CompareAvx512_Interop", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static unsafe partial bool CompareAvx512Interop(byte* pRegion1, byte* pRegion2, int itemCount);
    #endregion

    #endregion

    #region Benchmarks

    #region Setup
    /// <summary>
    /// Region 0
    /// </summary>
    private int[]? region0;

    /// <summary>
    /// Region 1
    /// </summary>
    private byte[]? region1;

    /// <summary>
    /// Test region 2
    /// </summary>
    private byte[]? region2;

    /// <summary>
    /// Region size
    /// </summary>
    [Params(1_003, 1_000_003)]
    public int Size { get; set; }

    /// <summary>
    /// Initialize benchmark data
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        region0 = MemoryService.GenerateRegionInt(Size);
        region1 = MemoryService.GenerateRegionByte(Size);
        region2 = MemoryService.CopyRegion(region1);
    }
    #endregion

    #region Sum
    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Baseline = true, Description = "Sum (Loop)"), BenchmarkCategory("Sum")]
    public int SumLoopBenchmark() => SumLoop(region0);

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (LINQ)"), BenchmarkCategory("Sum")]
    public int SumLinqBenchmark() => SumLinq(region0!);

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (Unrolled)"), BenchmarkCategory("Sum")]
    public int SumUnrolledBenchmark() => SumUnrolled(region0);

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (Vector)"), BenchmarkCategory("Sum")]
    public int SumVectorBenchmark() => SumVector(region0);

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (AVX2)"), BenchmarkCategory("Sum")]
    public int SumAvx2Benchmark() => Avx2.IsSupported ? SumAvx2(region0) : 0;

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (AVX2 Interop)"), BenchmarkCategory("Sum")]
    public unsafe int SumAvx2InteropBenchmark()
    {
        fixed (int* pRegion = region0)
            return Avx2.IsSupported ? SumAvx2Interop(pRegion, region0!.Length) : 0;
    }

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (AVX-512)"), BenchmarkCategory("Sum")]
    public int SumAvx512Benchmark() => Avx512F.IsSupported ? SumAvx512(region0) : 0;

    /// <summary>
    /// Sum memory region benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Sum (AVX-512 Interop)"), BenchmarkCategory("Sum")]
    public unsafe int SumAvx512InteropBenchmark()
    {
        fixed (int* pRegion = region0)
            return Avx512F.IsSupported ? SumAvx512Interop(pRegion, region0!.Length) : 0;
    }
    #endregion

    #region Compare
    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Baseline = true, Description = "Compare (Loop)"), BenchmarkCategory("Compare")]
    public bool CompareLoopBenchmark() => CompareLoop(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (LINQ)"), BenchmarkCategory("Compare")]
    public bool CompareLinqBenchmark() => CompareLinq(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (Vectors)"), BenchmarkCategory("Compare")]
    public bool CompareVectorBenchmark() => CompareVector(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (AVX2)"), BenchmarkCategory("Compare")]
    public bool CompareAvx2Benchmark() => Avx2.IsSupported && CompareAvx2(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (AVX2 Interop)"), BenchmarkCategory("Compare")]
    public unsafe bool CompareAvx2InteropBenchmark()
    {
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
            return Avx2.IsSupported && CompareAvx2Interop(pRegion1, pRegion2, region1!.Length);
    }

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (AVX-512)"), BenchmarkCategory("Compare")]
    public bool CompareAvx512Benchmark() => Avx512F.IsSupported && CompareAvx512(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (AVX-512 Interop)"), BenchmarkCategory("Compare")]
    public unsafe bool CompareAvx512InteropBenchmark()
    {
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
            return Avx512F.IsSupported && CompareAvx512Interop(pRegion1, pRegion2, region1!.Length);
    }
    #endregion

    #endregion

}