using ByteZoo.Blog.Common.Interop;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// P/Invoke controller
/// </summary>
[Verb("Concepts-PInvoke", HelpText = "P/Invoke operation.")]
public class PInvokeController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var size = 256;
        byte fillValue = 0x03;
        using var region1 = new ManagedHeapMemoryRegion(size);
        using var region2 = new ManagedHeapMemoryRegion(size);
        displayService.WriteInformation($"Memory regions allocated (Buffer 1 = {region1.Buffer.Sum()}, Buffer 2 = {region2.Buffer.Sum()}).");
        region1.Fill(fillValue);
        region2.Fill(fillValue);
        displayService.WriteInformation($"Memory regions filled (Buffer 1 = {GetSum(region1.Buffer)}, Buffer 2 = {GetSum(region2.Buffer)}).");
        displayService.WriteInformation($"Memory regions compared (Match = {region1.CompareWith(region2)}).");
        region1.Clear();
        region2.Clear();
        displayService.WriteInformation($"Memory regions cleared (Buffer 1 = {region1.Buffer.Sum()}, Buffer 2 = {region2.Buffer.Sum()}).");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return array sum (unchecked)
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetSum(int[] buffer) => buffer.Aggregate((sum, i) => unchecked(sum + i));
    #endregion

}