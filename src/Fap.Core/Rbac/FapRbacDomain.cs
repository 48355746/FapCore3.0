using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.Platform.Domain;
using Fap.Core.Platform.License;
using Fap.Core.Rbac.AC;
using Fap.Core.Utility;
using Fap.Core.Rbac.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Fap.Core.Rbac
{
    [Serializable]
    public class FapRbacDomain : PlatformDomainBase
    {
        private FapOption _fapOption;
        private ISessionFactory _sessionFactory;

        public FapRbacDomain(IOptions<FapOption> option, ILoggerFactory loggerFactory, ISessionFactory sessionFactory, string product)
        {
            this.Product = product;
            _fapOption = option.Value;
            _logger = loggerFactory.CreateLogger<FapRbacDomain>();
            _sessionFactory = sessionFactory;
            //加载注册码信息
            string name = System.Net.Dns.GetHostName();
            LoadRegisterInfo();
            Construct();
        }
        //public override IButtonSet ButtonSet
        //{
        //    get;
        //    protected set;
        //}
        /// <summary>
        /// 所有表元数据
        /// </summary>
        public override ITableSet TableSet
        {
            get;
            protected set;
        }
        ///// <summary>
        ///// 所有系统列元数据
        ///// </summary>
        public override IColumnSet ColumnSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 多语集
        /// </summary>
        public override IMultiLang MultiLangSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有系统模块
        /// </summary>
        public override IModuleSet ModuleSet { get; protected set; }
        /// <summary>
        /// 所有系统菜单
        /// </summary>
        public override IMenuSet MenuSet
        {
            get;
            protected set;
        }

        public override IOrgDeptSet OrgDeptSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有系统参数
        /// </summary>
        public override ISysParamSet SysParamSet
        {
            get;
            protected set;
        }

        /// <summary>
        /// 所有系统用户
        /// </summary>
        public override ISysUserSet SysUserSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有角色
        /// </summary>
        public override IRoleSet RoleSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 所有编码
        /// </summary>
        public override IDictSet DictSet
        {
            get;
            protected set;
        }
        /// <summary>
        /// 角色列
        /// </summary>
        public override IRoleColumnSet RoleColumnSet { get; protected set; }
        /// <summary>
        /// 角色部门
        /// </summary>
        public override IRoleDeptSet RoleDeptSet { get; protected set; }
        /// <summary>
        /// 角色菜单
        /// </summary>
        public override IRoleMenuSet RoleMenuSet { get; protected set; }
        /// <summary>
        /// 角色报表
        /// </summary>
        public override IRoleReportSet RoleReportSet { get; protected set; }
        /// <summary>
        /// 角色数据
        /// </summary>
        public override IRoleDataSet RoleDataSet { get; protected set; }
        /// <summary>
        /// 角色角色
        /// </summary>
        public override IRoleRoleSet RoleRoleSet { get; protected set; }
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
                //if(_sessionFactory.DatabaseDialect== Infrastructure.Enums.DatabaseDialectEnum.MSSQL)
                //{
                //    //检查clr enabled
                //    using (var session = _sessionFactory.CreateSession())
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
                FapConfig config = null;
                using (var dbSessin = _sessionFactory.CreateSession())
                {
                    config = dbSessin.QueryFirstOrDefault<FapConfig>("select * from FapConfig where ParamKey = 'sys.web.name'");
                }
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

                string currentDateTime = PublicUtils.CurrentDateTimeStr;
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
        private void Construct()
        {
            //this.ButtonSet = new ButtonSet(this);
            _logger.LogInformation("初始化元数据");
            this.TableSet = new TableSet(this, _sessionFactory);
            this.ColumnSet = new ColumnSet(this, _sessionFactory);
            _logger.LogInformation("初始化元数据结束");

            this.MultiLangSet = new MultiLangSet(this, _sessionFactory);
            _logger.LogInformation("初始化多语结束");
            this.DictSet = new DictSet(this, _sessionFactory);
            _logger.LogInformation("初始化字典结束");
            this.ModuleSet = new ModuleSet(this,_sessionFactory);
            _logger.LogInformation("初始化模块结束");
            this.MenuSet = new MenuSet(this, _sessionFactory);
            _logger.LogInformation("初始化菜单结束");
            this.OrgDeptSet = new OrgDeptSet(this, _sessionFactory);
            _logger.LogInformation("初始化组织部门结束");
            this.RoleSet = new RoleSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色结束");
            this.SysParamSet = new SysParamSet(this, _sessionFactory);
            _logger.LogInformation("初始化系统设置结束");
            this.SysUserSet = new SysUserSet(this, _sessionFactory);
            _logger.LogInformation("初始化用户结束");
            this.RoleColumnSet = new RoleColumnSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色列结束");
            this.RoleDeptSet = new RoleDeptSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色部门结束");
            this.RoleMenuSet = new RoleMenuSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色菜单结束");
            this.RoleReportSet = new RoleReportSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色报表结束");
            this.RoleDataSet = new RoleDataSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色数据结束");
            this.RoleRoleSet = new RoleRoleSet(this, _sessionFactory);
            _logger.LogInformation("初始化角色角色结束");
        }
        /// <summary>
        /// 清空所有权限相关缓存
        /// </summary>
        public override void Refresh()
        {
            //this.ButtonSet.Refresh();
            this.ColumnSet.Refresh();
            this.MultiLangSet.Refresh();
            this.DictSet.Refresh();
            this.TableSet.Refresh();
            this.MenuSet.Refresh();
            this.ModuleSet.Refresh();
            this.OrgDeptSet.Refresh();
            this.RoleSet.Refresh();
            this.SysUserSet.Refresh();
            this.RoleColumnSet.Refresh();
            this.RoleDeptSet.Refresh();
            this.RoleMenuSet.Refresh();
            this.RoleReportSet.Refresh();
            this.RoleDataSet.Refresh();
            this.RoleRoleSet.Refresh();
            this.SysParamSet.Refresh();
        }
        public override void Configure()
        {
        }

    }
}
