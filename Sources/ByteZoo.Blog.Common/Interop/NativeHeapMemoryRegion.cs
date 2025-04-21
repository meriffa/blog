using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ByteZoo.Blog.Common.Interop;

/// <summary>
/// Native heap memory region
/// </summary>
public class NativeHeapMemoryRegion : SafeHandleZeroOrMinusOneIsInvalid
{

    #region Properties
    /// <summary>
    /// Native heap memory region pointer
    /// </summary>
    public IntPtr Pointer { get; }

    /// <summary>
    /// Native heap memory region size
    /// </summary>
    public int Size { get; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="size"></param>
    public NativeHeapMemoryRegion(int size) : base(true)
    {
        Size = size;
        if ((Pointer = Marshal.AllocHGlobal(size)) != IntPtr.Zero)
            GC.AddMemoryPressure(size);
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        Marshal.FreeHGlobal(Pointer);
        GC.RemoveMemoryPressure(Size);
        return true;
    }
    #endregion

}