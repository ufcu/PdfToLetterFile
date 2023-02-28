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
                var fileName = fileInfo.Name;
                Console.WriteLine($"Processing file {i + 1} of {entireFileInfoList.Count}: {fileName}");

                var response = _pdfProcessor.ReadPdf(fileInfo.FullName);

                if (response.NotFound)
                {
                    Console.WriteLine($"File Not Found: {fileName}");
                }

                if (response.WasSkipped)
                {
                    Console.WriteLine($"File Skipped (Invalid): {fileName}");
                    MoveFile(fileInfo, _appsettings.InvalidDirectory);
                }

                if (response.ParsedPdf != null)
                {
                    Console.WriteLine($"File Parsed: {fileName}");
                    await SaveToText(response.ParsedPdf);
                    MoveFile(fileInfo, _appsettings.ProcessedDirectory);
                }
            }
        }

        private async Task SaveToText(ParsedPdf parsedPdf)
        {
            CreateDirectory(_appsettings.OutputDirectory);

            var outputFilePath = Path.Combine(_appsettings.OutputDirectory, _appsettings.OutputLetterFileName);

            using TextWriter tw = new StreamWriter(outputFilePath);

            foreach (var detail in parsedPdf.Details)
            {
                await tw.WriteLineAsync(
                    detail.GetLetterFileLineItem(
                        parsedPdf.FormattedAccountNumber,
                        parsedPdf.CompanyId,
                        parsedPdf.CompanyName));
            }
        }

        private void MoveFile(FileInfo fileInfo, string destinationDirectory)
        {
            CreateDirectory(destinationDirectory);

            var sourceFileName = fileInfo.FullName;
            var destFileName = Path.Combine(destinationDirectory, fileInfo.Name);

            //Copy File
            Console.WriteLine($"Copying file {sourceFileName} to {destinationDirectory}...");
            File.Copy(sourceFileName, destFileName);

            //Delete File
            Console.WriteLine($"Deleting file {sourceFileName} from {_appsettings.SourceDirectory}...");
            File.Delete(sourceFileName);
        }

        private void CreateDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Console.WriteLine($"Creating Directory {directoryName}...");
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}
