using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The Not For Replication Column Property.
    /// </summary>
    public class NotForReplicationColumnProperty : IColumnProperty
    {
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitNotForReplicationColumnProperty(this);
        }
    }
}
