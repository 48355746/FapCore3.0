using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public interface IDbMetadataContext
    {
        void CreateTable(int id);
        void AddMultiLangColumn(FapColumn fapColumn);
        void DropMultiLangColumn(FapColumn fapColumn);
        void AddColumn(FapColumn fapColumn);
        void RenameColumn(FapColumn newColumn, string oldName);
        void AlterColumn(FapColumn fapColumn);
        void DropColumn(FapColumn fapColumn);
    }
}
