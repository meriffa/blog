using CommandLine;
using System.Text;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// String controller
/// </summary>
[Verb("Concepts-String", HelpText = "String operation.")]
public class StringController : Controller
{

    #region Constants
    private const string STRING_CONSTANT = "Constant string instance.";
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayConstantString();
        DisplayDynamicStringConcat(["Dynamic string instance", " ", "(generated using String.Concat)."]);
        DisplayDynamicStringStringBuilder(["Dynamic string instance", " ", "(generated using StringBuilder)."]);
        DisplayDynamicStringFormat("Dynamic string instance {0}", "(generated using String.Format).");
        DisplayDynamicStringInterpolation(Random.Shared.Next(100));
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display constant string
    /// </summary>
    private void DisplayConstantString()
    {
        displayService.WriteInformation(STRING_CONSTANT);
    }

    /// <summary>
    /// Display dynamic string (String.Concat)
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="third"></param>
    private void DisplayDynamicStringConcat(string[] values)
    {
        var text = "";
        foreach (var value in values)
            text += value;
        displayService.WriteInformation(text);
    }

    /// <summary>
    /// Display dynamic string (StringBuilder)
    /// </summary>
    /// <param name="values"></param>
    private void DisplayDynamicStringStringBuilder(string[] values)
    {
        var builder = new StringBuilder();
        foreach (var value in values)
            builder.Append(value);
        var text = builder.ToString();
        displayService.WriteInformation(text);
    }

    /// <summary>
    /// Display dynamic string (String.Format)
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    private void DisplayDynamicStringFormat(string format, string arg0)
    {
        var text = string.Format(format, arg0);
        displayService.WriteInformation(text);
    }

    /// <summary>
    /// Display dynamic string (string interpolation)
    /// </summary>
    /// <param name="data"></param>
    private void DisplayDynamicStringInterpolation(int data)
    {
        var text = $"Dynamic string instance (generated using string interpolation, Data = {data})";
        displayService.WriteInformation(text);
    }
    #endregion

}