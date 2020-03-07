using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Cache;
using Fap.Core.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.ExcelReport.Reports
{
    public class FapDefaultReport : ReportBase
    {
        private readonly IDbContext _dbContext;
        private readonly RptSimpleTemplate _rptSimpleTemplate;
        private readonly ICacheService _cacheService;
        private readonly IFapApplicationContext _applicationContext;
        public FapDefaultReport(IDbContext dbContext, IFapApplicationContext applicationContext, ICacheService cacheService, RptSimpleTemplate rptSimpleTemplate)
        {
            _dbContext = dbContext;
            _applicationContext = applicationContext;
            _cacheService = cacheService;
            _rptSimpleTemplate =rptSimpleTemplate;
        }
        public override string ReportName => _rptSimpleTemplate.ReportName;

        public override string EmployeeName => _applicationContext.EmpName;
        public IEnumerable<IDictionary<string, object>> GetEntity(string entityName)
        {
            string c_key = $"reports_{entityName}";
            var entityListCache = _cacheService.Get<IEnumerable<IDictionary<string, object>>>(c_key);
            if (entityListCache == null)
            {
                var entityList = _dbContext.QueryAll(entityName, true);
                entityListCache = TranslateDynamic(entityList);
                _cacheService.Add(c_key, entityListCache);
            }
            return entityListCache;
        }
        private IEnumerable<IDictionary<string, object>> TranslateDynamic(IEnumerable<dynamic> entityList)
        {
            foreach (var entity in entityList)
            {
                yield return (IDictionary<string, object>)entity;
            }
        }
        public IEnumerable<IDictionary<string, object>> GetEntityWhere(string entityName, string fieldName,string fieldValue)
        {
            var entityList = GetEntity(entityName).Where(c=>c[fieldName]?.ToString()==fieldValue);
            return TranslateDynamic(entityList);
        }
    }
}
