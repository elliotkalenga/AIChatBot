using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Reflection.PortableExecutable;

namespace ChatBot.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        public string Parse(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();

            return ext switch
            {
                ".pdf" => ParsePdf(filePath),
                ".docx" => ParseWord(filePath),
                ".xlsx" => ParseExcel(filePath),
                _ => throw new NotSupportedException("Unsupported file type")
            };
        }

        // ✅ PDF — iText7 (WORKS)
        private string ParsePdf(string path)
        {
            using var reader = new PdfReader(path);
            using var pdf = new PdfDocument(reader);

            var text = "";
            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                text += PdfTextExtractor.GetTextFromPage(pdf.GetPage(i)) + "\n";
            }

            return text;
        }

        // ✅ WORD — OpenXML
        private string ParseWord(string path)
        {
            using var doc = WordprocessingDocument.Open(path, false);
            return doc.MainDocumentPart?.Document?.Body?.InnerText ?? "";
        }

        // ✅ EXCEL — ClosedXML
        private string ParseExcel(string path)
        {
            using var workbook = new XLWorkbook(path);
            return string.Join("\n",
                workbook.Worksheets.SelectMany(ws =>
                    ws.RowsUsed().Select(row =>
                        string.Join(" ",
                            row.Cells().Select(c => c.Value.ToString()))
                )));
        }
    }
}
