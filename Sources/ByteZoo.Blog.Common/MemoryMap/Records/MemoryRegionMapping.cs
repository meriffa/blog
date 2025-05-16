namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region mapping
/// </summary>
/// <param name="Path"></param>
/// <param name="Start"></param>
/// <param name="End"></param>
public readonly record struct MemoryRegionMapping(string Path, long Start, long End);