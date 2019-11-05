using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The Row Guid Column Property.
    /// </summary>
    public class RowGuidColumnProperty : IColumnProperty
    {
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitRowGuidColumnProperty(this);
        }
    }
}
