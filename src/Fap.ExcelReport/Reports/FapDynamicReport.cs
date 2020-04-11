using ExcelReportGenerator.Enums;
using ExcelReportGenerator.Rendering.EventArgs;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Cache;
using Fap.Core.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport.Reports
{
    public class FapDynamicReport : ReportBase
    {
        private readonly IDbContext _dbContext;
        private readonly RptSimpleTemplate _rptSimpleTemplate;
        private readonly ICacheService _cacheService;
        private readonly IFapApplicationContext _applicationContext;
        public FapDynamicReport(IDbContext dbContext, IFapApplicationContext applicationContext, ICacheService cacheService, RptSimpleTemplate rptSimpleTemplate)
        {
            _dbContext = dbContext;
            _applicationContext = applicationContext;
            _cacheService = cacheService;
            _rptSimpleTemplate = rptSimpleTemplate;
        }
        public override string ReportName => _rptSimpleTemplate.ReportName;

        public override string EmployeeName => _applicationContext.EmpName;
        public IEnumerable<dynamic> GetDynamicSql(string sql)
        {
            sql = sql.TrimStart('(').TrimEnd(')');
            string c_key = $"reports_{sql.Md5()}";
            var entityListCache = _cacheService.Get<IEnumerable<dynamic>>(c_key);
            if (entityListCache == null)
            {               
                entityListCache = _dbContext.Query(sql);
                _cacheService.Add(c_key, entityListCache);
            }
            return entityListCache;
        }


        public void BeforeHeadersRender(DataSourceDynamicPanelBeforeRenderEventArgs args)
        {
            foreach (var column in args.Columns)
            {

            }
            //args.Columns[6].AggregateFunction = AggregateFunction.Max;
            //args.Columns[7].AggregateFunction = AggregateFunction.Min;
            //args.Columns[8].DisplayFormat = "$#,0.00";
        }
        public void AfterDataTemplatesRender(DataSourceDynamicPanelEventArgs args)
        {
            //args.Range.FirstCell().Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Background2);
            //args.Range.Range(1, 7, 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }

        public void AfterTotalsRender(DataSourcePanelEventArgs args)
        {
            //args.Range.Range(1, 7, 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            //IXLRange mergedRange = args.Range.Range(1, 1, 1, 6).Merge();
            //mergedRange.Value = "Totals";
            //mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }
    }
}
