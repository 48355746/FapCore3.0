using Fap.Core.Infrastructure.Interface;
using Fap.Model;
using Fap.Model.Infrastructure;
using System.Collections.Generic;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Rbac;
using Fap.Core.Infrastructure.Thirdparty;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.Events;
using System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Model.MetaData;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    public class PositionDataInterceptor : DataInterceptorBase
    {
       
        private ILogger _logger;
        private IEventBus _eventBus;

        public PositionDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _logger = _loggerFactory.CreateLogger<PositionDataInterceptor>();
            _eventBus = provider.GetService<IEventBus>();
        }




        #region 动态对象
        /// <summary>
        /// 新增后
        /// </summary>
        public override void AfterDynamicObjectInsert(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_ADD);
        }

        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_UPDATE);
        }

        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_DELETE);
        }

        private void DataSynchDynamicObject(dynamic dynamicData, string oper)
        {
            if (dynamicData == null) { return; }

            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "position";
            List<dynamic> positions = new List<dynamic>();
            //positions.Add(BaseModel.ToEntity<OrgPosition>(dynamicData));
            positions.Add(dynamicData);
            data.Data = positions;

            RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
        }

        #endregion 动态对象

        #region 实体对象
        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override void AfterEntityUpdate(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_UPDATE);
        }

        /// <summary>
        /// 新增实体对象后
        /// </summary>
        public override void AfterEntityInsert(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_ADD);
        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public override void AfterEntityDelete(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_DELETE);
        }

        private void DataSynchEntity(object entity, string oper)
        {
            if (entity == null) { return; }
            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "position";
            if (entity is OrgPosition)
            {
                OrgPosition position = entity as OrgPosition;
                List<OrgPosition> positions = new List<OrgPosition>();
                positions.Add(position);
                data.Data = positions;
            }
            else if (entity is List<OrgPosition>)
            {
                List<OrgPosition> positions = entity as List<OrgPosition>;
                data.Data = positions;
            }
            this._eventBus.PublishAsync(new RealtimeSynEvent(data));
            //RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
        }

       

        #endregion 实体对象
    }
}
