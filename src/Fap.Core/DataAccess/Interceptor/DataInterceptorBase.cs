using Fap.Core.Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.DataAccess.Interceptor
{
    public class DataInterceptorBase : IDataInterceptor
    {
        protected readonly IDbContext _dbContext;
        protected readonly IFapPlatformDomain _appDomain;
        protected readonly IFapApplicationContext _applicationContext;
        protected readonly ILoggerFactory _loggerFactory;

        protected readonly IServiceProvider _provider;
        public DataInterceptorBase(IServiceProvider provider, IDbContext dbContext)
        {
            _provider = provider;
            _appDomain = provider.GetService<IFapPlatformDomain>();
            _applicationContext = provider.GetService<IFapApplicationContext>();
            _loggerFactory = provider.GetService<ILoggerFactory>();
            _dbContext = dbContext;
        }

        #region 动态对象
        /// <summary>
        /// 新增前
        /// </summary>
        public virtual void BeforeDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
        }

        /// <summary>
        /// 新增后
        /// </summary>
        public virtual void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
        }
        /// <summary>
        /// 更新前
        /// </summary>
        public virtual void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
        }
        /// <summary>
        /// 更新后
        /// </summary>
        public virtual void AfterDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
        }

        /// <summary>
        /// 删除前
        /// </summary>
        public virtual void BeforeDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
        }

        /// <summary>
        /// 删除后
        /// </summary>
        public virtual void AfterDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
        }

        #endregion

        #region 实体对象
        /// <summary>
        /// 更新实体对象前
        /// </summary>
        public virtual void BeforeEntityUpdate(object entity)
        {
        }

        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public virtual void AfterEntityUpdate(object entity)
        {
        }

        /// <summary>
        /// 新增实体对象前
        /// </summary>
        public virtual void BeforeEntityInsert(object entity)
        {
        }

        /// <summary>
        /// 新增实体对象后
        /// </summary>
        public virtual void AfterEntityInsert(object entity)
        {
        }

        /// <summary>
        /// 删除实体对象前
        /// </summary>
        public virtual void BeforeEntityDelete(object entity)
        {
        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public virtual void AfterEntityDelete(object entity)
        {
        }

        #endregion
    }
}
