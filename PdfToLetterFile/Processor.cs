using PdfProcessor;

namespace PdfToLetterFile
{
    public interface IProcessor
    {
        Task ProcessPdfsInDirectory();
        Task WriteTextToFile(string content);
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

        public async Task ProcessPdfsInDirectory()
        {
            var appStartTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();
            Console.WriteLine($"ProcessPdfsInDirectory() has begun at {appStartTimeInCentralTimeZone}");

            var dirInfo = new DirectoryInfo(_appsettings.SourceDirectory);
            var entireFileInfoList = dirInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly).ToList();

            for (int i = 0; i < entireFileInfoList.Count; i++)
            {
                FileInfo? fileInfo = entireFileInfoList[i];
                Console.WriteLine($"Processing file {i} of {entireFileInfoList.Count}: {fileInfo.Name}");
                var pdfText = await _pdfProcessor.ReadPdfToText(fileInfo.FullName);

                if (string.IsNullOrEmpty(pdfText))
                {
                    Console.WriteLine("There is nothing to do here.");
                    return;
                }
            }
        }

        public async Task WriteTextToFile(string content)
        {
            var fileName = "C:\\tmp\\uploads\\test.txt";
            string[] lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"This pdf has {lines.Length} lines to process");

            await File.WriteAllLinesAsync(fileName, lines);
        }
    }
}
