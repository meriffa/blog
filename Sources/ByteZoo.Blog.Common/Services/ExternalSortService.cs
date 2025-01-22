using ByteZoo.Blog.Common.Models.ExternalSort;

namespace ByteZoo.Blog.Common.Services;

/// <summary>
/// External sort service
/// </summary>
public class ExternalSortService(Options? options = null)
{

    #region Constants
    private const string EXTENSION_UNSORTED = ".unsorted";
    private const string EXTENSION_SORTED = ".sorted";
    private const string EXTENSION_TEMP = ".tmp";
    #endregion

    #region Private Members
    private int maxUnsortedRows;
    private readonly Options options = options ?? new Options();
    #endregion

    #region Public Methods
    /// <summary>
    /// Sort file contents
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Sort(string input, string output, CancellationToken? cancellationToken = null)
    {
        using var source = File.OpenRead(input);
        using var target = File.OpenWrite(output);
        await Sort(source, target, cancellationToken ?? CancellationToken.None);
    }

    /// <summary>
    /// Sort file contents
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Sort(FileStream source, FileStream target, CancellationToken cancellationToken)
    {
        var unsortedFiles = await SplitFile(source, cancellationToken);
        var sortBuffer = new string[maxUnsortedRows];
        if (unsortedFiles.Count == 1)
        {
            var fileName = Path.Combine(options.FileLocation, unsortedFiles.First());
            using var stream = File.OpenRead(fileName);
            await SortFile(stream, target, sortBuffer, cancellationToken);
            File.Delete(fileName);
        }
        else
        {
            var sortedFiles = await SortFiles(unsortedFiles, sortBuffer, cancellationToken);
            await MergeFiles(sortedFiles, target, GetTotalMergeFiles(sortedFiles.Count), cancellationToken);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Split file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<IReadOnlyCollection<string>> SplitFile(FileStream source, CancellationToken cancellationToken)
    {
        var unsortedFiles = new List<string>();
        var buffer = new byte[options.Split.FileSize];
        var lineRemainder = new List<byte>();
        var totalFiles = Math.Ceiling(source.Length / (double)options.Split.FileSize);
        var fileIndex = 0;
        await using (source)
            while (source.Position < source.Length)
            {
                (var rowsRead, var bytesRead) = SplitFileRead(source, buffer);
                SplitFileReadLineRemainder(source, buffer, lineRemainder);
                var fileName = $"{Path.GetFileName(source.Name)}.{++fileIndex}{EXTENSION_UNSORTED}";
                await using var stream = File.Create(Path.Combine(options.FileLocation, fileName));
                await stream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                if (lineRemainder.Count > 0)
                {
                    rowsRead++;
                    await stream.WriteAsync(lineRemainder.ToArray().AsMemory(0, lineRemainder.Count), cancellationToken);
                }
                maxUnsortedRows = Math.Max(rowsRead, maxUnsortedRows);
                options.Split.ProgressHandler?.Report(fileIndex / totalFiles);
                unsortedFiles.Add(fileName);
                lineRemainder.Clear();
            }
        return unsortedFiles;
    }

    /// <summary>
    /// Split file read chunk
    /// </summary>
    /// <param name="source"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private (int rowsRead, int bytesRead) SplitFileRead(Stream source, byte[] buffer)
    {
        var rowsRead = 0;
        var bytesRead = 0;
        while (bytesRead < options.Split.FileSize)
        {
            var value = source.ReadByte();
            if (value == -1)
                break;
            var valueByte = (byte)value;
            buffer[bytesRead] = valueByte;
            bytesRead++;
            if (valueByte == options.Split.NewLineSeparator)
                rowsRead++;
        }
        return (rowsRead, bytesRead);
    }

    /// <summary>
    /// Split file read chunk line remainder
    /// </summary>
    /// <param name="source"></param>
    /// <param name="buffer"></param>
    /// <param name="lineRemainder"></param>
    private void SplitFileReadLineRemainder(Stream source, byte[] buffer, List<byte> lineRemainder)
    {
        var lastByte = buffer[options.Split.FileSize - 1];
        while (lastByte != options.Split.NewLineSeparator)
        {
            var flag = source.ReadByte();
            if (flag == -1)
                break;
            lastByte = (byte)flag;
            lineRemainder.Add(lastByte);
        }
    }

    /// <summary>
    /// Sort files
    /// </summary>
    /// <param name="unsortedFiles"></param>
    /// <param name="sortBuffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<IReadOnlyList<string>> SortFiles(IReadOnlyCollection<string> unsortedFiles, string[] sortBuffer, CancellationToken cancellationToken)
    {
        var sortedFiles = new List<string>(unsortedFiles.Count);
        double totalFiles = unsortedFiles.Count;
        foreach (var unsortedFile in unsortedFiles)
        {
            var fileName = unsortedFile.Replace(EXTENSION_UNSORTED, EXTENSION_SORTED);
            var inputFile = Path.Combine(options.FileLocation, unsortedFile);
            var outputFile = Path.Combine(options.FileLocation, fileName);
            using var source = File.OpenRead(inputFile);
            using var target = File.OpenWrite(outputFile);
            await SortFile(source, target, sortBuffer, cancellationToken);
            File.Delete(inputFile);
            sortedFiles.Add(fileName);
            options.Sort.ProgressHandler?.Report(sortedFiles.Count / totalFiles);
        }
        return sortedFiles;
    }

    /// <summary>
    /// Sort file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="sortBuffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task SortFile(Stream source, Stream target, string[] sortBuffer, CancellationToken cancellationToken)
    {
        var counter = 0;
        using var reader = new StreamReader(source, bufferSize: options.Sort.InputBufferSize);
        while (!reader.EndOfStream)
            sortBuffer[counter++] = (await reader.ReadLineAsync(cancellationToken))!;
        Array.Sort(sortBuffer, options.Sort.Comparer);
        await using var writer = new StreamWriter(target, bufferSize: options.Sort.OutputBufferSize);
        foreach (var row in sortBuffer.Where(i => i is not null))
            await writer.WriteLineAsync(row);
        Array.Clear(sortBuffer, 0, sortBuffer.Length);
    }

    /// <summary>
    /// Return total number of merge files
    /// </summary>
    /// <param name="sortedFilesCount"></param>
    /// <returns></returns>
    private double GetTotalMergeFiles(int sortedFilesCount)
    {
        var totalFiles = sortedFilesCount;
        var result = sortedFilesCount / options.Merge.BatchSize;
        while (true)
        {
            if (result <= 0)
                break;
            totalFiles += result;
            result /= options.Merge.BatchSize;
        }
        return totalFiles;
    }

    /// <summary>
    /// Merge files
    /// </summary>
    /// <param name="sortedFiles"></param>
    /// <param name="target"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task MergeFiles(IReadOnlyList<string> sortedFiles, FileStream target, double totalFilesToMerge, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (sortedFiles.Count <= options.Merge.BatchSize)
            {
                await MergeFilesBatch(sortedFiles, target, totalFilesToMerge, cancellationToken);
                break;
            }
            var fileIndex = 0;
            foreach (var fileBatch in sortedFiles.Chunk(options.Merge.BatchSize))
            {
                var fileName = $"{Path.GetFileName(target.Name)}.{++fileIndex}{EXTENSION_SORTED}{EXTENSION_TEMP}";
                if (fileBatch.Length == 1)
                {
                    OverwriteFile(fileBatch.First(), fileName);
                    continue;
                }
                using var outputStream = File.OpenWrite(GetFullPath(fileName));
                await MergeFilesBatch(fileBatch, outputStream, totalFilesToMerge, cancellationToken);
                OverwriteFile(fileName, fileName);
            }
            sortedFiles = [.. Directory.GetFiles(options.FileLocation, $"*{EXTENSION_SORTED}").OrderBy(i =>
            {
                var path = Path.GetFileNameWithoutExtension(i);
                return int.Parse(path[(path.LastIndexOf('.') + 1)..]);
            })];
            if (sortedFiles.Count == 1)
                break;
        }
    }

    /// <summary>
    /// Merge files batch
    /// </summary>
    /// <param name="batchFiles"></param>
    /// <param name="target"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task MergeFilesBatch(IReadOnlyList<string> batchFiles, Stream target, double totalFilesToMerge, CancellationToken cancellationToken)
    {
        var mergeFilesProcessed = 0;
        var (readers, rows) = await MergeFilesGetStreamReaders(batchFiles);
        var completedReaders = new List<int>(readers.Length);
        await using var writer = new StreamWriter(target, bufferSize: options.Merge.OutputBufferSize);
        while (true)
        {
            rows.Sort((x, y) => options.Sort.Comparer.Compare(x.Value, y.Value));
            var readerIndex = rows[0].Index;
            await writer.WriteLineAsync(rows[0].Value.AsMemory(), cancellationToken);
            if (readers[readerIndex].EndOfStream)
            {
                rows.RemoveAt(rows.FindIndex(x => x.Index == readerIndex));
                completedReaders.Add(readerIndex);
                if (completedReaders.Count == readers.Length)
                    break;
                options.Merge.ProgressHandler?.Report(++mergeFilesProcessed / totalFilesToMerge);
                continue;
            }
            var value = await readers[readerIndex].ReadLineAsync(cancellationToken);
            rows[0] = new() { Value = value!, Index = readerIndex };
        }
        MergeFilesCleanupStreamReaders(readers, batchFiles);
    }

    /// <summary>
    /// Return merge stream readers (including the first line of each file)
    /// </summary>
    /// <param name="sortedFiles"></param>
    /// <returns></returns>
    private async Task<(StreamReader[] readers, List<SortSegment> segments)> MergeFilesGetStreamReaders(IReadOnlyList<string> sortedFiles)
    {
        var readers = new StreamReader[sortedFiles.Count];
        var segments = new List<SortSegment>(sortedFiles.Count);
        for (var i = 0; i < sortedFiles.Count; i++)
        {
            readers[i] = new StreamReader(File.OpenRead(GetFullPath(sortedFiles[i])), bufferSize: options.Merge.InputBufferSize);
            var value = await readers[i].ReadLineAsync();
            segments.Add(new() { Value = value!, Index = i });
        }
        return (readers, segments);
    }

    /// <summary>
    /// Cleanup merge stream readers
    /// </summary>
    /// <param name="streamReaders"></param>
    /// <param name="filesToMerge"></param>
    private void MergeFilesCleanupStreamReaders(StreamReader[] streamReaders, IReadOnlyList<string> filesToMerge)
    {
        for (var i = 0; i < streamReaders.Length; i++)
        {
            streamReaders[i].Dispose();
            var fileName = GetFullPath($"{filesToMerge[i]}.remove");
            // Move before delete to speed up large file removal
            File.Move(GetFullPath(filesToMerge[i]), fileName);
            File.Delete(fileName);
        }
    }

    /// <summary>
    /// Overwrite file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private void OverwriteFile(string source, string target) => File.Move(GetFullPath(source), GetFullPath(target.Replace(EXTENSION_TEMP, string.Empty)), true);

    /// <summary>
    /// Return full path
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private string GetFullPath(string fileName) => Path.Combine(options.FileLocation, Path.GetFileName(fileName));
    #endregion

}