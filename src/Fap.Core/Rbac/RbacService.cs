using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
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
        private IDbContext _dataAccessor;
        private IFapPlatformDomain _appDomain;
        private IFapApplicationContext _applicationContext;
        private ILoginService _loginService;
        public RbacService(IDbContext db, IFapPlatformDomain appDomain, IFapApplicationContext applicationContext, ILoginService loginService)
        {
            _dataAccessor = db;
            _appDomain = appDomain;
            _applicationContext = applicationContext;
            _loginService = loginService;
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
            //    return _appDomain.OrgDeptSet.ToList<OrgDept>();
            //}
            //IEnumerable<string> roles =_loginService.GetUserRoles(_session.UserUid).Select(r => r.Fid);
            IEnumerable<OrgDept> roleOrgDepts = new List<OrgDept>();
            string roleId = _applicationContext.CurrentRoleUid;
            if (historyDate.IsPresent())
            {
                IEnumerable<OrgDept> roleDepts = null;
                List<OrgDept> powerDepts = new List<OrgDept>();
                var result = _appDomain.RoleDeptSet.Where<FapRoleDept>(f => f.RoleUid == roleId);
                IEnumerable<OrgDept> allDepts = null;

                _dataAccessor.HistoryDateTime = historyDate;
                allDepts = _dataAccessor.QueryAll<OrgDept>();

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
                if (_appDomain.RoleDeptSet.TryGetValueByRole(roleId, out depts))
                {
                    if (depts != null && depts.Any())
                    {
                        roleOrgDepts = roleOrgDepts.Concat(depts);
                    }
                }
                //管辖部门，作为部门经理或直属领导
                var myDepts = _appDomain.OrgDeptSet.Where(d => d.DeptManager == _applicationContext.EmpUid || d.Director == _applicationContext.EmpUid);
                if (myDepts.Any())
                {
                    foreach (var mydept in myDepts)
                    {
                        var myAllDepts = _appDomain.OrgDeptSet.Where(d => d.DeptCode.StartsWith(mydept.DeptCode)).ToList();
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
            if (_appDomain.RoleDataSet.TryGetValueByRole(roleUid, out roleDatas))
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
                                    OrgDept dept = _dataAccessor.Get<OrgDept>(_applicationContext.DeptUid);
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
            if (_appDomain.RoleReportSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out roleReports))
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
            if (_appDomain.RoleMenuSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out roleMenuUids))
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
            if (_appDomain.RoleColumnSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out columns))
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
            var list = _dataAccessor.Query<FapRole>(sql, param).AsList();
            if (list == null)
            {
                list = new List<FapRole>();
            }

            list.Insert(0, new FapRole { Id = -1, Fid = FapPlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" });
            return list;
        }
    }
}
