using System;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading.Tasks;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        string ReadPdfToText(string fileName);
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
                    var fields = pdfReader.AcroFields.Fields;

                    foreach (var key in fields.Keys)
                    {
                        var value = pdfReader.AcroFields.GetField(key);
                        Console.WriteLine(key + " : " + value);
                    }
                }
            }
            return text.ToString();
        }
    }
}
