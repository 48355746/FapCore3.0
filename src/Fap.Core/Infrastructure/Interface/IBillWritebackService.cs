/* ==============================================================================
* 功能描述：单据回写类  
* 创 建 者：wyf
* 创建日期：2017-08-10 11:48:37
* ==============================================================================*/

using Fap.Core.DataAccess;

namespace Fap.Core.Infrastructure.Interface
{
    public interface IBillWritebackService
    {
        /// <summary>
        /// 单据回写接口
        /// </summary>
        /// <param name="dataAccessor">数据库访问</param>
        /// <param name="billData">FapDynamicObject单据实体</param>
        /// <param name="bizData">FapDynamicObject业务实体，只有映射了业务实体的时候才会有值，默认null</param>
        void Exec(IDbContext dataAccessor, dynamic billData, dynamic bizData);
    }
}
