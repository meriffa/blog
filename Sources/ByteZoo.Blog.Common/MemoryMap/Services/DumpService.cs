using ByteZoo.Blog.Common.MemoryMap.Enums;
using ByteZoo.Blog.Common.MemoryMap.Records;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ByteZoo.Blog.Common.MemoryMap.Services;

/// <summary>
/// Core dump service
/// </summary>
/// <param name="dumpFile"></param>
public partial class DumpService(string dumpFile) : MemoryMapService
{

    #region Regular Expressions
    /// <summary>
    /// Segments count line
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^There are (?<Count>\d+) program headers, starting at offset \d+$")]
    private static partial Regex SegmentsCountLine();

    /// <summary>
    /// Segments line 1
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\s+(?<Type>[A-Z]+)\s+0x(?<Offset>[0-9a-f]+)\s+0x(?<VirtAddr>[0-9a-f]+)\s+0x(?<PhysAddr>[0-9a-f]+)$")]
    private static partial Regex SegmentsLine1();

    /// <summary>
    /// Segments line 2
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\s+0x(?<FileSiz>[0-9a-f]+)\s+0x(?<MemSiz>[0-9a-f]+)(?<Flags>[RWE\s]+)0x(?<Align>[0-9a-f]+)$")]
    private static partial Regex SegmentsLine2();

    /// <summary>
    /// Notes header line
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\s+CORE\s+0x[0-9a-f]+\tNT_FILE \(mapped files\)$")]
    private static partial Regex NotesHeaderLine();

    /// <summary>
    /// Notes line
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\s+(?<Start>0x[0-9a-f]+)\s+(?<End>0x[0-9a-f]+)\s+(?<PageOffset>0x[0-9a-f]+)$")]
    private static partial Regex NotesLine();
    #endregion

    #region Public Methods
    /// <summary>
    /// Return memory regions
    /// </summary>
    /// <returns></returns>
    public override List<MemoryRegion> GetMemoryRegions()
    {
        var regions = new List<MemoryRegion>();
        if (!Path.Exists(dumpFile))
            throw new($"Core dump '{dumpFile}' not found.");
        using var process = Process.Start(new ProcessStartInfo(GetReadElfPath()) { Arguments = $"--segments \"{dumpFile}\"", RedirectStandardOutput = true });
        process!.WaitForExit();
        if (process.ExitCode != 0)
            throw new($"Core dump '{dumpFile}' segment read failed.");
        var lines = process.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0 || lines[0] != "Elf file type is CORE (Core file)")
            throw new($"Invalid core dump '{dumpFile}' segment header.");
        var segmentsCount = Convert.ToInt32(SegmentsCountLine().Match(lines[2]).Groups["Count"].Value);
        for (int i = 0; i < segmentsCount; i++)
        {
            var matchSegmentsLine1 = SegmentsLine1().Match(lines[2 * i + 6]);
            if (!matchSegmentsLine1.Success)
                throw new($"Invalid core dump '{dumpFile}' segment line #1 ('{lines[2 * i + 6]}').");
            if (Enum.Parse<CoreDumpSegmentType>(matchSegmentsLine1.Groups["Type"].Value, true) != CoreDumpSegmentType.Note)
            {
                var start = Convert.ToInt64(matchSegmentsLine1.Groups["VirtAddr"].Value, 16);
                if (Convert.ToInt64(matchSegmentsLine1.Groups["PhysAddr"].Value, 16) != 0L)
                    throw new($"Invalid core dump '{dumpFile}' segment physical address ('{lines[2 * i + 6]}').");
                var matchSegmentsLine2 = SegmentsLine2().Match(lines[2 * i + 7]);
                if (!matchSegmentsLine2.Success)
                    throw new($"Invalid core dump '{dumpFile}' segment line #2 ('{lines[2 * i + 7]}').");
                var size = Convert.ToInt64(matchSegmentsLine2.Groups["MemSiz"].Value, 16);
                if (size != Convert.ToInt64(matchSegmentsLine2.Groups["FileSiz"].Value, 16))
                    throw new($"Invalid core dump '{dumpFile}' memory size ('{lines[2 * i + 7]}').");
                var permissions = GetMemoryRegionPermissions(matchSegmentsLine2.Groups["Flags"].Value.Replace(" ", "").ToLower().Trim());
                regions.Add(new(Path: Anonymous, Vss: size, Rss: size, Pss: size, Uss: size, Start: start, End: start + size, Permissions: permissions));
            }
        }
        return UpdateDumpMemoryRegions(regions, GetMemoryRegionMappings());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return ReadElf path
    /// </summary>
    /// <returns></returns>
    private static string GetReadElfPath()
    {
        var path = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/", "binutils", "readelf");
        return Path.Exists(path) ? path : "readelf";
    }

    /// <summary>
    /// Return core dump memory region mappings
    /// </summary>
    /// <returns></returns>
    private List<MemoryRegionMapping> GetMemoryRegionMappings()
    {
        var mappings = new List<MemoryRegionMapping>();
        if (!Path.Exists(dumpFile))
            throw new($"Core dump '{dumpFile}' not found.");
        using var process = Process.Start(new ProcessStartInfo(GetReadElfPath()) { Arguments = $"--notes \"{dumpFile}\"", RedirectStandardOutput = true });
        process!.WaitForExit();
        if (process.ExitCode != 0)
            throw new($"Core dump '{dumpFile}' notes read failed.");
        var lines = process.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0 || !lines[0].StartsWith("Displaying notes found at file offset "))
            throw new($"Invalid core dump '{dumpFile}' notes header.");
        var index = 0;
        while (++index < lines.Length)
            if (NotesHeaderLine().IsMatch(lines[index]))
            {
                index += 3;
                Match match;
                while ((match = NotesLine().Match(lines[index])).Success)
                {
                    var start = Convert.ToInt64(match.Groups["Start"].Value, 16);
                    var end = Convert.ToInt64(match.Groups["End"].Value, 16);
                    var path = lines[index + 1].Trim();
                    mappings.Add(new(Path: path, Start: start, End: end));
                    index += 2;
                }
                index--;
            }
        return mappings;
    }

    /// <summary>
    /// Update core dump memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="mappings"></param>
    /// <returns></returns>
    private static List<MemoryRegion> UpdateDumpMemoryRegions(List<MemoryRegion> regions, List<MemoryRegionMapping> mappings)
    {
        for (int i = 0; i < regions.Count; i++)
        {
            var region = regions[i];
            var regionMappings = mappings.Where(i => i.Start <= region.Start && region.End <= i.End);
            if (regionMappings.Count() > 0)
            {
                if (regionMappings.Count() > 1)
                    throw new($"Multiple region mappings found (Start = {FormatAddress(region.Start)}, End = {FormatAddress(region.End)}).");
                regions[i] = region with { Path = regionMappings.First().Path };
            }
        }
        return regions;
    }
    #endregion

}