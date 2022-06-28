using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Altium.DataGeneration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetService<IGenerationService>()!.GenerateDataAsync().Wait();
            
            //BenchmarkRunner.Run<GenerationService>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IGenerationService, GenerationService>();
                });

            return hostBuilder;
        }
    }
}