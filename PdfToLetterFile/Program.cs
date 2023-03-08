using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace PdfToLetterFile
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = Configure();
            var serviceProvider = services.BuildServiceProvider();
            var processor = serviceProvider.GetRequiredService<IProcessor>();
            await processor.ProcessPdfsInDirectory();
        }

        private static IServiceCollection Configure()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Destructure.UsingAttributes()
                .CreateLogger();
            Log.Logger = logger;

            IServiceCollection services = new ServiceCollection();
            services.TryAddSingleton<IProcessor, Processor>();

            var appSettings = new AppSettings();
            configuration.Bind(nameof(AppSettings), appSettings);
            services.AddProcessor(appSettings);

            var emailSettings = configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>();
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.AddSingleton(emailSettings);

            return services;
        }
    }
}