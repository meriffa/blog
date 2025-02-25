namespace ByteZoo.Blog.Common.TypeLayout.Extensions;

/// <summary>
/// IEnumerable extension methods
/// </summary>
public static class IEnumerableExtensions
{

    #region Public Methods
    /// <summary>
    /// Return maximum sequence element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static T? GetMaximum<T>(this IEnumerable<T> sequence, Func<T?, int> selector)
    {
        bool initialized = false;
        T? result = default;
        foreach (T item in sequence)
            if (!initialized)
            {
                result = item;
                initialized = true;
            }
            else
            {
                int current = selector(result);
                int candidate = selector(item);
                if (Math.Max(current, candidate) == candidate)
                    result = item;
            }
        return result;
    }
    #endregion

}