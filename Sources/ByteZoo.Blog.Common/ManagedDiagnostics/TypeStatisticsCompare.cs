namespace ByteZoo.Blog.Common.ManagedDiagnostics;

/// <summary>
/// Type statistics compare
/// </summary>
/// <param name="Source"></param>
/// <param name="Target"></param>
public record TypeStatisticsCompare(TypeStatistics? Source, TypeStatistics? Target);