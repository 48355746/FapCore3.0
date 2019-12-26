using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Office.Excel.Export;
using Fap.Core.Office.Excel.Import;
using Fap.Core.Office.Word;
using Microsoft.Extensions.Logging;
using NPOI.SS.Converter;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fap.Core.Office
{
    [Service]
    public class OfficeService : IOfficeService
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<OfficeService> _logger;
        public OfficeService(IDbContext dbContext, ILogger<OfficeService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public bool ExportExcel(ExportModel exportModel)
        {
            try
            {
                ExcelExportBase excelExport = new ExcelEntityDataExport(_dbContext, exportModel);
                excelExport.Export();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;
        }
        [Transactional]
        public bool ImportExcel(string fileName, string tableName, ImportMode importMode)
        {
            try
            {
                ExcelImportBase excelImport = new ExcelEntityDataImport(_dbContext, fileName, tableName, importMode);

                excelImport.Import();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;

        }
        private void ExcelToHtml(string fileName)
        {
            
            var workbook= ExcelToHtmlUtils.LoadXls(fileName);
            ExcelToHtmlConverter excelToHtmlConverter = new ExcelToHtmlConverter()
            {
                // Set output parameters
                OutputColumnHeaders = false,
                OutputHiddenColumns = true,
                OutputHiddenRows = true,
                OutputLeadingSpacesAsNonBreaking = false,
                OutputRowNumbers = true,
                UseDivsToSpan = true

            };
            // Process the Excel file
            excelToHtmlConverter.ProcessWorkbook(workbook);

            // Output the HTML file
            excelToHtmlConverter.Document.Save(Path.ChangeExtension(fileName, "html"));
        }

        public void PrintWordTemplate(string templateFile,string outputFile,IDictionary<string,string> keyValues)
        {
            WordTemplate wordTemplate = new WordTemplate();
            wordTemplate.ReplaceTemplate(templateFile, outputFile, keyValues);
        }       
    }
}
