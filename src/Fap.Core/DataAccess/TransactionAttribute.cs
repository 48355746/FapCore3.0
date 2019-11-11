using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    /// <summary>
    /// 事务标记属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : Attribute
    {
    }
}
