namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Position changed arguments
/// </summary>
/// <param name="originalPosition"></param>
/// <param name="currentPosition"></param>
public class PositionChangedArgs(string originalPosition, string currentPosition) : EventArgs
{

    #region Properties
    /// <summary>
    /// Position changed arguments original position
    /// </summary>
    public string OriginalPosition { get; } = originalPosition;

    /// <summary>
    /// Position changed arguments current position
    /// </summary>
    public string CurrentPosition { get; } = currentPosition;
    #endregion

}