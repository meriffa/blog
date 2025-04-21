using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Managed memory region
/// </summary>
public class MemoryRegionManaged : MemoryRegion
{

    #region Properties
    /// <summary>
    /// Managed memory region buffer
    /// </summary>
    public int[] Buffer { get; }

    /// <summary>
    /// Managed memory region handle
    /// </summary>
    public GCHandle Handle { get; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="size"></param>
    public MemoryRegionManaged(int size)
    {
        Buffer = new int[size];
        Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
        Pointer = Handle.AddrOfPinnedObject();
        Size = (nuint)Buffer.Length * sizeof(int);
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        Handle.Free();
        return true;
    }
    #endregion

}