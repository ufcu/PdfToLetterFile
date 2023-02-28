using System.Collections.Generic;

namespace PdfProcessor
{
    public class ParsedPdfDetail
    {
        public string Date { get; set; }
        public string DollarAmount { get; set; }

        public bool HasValues => !string.IsNullOrWhiteSpace(Date) && !string.IsNullOrWhiteSpace(DollarAmount);

        public string GetLetterFileLineItem(string accountNumber, string companyId, string companyName)
        {
            var items = new List<string>
            {
                accountNumber,
                Date,
                companyId,
                DollarAmount,
                companyName
            };

            return string.Join(" ", items);
        }
    }
}
