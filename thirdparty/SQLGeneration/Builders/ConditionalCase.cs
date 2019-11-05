using System;
using System.Collections.Generic;
using System.Linq;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a conditional statement.
    /// </summary>
    public class ConditionalCase : IProjectionItem, IFilterItem, IGroupByItem
    {
        private readonly List<ConditionalCaseBranch> branches;

        /// <summary>
        /// Initializes a new instance of a ConditionalCase.
        /// </summary>
        public ConditionalCase()
        {
            branches = new List<ConditionalCaseBranch>();
        }

        /// <summary>
        /// Gets the conditions and values of each branch.
        /// </summary>
        public IEnumerable<ConditionalCaseBranch> Branches
        {
            get { return branches; }
        }

        /// <summary>
        /// Adds the case option to the case expression.
        /// </summary>
        /// <param name="filter">The value that the case item must equal for given the result to be returned.</param>
        /// <param name="result">The value to return when the item equals the option.</param>
        public ConditionalCaseBranch AddBranch(IFilter filter, IProjectionItem result)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            ConditionalCaseBranch branch = new ConditionalCaseBranch();
            branch.Condition = filter;
            branch.Value = result;
            branches.Add(branch);
            return branch;
        }

        /// <summary>
        /// Removes the case branch.
        /// </summary>
        /// <param name="branch">The branch to be removed.</param>
        /// <returns>True if branch was found; otherwise, false.</returns>
        public bool RemoveBranch(ConditionalCaseBranch branch)
        {
            if (branch == null)
            {
                throw new ArgumentNullException("branch");
            }
            return branches.Remove(branch);
        }

        /// <summary>
        /// Gets or sets the default value to return if no options match the item.
        /// </summary>
        public IProjectionItem Default
        {
            get;
            set;
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitConditionalCase(this);
        }
    }
}
