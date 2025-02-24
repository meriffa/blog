using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Struct decoder controller
/// </summary>
[Verb("Tools-StructDecoder", HelpText = "Struct decoder operation.")]
public class StructDecoderController : Controller
{

    #region Properties
    /// <summary>
    /// Double precision floating point value
    /// </summary>
    [Option("double", SetName = "Double", Required = true, HelpText = "Double precision floating point value.")]
    public string? DoubleValue { get; set; }

    /// <summary>
    /// Decimal value (_lo64)
    /// </summary>
    [Option("decimalLow", SetName = "Decimal", Required = true, HelpText = "Decimal low 64-bits value.")]
    public ulong? DecimalValueLow { get; set; }

    /// <summary>
    /// Decimal value (_hi32)
    /// </summary>
    [Option("decimalHigh", SetName = "Decimal", Required = true, HelpText = "Decimal high 32-bit value.")]
    public uint? DecimalValueHigh { get; set; }

    /// <summary>
    /// Decimal value (_flags)
    /// </summary>
    [Option("decimalFlags", SetName = "Decimal", Required = true, HelpText = "Decimal flags value.")]
    public int? DecimalValueFlags { get; set; }

    /// <summary>
    /// DateTime (_dateData) value
    /// </summary>
    [Option("dateTime", SetName = "DateTime", Required = true, HelpText = "DateTime date data value.")]
    public ulong? DateTimeValue { get; set; }

    /// <summary>
    /// DateOnly (_dayNumber) value
    /// </summary>
    [Option("dateOnly", SetName = "DateOnly", Required = true, HelpText = "DateOnly day number value.")]
    public int? DateOnlyValue { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        if (DoubleValue != null)
            DisplayDouble(DoubleValue);
        if (DecimalValueLow != null && DecimalValueHigh != null && DecimalValueFlags != null)
            DisplayDecimal(DecimalValueLow.Value, DecimalValueHigh.Value, DecimalValueFlags.Value);
        if (DateTimeValue != null)
            DisplayDateTime(DateTimeValue.Value);
        if (DateOnlyValue != null)
            DisplayDateOnly(DateOnlyValue.Value);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display System.Double
    /// </summary>
    /// <param name="input"></param>
    private void DisplayDouble(string input)
    {
        var number = input.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? input[2..] : input;
        var value = ulong.Parse(number, System.Globalization.NumberStyles.AllowHexSpecifier);
        displayService.WriteInformation($"Double = {BitConverter.ToDouble(BitConverter.GetBytes(value), 0):f}");
    }

    /// <summary>
    /// Display System.Decimal
    /// </summary>
    /// <param name="input"></param>
    private void DisplayDecimal(ulong low, uint high, int flags)
    {
        var value = new int[] { (int)(low & 0xFFFFFFFF), (int)(low >> 32), (int)high, flags };
        displayService.WriteInformation($"Decimal = {new decimal(value)}");
    }

    /// <summary>
    /// Display System.DateTime
    /// </summary>
    /// <param name="dateData"></param>
    private void DisplayDateTime(ulong dateData) => displayService.WriteInformation($"DateTime = {DateTime.FromBinary((long)dateData):yyyy-MM-dd HH:mm:ss.ffff}");

    /// <summary>
    /// Display System.DateOnly
    /// </summary>
    /// <param name="dayNumber"></param>
    private void DisplayDateOnly(int dayNumber) => displayService.WriteInformation($"DateOnly = {DateOnly.FromDayNumber(dayNumber):yyyy-MM-dd}");
    #endregion

}