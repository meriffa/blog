using BenchmarkDotNet.Attributes;
using ByteZoo.Blog.Common.Models.Memory;
using CommandLine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Memory allocation controller
/// </summary>
[Verb("Concepts-MemoryAllocation", HelpText = "Memory allocation operation.")]
public class MemoryAllocationController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        Setup();
        Size = 500;
        AllocateManagedStackPointer();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Managed Stack (Pointer)).");
        AllocateManagedStackSpan();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Managed Stack (Span)).");
        AllocateManagedHeapArray();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Managed Heap (Array)).");
        AllocateManagedHeapSpan();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Managed Heap (Span)).");
        AllocateNativeHeapPointer();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Native Heap (Pointer)).");
        AllocateNativeHeapSpan();
        displayService.WriteInformation($"Memory region allocated (Size = {Size}, Type = Native Heap (Span)).");
        displayService.Wait();
    }
    #endregion

    #region Benchmarks
    /// <summary>
    /// Fill value
    /// </summary>
    private byte fillValue;

    /// <summary>
    /// Region size.
    /// </summary>
    [Params(100, 1_000)]
    public int Size { get; set; }

    /// <summary>
    /// Initialize benchmark data
    /// </summary>
    [GlobalSetup]
    public void Setup() => fillValue = (byte)Random.Shared.Next(0, 255);

    /// <summary>
    /// Allocates managed stack region and use pointer.
    /// </summary>
    [Benchmark(Description = "Managed Stack (Pointer)")]
    public unsafe void AllocateManagedStackPointer()
    {
        int* buffer = stackalloc int[Size];
        nuint size = (nuint)Size * sizeof(int);
        void* destination = buffer;
        FillRegion(destination, size, fillValue);
        ClearRegion(destination, size);
    }

    /// <summary>
    /// Allocates managed stack region and use span.
    /// </summary>
    [Benchmark(Baseline = true, Description = "Managed Stack (Span)")]
    public unsafe void AllocateManagedStackSpan()
    {
        ReadOnlySpan<int> buffer = stackalloc int[Size];
        void* destination = Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));
        nuint size = (nuint)buffer.Length * sizeof(int);
        FillRegion(destination, size, fillValue);
        ClearRegion(destination, size);
    }

    /// <summary>
    /// Allocates managed heap region and use array.
    /// </summary>
    [Benchmark(Description = "Managed Heap (Array)")]
    public unsafe void AllocateManagedHeapArray()
    {
        int[] buffer = new int[Size];
        fixed (int* ptr = &buffer[0])
        {
            void* destination = ptr;
            nuint size = (nuint)buffer.Length * sizeof(int);
            FillRegion(destination, size, fillValue);
            ClearRegion(destination, size);
        }
    }

    /// <summary>
    /// Allocates managed heap region and use span.
    /// </summary>
    [Benchmark(Description = "Managed Heap (Span)")]
    public unsafe void AllocateManagedHeapSpan()
    {
        ReadOnlySpan<int> buffer = new int[Size];
        fixed (int* ptr = &MemoryMarshal.GetReference(buffer))
        {
            void* destination = Unsafe.AsPointer(ref Unsafe.AsRef<int>(ptr));
            nuint size = (nuint)buffer.Length * sizeof(int);
            FillRegion(destination, size, fillValue);
            ClearRegion(destination, size);
        }
    }

    /// <summary>
    /// Allocates native heap region and use IntPtr.
    /// </summary>
    [Benchmark(Description = "Native Heap (Pointer)")]
    public unsafe void AllocateNativeHeapPointer()
    {
        int size = Size * Unsafe.SizeOf<int>();
        using NativeHeapRegion buffer = new(size);
        void* destination = buffer.Start.ToPointer();
        FillRegion(destination, (nuint)size, fillValue);
        ClearRegion(destination, (nuint)size);
    }

    /// <summary>
    /// Allocates native heap region and use span.
    /// </summary>
    [Benchmark(Description = "Native Heap (Span)")]
    public unsafe void AllocateNativeHeapSpan()
    {
        int size = Size * Unsafe.SizeOf<int>();
        using NativeHeapRegion buffer = new(size);
        ReadOnlySpan<int> span = new(buffer.Start.ToPointer(), Size);
        void* destination = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        FillRegion(destination, (nuint)size, fillValue);
        ClearRegion(destination, (nuint)size);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Fill memory region
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="byteCount"></param>
    /// <param name="value"></param>
    private static unsafe void FillRegion(void* ptr, nuint byteCount, byte value) => NativeMemory.Fill(ptr, byteCount, value);

    /// <summary>
    /// Clear memory region
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="byteCount"></param>
    private static unsafe void ClearRegion(void* ptr, nuint byteCount) => NativeMemory.Clear(ptr, byteCount);
    #endregion

}