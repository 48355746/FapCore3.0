using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using Fap.Core.Utility;
using System.Linq;
using Fap.Core.Exceptions;
using Fap.Core.DI;
using Fap.Hcm.Service.Payroll;
using Fap.Core.Infrastructure.Model;

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
        [Transactional]
        public void InitInsuranceData(InitDataViewModel initData)
        {
            InsCase insCase = _dbContext.Get<InsCase>(initData.CaseUid);
            InsRecord iRecord = _dbContext.Get<InsRecord>(initData.RecordUid);

            string where = " InsYM='" + iRecord.InsYM + "' and InsCaseUid='" + iRecord.CaseUid + "' and InsmentTimes=" + iRecord.InsCount;
            var colList = _dbContext.Columns(insCase.TableName);
            //基础列和 特性列
            List<FapColumn> baseCols = colList.Where(c => (c.IsDefaultCol == 1 || c.ColProperty == "3") && c.ColName != "Id" && c.ColName != "Fid").ToList();
            string pCols = string.Join(",", baseCols.Select(c => c.ColName));
            if (initData.ReservedItems.IsPresent())
            {
                pCols += "," + initData.ReservedItems;
            }
            pCols = pCols.ReplaceIgnoreCase("InsYM", "'" + initData.InitYm + "' as InsYM");

            //检查当月是否有发送记录
            DynamicParameters param = new DynamicParameters();
            param.Add("InsYM", initData.InitYm);
            param.Add("CaseUid", initData.CaseUid);
            var records = _dbContext.QueryWhere<InsRecord>(" InsYM=@InsYM and CaseUid=@CaseUid", param);
            int pcount = 1;
            if (records.Any())
            {
                var existRecord = records.FirstOrDefault(r => r.InsFlag == 0);
                if (existRecord == null)
                {
                    pcount = records.Max(r => r.InsCount) + 1;
                    //添加发放记录
                    InsRecord newRecord = new InsRecord();
                    newRecord.CaseUid = insCase.Fid;
                    newRecord.InsCount = pcount;
                    newRecord.InsYM = initData.InitYm;
                    newRecord.InsFlag = 0;
                    _dbContext.Insert(newRecord);
                }
                else
                {
                    pcount = existRecord.InsCount;
                }
            }
            else
            {
                //添加发放记录
                InsRecord newRecord = new InsRecord();
                newRecord.CaseUid = insCase.Fid;
                newRecord.InsCount = pcount;
                newRecord.InsYM = initData.InitYm;
                newRecord.InsFlag = 0;
                _dbContext.Insert(newRecord);
            }
            pCols = pCols.ReplaceIgnoreCase("InsmentTimes", pcount.ToString() + " as InsmentTimes");
            pCols = pCols.ReplaceIgnoreCase("InsConfirm", "0 as InsConfirm");
            string sql = $"select {pCols} from InsCenter where {where}";
            var pastData = _dbContext.QueryOriSql(sql);
            IList<FapDynamicObject> listCase = new List<FapDynamicObject>();
            foreach (var pd in pastData)
            {
                var dicPd = pd as IDictionary<string, object>;
                FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(insCase.TableName));
                foreach (var key in dicPd.Keys)
                {
                    fdo.SetValue(key, dicPd[key]);
                }
                listCase.Add(fdo);
            }
            _dbContext.ExecuteOriginal($"truncate table {insCase.TableName}");
            _dbContext.InsertDynamicDataBatchSql(listCase);

            //更新工资套
            insCase.InsYM = initData.InitYm;
            insCase.InsCount = pcount;
            insCase.InsFlag = 0;
            _dbContext.Update(insCase);
        }
        [Transactional]
        public void InsuranceOff(string caseUid)
        {
            InsCase insCase = _dbContext.Get<InsCase>(caseUid);
            if (insCase.InsFlag == 1)
            {
                throw new FapException("保险已经完成，请不要重复操作");
            }
            insCase.InsFlag = 1;
            //标记已经完成不能再改变
            insCase.Unchanged = 1;
            _dbContext.Update(insCase);
            //获取发放记录
            DynamicParameters paramR = new DynamicParameters();
            paramR.Add("InsYM", insCase.InsYM);
            paramR.Add("InsCount", insCase.InsCount);
            paramR.Add("CaseUid", insCase.Fid);
            InsRecord InsRecord = _dbContext.QueryFirstOrDefaultWhere<InsRecord>($"{nameof(InsRecord.InsYM)}=@InsYM and {nameof(InsRecord.InsCount)}=@InsCount and CaseUid=@CaseUid and InsFlag=0", paramR);
            if (InsRecord != null)
            {
                InsRecord.InsFlag = 1;
                InsRecord.InsEmpUid = _applicationContext.EmpUid;
                InsRecord.InsDate = DateTimeUtils.CurrentDateTimeStr;
                _dbContext.Update(InsRecord);
            }

            var caseCols = _dbContext.Columns(insCase.TableName)
                .Where(c => !c.ColName.EqualsWithIgnoreCase("Id"))
                .Select(c => c.ColName);
            string cols = string.Join(",", caseCols);
            string insertSql = string.Format("insert into {0}({1}) (select {1} from {2})", INSCENTER, cols, insCase.TableName);
            _dbContext.ExecuteOriginal(insertSql);
            string updateSql = string.Format("update {0} set InsConfirm=1", insCase.TableName);
            _dbContext.ExecuteOriginal(updateSql);
        }
        /// <summary>
        /// 取消发放
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        [Transactional]
        public void InsuranceOffCancel(string caseUid)
        {
            InsCase ic = _dbContext.Get<InsCase>(caseUid);
            if (ic.InsFlag == 0)
            {
                throw new FapException("保险未完成，不用取消");
            }
            ic.InsFlag = 0;
            if (ic.InitYM == ic.InsYM && ic.InsCount == 1)
            {
                ic.Unchanged = 0;
            }
            _dbContext.Update(ic);
            //获取发放记录
            DynamicParameters paramR = new DynamicParameters();
            paramR.Add("InsYM", ic.InsYM);
            paramR.Add("InsCount", ic.InsCount);
            paramR.Add("CaseUid", ic.Fid);
            InsRecord ir = _dbContext.QueryFirstOrDefaultWhere<InsRecord>("InsYM=@InsYM and InsCount=@InsCount and CaseUid=@CaseUid and InsFlag=1", paramR);
            if (ir != null)
            {
                ir.InsFlag = 0;
                ir.InsDate = "";
                ir.InsEmpUid = "";
                _dbContext.Update(ir);
            }
            string deleteSql = "delete from InsCenter where InsCaseUid=@CaseUid and InsmentTimes=@InsTimes and InsYM=@InsYM";
            string updateSql = string.Format("update {0} set InsConfirm=0", ic.TableName);
            _dbContext.Execute(deleteSql, new DynamicParameters(new { CaseUid = ic.Fid, InsTimes = ic.InsCount, InsYM = ic.InsYM }));
            _dbContext.Execute(updateSql);
        }
        public GapEmployee InsGapAnalysis(string recordUid)
        {
            InsRecord insRecord = _dbContext.Get<InsRecord>(recordUid);
            InsCase insCase = _dbContext.Get<InsCase>(insRecord.CaseUid);
            //当前月有 历史数据没有的语句（入职的）
            string sql1 = string.Format($"select EmpUid from {insCase.TableName} where InsmentTimes={insCase.InsCount} and EmpUid NOT IN(select EmpUid from {INSCENTER} where  InsYM='{insRecord.InsYM}' and InsCaseUid='{insRecord.CaseUid}' and InsmentTimes={insRecord.InsCount})");
            //当前月没有 历史数据有的（离职的）
            string sql2 = string.Format($"select EmpUid from {INSCENTER} where  InsYM='{insRecord.InsYM}' and InsCaseUid='{insRecord.CaseUid}' and InsmentTimes={insRecord.InsCount} and EmpUid NOT IN(select EmpUid from {insCase.TableName} where InsmentTimes={insCase.InsCount})");
            var list1 = _dbContext.QueryOriSql(sql1);
            var list2 = _dbContext.QueryOriSql(sql2);
            GapEmployee emps = new GapEmployee();
            if (list1.Any())
            {
                emps.AddedList = _dbContext.QueryWhere<Employee>("Fid in @Fids", new DynamicParameters(new { Fids = list1.Select(l => l.EmpUid) }));
            }
            if (list2.Any())
            {
                emps.RemovedList = _dbContext.QueryWhere<Employee>("Fid in @Fids", new DynamicParameters(new { Fids = list2.Select(l => l.EmpUid) }));
            }
            return emps;
        }
    }
}
