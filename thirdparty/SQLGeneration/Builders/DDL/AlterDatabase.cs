using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The alter database builder.
    /// </summary>
    public class AlterDatabase : IDatabaseObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public AlterDatabase(string name)
        {
            this.Name = name;
            this.CurrentDatabase = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentDatabase">Whether the alter should target the CURRENT database.
        /// </param>
        public AlterDatabase(bool currentDatabase)
        {
            this.Name = null;
            this.CurrentDatabase = true;
        }

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The collation that the database will be altered to.
        /// </summary>
        public string NewCollation { get; set; }

        /// <summary>
        /// The name to modify the database name to.
        /// </summary>
        public string NewDatabaseName { get; set; }

        /// <summary>
        /// Whether the alter should target the CURRENT database.
        /// </summary>
        public bool CurrentDatabase { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAlterDatabase(this);
        }
    }
}
