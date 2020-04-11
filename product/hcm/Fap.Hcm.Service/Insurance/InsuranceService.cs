using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using Fap.Hcm.Service.Payroll;
using System;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Utility;
using System.Linq;
using Fap.Core.Exceptions;
using Fap.Core.DI;

namespace Fap.Hcm.Service.Insurance
{
    [Service]
    public class InsuranceService : IInsuranceService
    {
        private const string INSCENTER = "InsCenter";
        private readonly IDbContext _dbContext;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapPlatformDomain _platformDomain;
        public InsuranceService(IDbContext dataAccessor, IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _dbContext = dataAccessor;
            _platformDomain = platformDomain;
            _applicationContext = applicationContext;
        }
        [Transactional]
        public void UseInsPending(InsToDo insToDo)
        {
            //保险组
            InsCase insCase = _dbContext.Get<InsCase>(insToDo.CaseUid);
            //员工
            Employee employee = _dbContext.Get<Employee>(insToDo.EmpUid);
            //检查员工是否在保险组
            var caseEmployee = _dbContext.QueryFirstOrDefault($"select * from {insCase.TableName} where EmpUid=@EmpUid", new DynamicParameters(new { EmpUid = employee.Fid }));
            if (caseEmployee != null)
            {
                if (employee.EmpStatus == EmployeeStatus.Former)
                {
                    DeleteEmployeeFromInsCase();
                }
                else
                {
                    UpdateEmployeeToInsCase();
                }
            }
            else if (employee.EmpStatus == EmployeeStatus.Current)
            {
                AddEmployeeToInsCase();
            }
            MarkInsTodo();
            void MarkInsTodo()
            {
                insToDo.OperDate = DateTimeUtils.CurrentDateTimeStr;
                insToDo.OperEmpUid = _applicationContext.EmpUid;
                insToDo.OperFlag = "1";
                _dbContext.Update(insToDo);
            }
            void DeleteEmployeeFromInsCase()
            {
                string sql = $"delete from {insCase.TableName} where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void UpdateEmployeeToInsCase()
            {
                string sql = $"update {insCase.TableName} set EmpCode='{employee.EmpCode}',EmpCategory='{employee.EmpCategory}',DeptUid='{employee.DeptUid}' where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void AddEmployeeToInsCase()
            {
                FapDynamicObject caseEmp = new FapDynamicObject(_dbContext.Columns(insCase.TableName));
                //将此人的放入薪资套
                caseEmp.SetValue("EmpUid", employee.Fid);
                caseEmp.SetValue("EmpCode", employee.EmpCode);
                caseEmp.SetValue("EmpCategory", employee.EmpCategory);
                caseEmp.SetValue("DeptUid", employee.DeptUid);
                caseEmp.SetValue("InsCaseUid", insCase.Fid);
                caseEmp.SetValue("InsmentTimes", 1);
                caseEmp.SetValue("InsConfirm", 0);

                if (insCase.InsYM.IsPresent())
                {
                    caseEmp.SetValue("InsYM", insCase.InsYM);
                }
                else
                {
                    caseEmp.SetValue("InsYM", insCase.InitYM);
                }
                _dbContext.InsertDynamicData(caseEmp);
            }
        }

        /// <summary>
        /// 获取保险组的保险项
        /// </summary>
        /// <param name="caseUid">保险套</param>
        /// <returns></returns>
        public IEnumerable<CaseItem> GetInsItems(string caseUid)
        {
            CaseItem pi = new CaseItem();
            var insItems = _dbContext.Columns(INSCENTER).Where(c => c.IsDefaultCol != 1 && !c.ColProperty.Trim().EqualsWithIgnoreCase("3"))
                 .Select(c => new CaseItem { Fid = c.Fid, ColComment = c.ColComment, ColName = c.ColName, IsSelected = false }).ToList();
            var caseItems = _dbContext.QueryWhere<InsItem>("CaseUid=@CaseUid", new Dapper.DynamicParameters(new { CaseUid = caseUid }))
                .Select(c => c.ColumnUid);
            if (caseItems.Any())
            {
                foreach (var item in insItems)
                {
                    if (caseItems.Contains(item.Fid))
                    {
                        item.IsSelected = true;
                    }
                }
            }
            return insItems;

        }
        [Transactional]
        public void AddInsItems(string caseUid, string[] insItems)
        {
            if (insItems == null || insItems.Length < 1 || caseUid.IsMissing())
            {
                return;
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", caseUid);
            var insCase = _dbContext.Get<InsCase>(caseUid);
            if (insCase.Unchanged == 1)
            {
                throw new FapException("已经存在封存记录，不能再调整保险项");
            }
            var items = _dbContext.Columns(INSCENTER).Where(f => f.ColProperty.EqualsWithIgnoreCase("3") || insItems.Contains(f.Fid));
            IList<InsItem> list = new List<InsItem>();
            foreach (var item in items)
            {
                list.Add(new InsItem { CaseUid = caseUid, ColumnUid = item.Fid, ItemSort = item.ColOrder, ShowAble = 1, ShowCard = 0, TransEnable = 0 });
            }
            _dbContext.DeleteExec(nameof(InsItem), "CaseUid=@CaseUid", param);
            _dbContext.InsertBatchSql<InsItem>(list);
        }
        [Transactional]
        public long CreateInsCase(string caseUid)
        {
            var insCase = _dbContext.Get<InsCase>(caseUid);
            //生成工资套对应表元数据
            FapTable ft = new FapTable();
            ft.Fid = UUIDUtils.Fid;
            ft.TableName = $"InsCase{_applicationContext.TenantID}{insCase.CaseCode}";
            ft.TableType = "BUSINESS";
            ft.TableCategory = "Ins";
            ft.TableComment = $"{insCase.CaseName}保险组";
            ft.TableMode = "SINGLE";
            ft.TableFeature = "";//根据ColProperty='3'过滤，不在用TableFeature
            ft.IsSync = 1;
            ft.IsBasic = 1;
            ft.ProductUid = "HCM";

            IEnumerable<FapColumn> centerCols = _dbContext.Columns(INSCENTER);
            var caseItems = _dbContext.QueryWhere<InsItem>("CaseUid=@CaseUid", new Dapper.DynamicParameters(new { CaseUid = caseUid }))
               .Select(c => c.ColumnUid);
            List<FapColumn> cols = new List<FapColumn>();
            foreach (var col in centerCols.Where(c => caseItems.Contains(c.Fid)))
            {
                col.Fid = null;
                col.TableName = ft.TableName;
                cols.Add(col);
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", ft.TableName);
            _dbContext.Execute("delete from FapTable where TableName=@TableName", param);
            _dbContext.Execute("delete from FapColumn where TableName=@TableName", param);
            _dbContext.Execute($"delete from FapMultiLanguage where LangKey like '{ft.TableName}_%'");
            _dbContext.Insert<FapTable>(ft);
            _dbContext.InsertBatch<FapColumn>(cols);
            _platformDomain.TableSet.Refresh();
            _platformDomain.ColumnSet.Refresh();
            _platformDomain.MultiLangSet.Refresh();
            insCase.TableName = ft.TableName;
            _dbContext.Update(insCase);
            return ft.Id;
        }
        [Transactional]
        public void InitEmployeeToInsCase(InsCase insCase,string empWhere)
        {
            if (insCase.Unchanged == 1)
            {
                throw new FapException("已存在保险记录，不能再初始化员工");
            }
            if (!empWhere.Contains("IsMainJob", StringComparison.OrdinalIgnoreCase))
            {
                empWhere = empWhere.IsMissing() ? " IsMainJob=1" : empWhere + " and IsMainJob=1";
            }
            var employees = _dbContext.QueryWhere<Employee>(empWhere);
            IList<FapDynamicObject> listCase = new List<FapDynamicObject>();
            foreach (var emp in employees)
            {
                FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(insCase.TableName));
                fdo.SetValue("EmpUid", emp.Fid);
                fdo.SetValue("EmpCode", emp.EmpCode);
                fdo.SetValue("EmpCategory", emp.EmpCategory);
                fdo.SetValue("DeptUid", emp.DeptUid);
                fdo.SetValue("InsCaseUid", insCase.Fid);
                fdo.SetValue("InsYM", insCase.InsYM.IsPresent() ? insCase.InsYM : insCase.InitYM);
                fdo.SetValue("InsmentTimes", 1);
                fdo.SetValue("InsConfirm", 0);
                listCase.Add(fdo);
            }
            _dbContext.ExecuteOriginal($"truncate table {insCase.TableName}");
            _dbContext.InsertDynamicDataBatchSql(listCase);
            //添加发放记录
            var records = _dbContext.DeleteExec(nameof(InsRecord), "CaseUid=@CaseUid"
                , new DynamicParameters(new { CaseUid = insCase.Fid }));
            InsRecord pr = new InsRecord();
            pr.CaseUid = insCase.Fid;
            pr.InsCount = 1;
            pr.InsYM = insCase.InitYM;
            pr.InsFlag = 0;
            _dbContext.Insert<InsRecord>(pr);
            //更新薪资套本次发放内容
            insCase.InsYM = insCase.InitYM;
            insCase.InsCount = 1;
            insCase.InsFlag = 0;
            _dbContext.Update(insCase);
        }
    }
}
