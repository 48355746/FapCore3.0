using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    public interface IFapConfigService
    {
        Dictionary<string, string> GetBillCode(string tableName);
        int GetSequence(string seqName, int stepBy = 1);
        string GetSysParamValue(string paramKey);
    }
}
