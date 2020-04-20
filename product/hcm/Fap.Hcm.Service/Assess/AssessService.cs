using Dapper;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Hcm.Service.Assess
{
    [Service]
    public class AssessService : IAssessService
    {
        private readonly IDbContext _dbContext;
        public AssessService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<TreeDataView> GetSchemeCategoryTree()
        {
            IEnumerable<dynamic> prmCategory = _dbContext.Query("select * from PerfProgramCategory");
            List<TreeDataView> oriList = prmCategory.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { group = "1" }, Text = t.Name, Icon = "icon-folder blue ace-icon fa fa-file-word-o" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "方案分类",
                Data = new { group = "0" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return tree;
        }
        public ResponseViewModel OperSchemeCategory(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.DeleteExec(nameof(PerfProgramCategory), "Fid=@Fid", new DynamicParameters(new { Fid = postData.Id }));
                vm.success = c > 0 ? true : false;
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                PerfProgramCategory ppc = new PerfProgramCategory
                {
                    Pid = postData.Id,
                    Name = postData.Text
                };
                _dbContext.Insert(ppc);
                vm.success = true;
                vm.data = ppc.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var prmCategory = _dbContext.Get<PerfProgramCategory>(postData.Id);
                prmCategory.Name = postData.Text;
                vm.success = _dbContext.Update(prmCategory);
            }
            else if (postData.Operation == TreeNodeOper.MOVE_NODE)
            {
                var prmCategory = _dbContext.Get<PerfProgramCategory>(postData.Id);
                prmCategory.Pid = postData.Parent;
                vm.success = _dbContext.Update(prmCategory);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }
        public void CreateExaminer(ExaminerViewModel examinerVM)
        {
            IList<PerfExaminer> perfExaminers = new List<PerfExaminer>();
            //存放的是fid 非ObjUid
            var objectiveUids = examinerVM.Objectives;
            if (!objectiveUids.Any())
            {
                return;
            }
            var objectivesList= _dbContext.Query<PerfObject>("select * from PerfObject where Fid in @Fids", new DynamicParameters(new { Fids = objectiveUids }));
            var param= new DynamicParameters(new { EmpUids = objectivesList.Select(o=>o.ObjUid) });
            if (examinerVM.IsOrgDept)
            {
                DeptExaminer();
            }
            if (examinerVM.IsLeaderShip)
            {
                LeadershipExaminer();
            }
            if (examinerVM.IsCustom)
            {
                CustomExaminer();
            }
            if (perfExaminers.Any())
            {
                _dbContext.InsertBatchSql(perfExaminers);
            }
            void CustomExaminer() {
                if (examinerVM.CustomExaminers != null && examinerVM.CustomExaminers.Any())
                {
                    foreach (var obj in objectivesList)
                    {
                        foreach (var empuid in examinerVM.CustomExaminers)
                        {
                            PerfExaminer examiner = new PerfExaminer
                            {
                                ProgramUid = examinerVM.SchemeUid,
                                ObjectUid = obj.Fid,
                                AssessModel = examinerVM.CustomModelName??"自定义",
                                EmpUid = empuid,
                                Weights = examinerVM.CustomWeights
                            };
                            perfExaminers.Add(examiner);
                        }
                    }
                }
            }
            void LeadershipExaminer()
            {
                string sql = $"select {nameof(Employee.Fid)},{nameof(Employee.Leadership)} from {nameof(Employee)} where Fid in @EmpUids";
                var leaderships = _dbContext.Query<Employee>(sql, param);
                foreach (var obj in objectivesList)
                {
                    var emp = leaderships.FirstOrDefault(e => e.Fid == obj.ObjUid);
                    if (emp != null&&emp.Leadership.IsPresent())
                    {
                            PerfExaminer examiner = new PerfExaminer
                            {
                                ProgramUid = examinerVM.SchemeUid,
                                ObjectUid = obj.Fid,
                                AssessModel = examinerVM.LeadershipModelName,
                                EmpUid = emp.Leadership,
                                Weights = examinerVM.LeaderShipWeights
                            };
                            perfExaminers.Add(examiner);
                        
                    }
                }
            }
            void DeptExaminer()
            {
                string sql = "select Fid,DeptUid from Employee where DeptUid in(select DeptUid from Employee where Fid in @EmpUids)";
                var employees = _dbContext.Query<Employee>(sql, param);
                foreach (var obj in objectivesList)
                {
                    var currEmployee = employees.FirstOrDefault(e => e.Fid == obj.ObjUid);
                    var currDeptEmps = employees.Where(e => e.DeptUid == currEmployee.DeptUid && e.Fid != currEmployee.Fid);
                    foreach (var emp in currDeptEmps)
                    {
                        PerfExaminer examiner = new PerfExaminer
                        {
                            ProgramUid = examinerVM.SchemeUid,
                            ObjectUid = obj.Fid,
                            AssessModel = examinerVM.DeptModelName,
                            EmpUid = emp.Fid,
                            Weights = examinerVM.DeptWeights
                        };
                        perfExaminers.Add(examiner);
                    }
                }
            }
        }
        [Transactional]
        public void CopyScheme(string fid)
        {
            PerfProgram program = _dbContext.Get<PerfProgram>(fid);
            program.Id = -1;
            program.Fid = "";
            program.PrmCode = "copy_" + program.PrmCode;
            program.PrmName = "copy_" + program.PrmName;
            program.PrmStatus = PerfPrmStatus.Init;
            program.CreateBy = "";
            program.CreateDate = "";
            program.CreateName = "";
            program.UpdateBy = "";
            program.UpdateDate = "";
            program.UpdateName = "";
            //拷贝方案
            _dbContext.Insert(program);
            PerfProgram newPrm = program;
            DynamicParameters param = new DynamicParameters();
            param.Add("PrmUid", fid);
            //获取指标分类
            var kpiTypes = _dbContext.QueryWhere<PerfKPIType>("PerfProgram=@PrmUid", param);
            //获取指标
            var kpis = _dbContext.QueryWhere<PerfKPIs>("ProgramUid=@PrmUid", param);
          
            kpiTypes.ToList().ForEach((k) =>
            {
                //获取指标分类下的指标
                string ktypeFid = k.Fid;
                k.Id = -1;
                k.Fid = "";
                k.PerfProgram = newPrm.Fid;
                k.CreateBy = "";
                k.CreateDate = "";
                k.CreateName = "";
                k.UpdateBy = "";
                k.UpdateDate = "";
                k.UpdateName = "";
                //拷贝指标分类
                _dbContext.Insert<PerfKPIType>(k);
                PerfKPIType newKpiType = k;
                var selKpis = kpis.Where(kpi => kpi.KpiType == ktypeFid);
                if (selKpis != null && selKpis.Any())
                {
                    selKpis.ToList().ForEach((kpi) =>
                    {
                        kpi.Id = -1;
                        kpi.Fid = "";
                        kpi.ProgramUid = newPrm.Fid;
                        kpi.KpiType = newKpiType.Fid;
                        kpi.CreateBy = "";
                        kpi.CreateDate = "";
                        kpi.CreateName = "";
                        kpi.UpdateBy = "";
                        kpi.UpdateDate = "";
                        kpi.UpdateName = "";
                        //复制指标
                        _dbContext.Insert<PerfKPIs>(kpi);
                    });
                }
            });
            //复制考核对象
            var objects = _dbContext.QueryWhere<PerfObject>("ProgramUid=@PrmUid", param);
            if ( objects.Any())
            {
                objects.ToList().ForEach((m) =>
                {
                    m.Id = -1;
                    m.Fid = "";
                    m.ProgramUid = newPrm.Fid;
                    m.CreateBy = "";
                    m.CreateDate = "";
                    m.CreateName = "";
                    m.UpdateBy = "";
                    m.UpdateDate = "";
                    m.UpdateName = "";
                    _dbContext.Insert<PerfObject>(m);
                });
            }
        }
        [Transactional]
        public void AssessCalculate(string schemeUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PrmUid", schemeUid);
            string sql = $"select avg({nameof(PerfExaminer.Score)}) {nameof(PerfExaminer.Score)},{nameof(PerfExaminer.ObjectUid)},{nameof(PerfExaminer.AssessModel)},{nameof(PerfExaminer.Weights)} from PerfExaminer where  {nameof(PerfExaminer.ProgramUid)}=@PrmUid and {nameof(PerfExaminer.Score)}>0  group by {nameof(PerfExaminer.ObjectUid)}, {nameof(PerfExaminer.AssessModel)},{nameof(PerfExaminer.Weights)}";
            
            IEnumerable<PerfExaminer> examiners = _dbContext.Query<PerfExaminer>(sql, param);
            IEnumerable<PerfObject> objectives = _dbContext.QueryWhere<PerfObject>("ProgramUid=@PrmUid", param);
            if (examiners.Any())
            {
                var examinerScores= examiners.GroupBy(e => e.ObjectUid);
                foreach (var escore in examinerScores)
                {
                    var objective= examiners.FirstOrDefault(p => p.Fid == escore.Key);
                    if (objective != null)
                    {
                        double score = 0.0;
                        foreach (var es in escore)
                        {
                            score += (es.Score * es.Weights) / escore.Sum(e => e.Weights);
                        }
                        objective.Score =Math.Round(score,2);
                    }
                }
                _dbContext.UpdateBatchSql(objectives);
            }
            //修改考核方案状态为 成绩发布
            PerfProgram prm = _dbContext.Get<PerfProgram>(schemeUid);
            prm.PrmStatus = PerfPrmStatus.Result;
            _dbContext.Update<PerfProgram>(prm);
        }
    }
}
