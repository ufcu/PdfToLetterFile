namespace PdfToLetterFile
{
    public interface IAppSettings
    {
        public string SourceDirectory { get; set; }
        public string ProcessedDirectory { get; set; }
        public string InvalidDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string OutputLetterFileName { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public string SourceDirectory { get; set; }
        public string ProcessedDirectory { get; set; }
        public string InvalidDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string OutputLetterFileName { get; set; }
    }
}
