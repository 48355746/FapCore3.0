using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Infrastructure.Interface
{
    /// <summary>
    /// 菜单徽章接口
    /// </summary>
    public interface IMenuBadgeService
    {
        /// <summary>
        /// 返回显示徽章数字
        /// </summary>
        /// <returns></returns>
        int GetBadge();
    }
}
