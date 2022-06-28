using System.Collections.Concurrent;

namespace Altium.DataProcessing
{
    public class FileSorterService : IFileSorterService
    {
        private readonly ParallelOptions _parallelOptions = new() // depends on machine
        {
            MaxDegreeOfParallelism = 4
        };

        public async Task<string[]> SortFilesAsync(string[] sourceFiles, CancellationToken token, int maxLineCapacity)
        {
            var sortedFiles = new ConcurrentBag<string>();
            await Parallel.ForEachAsync(sourceFiles, _parallelOptions, async (unsortedFile, token) =>
            {
                var sortedFilename = unsortedFile.Replace(FileSortOptions.SplitFileEnd, FileSortOptions.SortedFileEnd);
                var unsortedFilePath = $"{FileSortOptions.CurrentDirectory}/{unsortedFile}";
                var sortedFilePath = $"{FileSortOptions.CurrentDirectory}/{sortedFilename}";
                await SortFileAsync(File.OpenRead(unsortedFilePath), File.Create(sortedFilePath), maxLineCapacity);
                File.Delete(unsortedFilePath);
                sortedFiles.Add(sortedFilename);
            });

            return sortedFiles.ToArray();
        }

        private async Task SortFileAsync(Stream source, Stream target, int maxLineCapacity)
        {
            using var reader = new StreamReader(source);
            var counter = 0;
            var sourceRows = new List<string>();
            while (!reader.EndOfStream)
            {
                var value = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    sourceRows.Add(value);
                }
            }

            var memoryRows = sourceRows.ToArray();
            
            Array.Sort(memoryRows, LineComparer.Comparer);
            await using var writer = new StreamWriter(target);
            foreach (var row in memoryRows)
            {
                await writer.WriteLineAsync(row);
            }
        }
    }
}