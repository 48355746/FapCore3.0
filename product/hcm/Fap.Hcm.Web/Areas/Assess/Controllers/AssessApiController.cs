using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Assess;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ardalis.GuardClauses;
using Dapper;

namespace Fap.Hcm.Web.Areas.Assess.Controllers
{
    [Area("Assess")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class AssessApiController : FapController
    {
        private readonly IAssessService _assessService;
        public AssessApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _assessService = serviceProvider.GetService<IAssessService>();
        }
        [HttpGet("SchemeCategory")]
        public JsonResult SchemeCategory()
        {
            var tree= _assessService.GetSchemeCategoryTree();
            return Json(tree);
        }
        [HttpPost("SchemeCategory")]
        public JsonResult OperSchemeCategory(TreePostData postData)
        {
            var result = _assessService.OperSchemeCategory(postData);
            return Json(result);
        }
        [HttpPost("Objectives")]
        public JsonResult Objectives(string schemeUid,string objective,string filter)
        {            
            JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
            string filterWhere = jfs.BuilderFilter(objective, filter);
            string sql = string.Empty;
            if (objective.EqualsWithIgnoreCase("Employee"))
            {
                if (!filterWhere.Contains("IsMainJob", StringComparison.OrdinalIgnoreCase))
                {
                    filterWhere = filterWhere.IsMissing() ? " IsMainJob=1" : filterWhere + " and IsMainJob=1";
                }
                sql = $"select Fid ObjUid,EmpCode as ObjCode,EmpName as ObjName,'{schemeUid}' ProgramUid from Employee where " + filterWhere;
            }
            else
            {
                sql = $"select Fid ObjUid,DeptCode as ObjCode,DeptName as ObjName,'{schemeUid}' ProgramUid from OrgDept where " + filterWhere;
            }
            if (sql.IsMissing())
            {
                Guard.Against.FapBusiness("不存在此类考核对象类型");
            }
            sql += " and Fid not in(select ObjUid from PerfObject where ProgramUid=@SchemeUid)";
            var objectives= _dbContext.Query<PerfObject>(sql,new Dapper.DynamicParameters(new { SchemeUid=schemeUid }));
            if (objectives.Any())
            {
                _dbContext.InsertBatchSql(objectives);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Examiner")]
        public JsonResult Examiner(ExaminerViewModel examiner)
        {
            Guard.Against.Null(examiner,nameof(examiner));
            _assessService.CreateExaminer(examiner);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("SchemeStatus")]
        public JsonResult SchemeStatus(string schemeUids,string status)
        {
            var schemeList = schemeUids.SplitComma();
            string sql = $"select * from {nameof(PerfProgram)} where Fid in @Fids";
            var schemes= _dbContext.Query<PerfProgram>(sql, new Dapper.DynamicParameters(new { Fids = schemeList }));
            foreach (var scheme in schemes)
            {
                scheme.PrmStatus = status;
            }
            _dbContext.UpdateBatch(schemes);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("CopyScheme")]
        public JsonResult CopyScheme(string schemeUid)
        {
            _assessService.CopyScheme(schemeUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Calculate")]
        public JsonResult AssessCalculate(string schemeUid)
        {
            _assessService.AssessCalculate(schemeUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("AssessChart")]
        public JsonResult AssessChart(string schemeUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PrmUid", schemeUid);
            int yx = _dbContext.Count("PerfObject", "ProgramUid=@PrmUid and Score>=90", param);
            int lh = _dbContext.Count("PerfObject", "ProgramUid=@PrmUid  and Score>=80 and Score<90", param);
            int yb = _dbContext.Count("PerfObject", "ProgramUid=@PrmUid  and Score<80", param);
            return Json(new int[] { yx, lh, yb });
        }
        [HttpGet("ComplementChart")]
        public JsonResult ComplementChart(string schemeUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PrmUid", schemeUid);
            int df = _dbContext.Count("PerfExaminer", "ProgramUid=@PrmUid and Score>0", param);
            int all = _dbContext.Count("PerfExaminer", "ProgramUid=@PrmUid", param);
            int wdf = all - df;
            return Json(new int[] { all, df, wdf });
        }
    }
}