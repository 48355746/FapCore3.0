using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess.Interceptor
{
    /// <summary>
    /// 元数据的数据拦截器
    /// </summary>
    public interface IDataInterceptor
    {
        #region 动态对象
        /// <summary>
        /// 新增前
        /// </summary>
        void BeforeDynamicObjectInsert(FapDynamicObject dynamicData);

        /// <summary>
        /// 新增后
        /// </summary>
        void AfterDynamicObjectInsert(FapDynamicObject dynamicData);

        /// <summary>
        /// 更新前
        /// </summary>
        void BeforeDynamicObjectUpdate(FapDynamicObject dynamicData);

        /// <summary>
        /// 更新后
        /// </summary>
        void AfterDynamicObjectUpdate(FapDynamicObject dynamicData);

        /// <summary>
        /// 删除前
        /// </summary>
        void BeforeDynamicObjectDelete(FapDynamicObject dynamicData);

        /// <summary>
        /// 删除后
        /// </summary>
        void AfterDynamicObjectDelete(FapDynamicObject dynamicData);

        #endregion

        #region 实体对象
        /// <summary>
        /// 更新实体对象前
        /// </summary>
        void BeforeEntityUpdate(object entity);

        /// <summary>
        /// 更新实体对象后
        /// </summary>
        void AfterEntityUpdate(object entity);

        /// <summary>
        /// 新增实体对象前
        /// </summary>
        void BeforeEntityInsert(object entity);

        /// <summary>
        /// 新增实体对象后
        /// </summary>
        void AfterEntityInsert(object entity);

        /// <summary>
        /// 删除实体对象前
        /// </summary>
        void BeforeEntityDelete(object entity);

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        void AfterEntityDelete(object entity);

        #endregion
    }
}
