using CommandLine;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Enum values viewer controller
/// </summary>
[Verb("ClrMD-EnumValues", HelpText = "Enum values viewer.")]
public class EnumValuesController : DumpController
{

    #region Properties
    /// <summary>
    /// Enum module name
    /// </summary>
    [Option("moduleName", Required = true, HelpText = "Enum module name.")]
    public string ModuleName { get; set; } = null!;

    /// <summary>
    /// Enum type name
    /// </summary>
    [Option("enumName", Required = true, HelpText = "Enum type name.")]
    public string EnumName { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        var type = GetModule(runtime, ModuleName).GetTypeByName(EnumName) ?? throw new($"Type '{EnumName}' is not found.");
        if (type.IsEnum)
            foreach (var (name, value) in type.AsEnum().EnumerateValues())
                displayService.WriteInformation($"{EnumName}.{name} = {value}");
        else
            throw new($"Type '{EnumName}' is not an enum.");
    }
    #endregion

}