using Fap.Core.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public interface IDbMetadataContext
    {
        void CreateTable(int id);
    }
}
