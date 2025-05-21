namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region total
/// </summary>
/// <param name="Count"></param>
/// <param name="Size"></param>
public readonly record struct MemoryRegionTotal(int Count, ulong Size);