using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PdfToLetterFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = Configure();
            var serviceProvider = services.BuildServiceProvider();
            var processor = serviceProvider.GetRequiredService<IProcessor>();

            try
            {
                processor.ProcessPdfsInDirectory();
            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal Error: {0}", e.Message);
            }
        }

        private static IServiceCollection Configure()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.TryAddSingleton<IProcessor, Processor>();

            var appSettings = new AppSettings();
            configuration.Bind("AppSettings", appSettings);
            services.AddProcessor(appSettings);            

            return services;
        }
    }
}