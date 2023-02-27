using System.Collections.Generic;

namespace PdfProcessor
{
    public class ParsedPdf
    {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string AccountNumber { get; set;}
        public List<ParsedPdfDetail> Details { get; set; }
    }
}
