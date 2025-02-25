namespace ByteZoo.Blog.Common.ObjectQuery.Models;

/// <summary>
/// Object query
/// </summary>
/// <param name="Filter"></param>
/// <param name="Fields"></param>
public record ObjectQuery(string Filter, List<ObjectQueryFields> Fields)
{

    #region Properties
    /// <summary>
    /// Object query description
    /// </summary>
    public string? Description { get; init; }
    #endregion

}