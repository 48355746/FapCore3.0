using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 元数据列的默认值提供的接口定义
    /// </summary>
    public interface IDefaultValueSupport
    {
        object initValue(object column, string param);
    }
}
