using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Defines a builder that can be used to build "Create" statements.
    /// </summary>
    public interface ICreateBuilder : ICommand
    {
        /// <summary>
        /// The object to be created.
        /// </summary>
        IDatabaseObject CreateObject { get; set; }
    }
}
