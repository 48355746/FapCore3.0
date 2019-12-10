using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fap.Core.Rbac
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class RbacService : IRbacService
    {
        /// <summary>
        /// 这样写，RbacServcie就不暴露Commonservice的方法。RbacServcie只实现自己的内部方法
        /// </summary>
        private IDbContext _dbContext;
        private IFapPlatformDomain _platformDomain;
        private IFapApplicationContext _applicationContext;
        public RbacService(IDbContext dbContext, IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _dbContext = dbContext;
            _platformDomain = platformDomain;
            _applicationContext = applicationContext;
        }
        /// <summary>
        /// 用户组
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapUserGroup> GetUserGroups()
        {
            return _dbContext.QueryAll<FapUserGroup>();
        }
        public string UserGroupOperation(string operation, string id, string parent, string text)
        {
            string result = "0";
            if (operation == TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.Count("FapUser", "UserGroupUid=@GroupUid", new DynamicParameters(new { GroupUid = id }));
                if (c == 0)
                {
                    result = "" + _dbContext.Execute("delete from FapUserGroup where Fid=@Id", new DynamicParameters(new { Id = id }));
                }
            }
            else if (operation == TreeNodeOper.CREATE_NODE)
            {
                dynamic fdo = new FapDynamicObject("FapUserGroup");
                fdo.Pid = id;
                fdo.UserGroupName = text;
                long rv = _dbContext.InsertDynamicData(fdo);
                result = fdo.Fid;
            }
            else if (operation == TreeNodeOper.RENAME_NODE)
            {
                result = "" + _dbContext.Execute("update FapUserGroup set UserGroupName=@UserGroupName where Fid=@Id", new DynamicParameters(new { UserGroupName = text, Id = id }));
            }
            else if (operation == "move_node")
            {
                result = "" + _dbContext.Execute("update FapUserGroup set Pid=@Pid where Fid=@Id", new DynamicParameters(new { Pid = parent, Id = id }));
            }
            else if (operation == TreeNodeOper.COPY_NODE)
            {

            }

            return result;
        }
        public FapRole GetCurrentRole()
        {
            return _dbContext.QueryFirstOrDefault<FapRole>("select * from FapRole where Fid=@Fid", new DynamicParameters(new { Fid = _applicationContext.CurrentRoleUid }));
        }
        public string RoleGroupOperation(string operation, string id, string parent, string text)
        {
            string result = "0";
            if (operation ==TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.Count("FapRole", "RoleGroupUid=@RoleGroupUid", new DynamicParameters(new { RoleGroupUid = id }));
                if (c == 0)
                {
                    result = "" + _dbContext.Execute("delete from FapRoleGroup where Fid=@Id", new DynamicParameters(new { Id = id }));
                }
            }
            else if (operation ==TreeNodeOper.CREATE_NODE)
            {
                dynamic fdo = new FapDynamicObject("FapRoleGroup");
                fdo.Pid = id;
                fdo.RoleGroupName = text;
                long rv = _dbContext.InsertDynamicData(fdo);
                result = fdo.Fid;
            }
            else if (operation ==TreeNodeOper.RENAME_NODE)
            {
                result = "" + _dbContext.Execute("update FapRoleGroup set RoleGroupName=@RoleGroupName where Fid=@Id", new DynamicParameters(new { RoleGroupName = text, Id = id }));
            }
            else if (operation == "move_node")
            {
                _dbContext.Execute("update FapRoleGroup set Pid=@Pid where Fid=@Id", new DynamicParameters(new { Pid = parent, Id = id }));
            }
            else if (operation ==TreeNodeOper.COPY_NODE)
            {

            }

            return result;
        }

        public string BusinessRoleOperation(string operation, string id, string parent, string text)
        {
            string result = "0";
            if (operation ==TreeNodeOper.DELETE_NODE)
            {
                result = "" + _dbContext.Execute("delete from FapBizRole where Fid=@Id", new DynamicParameters(new { Id = id }));
            }
            else if (operation ==TreeNodeOper.CREATE_NODE)
            {
                dynamic fdo = new FapDynamicObject("FapBizRole");
                fdo.Pid = id;
                fdo.BizRoleName = text;
                long rv = _dbContext.InsertDynamicData(fdo);
                result = fdo.Fid;
            }
            else if (operation == TreeNodeOper.RENAME_NODE)
            {
                result = "" + _dbContext.Execute("update FapBizRole set BizRoleName=@BizRoleName where Fid=@Id", new DynamicParameters(new { BizRoleName = text, Id = id }));
            }
            else if (operation ==TreeNodeOper.MOVE_NODE)
            {
                result = "" + _dbContext.Execute("update FapBizRole set Pid=@Pid where Fid=@Id", new DynamicParameters(new { Pid = parent, Id = id }));
            }
            else if (operation ==TreeNodeOper.COPY_NODE)
            {

            }

            return result;
        }
        [Transactional]
        public bool AddRoleMenu(string roleUid,IEnumerable<FapRoleMenu> menus)
        {
            _dbContext.Execute("delete from FapRoleMenu where RoleUid=@RoleUid", new DynamicParameters(new { RoleUid =roleUid}));
            if (menus.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleMenu>(menus);
               
            }
            return true;
        }
        [Transactional]
        public bool AddRoleDept(string roleUid,IEnumerable<FapRoleDept> depts)
        {
            _dbContext.Execute("delete from FapRoleDept where RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (depts.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleDept>(depts);
            }
            return true;
        }
        [Transactional]
        public bool AddRoleColumn(string roleUid,IEnumerable<FapRoleColumn> columns,int editType)
        {
            if (columns.Count() > 0)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("RoleUid", roleUid);
                if (editType == 3)
                {
                    string sql = "delete from FapRoleColumn where RoleUid=@RoleUid and EditAble=1";
                    _dbContext.Execute(sql, new DynamicParameters(new { RoleUid = roleUid }));
                }
                else
                {
                    string sql = "delete from FapRoleColumn where RoleUid=@RoleUid and ViewAble=1";
                    _dbContext.Execute(sql, new DynamicParameters(new { RoleUid = roleUid }));
                }
                var cids = columns.Select(c => "'" + c.ColumnUid + "'");
                if (cids != null)
                {
                    //删除包含权限的列，重新分配
                    string delSql = "delete from FapRoleColumn where RoleUid=@RoleUid and ColumnUid in @ColumnUids";
                    _dbContext.Execute(delSql, new DynamicParameters(new { RoleUid = roleUid , ColumnUids =cids}));
                }
                _dbContext.InsertBatch<FapRoleColumn>(columns);
            }
            else
            {
                if (editType == 3)
                {
                    string sql = "delete from FapRoleColumn where RoleUid=@RoleUid and EditAble=1";
                    _dbContext.Execute(sql, new DynamicParameters(new { RoleUid = roleUid }));
                }
                else
                {
                    string sql = "delete from FapRoleColumn where RoleUid=@RoleUid and ViewAble=1";
                    _dbContext.Execute(sql, new DynamicParameters(new { RoleUid = roleUid }));
                }
            }
            return true;
        }

        public void AddRoleUser(IEnumerable<FapRoleUser> users)
        {
            _dbContext.InsertBatch<FapRoleUser>(users);
        }
        [Transactional]
        public void AddRoleReport(string roleUid,IEnumerable<FapRoleReport> rpts)
        {
            _dbContext.Execute("delete from FapRoleReport where RoleUid=@RoleUid", new DynamicParameters(new{ RoleUid = roleUid }));
            if (rpts.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleReport>(rpts);
            }
        }
        [Transactional]
        public void AddRoleRole(string roleUid,IEnumerable<FapRoleRole> roleRoles)
        {
            _dbContext.Execute("delete from FapRoleRole where RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (roleRoles.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleRole>(roleRoles);           
            }
        }
        /// <summary>
        /// 判断属于某个角色
        /// </summary>
        /// <param name="roleFid"></param>
        /// <returns></returns>
        public bool IsInRole(string roleFid)
        {
            if (GetUserRoleList().FirstOrDefault(r => r.Fid == roleFid) != null)
                return true;
            return false;
        }
        /// <summary>
        /// 用户角色拥有的部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrgDept> GetUserDeptList(string historyDate = "")
        {
            //if (_session.IsDeveloper)
            //{
            //    return _platformDomain.OrgDeptSet.ToList<OrgDept>();
            //}
            //IEnumerable<string> roles =_loginService.GetUserRoles(_session.UserUid).Select(r => r.Fid);
            IEnumerable<OrgDept> roleOrgDepts = new List<OrgDept>();
            string roleId = _applicationContext.CurrentRoleUid;
            if (historyDate.IsPresent())
            {
                IEnumerable<OrgDept> roleDepts = null;
                List<OrgDept> powerDepts = new List<OrgDept>();
                var result = _platformDomain.RoleDeptSet.Where<FapRoleDept>(f => f.RoleUid == roleId);
                IEnumerable<OrgDept> allDepts = null;

                _dbContext.HistoryDateTime = historyDate;
                allDepts = _dbContext.QueryAll<OrgDept>();

                if (allDepts.Any())
                {
                    OrgDept rootDept = allDepts.FirstOrDefault(d => string.IsNullOrWhiteSpace(d.Pid) || d.Pid == "#" || d.Pid == "~" || d.Pid == "");
                    if (result != null && result.Any())
                    {
                        List<FapRoleDept> rds = result.ToList();
                        if (rds.Exists(r => r.DeptUid == rootDept.Fid))
                        {
                            roleDepts = allDepts;
                        }
                        else
                        {
                            rootDept.HasPartPower = true;
                            powerDepts.Add(rootDept);
                            foreach (var rd in result)
                            {
                                OrgDept tempDept = allDepts.FirstOrDefault<OrgDept>(d => d.Fid == rd.DeptUid);
                                if (tempDept != null)
                                {
                                    powerDepts.Add(tempDept);
                                    AddParentOrgDept(rds, allDepts, powerDepts, tempDept);
                                }
                            }
                            roleDepts = powerDepts;
                        }

                    }
                }


                if (roleDepts != null && roleDepts.Any())
                {
                    roleOrgDepts = roleOrgDepts.Concat(roleDepts);
                }

            }
            else
            {
                IEnumerable<OrgDept> depts = new List<OrgDept>();
                if (_platformDomain.RoleDeptSet.TryGetValueByRole(roleId, out depts))
                {
                    if (depts != null && depts.Any())
                    {
                        roleOrgDepts = roleOrgDepts.Concat(depts);
                    }
                }
                //管辖部门，作为部门经理或直属领导
                var myDepts = _platformDomain.OrgDeptSet.Where(d => d.DeptManager == _applicationContext.EmpUid || d.Director == _applicationContext.EmpUid);
                if (myDepts.Any())
                {
                    foreach (var mydept in myDepts)
                    {
                        var myAllDepts = _platformDomain.OrgDeptSet.Where(d => d.DeptCode.StartsWith(mydept.DeptCode)).ToList();
                        roleOrgDepts = roleOrgDepts.Union(myAllDepts);
                    }
                }
            }
            if (roleOrgDepts != null && roleOrgDepts.Any())
            {
                return roleOrgDepts.Distinct().OrderBy(d => d.DeptOrder);
            }
            else
            {
                return roleOrgDepts;
            }
        }
        private void AddParentOrgDept(List<FapRoleDept> rds, IEnumerable<OrgDept> allDepts, List<OrgDept> powerDepts, OrgDept tempDept)
        {
            if (tempDept != null && !(string.IsNullOrWhiteSpace(tempDept.Pid) || tempDept.Pid == "#" || tempDept.Pid == "~" || tempDept.Pid == ""))
            {
                var tempDeptParent = allDepts.FirstOrDefault<OrgDept>(d => d.Fid == tempDept.Pid);
                //存在父部门
                if (tempDeptParent != null)
                {
                    //父部门没在权限中,且还没包含进去
                    if (!rds.Exists(r => r.DeptUid == tempDeptParent.Fid) && !powerDepts.Contains(tempDeptParent))
                    {
                        tempDeptParent.HasPartPower = true;
                        powerDepts.Add(tempDeptParent);
                    }

                    AddParentOrgDept(rds, allDepts, powerDepts, tempDeptParent);
                }
            }
        }
        /// <summary>
        /// 获取部门的权限where条件
        /// </summary>
        /// <param name="hasPartPower">是否包含部分权限的部门，默认不包含</param>
        /// <returns></returns>
        public string GetUserDeptAuthorityWhere(bool hasPartPower = false)
        {
            IEnumerable<OrgDept> depts = GetUserDeptList();
            if (depts != null && depts.Any())
            {
                IEnumerable<string> deptWhere = null;
                if (hasPartPower)
                {
                    deptWhere = depts.Select(d => "'" + d.Fid + "'");
                }
                else
                {
                    deptWhere = depts.Where(d => d.HasPartPower == false).Select(d => "'" + d.Fid + "'");
                }
                if (deptWhere != null && deptWhere.Any())
                {
                    return string.Join(",", deptWhere);
                }
                else
                {
                    return "'meiyou'";
                }
            }
            else
            {
                return "'meiyou'";
            }
        }
        /// <summary>
        /// 获取数据权限
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetRoleDataWhere(string tableName)
        {
            string where = string.Empty;
            string roleUid = _applicationContext.CurrentRoleUid;
            IEnumerable<FapRoleData> roleDatas = null;
            if (_platformDomain.RoleDataSet.TryGetValueByRole(roleUid, out roleDatas))
            {
                if (roleDatas != null && roleDatas.Any())
                {
                    var rd = roleDatas.FirstOrDefault<FapRoleData>(r => r.TableUid == tableName);
                    if (rd != null)
                    {
                        where = rd.SqlCondition;
                        string pattern = FapPlatformConstants.VariablePattern;
                        Regex reg = new Regex(pattern);
                        MatchCollection matchs = reg.Matches(where);
                        foreach (var mtch in matchs)
                        {

                            int length = mtch.ToString().Length - 3;
                            string colName = mtch.ToString().Substring(2, length);
                            if (colName.EqualsWithIgnoreCase("DeptUid"))
                            {
                                where = where.Replace(mtch.ToString(),_applicationContext.DeptUid);
                            }
                            else if (colName.EqualsWithIgnoreCase("EmpUid"))
                            {
                                where = where.Replace(mtch.ToString(), _applicationContext.EmpUid);
                            }
                            else if (colName.EqualsWithIgnoreCase("DeptCode"))
                            {
                                string deptCode = _applicationContext.DeptCode;
                                if (deptCode.IsMissing())
                                {
                                    OrgDept dept = _dbContext.Get<OrgDept>(_applicationContext.DeptUid);
                                    deptCode = dept.DeptCode;
                                }
                                where = where.Replace(mtch.ToString(), deptCode);
                            }
                        }
                    }
                }

            }
            return where;
        }

        /// <summary>
        /// 获取当前角色拥有的报表权限
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRoleReport> GetUserReportList()
        {
            IEnumerable<FapRoleReport> roleReports = new List<FapRoleReport>();
            if (_platformDomain.RoleReportSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out roleReports))
            {
                return roleReports;
            }
            else
            {
                return new List<FapRoleReport>();
            }
        }
        /// <summary>
        /// 用户当前角色所拥有的菜单
        /// </summary>
        public IEnumerable<FapRoleMenu> GetUserMenuList()
        {
            IEnumerable<FapRoleMenu> roleMenuUids = new List<FapRoleMenu>();
            if (_platformDomain.RoleMenuSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out roleMenuUids))
            {
                return roleMenuUids;
            }
            else
            {
                return new List<FapRoleMenu>();

            }
        }
        /// <summary>
        /// 获取用户当前角色拥有的列
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRoleColumn> GetUserColumnList()
        {
            IEnumerable<FapRoleColumn> columns = new List<FapRoleColumn>();
            if (_platformDomain.RoleColumnSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out columns))
            {
                return columns;
            }
            else
            {
                return new List<FapRoleColumn>();
            }

        }
        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRole> GetUserRoleList()
        {
            string sql = "select * from FapRole where Fid in(select RoleUid  from FapRoleUser where UserUid=@UserUid)";
            DynamicParameters param = new DynamicParameters();
            param.Add("UserUid",_applicationContext.UserUid);
            var list = _dbContext.Query<FapRole>(sql, param).AsList();
            if (list == null)
            {
                list = new List<FapRole>();
            }

            list.Insert(0, new FapRole { Id = -1, Fid = FapPlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" });
            return list;
        }
    }
}
