using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Manager
{
    /// <summary>
    /// 流程图实例
    /// </summary>
    class DiagramInstanceManager : ManagerBase
    {
        public DiagramInstanceManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public WfDiagramInstance CreateDiagramInstance(string processUid,string processInsUid)
        {
            var process= _dataAccessor.Get<WfProcess>(processUid,false);
            var diagram= _dataAccessor.Get<WfDiagram>(process.DiagramId, false);
            WfDiagramInstance instance = new WfDiagramInstance();
            instance.ProcessInsUid = processInsUid;
            instance.XmlContent = diagram.XmlContent;
            instance.Version = diagram.Version;
            return instance;
        }
        public void Insert(WfDiagramInstance diagramInstance)
        {
            _dataAccessor.Insert<WfDiagramInstance>(diagramInstance);
        }
        public WfDiagramInstance Get(string processInsUid)
        {
            return _dataAccessor.QueryFirstOrDefaultWhere<WfDiagramInstance>("ProcessInsUid=@ProcessInsUid", new Dapper.DynamicParameters(new { ProcessInsUid = processInsUid }),false);
        }
    }
}
