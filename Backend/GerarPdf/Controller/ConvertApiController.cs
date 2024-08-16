using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Layout;
using PdfSharp.Pdf;
using PdfSharp.Snippets;
using PdfSharp.Pdf.IO;
using Microsoft.AspNetCore.Authorization;

namespace GerarPdf.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertApiController : ControllerBase
    {
        private readonly ILogger<ConvertApiController> _logger;

        public ConvertApiController(ILogger<ConvertApiController> logger){
            _logger = logger;
        }   

        [HttpPost("/CovertToCsv")]
        public IActionResult ConverterPdfToCsv(IFormFile? file){
            if (file == null || file.Length == 0)
    {
        _logger.LogError("File parameter is null or empty");
        return BadRequest("File is null or empty");
    }

    try
    {
        using var reader = new iText.Kernel.Pdf.PdfReader(file.OpenReadStream());
        using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader);
        var sb = new StringBuilder();

        for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
        {
            var strategy = new SimpleTextExtractionStrategy();
            string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
            sb.AppendLine(pageText);
        }

        var csvContent = sb.ToString().Replace("\n", ",").Replace("\r", "");
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var csvFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".csv";

        _logger.LogInformation("Arquivo CSV criado com sucesso: {csvFileName}", csvFileName);
        return File(csvStream.ToArray(), "text/csv", csvFileName);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao converter PDF para CSV.");
        return StatusCode(500, $"An error occurred while converting the file: {ex.Message}");
    }
        }


    [HttpPost("/CovertToDocs")]
    public IActionResult ConvertPdfToWord(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogError("File parameter is null or empty");
            return BadRequest("File is null or empty");
        }

        try
        {
            using var reader = new iText.Kernel.Pdf.PdfReader(file.OpenReadStream());
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader);

            // Criar MemoryStream fora do bloco using para evitar problemas de fechamento
            var ms = new MemoryStream();
            
            using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new Body());

                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);

                    var paragraph = body.AppendChild(new Paragraph());
                    paragraph.AppendChild(new Run(new Text(pageText)));
                }
            }

            // Certificar-se de que o MemoryStream está posicionado no início antes de retornar
            ms.Seek(0, SeekOrigin.Begin);

            var wordFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".docx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while converting the file: {ex.Message}");
            return StatusCode(500, $"An error occurred while converting the file: {ex.Message}");
        }
    }


        
        [HttpPost("/JuntarPDF")]
        public IActionResult JuntarPDF(IFormFile? file1, IFormFile? file2){
            if (file1 == null || file2 == null)
            {
                _logger.LogError("One or both file parameters are null");
                return BadRequest("One or both files are null");
            }

            try
            {
                IFormFile[] files = {file1, file2};
                PdfSharp.Pdf.PdfDocument outputDocument =  new PdfSharp.Pdf.PdfDocument();
                 foreach (var file in files)
                {
                    using (var inputStream = new MemoryStream())
                    {
                        file.CopyTo(inputStream);
                        inputStream.Position = 0;
                        
                        PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);

                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                }

                // Salve o documento de saída em um MemoryStream
                using (var outputStream = new MemoryStream())
                {
                    outputDocument.Save(outputStream);
                    outputStream.Position = 0;

                    // Retorne o arquivo PDF resultante
                    return File(outputStream.ToArray(), "application/pdf", "MergedDocument.pdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while merging the PDFs: {ex.Message}");
            }
        }
    }
}