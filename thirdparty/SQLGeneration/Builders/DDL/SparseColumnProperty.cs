using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The Sparse Column Property.
    /// </summary>
    public class SparseColumnProperty : IColumnProperty
    {
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitSparseColumnProperty(this);
        }
    }
}
