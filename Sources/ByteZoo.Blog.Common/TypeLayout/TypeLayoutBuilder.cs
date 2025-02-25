using ByteZoo.Blog.Common.TypeLayout.Extensions;
using ByteZoo.Blog.Common.TypeLayout.Models;
using System.Collections.Concurrent;

namespace ByteZoo.Blog.Common.TypeLayout;

/// <summary>
/// Type layout builder
/// </summary>
public sealed class TypeLayoutBuilder
{

    #region Private Members
    private static readonly ConcurrentDictionary<Type, Models.TypeLayout> cacheItems = new();
    #endregion

    #region Public Methods
    /// <summary>
    /// Return type layout
    /// </summary>
    /// <param name="type"></param>
    /// <param name="includePadding"></param>
    /// <returns></returns>
    public static Models.TypeLayout? TryGet(Type type, bool includePadding = true) => type.CanCreateInstance() ? Get(type, includePadding) : null;

    /// <summary>
    /// Return type layout
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="includePadding"></param>
    /// <returns></returns>
    public static Models.TypeLayout Get<T>(bool includePadding = true) => Get(typeof(T), includePadding);

    /// <summary>
    /// Return type layout
    /// </summary>
    /// <param name="type"></param>
    /// <param name="includePadding"></param>
    /// <returns></returns>
    public static Models.TypeLayout Get(Type type, bool includePadding = true)
    {
        if (cacheItems.TryGetValue(type, out var result))
            return result;
        try
        {
            var (size, overhead) = type.GetSize();
            var fields = type.GetFieldOffsets().Select(i => new FieldLayoutActual(i.offset, i.field, i.field.FieldType.GetFieldSize())).ToArray();
            result = Get(type, size, overhead, [.. GetFields(fields, size, includePadding)], includePadding);
            cacheItems.TryAdd(type, result);
            return result;
        }
        catch (Exception ex)
        {
            throw new($"Type instance creation failed ({ex.Message}).");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return type layout
    /// </summary>
    /// <param name="type"></param>
    /// <param name="size"></param>
    /// <param name="overhead"></param>
    /// <param name="fields"></param>
    /// <param name="includePadding"></param>
    /// <returns></returns>
    private static Models.TypeLayout Get(Type type, int size, int overhead, FieldLayout[] fields, bool includePadding)
    {
        // Assume no padding for unsafe structs
        var instancePaddings = type.IsUnsafeValueType() ? 0 : fields.OfType<FieldLayoutPadding>().Sum(i => i.Size);
        // Include paddings for value types only, since the reference can be exclusive or shared. Primitive types can be recursive.
        var nestedPaddings = fields
            .OfType<FieldLayoutActual>()
            .Where(i => i.FieldInfo.FieldType.IsValueType && !i.FieldInfo.FieldType.IsPrimitive)
            .Select(i => Get(i.FieldInfo.FieldType, includePadding))
            .Sum(i => i.Paddings);
        var result = new Models.TypeLayout(type, size, overhead, fields, instancePaddings + nestedPaddings);
        cacheItems.AddOrUpdate(type, result, (_, layout) => layout);
        return result;
    }

    /// <summary>
    /// Return type layout fields
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="size"></param>
    /// <param name="includePadding"></param>
    /// <returns></returns>
    private static List<FieldLayout> GetFields(FieldLayoutActual[] fields, int size, bool includePadding)
    {
        var result = new List<FieldLayout>();
        if (includePadding && fields.Length != 0 && fields[0].Offset != 0)
            result.Add(new FieldLayoutPadding(0, fields[0].Offset));
        for (var index = 0; index < fields.Length; index++)
        {
            var field = fields[index];
            result.Add(field);
            if (includePadding)
            {
                var nextOffsetOrSize = index != fields.Length - 1 ? fields[index + 1].Offset : size;
                var nextSectionOffsetCandidate = field.Offset + field.Size;
                if (nextSectionOffsetCandidate < nextOffsetOrSize)
                    result.Add(new FieldLayoutPadding(nextSectionOffsetCandidate, nextOffsetOrSize - nextSectionOffsetCandidate));
            }
        }
        return result;
    }
    #endregion

}