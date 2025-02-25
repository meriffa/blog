using ByteZoo.Blog.Common.TypeLayout;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Reflection;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Type layout viewer controller
/// </summary>
[Verb("Tools-TypeLayout", HelpText = "Type layout viewer operation.")]
public class TypeLayoutController : Controller
{

    #region Properties
    /// <summary>
    /// Type input file
    /// </summary>
    [Option("inputFile", Required = true, HelpText = "Type input file.")]
    public string InputFile { get; set; } = null!;

    /// <summary>
    /// Type name
    /// </summary>
    [Option("typeName", Required = true, HelpText = "Type name.")]
    public string TypeName { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var stream = new MemoryStream();
        var compiler = GetCompiler();
        var result = compiler.Emit(stream);
        if (result.Success)
            DisplayTypeLayout(Assembly.Load(stream.ToArray()));
        else
            DisplayErrors(result.Diagnostics);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return assembly references
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<MetadataReference> GetAssemblyReferences()
    {
        var path = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        return
        [
            MetadataReference.CreateFromFile(Path.Combine(path, "System.Private.CoreLib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(path, "System.Collections.dll")),
            MetadataReference.CreateFromFile(Path.Combine(path, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(path, "System.Linq.dll"))
        ];
    }

    /// <summary>
    /// Return compiler instance
    /// </summary>
    /// <returns></returns>
    private CSharpCompilation GetCompiler()
    {
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(File.ReadAllText(InputFile)) };
        var references = GetAssemblyReferences();
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release);
        return CSharpCompilation.Create(Path.GetRandomFileName(), syntaxTrees, references, options);
    }

    /// <summary>
    /// Display type layout
    /// </summary>
    /// <param name="assembly"></param>
    void DisplayTypeLayout(Assembly assembly)
    {
        var type = assembly.GetType(TypeName);
        if (type != null)
            foreach (var line in TypeLayoutFormatter.Format(type))
                displayService.WriteInformation(line);
        else
            throw new($"Type name '{TypeName}' is not found.");
    }

    /// <summary>
    /// Display compile errors
    /// </summary>
    /// <param name="diagnostics"></param>
    private void DisplayErrors(ImmutableArray<Diagnostic> diagnostics)
    {
        foreach (var error in diagnostics.Where(i => i.IsWarningAsError || i.Severity == DiagnosticSeverity.Error))
            displayService.WriteWarning(error.GetMessage());
        throw new($"Input file '{InputFile}' compile failed.");
    }
    #endregion

}