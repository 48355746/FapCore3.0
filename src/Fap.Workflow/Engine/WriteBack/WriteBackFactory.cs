using Dapper;
using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Model.Constants;
using Fap.Workflow.Model;

namespace Fap.Workflow.Engine.WriteBack
{
    /// <summary>
    /// 流程回写工厂
    /// </summary>
    public class WriteBackFactory
    {
        //public static WriteBackFactory instance = null;
        
        //private static object obj = new object();

        //private WriteBackFactory()
        //{

        //}

        //public static WriteBackFactory GetInstance()
        //{
        //    if (instance == null)
        //    {
        //        lock (obj)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new WriteBackFactory();
        //            }
        //        }
        //    }
        //    return instance;
        //}

        public static IWriteBackRule GetWriteBackRule(string processId,string taskId, IDbSession dbSession,IFapConfigService config)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", processId);
            WfFormInstance form = dbSession.QueryFirstOrDefault<WfFormInstance>("select * from WfFormInstance where ProcessId=@ProcessId", parameters);
            if (form.FormType != WfFormType.NoneForm) //外挂表单
            {
                //if (form.AddonType == WfFormAddonType.Internal) //内置单据
                //{
                   // return new AddonFormInternalWriteBack(processId,taskId, dbSession, config);
                //}
            }

            return null;
        }
    }
}
