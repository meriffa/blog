using CommandLine;
using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Tiered compilation controller
/// </summary>
[Verb("Concepts-TieredCompilation", HelpText = "Tiered compilation operation.")]
public class TieredCompilationController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        // Part 1
        int[] values = [.. Enumerable.Range(0, 1_000)];
        var multiplier = 42;
        GetValuesMultiply(values, multiplier);
        displayService.WriteInformation("Part 1 completed.");
        displayService.Wait();
        // Part 2
        for (var i = 1; i <= 30; i++)
            GetValuesMultiply(values, multiplier);
        displayService.WriteInformation("Part 2 completed.");
        displayService.Wait();
        // Part 3
        for (var i = 1; i <= 30; i++)
            GetValuesMultiply(values, multiplier);
        displayService.WriteInformation("Part 3 completed.");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return values multiplied by constant
    /// </summary>
    /// <param name="values"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int GetValuesMultiply(int[] values, int multiplier) => GetValuesSum(values, i => i * multiplier);

    /// <summary>
    /// Return sum of values
    /// </summary>
    /// <param name="values"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    private static int GetValuesSum(int[] values, Func<int, int> func)
    {
        var result = 0;
        foreach (var value in values)
            result += func(value);
        return result;
    }
    #endregion

}