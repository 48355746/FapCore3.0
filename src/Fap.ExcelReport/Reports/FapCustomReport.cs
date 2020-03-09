using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Cache;
using Fap.Core.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fap.ExcelReport.Reports
{
    public class FapCustomReport : ReportBase
    {
        private readonly IDbContext _dbContext;
        private readonly RptSimpleTemplate _rptSimpleTemplate;
        private readonly ICacheService _cacheService;
        private readonly IFapApplicationContext _applicationContext;
        public FapCustomReport(IDbContext dbContext, IFapApplicationContext applicationContext, ICacheService cacheService, RptSimpleTemplate rptSimpleTemplate)
        {
            _dbContext = dbContext;
            _applicationContext = applicationContext;
            _cacheService = cacheService;
            _rptSimpleTemplate = rptSimpleTemplate;
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
        public IEnumerable<IDictionary<string, object>> GetEntityWhere(string entityName, string fieldName, string fieldValue)
        {
            var entityList = GetEntity(entityName).Where(c => c[fieldName]?.ToString() == fieldValue);
            return TranslateDynamic(entityList);
        }
        public IEnumerable<IDictionary<string, object>> GetEntitySql(string sql, object p0 = null,
            object p1 = null, object p2 = null, object p3 = null, object p4 = null, object p5 = null, object p6 = null, object p7 = null, object p8 = null, object p9 = null, object p10 = null)
        {
            sql = sql.TrimStart('(').TrimEnd(')');
            List<object> sqlParams = new List<object>();
            if (p0 != null)
            {
                sqlParams.Add(p0);
            }
            if (p1 != null)
            {
                sqlParams.Add(p1);
            }
            if (p2 != null)
            {
                sqlParams.Add(p2);
            }
            if (p3 != null)
            {
                sqlParams.Add(p3);
            }
            if (p4 != null)
            {
                sqlParams.Add(p4);
            }
            if (p5 != null)
            {
                sqlParams.Add(p5);
            }
            if (p6 != null)
            {
                sqlParams.Add(p6);
            }
            if (p7 != null)
            {
                sqlParams.Add(p7);
            }
            if (p8 != null)
            {
                sqlParams.Add(p8);
            }
            if (p9 != null)
            {
                sqlParams.Add(p9);
            }
            if (p10 != null)
            {
                sqlParams.Add(p10);
            }
            string sps = sqlParams.Select(p => p)?.ToString() ?? "";
            string c_key = $"reports_{(sql + sps).Md5()}";
            var entityListCache = _cacheService.Get<IEnumerable<IDictionary<string, object>>>(c_key);
            if (entityListCache == null)
            {
                DynamicParameters param = new DynamicParameters();
                if (sqlParams.Count() > 0)
                {
                    MatchCollection mc = Regex.Matches(sql, @"\@\d");
                    foreach (Match m in mc)
                    {
                        int p = m.Value.Replace("@", "").ToInt();
                        param.Add(p.ToString(), sqlParams[p]);
                    }
                }
                var entityList = _dbContext.QueryOriSql(sql, param);
                entityListCache = TranslateDynamic(entityList);
                _cacheService.Add(c_key, entityListCache);
            }
            return entityListCache;
        }
        public string Dictionary(string category, string code)
        {
            var dic = _dbContext.Dictionary(category, code);
            if (dic != null)
            {
                return dic.Name;
            }
            return "未知";
        }
    }
}
