using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Office.Excel.Export;
using Fap.Core.Office.Excel.Import;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Core.Office
{
    [Service]
    public class OfficeService : IOfficeService
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<OfficeService> _logger;
        public OfficeService(IDbContext dbContext,ILogger<OfficeService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public bool ExportExcel(ExportModel exportModel)
        {
            try
            {
                ExcelEntityDataExport excelExport = new ExcelEntityDataExport(_dbContext, exportModel);
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
                ExcelEntityDataImport excelImport = new ExcelEntityDataImport(_dbContext,  fileName, tableName, importMode);

                excelImport.Import();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;

        }
    }
}
