# PdfToLetterFile

This is a C# console application which makes use of iTextSharp to read PDF files from a source directory. The file will be read to extract fields and insert into a text file. 

Field to process: 

- Account Number
- Company Id
- Company Name
- Date
- Dollar Ammount (one with info for each amount)

The program will process multiple pdfs and add a line to the text file for each. 

Note: We are using [iTextSharp](https://www.nuget.org/packages/iTextSharp) to read the PDFs.