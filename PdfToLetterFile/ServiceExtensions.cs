using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PdfToLetterFile
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProcessor(
            this IServiceCollection services,
            IAppSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            if (string.IsNullOrWhiteSpace(appSettings.SourceDirectory))
            {
                throw new ArgumentException("AppSettings: SourceDirectory is null or empty");
            }

            services.TryAddSingleton(appSettings);
            services.TryAddSingleton<PdfProcessor.IPdfProcessor, PdfProcessor.PdfProcessor>();

            return services;
        }
    }
}
