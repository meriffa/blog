namespace ByteZoo.Blog.Common.Services;

/// <summary>
/// Memory service
/// </summary>
public static class MemoryService
{

    #region Public Methods
    /// <summary>
    /// Generates random memory region
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static int[] GenerateRegionInt(int length)
    {
        int range = Random.Shared.Next();
        int[] result = new int[length];
        for (int i = 0; i < length; i++)
            result[i] = Random.Shared.Next(-range, range);
        return result;
    }

    /// <summary>
    /// Generates random memory region
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static byte[] GenerateRegionByte(int length)
    {
        byte[] result = new byte[length];
        Random.Shared.NextBytes(result);
        return result;
    }

    /// <summary>
    /// Returns memory region copy
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public static int[] CopyRegion(int[] region)
    {
        int[] result = new int[region.Length];
        region.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    /// Returns memory region copy
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public static byte[] CopyRegion(byte[] region)
    {
        byte[] result = new byte[region.Length];
        region.CopyTo(result, 0);
        return result;
    }
    #endregion

}