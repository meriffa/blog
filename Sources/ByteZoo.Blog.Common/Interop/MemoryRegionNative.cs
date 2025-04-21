using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Native memory region
/// </summary>
public unsafe class MemoryRegionNative : MemoryRegion
{

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="size"></param>
    /// <param name="alignment"></param>
    public MemoryRegionNative(nuint size, nuint alignment = 16)
    {
        if ((Pointer = (nint)NativeMemory.AlignedAlloc(size, alignment)) != IntPtr.Zero)
            GC.AddMemoryPressure((long)size);
        Size = size;
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        NativeMemory.AlignedFree(Pointer.ToPointer());
        if (Pointer != IntPtr.Zero)
            GC.RemoveMemoryPressure((long)Size);
        return true;
    }
    #endregion

}