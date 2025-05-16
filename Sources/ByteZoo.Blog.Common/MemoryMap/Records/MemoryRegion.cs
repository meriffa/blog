using ByteZoo.Blog.Common.MemoryMap.Enums;

namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region
/// </summary>
/// <param name="Path"></param>
/// <param name="Vss">Virtual Set Size</param>
/// <param name="Rss">Resident Set Size</param>
/// <param name="Pss">Proportional Set Size</param>
/// <param name="Uss">Unique Set Size</param>
/// <param name="Start"></param>
/// <param name="End"></param>
/// <param name="Permissions"></param>
public readonly record struct MemoryRegion(string Path, long Vss, long Rss, long Pss, long Uss, long Start, long End, MemoryRegionPermissions Permissions);