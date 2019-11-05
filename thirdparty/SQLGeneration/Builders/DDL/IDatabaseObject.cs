using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Defines an object that can be subjected to "CREATE", "ALTER" and "DROP" statements.
    /// </summary>
    public interface IDatabaseObject : IVisitableBuilder
    {

    }

}
