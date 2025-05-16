namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region total
/// </summary>
/// <param name="Count"></param>
/// <param name="Vss">Virtual Set Size</param>
/// <param name="Rss">Resident Set Size</param>
/// <param name="Pss">Proportional Set Size</param>
/// <param name="Uss">Unique Set Size</param>
public readonly record struct MemoryRegionTotal(int Count, long Vss, long Rss, long Pss, long Uss);