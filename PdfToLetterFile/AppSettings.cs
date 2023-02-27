namespace PdfToLetterFile
{
    public interface IAppSettings
    {
        public string SourceDirectory { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public string SourceDirectory { get; set; }
    }
}
