using System.Text;
using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        string ReadPdfToText(string fileName);
        void WriteTextToFile(string content);
    }

    public class PdfProcessor : IPdfProcessor
    {
        public string ReadPdfToText(string fileName)
        {
            var text = new StringBuilder();

            if (File.Exists(fileName))
            {
                using var pdfReader = new PdfReader(fileName);
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

        public void WriteTextToFile(string content)
        {
            var fileName = "C:\\tmp\\uploads\\test.txt";
            string[] lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"This pdf has {lines.Length} lines to process");

            File.WriteAllLines(fileName, lines);
        }
    }
}
