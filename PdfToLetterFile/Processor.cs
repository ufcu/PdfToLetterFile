
using Microsoft.Extensions.Options;
using PdfProcessor;

namespace PdfToLetterFile
{
    public interface IProcessor
    {
        void ProcessPdfsInDirectory();
    }

    public class Processor : IProcessor
    {
        private readonly IAppSettings _appsettings;
        private readonly IPdfProcessor _pdfProcessor;

        public Processor(IAppSettings appSettings, IPdfProcessor pdfProcessor)
        {
            _appsettings = appSettings;
            _pdfProcessor = pdfProcessor;
        }

        public void ProcessPdfsInDirectory()
        {
            var appStartTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();
            Console.WriteLine($"ProcessPdfsInDirectory() has begun at {appStartTimeInCentralTimeZone}");

            var dirInfo = new DirectoryInfo(_appsettings.SourceDirectory);
            var entireFileInfoList = dirInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly).ToList();

            for (int i = 0; i < entireFileInfoList.Count; i++)
            {
                FileInfo? fileInfo = entireFileInfoList[i];
                Console.WriteLine($"Processing file {i} of {entireFileInfoList.Count}: {fileInfo.Name}");
                var pdfText = _pdfProcessor.ReadPdfToText(fileInfo.FullName);

                if (string.IsNullOrEmpty(pdfText))
                {
                    Console.WriteLine("There is nothing to do here.");
                    return;
                }
            }
        }
    }
}
