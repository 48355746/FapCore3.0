using Fap.Core.Utility;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.Tracker;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.Infrastructure.Interceptor
{
    /// <summary>
    /// FapUser元数据的数据拦截器
    /// </summary>
    public class FapUserDataInterceptor : DataInterceptorBase
    {
        PasswordHasher passwordHasher = new PasswordHasher();
        private static string TableName = "FapUser";

        private ILogger _logger;

        public FapUserDataInterceptor(IServiceProvider provider, IDbContext dataAccessor) : base(provider, dataAccessor)
        {
            _logger = _loggerFactory.CreateLogger<FapUserDataInterceptor>();
        }

        #region 动态对象

        /// <summary>
        /// 更新动态对象前
        /// </summary>
        public override void BeforeDynamicObjectUpdate(FapDynamicObject dynamicData)
        {
            if (dynamicData == null) { return; }

            if (dynamicData.TableName == TableName)
            {
                if (dynamicData.ContainsKey("UserPassword"))
                {
                    string orginPassword = dynamicData.Get("UserPassword").ToString();
                    if (!string.IsNullOrEmpty(orginPassword))// && orginPassword.Length < 20)
                    {
                        dynamicData.SetValue("UserPassword", passwordHasher.HashPassword(orginPassword));
                    }
                    else
                    {
                        //密码为空 将保持密码不变
                        dynamicData.Remove("UserPassword",out _);
                    }
                }
            }

        }

        /// <summary>
        /// 更新动态对象后
        /// </summary>
        public override void AfterDynamicObjectUpdate(FapDynamicObject dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, DataChangeTypeEnum.UPDATE);

        }

        /// <summary>
        /// 新增动态对象前
        /// </summary>
        public override void BeforeDynamicObjectInsert(FapDynamicObject dynamicData)
        {
            if (dynamicData.TableName == TableName)
            {
                if (dynamicData.ContainsKey("UserPassword"))
                {
                    string orginPassword = dynamicData.Get("UserPassword").ToString();
                    if (orginPassword.IsPresent())
                    {
                        dynamicData.SetValue("UserPassword", passwordHasher.HashPassword(orginPassword));
                    }
                    else
                    {
                        dynamicData.SetValue("UserPassword", passwordHasher.HashPassword("1"));
                    }
                }
                else
                {
                    //配置默认密码
                    string password = _provider.GetService<IFapConfigService>().GetSysParamValue("employee.user.password");
                    if (password.IsMissing())
                    {
                        password = "1";
                    }
                    PasswordHasher pwdHasher = new PasswordHasher();
                    password = pwdHasher.HashPassword(password);
                    dynamicData.SetValue("UserPassword", password);
                }
            }
        }

        /// <summary>
        /// 新增动态对象后
        /// </summary>
        public override void AfterDynamicObjectInsert(FapDynamicObject dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, DataChangeTypeEnum.ADD);
        }

        /// <summary>
        /// 新增动态对象后
        /// </summary>
        public override void AfterDynamicObjectDelete(FapDynamicObject dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, DataChangeTypeEnum.DELETE);
        }

        private void DataSynchDynamicObject(FapDynamicObject user, DataChangeTypeEnum oper)
        {
            if (user.ContainsKey("UserPassword"))
            {
                user.Remove("UserPassword", out _);
            }
            EventDataTracker tracker = _provider.GetService<EventDataTracker>();
            if (tracker != null)
            {
                EventData data = new EventData();
                data.ChangeDataType = oper.ToString();
                data.EntityName = TableName;
                List<dynamic> users = new List<dynamic>();
                users.Add(user);
                data.ChangeData = users;
                tracker.TrackEventData(data);
            }
        }


        #endregion 动态对象

        #region 实体对象
        /// <summary>
        /// 更新实体对象前
        /// </summary>
        public override void BeforeEntityUpdate(object entity)
        {
            if (entity != null && entity is FapUser)
            {
                FapUser user = (FapUser)entity;
                string orginPassword = user.UserPassword;
                if (orginPassword.IsPresent() && orginPassword.Length < 80)
                {
                    user.UserPassword = passwordHasher.HashPassword(orginPassword);
                }
            }
        }


        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override void AfterEntityUpdate(object entity)
        {
            this.DataSynchEntity(entity, DataChangeTypeEnum.UPDATE);
        }

        /// <summary>
        /// 新增实体对象前
        /// </summary>
        public override void BeforeEntityInsert(object entity)
        {
            if (entity != null && entity is FapUser)
            {
                FapUser user = (FapUser)entity;
                string orginPassword = user.UserPassword;
                if (orginPassword.IsPresent())
                {
                    user.UserPassword = passwordHasher.HashPassword(orginPassword);
                }
                else
                {
                    //配置默认密码
                    string password = _provider.GetService<IFapConfigService>().GetSysParamValue("employee.user.password");
                    if (password.IsMissing())
                    {
                        password = "1";
                    }
                    PasswordHasher pwdHasher = new PasswordHasher();
                    password = pwdHasher.HashPassword(password);
                    user.UserPassword = password;
                }
            }
        }

        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override void AfterEntityInsert(object entity)
        {
            this.DataSynchEntity(entity, DataChangeTypeEnum.ADD);
        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public override void AfterEntityDelete(object entity)
        {
            this.DataSynchEntity(entity, DataChangeTypeEnum.DELETE);
        }


        private void DataSynchEntity(object entity, DataChangeTypeEnum oper)
        {
            EventDataTracker tracker = _provider.GetService<EventDataTracker>();
            if (tracker != null)
            {
                EventData data = new EventData();
                data.ChangeDataType = oper.ToString();
                data.EntityName = TableName;
                if (entity is FapUser)
                {
                    List<FapUser> users = new List<FapUser>();
                    users.Add(entity as FapUser);
                    data.ChangeData = users;
                }
                else if (entity is List<FapUser>)
                {
                    data.ChangeData = entity as List<FapUser>;
                }
                tracker.TrackEventData(data);
            }
        }

        //public override void BeforeDynamicObjectDelete(FapDynamicObject dynamicData)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void BeforeEntityDelete(object entity)
        //{
        //    throw new System.NotImplementedException();
        //}

        #endregion

    }
}
