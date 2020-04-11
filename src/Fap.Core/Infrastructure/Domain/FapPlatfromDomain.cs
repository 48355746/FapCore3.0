using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.License;
using Fap.Core.Rbac.AC;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Domain
{
    public class FapPlatfromDomain:IFapPlatformDomain
    {
        public string Product { get; set; } = "HCM";
        private IDbSession _dbSession;
        private readonly ILogger<FapPlatfromDomain> _logger;
        public FapPlatfromDomain(ILogger<FapPlatfromDomain> logger, IDbSession dbSession)
        {
            _logger = logger;
            _dbSession = dbSession;
            InitPlatformDomain();
        }
        /// <summary>
        /// 产品注册码信息
        /// </summary>
        public RegisterInfo ServiceRegisterInfo
        {
            get;
            internal set;
        }
        /// <summary>
        /// 菜单按钮
        /// </summary>
        public IMenuButtonSet MenuButtonSet
        {
            get;
            protected set;
        }
        public IMenuColumnSet MenuColumnSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有表元数据
        /// </summary>
        public  ITableSet TableSet
        {
            get;
            protected set;
        }
        ///// <summary>
        ///// 所有系统列元数据
        ///// </summary>
        public  IColumnSet ColumnSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 多语集
        /// </summary>
        public  IMultiLanguage MultiLangSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有系统模块
        /// </summary>
        public  IModuleSet ModuleSet { get; protected set; }
        /// <summary>
        /// 所有系统菜单
        /// </summary>
        public  IMenuSet MenuSet
        {
            get;
            protected set;
        }

        public  IOrgDeptSet OrgDeptSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有系统参数
        /// </summary>
        public  IParamSet ParamSet
        {
            get;
            protected set;
        }

        /// <summary>
        /// 所有系统用户
        /// </summary>
        public  IUserSet UserSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有角色
        /// </summary>
        public  IRoleSet RoleSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有编码
        /// </summary>
        public  IDictSet DictSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 角色列
        /// </summary>
        public  IRoleColumnSet RoleColumnSet { get; protected set; }
        /// <summary>
        /// 角色部门
        /// </summary>
        public  IRoleDeptSet RoleDeptSet { get; protected set; }
        /// <summary>
        /// 角色菜单
        /// </summary>
        public  IRoleMenuSet RoleMenuSet { get; protected set; }
        /// <summary>
        /// 角色报表
        /// </summary>
        public  IRoleReportSet RoleReportSet { get; protected set; }
        /// <summary>
        /// 角色数据
        /// </summary>
        public  IRoleDataSet RoleDataSet { get; protected set; }
        /// <summary>
        /// 角色按钮
        /// </summary>
        public IRoleButtonSet RoleButtonSet { get; protected set; }
        /// <summary>
        /// 角色角色
        /// </summary>
        public  IRoleRoleSet RoleRoleSet { get; protected set; }
        /// <summary>
        /// 角色用户
        /// </summary>
        public IRoleUserSet RoleUserSet { get; protected set; }
        /// <summary>
        /// 单据编码配置规则集
        /// </summary>
        public ICfgBillCodeRuleSet CfgBillCodeRuleSet { get; protected set; }
        private object obj = new object();
        /// <summary>
        /// 重置服务注册状态
        /// </summary>
        private void LoadRegisterInfo()
        {
            lock (obj)
            {
                if (ServiceRegisterInfo != null)
                {
                    return;
                }
                //以下代码已经在docker发布脚本中执行，详见publish.sh文件
                //if(_dbSession.DatabaseDialect== Infrastructure.Enums.DatabaseDialectEnum.MSSQL)
                //{
                //    //检查clr enabled
                //    using (var session = _dbSession.CreateSession())
                //    {
                //       int enabled= session.ExecuteScalar<int>("select value from sys.configurations WHERE name = 'clr enabled'");
                //        if(enabled==0)
                //        {
                //            //设置clr enabled为1
                //            string sql = @"exec sp_configure 'show advanced options', '1';reconfigure;exec sp_configure 'clr enabled', '1'; reconfigure;exec sp_configure 'show advanced options', '1';";
                //            //数据库设置可信，OWNER设置成SA
                //            sql += $"ALTER DATABASE {session.Connection.Database} SET TRUSTWORTHY on;exec sp_changedbowner 'sa'";
                //            session.Execute(sql);
                //        }
                //    }
                //}
                FapConfig config =  _dbSession.QueryFirstOrDefault<FapConfig>("select * from FapConfig where ParamKey = 'sys.web.name'");
                //项目名称
                string projectName = string.Empty;
                if (config != null)
                {
                    projectName = config.ParamValue;
                }
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    ServiceRegisterInfo = new RegisterInfo();
                    ServiceRegisterInfo.RegisterState = EnumRegisterState.UnRegister;
                    ServiceRegisterInfo.RegisterMessage = "当前项目名称为空，未注册";
                    return;
                }

                string currentDateTime =DateTimeUtils.CurrentDateTimeStr;
                RegFileData regFileData = TGljZW5zZQTool.GetTGljZW5zZQDataFromReg();
                string TGljZW5zZQ = regFileData.TGljZW5zZQ;
                string trialExpire = regFileData.TrialExpire;
                if (!string.IsNullOrWhiteSpace(TGljZW5zZQ))
                {
                    string errMsg = "";
                    TGljZW5zZQInfo licenseInfo = TGljZW5zZQInfo.Parse(TGljZW5zZQ, out errMsg);
                    if (licenseInfo == null)
                    {
                        ServiceRegisterInfo = new RegisterInfo();
                        ServiceRegisterInfo.RegisterState = EnumRegisterState.UnRegister;
                        ServiceRegisterInfo.RegisterMessage = "当前项目[" + projectName + "]未注册";
                    }
                    else
                    {
                        ServiceRegisterInfo = licenseInfo.CheckLicense(projectName);
                    }
                }
                else //没有注册，则为未注册版
                {
                    ServiceRegisterInfo = new RegisterInfo();
                    ServiceRegisterInfo.RegisterState = EnumRegisterState.UnRegister;
                    ServiceRegisterInfo.RegisterMessage = "当前项目[" + projectName + "]未注册";
                }
            }
        }
        public void InitPlatformDomain()
        {
            LoadRegisterInfo();
            //this.ButtonSet = new ButtonSet(this);
            _logger.LogInformation("初始化元数据");
            this.TableSet = new TableSet( _dbSession);
            this.ColumnSet = new ColumnSet( _dbSession);
            _logger.LogInformation("初始化元数据完成");
            this.MenuButtonSet = new MenuButtonSet(_dbSession);
            _logger.LogInformation("初始化菜单按钮完成");
            this.MultiLangSet = new MultiLangSet( _dbSession);
            _logger.LogInformation("初始化多语完成");
            this.DictSet = new DictSet( _dbSession);
            _logger.LogInformation("初始化字典完成");
            this.ModuleSet = new ModuleSet( _dbSession,this);
            _logger.LogInformation("初始化模块完成");
            this.MenuSet = new MenuSet( _dbSession);
            _logger.LogInformation("初始化菜单完成");
            this.OrgDeptSet = new OrgDeptSet(_dbSession);
            _logger.LogInformation("初始化组织部门完成");
            this.RoleSet = new RoleSet( _dbSession);
            _logger.LogInformation("初始化角色完成");
            this.ParamSet = new ParamSet( _dbSession);
            _logger.LogInformation("初始化系统设置完成");
            this.UserSet = new UserSet( _dbSession);
            _logger.LogInformation("初始化用户完成");
            this.RoleColumnSet = new RoleColumnSet(_dbSession);
            _logger.LogInformation("初始化角色列完成");
            this.RoleDeptSet = new RoleDeptSet( _dbSession,this);
            _logger.LogInformation("初始化角色部门完成");
            this.RoleMenuSet = new RoleMenuSet( _dbSession);
            _logger.LogInformation("初始化角色菜单完成");
            this.RoleReportSet = new RoleReportSet( _dbSession);
            _logger.LogInformation("初始化角色报表完成");
            this.RoleDataSet = new RoleDataSet( _dbSession);
            _logger.LogInformation("初始化角色数据完成");
            this.RoleButtonSet = new RoleButtonSet(_dbSession);
            _logger.LogInformation("初始化角色按钮完成");
            this.RoleRoleSet = new RoleRoleSet( _dbSession);
            _logger.LogInformation("初始化角色角色完成");
            this.RoleUserSet = new RoleUserSet(_dbSession);
            _logger.LogInformation("初始化角色用户完成");
            this.MenuColumnSet = new MenuColumnSet(_dbSession);
            _logger.LogInformation("初始化菜单列完毕");
            this.CfgBillCodeRuleSet = new CfgBillCodeRuleSet(_dbSession);
            _logger.LogInformation("初始化单据编码规则集完成");
        }
        /// <summary>
        /// 清空所有权限相关缓存
        /// </summary>
        public  void Refresh()
        {
            this.MenuButtonSet.Refresh();
            this.MenuColumnSet.Refresh();
            this.ColumnSet.Refresh();
            this.MultiLangSet.Refresh();
            this.DictSet.Refresh();
            this.TableSet.Refresh();
            this.MenuSet.Refresh();
            this.ModuleSet.Refresh();
            this.OrgDeptSet.Refresh();
            this.RoleSet.Refresh();
            this.UserSet.Refresh();
            this.RoleButtonSet.Refresh();
            this.RoleColumnSet.Refresh();
            this.RoleDeptSet.Refresh();
            this.RoleMenuSet.Refresh();
            this.RoleReportSet.Refresh();
            this.RoleDataSet.Refresh();
            this.RoleRoleSet.Refresh();
            this.RoleUserSet.Refresh();
            this.ParamSet.Refresh();
            this.CfgBillCodeRuleSet.Refresh();
        }
        public  void Configure()
        {
        }
    }
}
