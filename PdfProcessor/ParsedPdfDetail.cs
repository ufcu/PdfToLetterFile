namespace PdfProcessor
{
    public class ParsedPdfDetail
    {
        public string Date { get; set; }
        public string DollarAmount { get; set; }

        public bool HasValues => !string.IsNullOrWhiteSpace(Date) && !string.IsNullOrWhiteSpace(DollarAmount);
    }
}
