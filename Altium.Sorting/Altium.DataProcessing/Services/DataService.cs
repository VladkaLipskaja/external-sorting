using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Altium.DataProcessing
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, targetCount: 0)]
    public class DataService : IDataService
    {
        private readonly IFileSplitterService _fileSplitterService;
        private readonly IFileSorterService _fileSorterService;
        private readonly IFileMergerService _fileMergerService;

        public DataService(
            // Just because of benchmark
            //IFileSplitterService fileSplitterService, IFileSorterService fileSorterService,
            //IFileMergerService fileMergerService
            
            )
        {
            // _fileSplitterService = fileSplitterService;
            // _fileSorterService = fileSorterService;
            // _fileMergerService = fileMergerService;

            _fileSplitterService = new FileSplitterService();
            _fileSorterService = new FileSorterService();
            _fileMergerService = new FileMergerService();
        }

        [Benchmark]
        public async Task GetFileAsync()
        {
            var path = FileSortOptions.CurrentDirectory; // only for benchmarking, should be done via method parameters
            
            if (FileExists(path))
            {
                await Sort(File.OpenRead($"{path}/input.txt"), File.Create($"{path}/output.txt"),
                    CancellationToken.None);
            }
        }

        public async Task Sort(Stream source, Stream target, CancellationToken cancellationToken)
        {
            var (files, maxLineCapacity) =
                await _fileSplitterService.GetFileChunksAsync(source, cancellationToken, FileSortOptions.TempFileSize);
            var sortedFiles = await _fileSorterService.SortFilesAsync(files, cancellationToken, maxLineCapacity);
            await _fileMergerService.MergeFilesAsync(sortedFiles, target, cancellationToken);
        }

        private bool FileExists(string directory)
        {
            var di = new DirectoryInfo(directory);
            var files = di.GetFiles("input.txt");

            if (files.Length == 0)
            {
                Console.WriteLine($"Please, check that a file exists in {Directory.GetCurrentDirectory()}");
                return false;
            }

            return true;
        }
    }
}