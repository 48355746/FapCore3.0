using ClosedXML.Excel;
using ExcelReportGenerator.Rendering;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Cache;
using Fap.Core.Infrastructure.Domain;
using Fap.ExcelReport.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Fap.ExcelReport
{
    [Service]
    public class ExcelReportService : IExcelReportService
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IFapApplicationContext _applicationContext;
        public ExcelReportService(IDbContext dbContext,IFapApplicationContext applicationContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _applicationContext = applicationContext;
        }
        private XLWorkbook GetReportTemplateWorkbook(string templateName)
        {
            string filePath= Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, templateName);
            return new XLWorkbook(filePath);
        }
        public async Task<string> Render(string rptUid)
        {
            var rptModel= _dbContext.Get<RptSimpleTemplate>(rptUid);
            ReportBase report = new FapDefaultReport(_dbContext, _applicationContext, _cacheService, rptModel.ReportName);
            var reportGenerator=  new DefaultReportGenerator(report);
            string outFilePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, $"{rptModel.ReportName}_Result.xlsx");
            await Task.Factory.StartNew(() =>
            {
                XLWorkbook result = reportGenerator.Render(GetReportTemplateWorkbook($"{rptModel.XlsFile}.xlsx"));
                result.SaveAs(outFilePath);
            });
            return FapPlatformConstants.TemporaryFolder+Path.DirectorySeparatorChar+ $"{rptModel.ReportName}_Result.xlsx";
        }
    }
}
