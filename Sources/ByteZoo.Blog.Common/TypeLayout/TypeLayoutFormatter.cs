using ByteZoo.Blog.Common.TypeLayout.Models;

namespace ByteZoo.Blog.Common.TypeLayout;

/// <summary>
/// Type layout formatter
/// </summary>
public static class TypeLayoutFormatter
{

    #region Public Methods
    /// <summary>
    /// Format type layout
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recursive"></param>
    public static string[] Format<T>(bool recursive = true) => Format(typeof(T), recursive);

    /// <summary>
    /// Format type layout
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recursive"></param>
    public static string[] Format(Type type, bool recursive = true) => Format(TypeLayoutBuilder.Get(type), recursive);

    /// <summary>
    /// Format type layout
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="recursive"></param>
    public static string[] Format(Models.TypeLayout layout, bool recursive = true)
    {
        var lines = new List<string>();
        lines.AddRange(GetHeader(layout));
        int level = 1;
        int offset = 0;
        if (!layout.Type.IsValueType)
        {
            // Reference type object header fields
            lines.AddRange(GetHeaderFields(level));
            offset += IntPtr.Size;
        }
        lines.AddRange(GetFields(layout, level, offset, recursive));
        return [.. lines];
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return type layout header
    /// </summary>
    /// <param name="layout"></param>
    /// <returns></returns>
    private static string[] GetHeader(Models.TypeLayout layout) => [$"Layout: Name = '{layout.Type.FullName}', Size = {GetBytes(layout.FullSize)}, Data = {GetBytes(layout.Size)}, Padding = {GetBytes(layout.Paddings)}"];

    /// <summary>
    /// Return type layout fields
    /// </summary>
    /// <param name="typeLayout"></param>
    /// <param name="level"></param>
    /// <param name="offset"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    private static List<string> GetFields(Models.TypeLayout typeLayout, int level, int offset, bool recursive)
    {
        var lines = new List<string>();
        foreach (var field in typeLayout.Fields)
        {
            lines.Add(GetField(field, level, offset));
            // Ignore recursion for reference type fields, since they are pointers only.
            if (recursive && field is FieldLayoutActual actualField && actualField.FieldInfo.FieldType.IsValueType)
            {
                var fieldLayout = TypeLayoutBuilder.Get(actualField.FieldInfo.FieldType);
                // Include field nested structure only if the field type has fields and the field type is not a primitive (e.g. Int32 will cause infinite recursion).
                if (fieldLayout.Fields.Length > 0 && !fieldLayout.Type.IsPrimitive)
                    lines.AddRange(GetFields(fieldLayout, level + 1, offset + actualField.Offset, recursive));
            }
        }
        return lines;
    }

    /// <summary>
    /// Return field layout
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="level"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private static string GetField(FieldLayout layout, int level, int offset) => $"{GetOffset(offset + layout.Offset, layout.Size)} {GetFieldPrefix(level)} {layout.Name} ({GetBytes(layout.Size)})";

    /// <summary>
    /// Return object header fields
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static string[] GetHeaderFields(int level) =>
    [
        $"{GetOffset(-8, IntPtr.Size)} {GetFieldPrefix(level)} <Object Header> ({IntPtr.Size} bytes)",
        $"{GetOffset(0, IntPtr.Size)} {GetFieldPrefix(level)} <MT> ({IntPtr.Size} bytes)"
    ];

    /// <summary>
    /// Return field prefix
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static string GetFieldPrefix(int level) => "".PadLeft(level, '-');

    /// <summary>
    /// Return field offset
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static string GetOffset(int offset, int size) => offset >= 0 ? $"{offset:X4}-{offset + size - 1:X4}:" : $"{offset:D3}-{offset + size - 1:D3}:";

    /// <summary>
    /// Return byte(s) text
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private static string GetBytes(int count) => count == 1 ? $"{count} byte" : $"{count} bytes";
    #endregion

}