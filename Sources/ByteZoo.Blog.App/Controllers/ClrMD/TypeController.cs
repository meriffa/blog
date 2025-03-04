using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Type viewer controller (name2ee, dumpclass, dumpmt)
/// </summary>
[Verb("ClrMD-Type", HelpText = "Type viewer.")]
public class TypeController : DumpController
{

    #region Properties
    /// <summary>
    /// Type MethodTable
    /// </summary>
    [Option("methodTable", SetName = "MethodTable", Required = true, HelpText = "Type MethodTable.")]
    public string? MethodTable { get; set; }

    /// <summary>
    /// Type module name
    /// </summary>
    [Option("moduleName", SetName = "Name", Required = true, HelpText = "Type module name.")]
    public string? ModuleName { get; set; }

    /// <summary>
    /// Type name
    /// </summary>
    [Option("typeName", SetName = "Name", Required = true, HelpText = "Type name.")]
    public string? TypeName { get; set; }

    /// <summary>
    /// Exclude type interfaces
    /// </summary>
    [Option("excludeInterfaces", HelpText = "Exclude type interfaces.")]
    public bool ExcludeInterfaces { get; set; }

    /// <summary>
    /// Exclude type generic parameters
    /// </summary>
    [Option("excludeGenericParameters", HelpText = "Exclude type generic parameters.")]
    public bool ExcludeGenericParameters { get; set; }

    /// <summary>
    /// Exclude type fields
    /// </summary>
    [Option("excludeFields", HelpText = "Exclude type fields.")]
    public bool ExcludeFields { get; set; }

    /// <summary>
    /// Exclude type methods
    /// </summary>
    [Option("excludeMethods", HelpText = "Exclude type methods.")]
    public bool ExcludeMethods { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        DisplayType(GetClrType(runtime));
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return CLR type
    /// </summary>
    /// <param name="runtime"></param>
    /// <returns></returns>
    private ClrType GetClrType(ClrRuntime runtime)
    {
        if (!string.IsNullOrEmpty(MethodTable))
            if (runtime.GetTypeByMethodTable(Convert.ToUInt64(MethodTable, 16)) is ClrType type)
                return type;
            else
                throw new Exception($"MethodTable {MethodTable} is not found.");
        else if (!string.IsNullOrEmpty(ModuleName) && !string.IsNullOrEmpty(TypeName))
            if (GetModule(runtime, ModuleName).GetTypeByName(TypeName) is ClrType type)
                return type;
            else
                throw new Exception($"Type name '{TypeName}' is not found.");
        throw new Exception($"Type filter is not specified.");
    }

    /// <summary>
    /// Display type information
    /// </summary>
    /// <param name="type"></param>
    private void DisplayType(ClrType type)
    {
        displayService.WriteInformation($"Type: Name = {type.Name}, MT = {GetAddress(type.MethodTable)}, Base = {type.BaseType!.Name}, Element = {type.ElementType}, Kind = {(type.IsValueType ? "Value" : "Reference")}, mdToken = {GetToken(type.MetadataToken)}");
        if (!ExcludeInterfaces)
            foreach (var @interface in type.EnumerateInterfaces())
                displayService.WriteInformation($"Interface: Name = {@interface.Name}");
        if (!ExcludeGenericParameters)
            foreach (var parameter in type.EnumerateGenericParameters())
                displayService.WriteInformation($"Generic Parameter: Name = {parameter.Name}, Index = {parameter.Index}");
        if (!ExcludeFields)
        {
            foreach (var field in type.Fields)
                displayService.WriteInformation($"Instance Field: Name = {field.Name}, Type = {field.Type?.Name}");
            foreach (var field in type.StaticFields)
                displayService.WriteInformation($"Static Field: Name = {field.Name}, Type = {field.Type!.Name}, Value = {ObjectController.GetValue(type.Module.AppDomain, field)}");
            foreach (var field in type.ThreadStaticFields)
                displayService.WriteInformation($"Thread Static Field: Name = {field.Name}, Type = {field.Type!.Name}, Values = {ObjectController.GetValues(field)}");
        }
        if (!ExcludeMethods)
            foreach (var method in type.Methods)
                displayService.WriteInformation($"Method: MD = {GetAddress(method.MethodDesc)}, Signature = {method.Signature}");
    }
    #endregion

}