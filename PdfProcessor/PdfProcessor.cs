using System.IO;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PdfProcessor
{
    public interface IPdfProcessor
    {
        ReadPdfResponse ReadPdf(string fileName);
    }

    public class PdfProcessor : IPdfProcessor
    {
        public ReadPdfResponse ReadPdf(string fileName)
        {
            // short-circuit
            if (!File.Exists(fileName))
            {
                return new ReadPdfResponse
                {
                    NotFound = true
                };
            }

            using var pdfReader = new PdfReader(fileName);

            var parsedPdf = new ParsedPdf();

            var activeKeys = pdfReader.AcroFields.Fields.Keys.Where(z => keysToParse.Contains(z)).ToList();

            // short-circuit
            if (!activeKeys.Any())
            {
                return new ReadPdfResponse
                {
                    WasSkipped = true
                };
            }

            ParsedPdfDetail[] detailArray = GetNewParsedPdfDetailArray(4);

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

        private void FillPropertyWithValue(ref ParsedPdf parsedPdf, ref ParsedPdfDetail[] detailArray, string key, string value)
        {
            //do nothing if there is no value
            if (string.IsNullOrWhiteSpace(value)) return;

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

        private ParsedPdfDetail[] GetNewParsedPdfDetailArray(int arrayLength)
        {
            var array = new ParsedPdfDetail[4];

            //instantiate the array's objects
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new ParsedPdfDetail();
            }

            return array;
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
