using ClosedXML.Excel;

using ExcelReportGenerator.Rendering;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Cache;
using Fap.Core.Infrastructure.Domain;
using Fap.ExcelReport.Reports;
using Fap.ExcelReport.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Fap.ExcelReport
{
    [Service]
    public class ExcelReportService : IExcelReportService
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IServiceProvider _serviceProvider;
        public ExcelReportService(IServiceProvider serviceProvider, IDbContext dbContext, IFapApplicationContext applicationContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _applicationContext = applicationContext;
            _serviceProvider = serviceProvider;
        }
        private XLWorkbook GetReportTemplateWorkbook(string templateName)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, templateName);
            return new XLWorkbook(filePath);
        }
        public async Task<string> Render(string rptUid)
        {
            var rptModel = _dbContext.Get<RptSimpleTemplate>(rptUid);
            ReportBase report = new FapDefaultReport(_dbContext, _applicationContext, _cacheService, rptModel);
            var reportGenerator = new FapReportGenerator(_serviceProvider, report);
            string outFilePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, $"{rptModel.ReportName}_Result.xlsx");
            await Task.Factory.StartNew(() =>
            {
                XLWorkbook result = reportGenerator.Render(GetReportTemplateWorkbook($"{rptModel.XlsFile}.xlsx"));
                result.SaveAs(outFilePath);
            });
            
            return FapPlatformConstants.TemporaryFolder + Path.DirectorySeparatorChar + $"{rptModel.ReportName}_Result.xlsx";
        }
    }
}
