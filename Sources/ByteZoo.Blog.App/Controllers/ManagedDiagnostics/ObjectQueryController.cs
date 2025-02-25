using ByteZoo.Blog.Common.ObjectQuery;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// Object query controller
/// </summary>
[Verb("ClrMD-ObjectQuery", HelpText = "Object query.")]
public class ObjectQueryController : DumpController
{

    #region Properties
    /// <summary>
    /// Query file
    /// </summary>
    [Option("queryFile", Required = true, HelpText = "Query file.")]
    public string QueryFile { get; set; } = null!;

    /// <summary>
    /// Output file
    /// </summary>
    [Option("outputFile", Required = true, HelpText = "Output file.")]
    public string OutputFile { get; set; } = null!;

    /// <summary>
    /// Output indented format
    /// </summary>
    [Option("indented", HelpText = "Output indented format.")]
    public bool WriteIndented { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        displayService.WriteInformation($"Object query started.");
        var query = new ObjectQueryBuilder(QueryFile);
        var writer = new ObjectQueryWriter(query.FieldFunctions, OutputFile, WriteIndented);
        foreach (var clrObject in runtime.Heap.EnumerateObjects().Where(query.FilterFunction))
            writer.Add(clrObject);
        writer.Write();
        displayService.WriteInformation($"Object query completed (Exported = {GetSize(writer.Count)}, Total = {GetSize(runtime.Heap.EnumerateObjects().Count())}, Output File = '{OutputFile}').");
    }
    #endregion

}