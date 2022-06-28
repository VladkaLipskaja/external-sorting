using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Altium.DataProcessing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //host.Services.GetService<IDataService>()!.GetFileAsync(FileSortOptions.CurrentDirectory).Wait();
            
            BenchmarkRunner.Run<DataService>();
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