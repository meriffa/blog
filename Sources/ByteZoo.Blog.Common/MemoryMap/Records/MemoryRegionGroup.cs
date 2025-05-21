namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region group
/// </summary>
/// <param name="Path"></param>
/// <param name="Size"></param>
/// <param name="Order"></param>
public readonly record struct MemoryRegionGroup(string Path, ulong Size, ulong Order);