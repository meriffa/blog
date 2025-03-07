using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.Profiler;

/// <summary>
/// Stack walk context
/// </summary>
public struct StackWalkContext
{

    #region Constants
    public const int MaxFrames = 1024;
    #endregion

    #region Public Members
    public FramesArray Frames;
    public int Count;
    [InlineArray(MaxFrames)]
    public struct FramesArray
    {
        private nint frames;
    }
    #endregion

}