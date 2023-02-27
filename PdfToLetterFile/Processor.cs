using PdfProcessor;

namespace PdfToLetterFile
{
    public interface IProcessor
    {
        Task ProcessPdfsInDirectory();
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
                FileInfo fileInfo = entireFileInfoList[i];
                Console.WriteLine($"Processing file {i} of {entireFileInfoList.Count}: {fileInfo.Name}");

                var response = _pdfProcessor.ReadPdf(fileInfo);

                if (response.NotFound)
                {
                    //todo: handle not found list (there shouldn't be any)
                }

                if (response.WasSkipped)
                {
                    //todo: handle was skipped for not having keys
                }

                if (response.ParsedPdf != null)
                {
                    Console.WriteLine("There is nothing to do here.");

                    await WriteTextToFile(response.ParsedPdf);
                    return;
                }

                //todo: move the file now that it has been processed
            }
        }

        private async Task WriteTextToFile(ParsedPdf parsedPdf)
        {
            var outputFile = "C:\\tmp\\uploads\\test.txt";
            //string[] lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //var text = new StreamBuilder();

           // await File.WriteAllTextAsync(outputFile, lines);
        }
    }
}
