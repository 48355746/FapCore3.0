using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fap.Core.Rbac
{
    [Service]
    public class RbacService : IRbacService
    {
        /// <summary>
        ///权限服务
        /// </summary>
        private readonly IDbContext _dbContext;
        private readonly IFapPlatformDomain _platformDomain;
        private readonly IFapApplicationContext _applicationContext;
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
        public IEnumerable<FapUserGroup> GetAllUserGroup()
        {
            return _dbContext.QueryAll<FapUserGroup>();
        }
        public long CreateUserGroup(FapUserGroup userGroup)
        {
            return _dbContext.Insert(userGroup);
        }
        public bool DeleteUserGroup(string fid)
        {
            int c = _dbContext.DeleteExec(nameof(FapUserGroup), "Fid=@Fid", new DynamicParameters(new { Fid = fid }));
            return c > 0 ? true : false;
        }
        public bool EditUserGroup(FapUserGroup userGroup)
        {
            return _dbContext.Update(userGroup);
        }
        public IEnumerable<FapRoleGroup> GetAllRoleGroup()
        {
            return _dbContext.QueryAll<FapRoleGroup>();
        }

        public long CreateRoleGroup(FapRoleGroup roleGroup)
        {
            return _dbContext.Insert(roleGroup);
        }

        public bool DeleteRoleGroup(string fid)
        {
            int c = _dbContext.DeleteExec(nameof(FapRoleGroup), "Fid=@Fid", new DynamicParameters(new { Fid = fid }));
            return c > 0 ? true : false;
        }

        public bool EditRoleGroup(FapRoleGroup roleGroup)
        {
            return _dbContext.Update(roleGroup);
        }
        public FapRole GetCurrentRole()
        {
            return GetAllRole().First(r => r.Fid == _applicationContext.CurrentRoleUid);
        }
        public IEnumerable<FapRole> GetAllRole()
        {
            return _platformDomain.RoleSet;
        }
        public IEnumerable<FapBizRole> GetAllBizRole()
        {
            return _dbContext.QueryAll<FapBizRole>();
        }

        public long CreateBizRole(FapBizRole bizRole)
        {
            return _dbContext.Insert(bizRole);
        }

        public bool DeleteBizRole(string fid)
        {
            int c = _dbContext.DeleteExec(nameof(FapBizRole), "Fid=@Fid", new DynamicParameters(new { Fid = fid }));
            return c > 0 ? true : false;
        }

        public bool EditBizRole(FapBizRole bizRole)
        {
            return _dbContext.Update(bizRole);
        }

        [Transactional]
        public bool AddRoleMenu(string roleUid, IEnumerable<FapRoleMenu> menus)
        {
            _dbContext.DeleteExec(nameof(FapRoleMenu), "RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (menus.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleMenu>(menus);
            }
            return true;
        }
        [Transactional]
        public bool AddRoleDept(string roleUid, IEnumerable<FapRoleDept> depts)
        {
            _dbContext.DeleteExec(nameof(FapRoleDept), "RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (depts.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleDept>(depts);
            }
            return true;
        }
        [Transactional]
        public bool AddRoleColumn(string roleUid, IEnumerable<FapRoleColumn> columns, int editType)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("RoleUid", roleUid);
            if (editType == 3)
            {
                _dbContext.DeleteExec(nameof(FapRoleColumn), " RoleUid=@RoleUid and EditAble=1", param);
            }
            else
            {
                _dbContext.DeleteExec(nameof(FapRoleColumn), " RoleUid=@RoleUid and ViewAble=1", param);
            }
            if (columns.Count() > 0)
            {
                var cids = columns.Select(c => "'" + c.ColumnUid + "'");
                if (cids != null)
                {
                    //删除包含权限的列，重新分配
                    _dbContext.DeleteExec(nameof(FapRoleColumn), "RoleUid=@RoleUid and ColumnUid in @ColumnUids", new DynamicParameters(new { RoleUid = roleUid, ColumnUids = cids }));
                }
                _dbContext.InsertBatch<FapRoleColumn>(columns);
            }
            return true;
        }

        public void AddRoleUser(IEnumerable<FapRoleUser> users)
        {
            _dbContext.InsertBatch<FapRoleUser>(users);
        }
        [Transactional]
        public void AddRoleReport(string roleUid, IEnumerable<FapRoleReport> rpts)
        {
            _dbContext.DeleteExec(nameof(FapRoleReport), "RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (rpts.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleReport>(rpts);
            }
        }
        [Transactional]
        public void AddRoleRole(string roleUid, IEnumerable<FapRoleRole> roleRoles)
        {
            _dbContext.DeleteExec(nameof(FapRoleRole), "RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            if (roleRoles.Count() > 0)
            {
                _dbContext.InsertBatch<FapRoleRole>(roleRoles);
            }
        }

        /// <summary>
        /// 用户角色拥有的部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrgDept> GetRoleDeptList(string roleUid, string historyDate = "")
        {
            if (!_platformDomain.RoleDeptSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleDept> roleDepts))
            {
                roleDepts = new List<FapRoleDept>();// Enumerable.Empty<OrgDept>();
            }
            var deptUids = roleDepts.Select(r => r.DeptUid).ToList();
            IEnumerable<OrgDept> allDepts;
            if (historyDate.IsPresent())
            {
                _dbContext.HistoryDateTime = historyDate;
                allDepts = _dbContext.QueryAll<OrgDept>();
            }
            else
            {
                allDepts = _platformDomain.OrgDeptSet;
            }
            //管辖部门，作为部门经理或直属领导
            var myDepts = _platformDomain.OrgDeptSet.Where(d => d.DeptManager == _applicationContext.EmpUid || d.Director == _applicationContext.EmpUid);
            if (myDepts.Any())
            {
                foreach (var mydept in myDepts)
                {
                    var myAllDepts = _platformDomain.OrgDeptSet.Where(d => d.DeptCode.StartsWith(mydept.DeptCode)).ToList();
                    myAllDepts.ForEach(dept =>
                    {
                        if (!deptUids.Contains(dept.Fid))
                        {
                            deptUids.Add(dept.Fid);
                        }
                    });
                }
            }
           return allDepts.Where(d => deptUids.Contains(d.Fid));
           
        }


        public IEnumerable<FapRoleData> GetRoleDataList(string roleUid)
        {
            if (_platformDomain.RoleDataSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleData> roleDatas))
            {
                return roleDatas;
            }
            return Enumerable.Empty<FapRoleData>();

        }

        /// <summary>
        /// 获取当前角色拥有的报表权限
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRoleReport> GetRoleReportList(string roleUid)
        {
            if (_platformDomain.RoleReportSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleReport> roleReports))
            {
                return roleReports;
            }
            return Enumerable.Empty<FapRoleReport>();
        }
        /// <summary>
        /// 用户当前角色所拥有的菜单
        /// </summary>
        public IEnumerable<FapRoleMenu> GetRoleMenuList(string roleUid)
        {
            if (_platformDomain.RoleMenuSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleMenu> roleMenuUids))
            {
                return roleMenuUids;
            }
            return Enumerable.Empty<FapRoleMenu>();
        }
        /// <summary>
        /// 获取用户当前角色拥有的列
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRoleColumn> GetRoleColumnList(string roleUid)
        {
            if (_platformDomain.RoleColumnSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleColumn> columns))
            {
                return columns;
            }
            return Enumerable.Empty<FapRoleColumn>();

        }
        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapRole> GetUserRoleList(string userUid)
        {
            if (_platformDomain.RoleUserSet.TryGetRoleValue(userUid, out IEnumerable<string> roleUids))
            {
                return _platformDomain.RoleSet.Where(r => roleUids.Contains(r.Fid));
            }
            return Enumerable.Empty<FapRole>();
        }
        public IEnumerable<FapRoleRole> GetRoleRoleList(string roleUid)
        {
            if (_platformDomain.RoleRoleSet.TryGetValueByRole(roleUid, out IEnumerable<FapRoleRole> roleRoles))
            {
                return roleRoles;
            }
            return Enumerable.Empty<FapRoleRole>();
        }
        public IEnumerable<FapRoleButton> GetRoleButtonList(string roleUid)
        {
            if(_platformDomain.RoleButtonSet.TryGetValue(roleUid,out IEnumerable<FapRoleButton> roleButtons))
            {
                return roleButtons;
            }
            return Enumerable.Empty<FapRoleButton>();
        }
        public string GetButtonAuthorized(FapMenuButton button)
        {
            bool isAdministrator = _applicationContext.IsAdministrator;
            var menu = GetCurrentMenu();
            if (menu != null)
            {
                if (_platformDomain.MenuButtonSet.TryGetValue(menu.Fid, out IEnumerable<FapMenuButton> list))
                {
                    if (list.Any() && list.ToList().Exists(m => m.ButtonID == button.ButtonID))
                    {
                        //检查授权
                        if (!isAdministrator && _platformDomain.RoleButtonSet.TryGetValue(_applicationContext.CurrentRoleUid, out IEnumerable<FapRoleButton> roleButtons))
                        {
                            return roleButtons.FirstOrDefault(b => b.ButtonId == button.ButtonID)?.ButtonValue;
                        }
                    }
                    else
                    {
                        //注册按钮
                        button.MenuUid = menu.Fid;
                        _dbContext.Insert(button);
                        _platformDomain.MenuButtonSet.Refresh();
                    }
                }
                if (isAdministrator)
                {
                    if (button.ButtonType == FapMenuButtonType.Grid)
                    {
                        return string.Join(',', typeof(OperEnum).EnumItems().Select(c => c.Key));
                    }
                    else if (button.ButtonType == FapMenuButtonType.Tree)
                    {
                        return "2,4,8,16";//增删改刷
                    }
                    else
                    {
                        return "1";
                    }
                }
            }
            return string.Empty;
        }


        [Transactional]
        public void AddRoleButton(string roleUid, IEnumerable<FapRoleButton> roleButtons)
        {
            _dbContext.DeleteExec(nameof(FapRoleButton), "RoleUid=@RoleUid", new DynamicParameters(new { RoleUid = roleUid }));
            _dbContext.InsertBatch(roleButtons);
        }

        public string GetColumnAuthorized(FapMenuColumn column)
        {
            bool isAdministrator = _applicationContext.IsAdministrator;
            var menu = GetCurrentMenu();
            if (menu != null)
            {
                if (_platformDomain.MenuColumnSet.TryGetValue(menu.Fid, out IEnumerable<FapMenuColumn> list))
                {
                    if (list.Any() && list.ToList().Exists(m => m.GridId == column.GridId))
                    {
                        //检查授权
                        if (!isAdministrator && _platformDomain.RoleColumnSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out IEnumerable<FapRoleColumn> roleColumns))
                        {
                            var cols= _platformDomain.ColumnSet.Where(c=> roleColumns.Where(r=>r.GridId==column.GridId).Select(r=>r.ColumnUid).Contains(c.Fid)).Select(c=>c.ColName);
                            if (cols.Any())
                            {
                                cols = cols.Union(BaseColumns());
                            }
                            return string.Join(',', cols);
                        }
                    }
                    else
                    {
                        //注册按钮
                        column.MenuUid = menu.Fid;
                        _dbContext.Insert(column);
                        _platformDomain.MenuColumnSet.Refresh();
                    }
                }
                if (isAdministrator)
                {
                    return column.GridColumn;
                }
            }
            return string.Empty;
            IEnumerable<string> BaseColumns()
            {
                yield return FapDbConstants.FAPCOLUMN_FIELD_Id;
                yield return FapDbConstants.FAPCOLUMN_FIELD_Fid;
                yield return FapDbConstants.FAPCOLUMN_FIELD_Ts;
            }
        }

        public FapMenu GetCurrentMenu()
        {
            string path = $"~{_applicationContext.Request.Path}";
            return _platformDomain.MenuSet.FirstOrDefault(m => m.MenuUrl.TrimEnd('/').Trim().EqualsWithIgnoreCase(path));
        }

      
    }
}
