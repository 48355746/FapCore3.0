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
        void BeforeDynamicObjectInsert(dynamic dynamicData);

        /// <summary>
        /// 新增后
        /// </summary>
        void AfterDynamicObjectInsert(dynamic dynamicData);

        /// <summary>
        /// 更新前
        /// </summary>
        void BeforeDynamicObjectUpdate(dynamic dynamicData);

        /// <summary>
        /// 更新后
        /// </summary>
        void AfterDynamicObjectUpdate(dynamic dynamicData);

        /// <summary>
        /// 删除前
        /// </summary>
        void BeforeDynamicObjectDelete(dynamic dynamicData);

        /// <summary>
        /// 删除后
        /// </summary>
        void AfterDynamicObjectDelete(dynamic dynamicData);

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
