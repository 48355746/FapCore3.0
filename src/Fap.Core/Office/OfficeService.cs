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
        private readonly IFapPlatformDomain _platformDomain;
        private readonly IDbContext _dbContext;
        private readonly ILogger<OfficeService> _logger;
        public OfficeService(IFapPlatformDomain platformDomain, IDbContext dbContext,ILogger<OfficeService> logger)
        {
            _platformDomain = platformDomain;
            _dbContext = dbContext;
            _logger = logger;
        }
        public bool ExportExcel(ExportModel exportModel)
        {
            try
            {
                ExcelEntityDataExport excelExport = new ExcelEntityDataExport(_dbContext, _platformDomain, exportModel);
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
                ExcelEntityDataImport excelImport = new ExcelEntityDataImport(_dbContext, _platformDomain, fileName, tableName, importMode);

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
