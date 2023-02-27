namespace PdfProcessor
{
    public class ReadPdfResponse
    {
        public ParsedPdf ParsedPdf { get; set; }
        public bool WasSkipped { get; set; }
        public bool NotFound { get; set; }
    }
}
