using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// Modules viewer controller
/// </summary>
[Verb("ClrMD-Modules", HelpText = "Modules viewer.")]
public class ModulesController : DumpController
{

    #region Properties
    /// <summary>
    /// Module name
    /// </summary>
    [Option("moduleName", HelpText = "Module name.")]
    public string? ModuleName { get; set; }

    /// <summary>
    /// Include module defined types
    /// </summary>
    [Option("includeTypes", HelpText = "Include module defined types.")]
    public bool IncludeTypes { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        if (runtime.SystemDomain != null)
            DisplayModules(runtime, runtime.SystemDomain);
        if (runtime.SharedDomain != null)
            DisplayModules(runtime, runtime.SharedDomain);
        foreach (var domain in runtime.AppDomains)
            DisplayModules(runtime, domain);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display domain modules
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="domain"></param>
    private void DisplayModules(ClrRuntime runtime, ClrAppDomain domain)
    {
        displayService.WriteInformation($"Domain: ID = {domain.Id}, Address = {GetAddress(domain.Address)}, Name = '{domain.Name}'");
        foreach (var module in domain.Modules)
            if (ModuleName == null || (module.Name != null && Path.GetFileName(module.Name).StartsWith(ModuleName)))
            {
                displayService.WriteInformation($"Module: Name = '{module.Name ?? "<N/A>"}', Address = {GetAddress(module.Address)}, Size = {GetSize(module.Size)}, Dynamic = {module.IsDynamic}");
                if (IncludeTypes)
                    DisplayModuleTypes(runtime, module);
            }
    }

    /// <summary>
    /// Display module types
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="module"></param>
    private void DisplayModuleTypes(ClrRuntime runtime, ClrModule module)
    {
        foreach (var (methodTable, _) in module.EnumerateTypeDefToMethodTableMap())
            if (runtime.GetTypeByMethodTable(methodTable) is ClrType type)
                displayService.WriteInformation($"Type: MT = {GetAddress(type.MethodTable)}, Name = '{type.Name}'");
    }
    #endregion

}