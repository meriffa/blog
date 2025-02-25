using ByteZoo.Blog.Common.ObjectQuery.Models;
using Microsoft.Diagnostics.Runtime;
using System.Text.Json;

namespace ByteZoo.Blog.Common.ObjectQuery;

/// <summary>
/// Object query writer
/// </summary>
/// <param name="path"></param>
/// <param name="writeIndented"></param>
public sealed class ObjectQueryWriter(Dictionary<string, Func<ClrObject, object>> fieldFunctions, string path, bool writeIndented = false)
{

    #region Private Members
    private readonly List<ObjectInstance> items = [];
    private static readonly JsonSerializerOptions optionsTrue = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions optionsFalse = new() { WriteIndented = false };
    #endregion

    #region Properties
    /// <summary>
    /// Item count
    /// </summary>
    public int Count => items.Count;
    #endregion

    #region Public Methods
    /// <summary>
    /// Add object instance
    /// </summary>
    /// <param name="clrObject"></param>
    public void Add(ClrObject clrObject)
    {
        var objectInstance = new ObjectInstance();
        foreach (var fieldFunction in fieldFunctions)
            objectInstance.Add(fieldFunction.Key, fieldFunction.Value(clrObject));
        items.Add(objectInstance);
    }

    /// <summary>
    /// Write object instances to file
    /// </summary>
    public void Write()
    {
        using var stream = File.Create(path);
        JsonSerializer.Serialize(stream, items, writeIndented ? optionsTrue : optionsFalse);
    }
    #endregion

}