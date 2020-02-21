using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2017-05-11 18:32:45
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{   
    /// <summary>
    /// 多语集
    /// </summary>
    public interface IMultiLanguage : IEnumerable<FapMultiLanguage>
    {
        void Refresh();
        bool TryGetValue(string qualifier, out IEnumerable<FapMultiLanguage> multiLanguages);
        bool TryGetValue(string qualifier,string langKey, out FapMultiLanguage fapMultiLang);
    }
}
