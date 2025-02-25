using ByteZoo.Blog.Common.ObjectQuery.Extensions;
using ByteZoo.Blog.Common.ObjectQuery.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Diagnostics.Runtime;
using System.Text.Json;

namespace ByteZoo.Blog.Common.ObjectQuery;

/// <summary>
/// Object query builder
/// </summary>
public sealed class ObjectQueryBuilder
{

    #region Private Members
    private readonly Models.ObjectQuery query;
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { RespectRequiredConstructorParameters = true };
    private static readonly ScriptOptions scriptOptions = ScriptOptions.Default
        .WithReferences(typeof(ClrObject).Assembly, typeof(ClrObjectExtensions).Assembly)
        .WithImports(typeof(ClrObjectExtensions).Namespace!);
    #endregion

    #region Properties
    /// <summary>
    /// Query builder filter function
    /// </summary>
    public Func<ClrObject, bool> FilterFunction { get; init; }

    public Dictionary<string, Func<ClrObject, object>> FieldFunctions { get; init; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="path"></param>
    public ObjectQueryBuilder(string path)
    {
        using var stream = File.OpenRead(path);
        query = JsonSerializer.Deserialize<Models.ObjectQuery>(stream, jsonSerializerOptions) ?? throw new($"Invalid query file '{path}' specified.");
        FilterFunction = GetFilterFunction(query.Filter);
        FieldFunctions = GetFieldFunctions(query.Fields);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return filter function
    /// </summary>
    /// <param name="filterExpression"></param>
    /// <returns></returns>
    private static Func<ClrObject, bool> GetFilterFunction(string filterExpression)
    {
        try
        {
            return CSharpScript.EvaluateAsync<Func<ClrObject, bool>>(filterExpression, scriptOptions).Result;
        }
        catch (Exception ex)
        {
            throw new($"Invalid query filter expression specified ({ex.Message}).", ex);
        }
    }

    /// <summary>
    /// Return field functions
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private static Dictionary<string, Func<ClrObject, object>> GetFieldFunctions(List<ObjectQueryFields> fields)
    {
        var result = new Dictionary<string, Func<ClrObject, object>>();
        foreach (var field in fields)
            try
            {
                result.Add(field.Name, CSharpScript.EvaluateAsync<Func<ClrObject, object>>(field.Expression, scriptOptions).Result);
            }
            catch (Exception ex)
            {
                throw new($"Invalid output field '{field.Name}' expression specified ({ex.Message}).", ex);
            }
        if (result.Count == 0)
            throw new("No output fields specified.");
        return result;
    }
    #endregion

}