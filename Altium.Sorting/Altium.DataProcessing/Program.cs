using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Altium.DataProcessing
{
    public interface IDataService
    {
        public Task GetFileAsync(string path);
        public Task Sort(Stream source, Stream target, CancellationToken cancellationToken);
    }

    public class DataService : IDataService
    {
        private readonly IFileSplitterService _fileSplitterService;
        private readonly IFileSorterService _fileSorterService;
        private readonly IFileMergerService _fileMergerService;

        public DataService(IFileSplitterService fileSplitterService, IFileSorterService fileSorterService,
            IFileMergerService fileMergerService)
        {
            _fileSplitterService = fileSplitterService;
            _fileSorterService = fileSorterService;
            _fileMergerService = fileMergerService;
        }
        
        public async Task GetFileAsync(string path)
        {
            if (FileExists(path))
            {
                await Sort(File.OpenRead($"{path}/input.txt"), File.Create($"{path}/output.txt"), CancellationToken.None);
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

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetService<IDataService>()!.GetFileAsync(FileSortOptions.CurrentDirectory).Wait();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDataService, DataService>();
                    services.AddSingleton<IFileSorterService, FileSorterService>();
                    services.AddSingleton<IFileSplitterService, FileSplitterService>();
                    services.AddSingleton<IFileMergerService, FileMergerService>();
                });

            return hostBuilder;
        }
    }
}