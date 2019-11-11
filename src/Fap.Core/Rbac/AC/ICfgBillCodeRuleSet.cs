using Fap.Core.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public interface ICfgBillCodeRuleSet:IEnumerable<CfgBillCodeRule>
    {
        void Refresh();
        bool TryGetValue(string tableName, out IEnumerable<CfgBillCodeRule> billCodeRules);
    }
}
