using CommandLine;
using System.Numerics;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Iterator controller
/// </summary>
[Verb("Concepts-Iterator", HelpText = "Iterator operation.")]
public class IteratorController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayFibonacciSequence(11);
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display Fibonacci sequence
    /// </summary>
    /// <param name="index"></param>
    private void DisplayFibonacciSequence(int index)
    {
        var sequence = GenerateFibonacciSequence<int>();
        var results = sequence.Take(index);
        var result = results.Last();
        displayService.WriteInformation($"[Fibonacci] F{index - 1} = {result}");
    }

    /// <summary>
    /// Generate Fibonacci sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static IEnumerable<T> GenerateFibonacciSequence<T>() where T : INumber<T>
    {
        var previous = T.Zero;
        yield return previous;
        var current = T.One;
        yield return current;
        while (true)
        {
            var savedPrevious = previous;
            previous = current;
            current += savedPrevious;
            yield return current;
        }
    }
    #endregion

}