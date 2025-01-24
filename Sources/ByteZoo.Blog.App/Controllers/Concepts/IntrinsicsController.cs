using BenchmarkDotNet.Attributes;
using ByteZoo.Blog.Common.Services;
using CommandLine;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Intrinsics controller
/// </summary>
[Verb("Concepts-Intrinsics", HelpText = "Intrinsics operation.")]
public class IntrinsicsController : Controller
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
        ExecuteIntrinsicsSum();
        ExecuteIntrinsicsCompare();
        ExecuteIntrinsicsItemCount();
        displayService.Wait();
    }
    #endregion

    #region Private Methods

    #region Sum
    /// <summary>
    /// Execute sum intrinsics
    /// </summary>
    private void ExecuteIntrinsicsSum()
    {
        int[] region = MemoryService.GenerateRegionInt(REGION_SIZE);
        displayService.WriteInformation($"Sum = {SumLoop(region):n0} (Loop)");
        displayService.WriteInformation($"Sum = {SumLinq(region):n0} (LINQ)");
        displayService.WriteInformation($"Sum = {SumUnrolled(region):n0} (Unrolled)");
        displayService.WriteInformation($"Sum = {SumVector(region):n0} (Vector)");
        if (Sse2.IsSupported)
            displayService.WriteInformation($"Sum = {SumVectorSse2(region):n0} (SSE2.Vector128)");
        if (Ssse3.IsSupported)
            displayService.WriteInformation($"Sum = {SumVectorSsse3(region):n0} (SSSE3.Vector128)");
        if (Avx2.IsSupported)
            displayService.WriteInformation($"Sum = {SumVectorAvx2(region):n0} (AVX2.Vector256)");
        if (Avx512F.IsSupported)
            displayService.WriteInformation($"Sum = {SumVectorAvx512(region):n0} (AVX512.Vector512)");
    }

    /// <summary>
    /// Return memory region sum (for loop)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int SumLoop(ReadOnlySpan<int> region)
    {
        int result = 0;
        for (int i = 0; i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private static int SumLinq(int[] region) => region.Aggregate<int, int>(0, (current, i) => current + i);

    /// <summary>
    /// Return memory region sum (unrolled)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumUnrolled(ReadOnlySpan<int> region)
    {
        int result = 0;
        int i = 0;
        int lastBlockIndex = region.Length - (region.Length % 4);
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
        Vector<int> vResult = Vector<int>.Zero;
        for (int i = 0; i <= region.Length - Vector<int>.Count; i += Vector<int>.Count)
            vResult += new Vector<int>(region[i..]);
        int result = Vector.Dot(vResult, Vector<int>.One);
        for (int i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (SSE2 128-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumVectorSse2(ReadOnlySpan<int> region)
    {
        Vector128<int> vResult = Vector128<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector128<int>.Count; i += Vector128<int>.Count)
                vResult = Sse2.Add(vResult, Sse2.LoadVector128(pRegion + i));
        vResult = Sse2.Add(vResult, Sse2.Shuffle(vResult, 0x4E));
        vResult = Sse2.Add(vResult, Sse2.Shuffle(vResult, 0xB1));
        int result = vResult.ToScalar();
        for (int i = region.Length - (region.Length % Vector128<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (SSSE3 128-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumVectorSsse3(ReadOnlySpan<int> region)
    {
        Vector128<int> vResult = Vector128<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector128<int>.Count; i += Vector128<int>.Count)
                vResult = Sse2.Add(vResult, Sse2.LoadVector128(pRegion + i));
        vResult = Ssse3.HorizontalAdd(vResult, vResult);
        vResult = Ssse3.HorizontalAdd(vResult, vResult);
        int result = vResult.ToScalar();
        for (int i = region.Length - (region.Length % Vector128<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (AVX2 256-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumVectorAvx2(ReadOnlySpan<int> region)
    {
        Vector256<int> vResult = Vector256<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector256<int>.Count; i += Vector256<int>.Count)
                vResult = Avx2.Add(vResult, Avx.LoadVector256(pRegion + i));
        int result = 0;
        int* sResult = stackalloc int[Vector256<int>.Count];
        Avx.Store(sResult, vResult);
        for (int i = 0; i < Vector256<int>.Count; i++)
            result += sResult[i];
        for (int i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }

    /// <summary>
    /// Return memory region sum (AVX512 512-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private unsafe int SumVectorAvx512(ReadOnlySpan<int> region)
    {
        Vector512<int> vResult = Vector512<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector512<int>.Count; i += Vector512<int>.Count)
                vResult = Avx512F.Add(vResult, Avx512F.LoadVector512(pRegion + i));
        int result = 0;
        int* sResult = stackalloc int[Vector512<int>.Count];
        Avx512F.Store(sResult, vResult);
        for (int i = 0; i < Vector512<int>.Count; i++)
            result += sResult[i];
        for (int i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
            result += region[i];
        return result;
    }
    #endregion

    #region Compare
    /// <summary>
    /// Execute compare intrinsics
    /// </summary>
    private void ExecuteIntrinsicsCompare()
    {
        byte[] region1 = MemoryService.GenerateRegionByte(REGION_SIZE);
        byte[] region2 = MemoryService.CopyRegion(region1);
        displayService.WriteInformation($"Match = {CompareLoop(region1, region2)} (Loop)");
        displayService.WriteInformation($"Match = {CompareLinq(region1, region2)} (LINQ)");
        displayService.WriteInformation($"Match = {CompareVector(region1, region2)} (Vector)");
        if (Avx2.IsSupported)
            displayService.WriteInformation($"Match = {CompareVectorAvx2(region1, region2)} (AVX2.Vector)");
        if (Avx512F.IsSupported)
            displayService.WriteInformation($"Match = {CompareVectorAvx512(region1, region2)} (AVX512.Vector)");
    }

    /// <summary>
    /// Compare memory regions (for loop)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private static bool CompareLoop(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        for (int i = 0; i < region1.Length; i++)
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
        for (int i = 0; i <= region1.Length - Vector<byte>.Count; i += Vector<byte>.Count)
            if (!Vector.EqualsAll(new Vector<byte>(region1[i..]), new Vector<byte>(region2[i..])))
                return false;
        for (int i = region1.Length - (region1.Length % Vector<byte>.Count); i < region1.Length; i++)
            if (region1[i] != region2[i])
                return false;
        return true;
    }

    /// <summary>
    /// Compare memory regions (AVX2 256-bit)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private unsafe bool CompareVectorAvx2(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        const int mask = unchecked((int)0xFFFF_FFFF);
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
        {
            for (int i = 0; i <= region1.Length - Vector256<byte>.Count; i += Vector256<byte>.Count)
                if (Avx2.MoveMask(Avx2.CompareEqual(Avx.LoadVector256(pRegion1 + i), Avx.LoadVector256(pRegion2 + i))) != mask)
                    return false;
            for (int i = region1.Length - (region1.Length % Vector256<int>.Count); i < region1.Length; i++)
                if (region1[i] != region2[i])
                    return false;
            return true;
        }
    }

    /// <summary>
    /// Compare memory regions (AVX512 512-bit)
    /// </summary>
    /// <param name="region1"></param>
    /// <param name="region2"></param>
    /// <returns></returns>
    private unsafe bool CompareVectorAvx512(ReadOnlySpan<byte> region1, ReadOnlySpan<byte> region2)
    {
        const int mask = unchecked((int)0xFFFF_FFFF);
        fixed (byte* pRegion1 = region1)
        fixed (byte* pRegion2 = region2)
        {
            for (int i = 0; i <= region1.Length - Vector512<byte>.Count; i += Vector512<byte>.Count)
                if (Avx2.MoveMask(Avx2.CompareEqual(Avx.LoadVector256(pRegion1 + i), Avx.LoadVector256(pRegion2 + i))) != mask)
                    return false;
            for (int i = region1.Length - (region1.Length % Vector512<int>.Count); i < region1.Length; i++)
                if (region1[i] != region2[i])
                    return false;
            return true;
        }
    }
    #endregion

    #region Item Count
    /// <summary>
    /// Execute item count intrinsics
    /// </summary>
    private void ExecuteIntrinsicsItemCount()
    {
        int[] region = MemoryService.GenerateRegionInt(REGION_SIZE);
        int item = region[^1];
        displayService.WriteInformation($"Item Count = {GetCountLoop(region, item):n0} (Loop)");
        displayService.WriteInformation($"Item Count = {GetCountLinq(region, item):n0} (LINQ)");
        displayService.WriteInformation($"Item Count = {GetCountVector(region, item):n0} (Vector)");
        if (Avx2.IsSupported)
            displayService.WriteInformation($"Item Count = {GetCountVectorAvx2(region, item):n0} (AVX2.Vector)");
        if (Avx512F.IsSupported)
            displayService.WriteInformation($"Item Count = {GetCountVectorAvx512(region, item):n0} (AVX512.Vector)");
    }

    /// <summary>
    /// Return instance count of item in memory region (for loop)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int GetCountLoop(ReadOnlySpan<int> region, int item)
    {
        int result = 0;
        foreach (int i in region)
            if (i == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return instance count of item in memory region (LINQ)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int GetCountLinq(int[] region, int item) => region.Count(i => i == item);

    /// <summary>
    /// Return instance count of item in memory region (Variable SIMD)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static int GetCountVector(ReadOnlySpan<int> region, int item)
    {
        Vector<int> mask = new(item);
        Vector<int> vResult = Vector<int>.Zero;
        for (int i = 0; i <= region.Length - Vector<int>.Count; i += Vector<int>.Count)
            vResult -= Vector.Equals(new Vector<int>(region.Slice(i)), mask);
        int result = 0;
        for (int i = region.Length - (region.Length % Vector<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result + Vector.Dot(vResult, Vector<int>.One);
    }

    /// <summary>
    /// Return instance count of item in memory region (AVX2 256-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private unsafe int GetCountVectorAvx2(ReadOnlySpan<int> region, int item)
    {
        int* sResult = stackalloc int[Vector256<int>.Count];
        for (int i = 0; i < Vector256<int>.Count; i++)
            sResult[i] = item;
        Vector256<int> mask = Avx.LoadVector256(sResult);
        Vector256<int> vResult = Vector256<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector256<int>.Count; i += Vector256<int>.Count)
                vResult = Avx2.Subtract(vResult, Avx2.CompareEqual(Avx.LoadVector256(pRegion + i), mask));
        int result = 0;
        Avx2.Store(sResult, vResult);
        for (int i = 0; i < Vector256<int>.Count; i++)
            result += sResult[i];
        for (int i = region.Length - (region.Length % Vector256<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result;
    }

    /// <summary>
    /// Return instance count of item in memory region (AVX512 512-bit)
    /// </summary>
    /// <param name="region"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private unsafe int GetCountVectorAvx512(ReadOnlySpan<int> region, int item)
    {
        int* sResult = stackalloc int[Vector512<int>.Count];
        for (int i = 0; i < Vector512<int>.Count; i++)
            sResult[i] = item;
        Vector512<int> mask = Avx512F.LoadVector512(sResult);
        Vector512<int> vResult = Vector512<int>.Zero;
        fixed (int* pRegion = region)
            for (int i = 0; i <= region.Length - Vector512<int>.Count; i += Vector512<int>.Count)
                vResult = Avx512F.Subtract(vResult, Avx512F.CompareEqual(Avx512F.LoadVector512(pRegion + i), mask));
        int result = 0;
        Avx512F.Store(sResult, vResult);
        for (int i = 0; i < Vector512<int>.Count; i++)
            result += sResult[i];
        for (int i = region.Length - (region.Length % Vector512<int>.Count); i < region.Length; i++)
            if (region[i] == item)
                result++;
        return result;
    }
    #endregion

    #endregion

    #region Benchmarks
    /// <summary>
    /// Region size
    /// </summary>
    [Params(1_000, 1_000_000)]
    public int Size { get; set; }

    /// <summary>
    /// Region 1
    /// </summary>
    private byte[]? region1;

    /// <summary>
    /// Test region 2
    /// </summary>
    private byte[]? region2;

    /// <summary>
    /// Initialize benchmark data
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        region1 = MemoryService.GenerateRegionByte(Size);
        region2 = MemoryService.CopyRegion(region1);
    }

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Baseline = true, Description = "Compare (Loop)")]
    public bool CompareLoopBenchmark() => CompareLoop(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (LINQ)")]
    public bool CompareLinqBenchmark() => CompareLinq(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (Vectors)")]
    public bool CompareVectorBenchmark() => CompareVector(region1, region2);

    /// <summary>
    /// Compare memory regions benchmark
    /// </summary>
    /// <returns></returns>
    [Benchmark(Description = "Compare (AVX2)")]
    public bool CompareVectorAvx2Benchmark() => CompareVectorAvx2(region1, region2);
    #endregion

}