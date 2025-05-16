namespace ByteZoo.Blog.Common.MemoryMap.Enums;

/// <summary>
/// Memory region permissions
/// </summary>
[Flags]
public enum MemoryRegionPermissions : int
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Private = 8,
    Shared = 16
}