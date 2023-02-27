using System.Text;
using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading.Tasks;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        Task<string> ReadPdfToText(string fileName);
    }

    public class PdfProcessor : IPdfProcessor
    {
        public async Task<string> ReadPdfToText(string fileName)
        {
            var text = new StringBuilder();

            if (File.Exists(fileName))
            {
                var bytes = await File.ReadAllBytesAsync(fileName);

                using var pdfReader = new PdfReader(bytes);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    currentText = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
            }
            return text.ToString();
        }
    }
}
