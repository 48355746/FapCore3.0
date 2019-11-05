using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The Constraint List.
    /// </summary>
    public class ConstraintList : IEnumerable<Constraint>
    {
        private readonly List<Constraint> _columnConstraints;

        /// <summary>
        /// Constructor.
        /// </summary>      
        public ConstraintList()
        {
            _columnConstraints = new List<Constraint>();
        }

        /// <summary>
        /// Adds the constraint to the list.
        /// </summary>
        /// <param name="constraint">The constraint to add.</param>
        public void AddConstraint(Constraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _columnConstraints.Add(constraint);
        }

        /// <summary>
        /// Removes the constraint from the list.
        /// </summary>
        /// <param name="constraint">The constraint to remove.</param>
        /// <returns>True if the constraint was removed; otherwise, false.</returns>
        public bool RemoveConstraint(Constraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return _columnConstraints.Remove(constraint);
        }

        /// <summary>
        /// Clears the constraint.
        /// </summary>
        public void Clear()
        {
            _columnConstraints.Clear();
        }

        /// <summary>
        /// Returns an enumerator that enumerates through the list of constraints.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Constraint> GetEnumerator()
        {
            return _columnConstraints.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _columnConstraints.GetEnumerator();
        }
    }
}
