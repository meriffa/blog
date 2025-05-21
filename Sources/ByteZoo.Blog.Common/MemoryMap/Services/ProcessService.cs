using ByteZoo.Blog.Common.MemoryMap.Records;
using Microsoft.Diagnostics.Runtime;
using System.Text.RegularExpressions;

namespace ByteZoo.Blog.Common.MemoryMap.Services;

/// <summary>
/// Live process service
/// </summary>
/// <param name="processId"></param>
public partial class ProcessService(int processId) : MemoryMapService
{

    #region Regular Expressions
    /// <summary>
    /// Maps header line
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(?<Start>[0-9a-f]+)-(?<End>[0-9a-f]+)\s(?<Perms>[rwxps-]+)\s(?<Offset>[0-9a-f]+)\s(?<Device>[0-9]{2}:[0-9]{2})\s(?<Inode>[0-9]+)\s+(?<Path>.*)$")]
    private static partial Regex MapsHeaderLine();

    /// <summary>
    /// Maps value line
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^[^:]+:\s+(?<Value>[0-9]+)\skB$")]
    private static partial Regex MapsValueLine();
    #endregion

    #region Public Methods
    /// <summary>
    /// Return memory regions
    /// </summary>
    /// <returns></returns>
    public override List<MemoryRegion> GetMemoryRegions()
    {
        var regions = new List<MemoryRegion>();
        using var reader = new StreamReader($"/proc/{processId}/smaps");
        var lines = reader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var index = 0;
        while (index < lines.Length)
            regions.Add(GetMemoryRegion(lines, ref index));
        return UpdateClrMemoryRegions(ConsolidateMemoryRegions(regions));
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return target instance
    /// </summary>
    /// <returns></returns>
    protected override DataTarget GetDataTarget() => DataTarget.AttachToProcess(processId, false);
    #endregion

    #region Private Methods
    /// <summary>
    /// Return memory region
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static MemoryRegion GetMemoryRegion(string[] lines, ref int index)
    {
        var match = MapsHeaderLine().Match(lines[index]);
        if (!match.Success)
            throw new($"Invalid /proc/pid/maps entry ('{lines[index]}').");
        var start = Convert.ToUInt64(match.Groups["Start"].Value, 16);
        var end = Convert.ToUInt64(match.Groups["End"].Value, 16) - 1UL;
        var permissions = GetMemoryRegionPermissions(match.Groups["Perms"].Value);
        var size = 0UL;
        while (!lines[index++].StartsWith("VmFlags: ") && index < lines.Length)
            if (lines[index].StartsWith("Size: "))
                size = GetMemoryRegionValue(lines[index]);
        if (size == 0L)
            throw new($"Entry /proc/pid/maps invalid size (Start = {start:X16}, End = {end:X16}, Size = {size:X16}).");
        if (end - start + 1UL != size)
            throw new($"Entry /proc/pid/maps size mismatch (Start = {start:X16}, End = {end:X16}, Size = {size:X16}).");
        if (start >= end)
            throw new($"Entry /proc/pid/maps invalid region range (Start = {start:X16}, End = {end:X16}, Size = {size:X16}).");
        return new MemoryRegion(Start: start, End: end, Permissions: permissions)
        {
            Path = !string.IsNullOrEmpty(match.Groups["Path"].Value) ? match.Groups["Path"].Value : Anonymous
        };
    }

    /// <summary>
    /// Return memory region value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static ulong GetMemoryRegionValue(string value) => Convert.ToUInt64(MapsValueLine().Match(value).Groups["Value"].Value) * 1024L;
    #endregion

}