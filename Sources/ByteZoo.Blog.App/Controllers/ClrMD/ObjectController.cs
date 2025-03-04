using CommandLine;
using Microsoft.Diagnostics.Runtime;
using System.Text;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Object viewer controller (dumpobj)
/// </summary>
[Verb("ClrMD-Object", HelpText = "Object viewer.")]
public class ObjectController : DumpController
{

    #region Properties
    /// <summary>
    /// Object address
    /// </summary>
    [Option("address", Required = true, HelpText = "Object address.")]
    public string Address { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        if (runtime.Heap.GetObject(ParseAddress(Address)) is ClrObject clrObject)
            DisplayObject(clrObject);
        else
            throw new Exception($"Object at address {Address} is not found.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display object value
    /// </summary>
    /// <param name="clrObject"></param>
    private void DisplayObject(ClrObject clrObject)
    {
        displayService.WriteInformation($"Object: Address = {GetAddress(clrObject.Address)}, Type = {clrObject.Type?.Name}, MT = {GetAddress(clrObject.Type!.MethodTable)}, Array = {clrObject.IsArray}, Delegate = {clrObject.IsDelegate}, Exception = {clrObject.IsException}, Boxed Value = {clrObject.IsBoxedValue}, Runtime Type = {clrObject.IsRuntimeType}, Free = {clrObject.IsFree}, Thin Lock = {clrObject.GetThinLock != null}, Sync Block = {clrObject.SyncBlock != null}");
        foreach (var field in clrObject.Type.Fields)
            displayService.WriteInformation($"Instance Field: Name = {field.Name}, Type = {field.Type?.Name}, Value Type = {field.IsValueType}, Value = {GetValue(clrObject, field)}, Offset = {field.Offset}, Size = {field.Size}");
        foreach (var field in clrObject.Type.StaticFields)
            displayService.WriteInformation($"Static Field: Name = {field.Name}, Type = {field.Type?.Name}, Value Type = {field.IsValueType}, Value = {GetValue(clrObject.Type.Module.AppDomain, field)}, Offset = {field.Offset}, Size = {field.Size}");
        foreach (var field in clrObject.Type.ThreadStaticFields)
            displayService.WriteInformation($"Thread Static Field: Name = {field.Name}, Type = {field.Type?.Name}, Value Type = {field.IsValueType}, Values = {GetValues(field)}, Offset = {field.Offset}, Size = {field.Size}");
    }

    /// <summary>
    /// Return field value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private static string? GetValue(ClrObject clrObject, ClrInstanceField field) => field.ElementType switch
    {
        ClrElementType.Boolean => clrObject.ReadField<bool>(field.Name!).ToString(),
        ClrElementType.Char => clrObject.ReadField<char>(field.Name!).ToString(),
        ClrElementType.Int8 => clrObject.ReadField<sbyte>(field.Name!).ToString(),
        ClrElementType.UInt8 => clrObject.ReadField<byte>(field.Name!).ToString(),
        ClrElementType.Int16 => clrObject.ReadField<short>(field.Name!).ToString(),
        ClrElementType.UInt16 => clrObject.ReadField<ushort>(field.Name!).ToString(),
        ClrElementType.Int32 => clrObject.ReadField<int>(field.Name!).ToString(),
        ClrElementType.UInt32 => clrObject.ReadField<uint>(field.Name!).ToString(),
        ClrElementType.Int64 => clrObject.ReadField<long>(field.Name!).ToString(),
        ClrElementType.UInt64 => clrObject.ReadField<ulong>(field.Name!).ToString(),
        ClrElementType.Float => clrObject.ReadField<float>(field.Name!).ToString(),
        ClrElementType.Double => clrObject.ReadField<double>(field.Name!).ToString(),
        ClrElementType.String => clrObject.ReadStringField(field.Name!),
        _ => $"<{field.ElementType}>",
    };

    /// <summary>
    /// Return field value
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string? GetValue(ClrAppDomain domain, ClrStaticField field) => field.ElementType switch
    {
        ClrElementType.Boolean => field.Read<bool>(domain).ToString(),
        ClrElementType.Char => field.Read<char>(domain).ToString(),
        ClrElementType.Int8 => field.Read<sbyte>(domain).ToString(),
        ClrElementType.UInt8 => field.Read<byte>(domain).ToString(),
        ClrElementType.Int16 => field.Read<short>(domain).ToString(),
        ClrElementType.UInt16 => field.Read<ushort>(domain).ToString(),
        ClrElementType.Int32 => field.Read<int>(domain).ToString(),
        ClrElementType.UInt32 => field.Read<uint>(domain).ToString(),
        ClrElementType.Int64 => field.Read<long>(domain).ToString(),
        ClrElementType.UInt64 => field.Read<ulong>(domain).ToString(),
        ClrElementType.Float => field.Read<float>(domain).ToString(),
        ClrElementType.Double => field.Read<double>(domain).ToString(),
        ClrElementType.String => field.ReadString(domain),
        _ => $"<{field.ElementType}>",
    };

    /// <summary>
    /// Return field values
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string GetValues(ClrThreadStaticField field)
    {
        var result = new StringBuilder();
        if (field.Type != null)
            foreach (var thread in field.Type.Module.AppDomain.Runtime.Threads)
                if (field.IsInitialized(thread) && field.IsObjectReference)
                    if (!field.ReadObject(thread).IsNull)
                        result.Append("<Object>, ");
        return result.Length != 0 ? result.ToString(0, result.Length - 2) : "<Unknown>";
    }
    #endregion

}