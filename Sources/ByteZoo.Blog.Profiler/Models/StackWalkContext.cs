using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.Profiler.Models;

/// <summary>
/// Stack walk context
/// </summary>
public struct StackWalkContext
{

    #region Constants
    public const int MaxFrames = 1024;
    #endregion

    #region Stack Frames
    /// <summary>
    /// Stack frames
    /// </summary>
    [InlineArray(MaxFrames)]
    public struct StackFrames
    {
        private nint frames;
    }
    #endregion

    #region Public Members
    /// <summary>
    /// Stack frames
    /// </summary>
    public StackFrames Frames;

    /// <summary>
    /// Stack frame count
    /// </summary>
    public int Count;
    #endregion

}