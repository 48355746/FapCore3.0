using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 集团
    /// </summary>
    public class FapGroup : BaseModel
    {
        /// <summary>
        /// 集团编码
        /// </summary>
        public string GrpCode { get; set; }
        /// <summary>
        /// 集团全称
        /// </summary>
        public string GrpFullName { get; set; }
        /// <summary>
        /// 集团简称
        /// </summary>
        public string GrpName { get; set; }
        /// <summary>
        /// 集团管理员
        /// </summary>
        public string GrpManager { get; set; }
        /// <summary>
        /// 管理员密码
        /// </summary>
        public string GrpPassword { get; set; }
        /// <summary>
        /// 集团地址
        /// </summary>
        public string GrpAddress { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string LinkPhone { get; set; }
        /// <summary>
        /// 集团类型
        /// </summary>
        public string GrpType { get; set; }
        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }
        /// <summary>
        /// 集团人数
        /// </summary>
        public string PeopleNumber { get; set; }
        /// <summary>
        /// 员工编号自动生成
        /// </summary>
        public int EmpCodeIsAuto { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 币种 的显性字段MC
        /// </summary>
        [Computed]
        public string CurrencyMC { get; set; }

    }
}
