using System.IO;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        ReadPdfResponse ReadPdf(FileInfo fileInfo);
    }

    public class PdfProcessor : IPdfProcessor
    {
        public ReadPdfResponse ReadPdf(FileInfo fileInfo)
        {
            var fullName = fileInfo.FullName;

            if (File.Exists(fullName))
            {
                using var pdfReader = new PdfReader(fullName);

                var parsedPdf = new ParsedPdf();

                var activeKeys = pdfReader.AcroFields.Fields.Keys.Where(z => keysToParse.Contains(z)).ToList();

                if (activeKeys.Any())
                {
                    var detailArray = new ParsedPdfDetail[4];

                    //instantiate the array's objects
                    for (int i = 0; i < detailArray.Length; i++)
                    {
                        detailArray[i] = new ParsedPdfDetail();
                    }

                    activeKeys.ForEach(key =>
                    {
                        var value = pdfReader.AcroFields.GetField(key);
                        FillPropertyWithValue(ref parsedPdf, ref detailArray, key, value);
                    });

                    //only assign those where values are set
                    parsedPdf.Details = detailArray.Where(z => z.HasValues).ToList();

                    return new ReadPdfResponse
                    {
                        ParsedPdf = parsedPdf
                    };
                }
                else
                {
                    Console.WriteLine($"File {fileInfo.Name} has been skipped. It has no keys from which we're trying to parse.");
                    return new ReadPdfResponse
                    {
                        WasSkipped = true
                    };
                }
            }

            Console.WriteLine($"File {fileInfo.Name} not found!");
            return new ReadPdfResponse
            {
                NotFound = true
            };
        }

        private void FillPropertyWithValue(ref ParsedPdf parsedPdf, ref ParsedPdfDetail[] detailArray, string key, string value)
        {
            //do nothing if there is no value
            if (string.IsNullOrWhiteSpace(value)) return;

            Console.WriteLine($"{key}:{value}");

            switch (key)
            {
                case "Account Number":
                    parsedPdf.AccountNumber = value;
                    break;
                case "Party Debiting the AccountOriginator of Debit":
                    parsedPdf.CompanyName = value;
                    break;
                case "Company ID":
                    parsedPdf.CompanyId = value;
                    break;
                case "Date":
                    detailArray[0].Date = value;
                    break;
                case "fill_11":
                    detailArray[0].DollarAmount = value;
                    break;
                case "Date_2":
                    detailArray[1].Date = value;
                    break;
                case "fill_16":
                    detailArray[1].DollarAmount = value;
                    break;
                case "Date_3":
                    detailArray[2].Date = value;
                    break;
                case "fill_14":
                    detailArray[2].DollarAmount = value;
                    break;
                case "Date_4":
                    detailArray[3].Date = value;
                    break;
                case "fill_17":
                    detailArray[3].DollarAmount = value;
                    break;
                default:
                    throw new Exception($"Missing key: {key}");
            }
        }

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
    }
}
