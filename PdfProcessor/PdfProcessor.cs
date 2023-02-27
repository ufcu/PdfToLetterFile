using System.IO;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        string ReadPdf(FileInfo fileInfo);
    }

    public class PdfProcessor : IPdfProcessor
    {
        private static readonly List<string> keysToParse = new List<string>
        {
            "Account Number",
            "Party Debiting the AccountOriginator of Debit",
            "Company ID",
            "Date",
            "fill_11",
            "Date_2",
            "fill_16",
            "Date_3",
            "fill_14",
            "Date_4",
            "fill_17"
        };

        public string ReadPdf(FileInfo fileInfo)
        {
            var fullName = fileInfo.FullName;

            if (File.Exists(fullName))
            {
                using var pdfReader = new PdfReader(fullName);

                var parsedPdf = new ParsedPdf();

                var activeKeys = pdfReader.AcroFields.Fields.Keys.Where(z => keysToParse.Contains(z)).ToList();

                if (activeKeys.Any())
                {
                    activeKeys.ForEach(key =>
                    {
                        var value = pdfReader.AcroFields.GetField(key);
                        FillPropertyWithValue(ref parsedPdf, key, value);
                    });
                }
                else
                {
                    //todo: add skip logic
                    Console.WriteLine($"File {fileInfo.Name} has been skipped. It has no keys from which we're trying to parse.");
                }
            }
            return string.Empty;
        }

        private void FillPropertyWithValue(ref ParsedPdf parsedPdf, string key, string value)
        {
            Console.WriteLine($"{key}:{value}");

            switch (key)
            {
                case "Account Number":
                    break;
                case "Party Debiting the AccountOriginator of Debit":
                    break;
                case "Company ID":
                    break;
                case "Date":
                    break;
                case "fill_11":
                    break;
                case "Date_2":
                    break;
                case "fill_16":
                    break;
                case "Date_3":
                    break;
                case "fill_14":
                    break;
                case "Date_4":
                    break;
                case "fill_17":
                    break;
                default:
                    throw new Exception($"Missing key: {key}");
            }
        }
    }
}
