using iTextSharp.text.log;
using PdfProcessor;
using System.Diagnostics;

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
            int numProcessed = 0;
            int numInvalid = 0;
            int numNotFound = 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //replace w/serilog maybe
            var appStartTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();
            Console.WriteLine($"ProcessPdfsInDirectory() starting at {appStartTimeInCentralTimeZone}");
            Console.WriteLine();

            var dirInfo = new DirectoryInfo(_appsettings.SourceDirectory);
            var entireFileInfoList = dirInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly).ToList();

            if (entireFileInfoList.Any())
            {
                for (int i = 0; i < entireFileInfoList.Count; i++)
                {
                    try
                    {
                        FileInfo fileInfo = entireFileInfoList[i];
                        var fileName = fileInfo.Name;

                        Console.WriteLine($"Processing file {i + 1} of {entireFileInfoList.Count}: {fileName}");

                        var response = _pdfProcessor.ReadPdf(fileInfo.FullName);

                        if (response.NotFound)
                        {
                            Console.WriteLine($"File Not Found: {fileName}");
                            numNotFound++;
                        }

                        if (response.WasSkipped)
                        {
                            Console.WriteLine($"File Skipped (Invalid): {fileName}");
                            MoveFile(fileInfo, _appsettings.InvalidDirectory);
                            numInvalid++;
                        }

                        if (response.ParsedPdf != null)
                        {
                            Console.WriteLine($"File Parsed: {fileName}");
                            await SaveToText(response.ParsedPdf);
                            MoveFile(fileInfo, _appsettings.ProcessedDirectory);
                            numProcessed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                Console.WriteLine($"There are not currently any .pdf files to process in the source directory: {_appsettings.SourceDirectory}.");
            }

            var completionTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();

            Console.WriteLine();
            Console.WriteLine("The Processor has completed its work!"); 
            Console.WriteLine("App StartTime: {0} {1}", appStartTimeInCentralTimeZone.ToShortDateString(), appStartTimeInCentralTimeZone.ToShortTimeString());
            Console.WriteLine("App EndTime: {0} {1}", completionTimeInCentralTimeZone.ToShortDateString(), completionTimeInCentralTimeZone.ToShortTimeString());
            Console.WriteLine("App Duration: {0}", stopwatch.Elapsed.ToTimerString(true));
            Console.WriteLine();
            Console.WriteLine("Total PDFs: {0}", entireFileInfoList.Count);
            Console.WriteLine("Processed PDFs: {0}", numProcessed);
            Console.WriteLine("Invalid PDFs: {0}", numInvalid);
            Console.WriteLine("NotFound PDFs: {0}", numNotFound);
        }

        private async Task SaveToText(ParsedPdf parsedPdf)
        {
            CreateDirectory(_appsettings.OutputDirectory);

            var outputFilePath = Path.Combine(_appsettings.OutputDirectory, _appsettings.OutputLetterFileName);

            using StreamWriter sw = File.AppendText(outputFilePath);

            foreach (var detail in parsedPdf.Details)
            {
                await sw.WriteLineAsync(
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

            Console.WriteLine($"Moving file {sourceFileName} to {destinationDirectory}...");
            File.Move(sourceFileName, destFileName);
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
