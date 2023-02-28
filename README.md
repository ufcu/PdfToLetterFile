# PdfToLetterFile

This is a C# console application which makes use of iTextSharp to read PDF files from a source directory. The file will be read to extract fields and insert into a text file. 

### Business Requirements

1) Read muitiple PDFs from a directory
2) Read in values for the following fields 

- Account Number
- *Date
- Company Id
- *Dollar Amount
- Company Name

*one line item  for each date & dollar amount w/the rest of the information duplicated on that line.

3) Using the data gathered by the parsedPDF, the program will write this info line-by-line to a text file in the order and manner explained above.

- All .pdf files encountered when the app runs will be moved from the original source location. A file will either end up in the 'processed' directory or the 'invalid' directory. A file is deemed invalid if 
  the application logic cannot find any key/value pairs in the PDF from which it can read.  
- All of the directories are configurable 

Feedback: Invalid File Path directory?  List of "invalid" files?

- The application will also log the files that were invalid during processing.

### Dev Notes

- We are using [iTextSharp](https://www.nuget.org/packages/iTextSharp) to read the PDFs.

- Go to \\fileshare00\Reports\ACH to get example files, but just copy them. Don't remove or edit them here. 

