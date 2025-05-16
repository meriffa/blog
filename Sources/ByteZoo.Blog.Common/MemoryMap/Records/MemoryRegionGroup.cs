namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region group
/// </summary>
/// <param name="Path"></param>
/// <param name="Vss">Virtual Set Size</param>
/// <param name="Rss">Resident Set Size</param>
/// <param name="Pss">Proportional Set Size</param>
/// <param name="Uss">Unique Set Size</param>
/// <param name="Order"></param>
public readonly record struct MemoryRegionGroup(string Path, long Vss, long Rss, long Pss, long Uss, long Order);