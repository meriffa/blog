using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Common.Models.Memory;

/// <summary>
/// Native heap region
/// </summary>
public class NativeHeapRegion : IDisposable
{

    #region Private Fields
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// Region start
    /// </summary>
    public IntPtr Start { get; private set; } = IntPtr.Zero;

    /// <summary>
    /// Region size (in bytes)
    /// </summary>
    public int Size { get; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="size"></param>
    public NativeHeapRegion(int size)
    {
        Size = size;
        Start = Marshal.AllocHGlobal(size);
        if (Start != IntPtr.Zero)
            GC.AddMemoryPressure(size);
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    ~NativeHeapRegion() => Dispose(false);

    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed and native resources
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (Start != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Start);
                GC.RemoveMemoryPressure(Size);
                Start = IntPtr.Zero;
            }
            disposed = true;
        }
    }
    #endregion

}