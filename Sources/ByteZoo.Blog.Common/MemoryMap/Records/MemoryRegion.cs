using ByteZoo.Blog.Common.MemoryMap.Enums;

namespace ByteZoo.Blog.Common.MemoryMap.Records;

/// <summary>
/// Memory region
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
/// <param name="Permissions"></param>
public record class MemoryRegion(ulong Start, ulong End, MemoryRegionPermissions Permissions)
{

    #region Properties
    /// <summary>
    /// Memory region path
    /// </summary>
    public required string Path { get; set; }

    /// <summary>
    /// Memory region size
    /// </summary>
    public ulong Size => End - Start + 1UL;
    #endregion

}