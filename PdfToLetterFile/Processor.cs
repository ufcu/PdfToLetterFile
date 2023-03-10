using iTextSharp.text.log;
using PdfProcessor;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using ILogger = Serilog.ILogger;

namespace PdfToLetterFile
{
    public interface IProcessor
    {
        Task ProcessPdfsInDirectory();
    }

    public class Processor : IProcessor
    {
        private readonly ILogger _logger = Log.ForContext<Processor>();

        private readonly IAppSettings _appsettings;
        private readonly IPdfProcessor _pdfProcessor;
        private readonly EmailSettings _emailSettings;

        public Processor(IAppSettings appSettings, IPdfProcessor pdfProcessor, EmailSettings emailSettings)
        {
            _appsettings = appSettings;
            _pdfProcessor = pdfProcessor;
            _emailSettings = emailSettings;
        }

        public async Task ProcessPdfsInDirectory()
        {
            using (LogContext.PushProperty("Method", nameof(ProcessPdfsInDirectory)))
            {
                try
                {
                    int numProcessed = 0;
                    int numInvalid = 0;
                    int numNotFound = 0;

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    //replace w/serilog maybe
                    var appStartTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();
                    _logger.Information($"ProcessPdfsInDirectory() starting at {appStartTimeInCentralTimeZone}");

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

                                _logger.Information($"Processing file {i + 1} of {entireFileInfoList.Count}: {fileName}");

                                var response = _pdfProcessor.ReadPdf(fileInfo.FullName);

                                if (response.NotFound)
                                {
                                    _logger.Information($"File Not Found: {fileName}");
                                    numNotFound++;
                                }

                                if (response.WasSkipped)
                                {
                                    _logger.Information($"File Skipped (Invalid): {fileName}");
                                    MoveFile(fileInfo, _appsettings.InvalidDirectory);
                                    numInvalid++;
                                }

                                if (response.ParsedPdf != null)
                                {
                                    _logger.Information($"File Parsed: {fileName}");
                                    await SaveToText(response.ParsedPdf);
                                    MoveFile(fileInfo, _appsettings.ProcessedDirectory);
                                    numProcessed++;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Information(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        _logger.Information($"There are not currently any .pdf files to process in the source directory: {_appsettings.SourceDirectory}.");
                    }

                    var completionTimeInCentralTimeZone = DateTime.UtcNow.ConvertToCentralTimeFromUtc();

                    _logger.Information("The Processor has completed its work!");
                    _logger.Information("App StartTime: {0} {1}", appStartTimeInCentralTimeZone.ToShortDateString(), appStartTimeInCentralTimeZone.ToShortTimeString());
                    _logger.Information("App EndTime: {0} {1}", completionTimeInCentralTimeZone.ToShortDateString(), completionTimeInCentralTimeZone.ToShortTimeString());
                    _logger.Information("App Duration: {0}", stopwatch.Elapsed.ToTimerString(true));
                    _logger.Information("Total PDFs: {0}", entireFileInfoList.Count);
                    _logger.Information("Processed PDFs: {0}", numProcessed);
                    _logger.Information("Invalid PDFs: {0}", numInvalid);
                    _logger.Information("NotFound PDFs: {0}", numNotFound);
                }
                catch (Exception e)
                {
                    SendEmailOnFatalError(e);
                }
            }
        }

        private void SendEmailOnFatalError(Exception e)
        {
            using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.EnableSsl = _emailSettings.EnableSsl;

                using (var mailMessage = ComposeMailMessage(e.ToString()))
                {
                    try
                    {
                        _logger.Debug("Attempting to send email");
                        client.Send(mailMessage);
                    }
                    catch { }
                }
            }
        }

        private MailMessage ComposeMailMessage(string exception)
        {
            var message = new MailMessage
            {
                From = new MailAddress("noreply@ufcu.org")
            };
            message.ReplyToList.Add(message.From);

            foreach (var toAddress in ToAddresses)
            {
                message.To.Add(new MailAddress(toAddress));
            }

            message.Subject = "PdfToLetterFile Unhandled Exception";
            message.IsBodyHtml = false;

            var body = new StringBuilder();
            body.AppendLine("The PDFToLetterFile console app encountered an exception!!");
            body.AppendLine("");
            body.AppendLine(exception);
            message.Body = body.ToString();

            string smtpApi = string.Format(@"{{""to"": [""{0}""]", String.Join("\",\"", ToAddresses.ToArray()));
            message.Headers.Add("X-SMTPAPI", smtpApi);

            return message;
        }

        private List<string> ToAddresses
        {
            get
            {
                return new List<string>()
                {
                    "_ITLAFHTeam@ufcu.org",
                };
            }
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
            using (LogContext.PushProperty("Method", nameof(MoveFile)))
            {
                CreateDirectory(destinationDirectory);

                var sourceFileName = fileInfo.FullName;
                var destFileName = Path.Combine(destinationDirectory, fileInfo.Name);

                _logger.Information($"Moving file {sourceFileName} to {destinationDirectory}...");
                File.Move(sourceFileName, destFileName);
            }
        }

        private void CreateDirectory(string directoryName)
        {
            using (LogContext.PushProperty("Method", nameof(CreateDirectory)))
            {
                if (!Directory.Exists(directoryName))
                {
                    _logger.Information($"Creating Directory {directoryName}...");
                    Directory.CreateDirectory(directoryName);
                }
            }
        }
    }
}
