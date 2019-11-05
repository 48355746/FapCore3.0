using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The Persisted Column Property.
    /// </summary>
    public class PersistedColumnProperty : IColumnProperty
    {
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitPersistedColumnProperty(this);
        }
    }
}
