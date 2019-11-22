using Dapper;
using Fap.AspNetCore.Model;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.ViewComponents
{
    /// <summary>
    /// 数据历史
    /// </summary>
    public class DataHistoryViewComponent : ViewComponent
    {
        private readonly IDbContext _dataAccessor;
        public DataHistoryViewComponent(IDbContext dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync(string bn, string fid)
        {
            var history = _dataAccessor.QueryDataHistory(bn, fid);
            return await Task.FromResult(View(history));
        }
    }
    public class QueryProgramViewComponent:ViewComponent
    {
        private readonly IDbContext _dataAccessor;
        private readonly IFapApplicationContext _applicationContext;
        public QueryProgramViewComponent(IDbContext dataAccessor, IFapApplicationContext appContext)
        {
            _dataAccessor = dataAccessor;
            _applicationContext = appContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(string tn, string gid)
        {
            string where = " (tablename=@TableName and UseEmployee=@UseEmployee) or IsGlobal=1 ";
            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", tn);
            param.Add("UseEmployee", _applicationContext.EmpUid);
            var qPlist = _dataAccessor.QueryWhere<CfgQueryProgram>(where, param);
            ViewBag.GrdId = gid;
            ViewBag.Tn = tn;
            return await Task.FromResult(View(qPlist));
        }
    }
    public class ConditionEditorViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string tableName)
        {
            ViewBag.TableName = tableName;
            return await Task.FromResult(View());
        }
    }
}
