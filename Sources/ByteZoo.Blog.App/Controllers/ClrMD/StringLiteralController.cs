using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// String literal finder controller
/// </summary>
[Verb("ClrMD-StringLiteral", HelpText = "String literal finder.")]
public class StringLiteralController : DumpController
{

    #region Constants
    private const string SYSTEM_STRING = "System.String";
    private const string FIELD_STRING_LENGTH = "_stringLength";
    private static readonly string CORE_LIB = "System.Private.CoreLib.dll";
    #endregion

    #region Properties
    /// <summary>
    /// String value
    /// </summary>
    [Option("value", Required = true, HelpText = "String value.")]
    public string Value { get; set; } = null!;

    /// <summary>
    /// String value
    /// </summary>
    [Option("ignoreCase", Default = false, HelpText = "Perform case insensitive search.")]
    public bool IgnoreCase { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var comparisonType = IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.Ordinal;
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        var type = runtime.Heap.GetTypeByName(GetModule(runtime, CORE_LIB), SYSTEM_STRING) ?? throw new($"Type '{SYSTEM_STRING}' is not found.");
        foreach (var clrObject in runtime.Heap.EnumerateObjects().Where(i => i.Type == type))
        {
            var length = GetStringLength(clrObject);
            var value = GetStringValue(clrObject, Value.Length);
            if (Value.Length == length && Value.Equals(value, comparisonType))
                displayService.WriteInformation($"Address = {GetAddress(clrObject.Address)}, String Length = {length}, Object Size = {GetStringSize(length)}, Value = '{value}'");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return string length
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static int GetStringLength(ClrObject clrObject) => clrObject.ReadField<int>(FIELD_STRING_LENGTH);

    /// <summary>
    /// Return string value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    private static string? GetStringValue(ClrObject clrObject, int maxLength) => clrObject.AsString(maxLength);

    /// <summary>
    /// Return string object size
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static int GetStringSize(int length) => 20 + (length + 1) * 2;
    #endregion

}