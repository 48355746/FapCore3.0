using System;
using System.Collections.Generic;
using System.Linq;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a conditional statement where an item is matched with another.
    /// </summary>
    public class MatchCase : IProjectionItem, IFilterItem, IGroupByItem
    {
        private readonly List<MatchCaseBranch> branches;

        /// <summary>
        /// Initializes a new instance of a MatchCase.
        /// </summary>
        /// <param name="item">The item that will be matched to the different values.</param>
        public MatchCase(IProjectionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Item = item;
            branches = new List<MatchCaseBranch>();
        }

        /// <summary>
        /// Gets the item that will be compared to the different branch options.
        /// </summary>
        public IProjectionItem Item 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the case branches.
        /// </summary>
        public IEnumerable<MatchCaseBranch> Branches
        {
            get { return branches; }
        }

        /// <summary>
        /// Adds a branch with the given option and value to the case expression.
        /// </summary>
        /// <param name="option">The value that the case item must equal for given the result to be returned.</param>
        /// <param name="value">The value to return when the item equals the option.</param>
        public MatchCaseBranch AddBranch(IProjectionItem option, IProjectionItem value)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            MatchCaseBranch branch = new MatchCaseBranch();
            branch.Option = option;
            branch.Value = value;
            branches.Add(branch);
            return branch;
        }

        /// <summary>
        /// Removes the branch.
        /// </summary>
        /// <param name="branch">The branch to be removed.</param>
        /// <returns>True if the branch was found; otherwise, false.</returns>
        public bool RemoveBranch(MatchCaseBranch branch)
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
            visitor.VisitMatchCase(this);
        }
    }
}
