using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Thirdparty;
using Fap.Core.Utility;
using Fap.Model;
using Fap.Model.Infrastructure;
using System.Collections.Generic;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Rbac;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.Events;
using System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Model.MetaData;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    /// <summary>
    /// FapUser元数据的数据拦截器
    /// </summary>
    public class FapUserDataInterceptor : DataInterceptorBase
    {
        PasswordHasher passwordHasher = new PasswordHasher();
        private static string TableName = "FapUser";

        private ILogger _logger;
        private readonly IEventBus _eventBus;

        public FapUserDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _eventBus = provider.GetService<IEventBus>();
            _logger = _loggerFactory.CreateLogger<FapUserDataInterceptor>();
        }

        #region 动态对象

        /// <summary>
        /// 更新动态对象前
        /// </summary>
        public override void BeforeDynamicObjectUpdate(dynamic dynamicData)
        {
            if (dynamicData == null) { return; }

            if (dynamicData.TableName == TableName)
            {
                if (dynamicData.ContainsKey("UserPassword"))
                {
                    string orginPassword = dynamicData.Get("UserPassword").ToString();
                    if (!string.IsNullOrEmpty(orginPassword))// && orginPassword.Length < 20)
                    {                        
                        dynamicData.Add("UserPassword", passwordHasher.HashPassword(orginPassword));
                    }
                    else
                    {
                        //密码为空 将保持密码不变
                        dynamicData.Remove("UserPassword");
                    }
                }
            }

        }

        /// <summary>
        /// 更新动态对象后
        /// </summary>
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_UPDATE);

        }

        /// <summary>
        /// 新增动态对象前
        /// </summary>
        public override void BeforeDynamicObjectInsert(dynamic dynamicData)
        {
            if (dynamicData.TableName == TableName)
            {
                if (dynamicData.ContainsKey("UserPassword"))
                {
                    string orginPassword = dynamicData.Get("UserPassword").ToString();
                    if (!string.IsNullOrEmpty(orginPassword))
                    {
                        dynamicData.Add("UserPassword",passwordHasher.HashPassword(orginPassword));
                    }
                    else
                    {
                        dynamicData.Add("UserPassword", passwordHasher.HashPassword("1"));
                    }
                }
            } 
        }

        /// <summary>
        /// 新增动态对象后
        /// </summary>
        public override void AfterDynamicObjectInsert(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_ADD);
        }

        /// <summary>
        /// 新增动态对象后
        /// </summary>
        public override  void AfterDynamicObjectDelete(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_DELETE);
        }

        private void DataSynchDynamicObject(dynamic user, string oper)
        {
            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "user";
            List<dynamic> users = new List<dynamic>();
            //users.Add(BaseModel.ToEntity<FapUser>(user));
            users.Add(user);
            data.Data = users;
            //采用事件驱动，发布事件
            this._eventBus.PublishAsync(new RealtimeSynEvent(data));
            //RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
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
                if (!string.IsNullOrEmpty(orginPassword) && orginPassword.Length < 80)
                {
                    user.UserPassword =passwordHasher.HashPassword(orginPassword);
                }
            }
        }


        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override  void AfterEntityUpdate(object entity)
        {
            this.DataSynchEntity(entity, RealtimeData.OPER_UPDATE);
        }

        /// <summary>
        /// 新增实体对象前
        /// </summary>
        public override  void BeforeEntityInsert(object entity)
        {
            if (entity != null && entity is FapUser)
            {
                FapUser user = (FapUser)entity;
                string orginPassword = user.UserPassword;
                if (!string.IsNullOrEmpty(orginPassword))
                {
                    user.UserPassword =passwordHasher.HashPassword(orginPassword);
                }
                else
                {
                    user.UserPassword = passwordHasher.HashPassword("1");
                }
            }
        }

        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override void AfterEntityInsert(object entity)
        {
            this.DataSynchEntity(entity, RealtimeData.OPER_ADD);
        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public override void AfterEntityDelete(object entity)
        {
            this.DataSynchEntity(entity, RealtimeData.OPER_DELETE);
        }
       

        private void DataSynchEntity(object entity, string oper)
        {
            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "user";
            if (entity is FapUser)
            {
                List<FapUser> users = new List<FapUser>();
                users.Add(entity as FapUser);
                data.Data = users;
            }
            else if (entity is List<FapUser>)
            {
                data.Data = entity as List<FapUser>;
            }

            RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
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
