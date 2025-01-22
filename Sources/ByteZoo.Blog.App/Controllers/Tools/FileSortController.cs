using ByteZoo.Blog.Common.Extensions;
using ByteZoo.Blog.Common.Models.ExternalSort;
using ByteZoo.Blog.Common.Models.Files;
using ByteZoo.Blog.Common.Services;
using CommandLine;
using System.Diagnostics;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// File sort controller
/// </summary>
[Verb("Tools-FileSort", HelpText = "File sort operation.")]
public class FileSortController : Controller
{

    #region Properties
    /// <summary>
    /// Input file
    /// </summary>
    [Option('i', "input", Required = true, HelpText = "Input file.")]
    public string InputFile { get; set; } = null!;

    /// <summary>
    /// Output file
    /// </summary>
    [Option('o', "output", Required = true, HelpText = "Output file.")]
    public string OutputFile { get; set; } = null!;

    /// <summary>
    /// Sort order
    /// </summary>
    [Option('t', "type", Required = false, Default = SortType.Direct, HelpText = "Sort type.")]
    public SortType SortType { get; set; }

    /// <summary>
    /// Sort order
    /// </summary>
    [Option('s', "sort", Required = false, Default = SortOrder.Ascending, HelpText = "Sort order.")]
    public SortOrder SortOrder { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        if (SortType == SortType.Direct)
            SortDirect();
        else if (SortType == SortType.External)
            SortExternal();
        else
            throw new($"Sort type {SortType} is not supported.");
        var elapsed = stopwatch.StopElapsed();
        displayService.WriteInformation($"File sort completed (Duration = {elapsed}, Heap = {GC.GetTotalMemory(false)}, Memory = {GC.GetTotalAllocatedBytes(false)}).");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Sort file in memory
    /// </summary>
    private void SortDirect()
    {
        var list = new List<string>(File.ReadAllLines(InputFile));
        var contents = SortOrder == SortOrder.Ascending ? list.Order() : list.OrderDescending();
        File.WriteAllLines(OutputFile, contents);
    }

    /// <summary>
    /// Sort external
    /// </summary>
    private void SortExternal()
    {
        var comparer = SortOrder == SortOrder.Ascending ? Comparer<string>.Default : Comparer<string>.Create((x, y) => Comparer<string>.Default.Compare(y, x));
        var options = new Options() { Sort = new() { Comparer = comparer } };
        Task.Run(async () => await new ExternalSortService(options).Sort(InputFile, OutputFile)).Wait();
    }
    #endregion

}