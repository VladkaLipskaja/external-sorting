namespace Altium.DataProcessing
{
    public class FileMergerService : IFileMergerService
    {
        public async Task MergeFilesAsync(string[] sortedFiles, Stream target,
            CancellationToken cancellationToken)
        {
            var chunkCounter = 0;
            while (sortedFiles.Length > 1)
            {
                var finalRun = sortedFiles.Length <= FileSortOptions.MergeChunkAmount;

                if (finalRun)
                {
                    await MergeAsync(sortedFiles, target, cancellationToken);
                    return;
                }

                var runs = sortedFiles.Chunk(FileSortOptions.MergeChunkAmount);
                var mergedFileNames = new List<string>();

                foreach (var files in runs)
                {
                    var outputFilename = $"{chunkCounter++}{FileSortOptions.MergedFileEnd}";
                    mergedFileNames.Add(outputFilename);
                    if (files.Length == 1)
                    {
                        File.Move($"{FileSortOptions.CurrentDirectory}/{files[0]}",
                            $"{FileSortOptions.CurrentDirectory}/{outputFilename}");
                        continue;
                    }

                    var outputStream = File.Create($"{FileSortOptions.CurrentDirectory}/{outputFilename}");
                    await MergeAsync(files, outputStream, cancellationToken);
                }

                sortedFiles = mergedFileNames.ToArray();
            }
        }

        private async Task MergeAsync(string[] sourceFiles, Stream outputStream, CancellationToken cancellationToken)
        {
            var (streamReaders, rows) = InitStreams(sourceFiles);
            var finishedStreamReaders = new List<int>(streamReaders.Length);
            var done = false;
            await using var outputWriter = new StreamWriter(outputStream);

            while (!done)
            {
                rows.Sort((row1, row2) =>
                    LineComparer.Comparer.Compare(row1.LineValue, row2.LineValue));

                var valueToWrite = rows[0].LineValue;
                var streamReaderIndex = rows[0].StreamIndex;
                await outputWriter.WriteLineAsync(valueToWrite.AsMemory(), cancellationToken);

                if (streamReaders[streamReaderIndex].EndOfStream)
                {
                    var indexToRemove = rows.FindIndex(x => x.StreamIndex == streamReaderIndex);
                    rows.RemoveAt(indexToRemove);
                    finishedStreamReaders.Add(streamReaderIndex);
                    done = finishedStreamReaders.Count == streamReaders.Length;
                    continue;
                }

                var value = await streamReaders[streamReaderIndex].ReadLineAsync();
                rows[0] = new MergedStreamValueModel {LineValue = value, StreamIndex = streamReaderIndex};
            }

            RemoveTempFiles(streamReaders, sourceFiles);
        }

        private (StreamReader[] StreamReaders, List<MergedStreamValueModel> rows) InitStreams(string[] sortedFiles)
        {
            var streamReaders = new StreamReader[sortedFiles.Length];
            var rows = new List<MergedStreamValueModel>(sortedFiles.Length);

            for (var i = 0; i < sortedFiles.Length; i++)
            {
                var sortedFilePath = $"{FileSortOptions.CurrentDirectory}/{sortedFiles[i]}";
                var sortedFileStream = File.OpenRead(sortedFilePath);
                streamReaders[i] = new StreamReader(sortedFileStream);
                var value = streamReaders[i].ReadLine();
                var row = new MergedStreamValueModel
                {
                    LineValue = value,
                    StreamIndex = i
                };
                rows.Add(row);
            }

            return (streamReaders, rows);
        }

        private void RemoveTempFiles(StreamReader[] streamReaders, string[] filesToMerge)
        {
            for (var i = 0; i < streamReaders.Length; i++)
            {
                streamReaders[i].Dispose();
                var usedFileNamePath =
                    $"{FileSortOptions.CurrentDirectory}/{filesToMerge[i]}{FileSortOptions.UsedFileEnd}";
                File.Move($"{FileSortOptions.CurrentDirectory}/{filesToMerge[i]}", usedFileNamePath);
                File.Delete(usedFileNamePath);
            }
        }
    }
}