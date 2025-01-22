using System.Diagnostics;

namespace ByteZoo.Blog.Common.Extensions;

/// <summary>
/// Stopwatch extension methods
/// </summary>
public static class StopwatchExtensions
{

    #region Public Methods
    /// <summary>
    /// Stop stopwatch and return elapsed time
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static TimeSpan StopElapsed(this Stopwatch stopwatch)
    {
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
    #endregion

}