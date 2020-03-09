using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SQLGeneration.Builders;



namespace SQLGeneration.Generators
{
    /// <summary>
    /// Visits the builders and generates text.
    /// </summary>
    public class FormattingVisitor : BuilderVisitor
    {
        private readonly TextWriter writer;
        private readonly CommandOptions options;
        private readonly int level;
        private readonly CommandType commandType;
        private readonly SourceReferenceType sourceReferenceType;
        private readonly ValueReferenceType valueReferenceType;

        /// <summary>
        /// Initializes a new instance of a FormatterVisitor.
        /// </summary>
        /// <param name="writer">The text writer to write the text to.</param>
        /// <param name="options">The command options to use to augment the text generation.</param>
        public FormattingVisitor(TextWriter writer, CommandOptions options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.writer = writer;
            this.options = options == null ? new CommandOptions() : options.Clone();
            this.commandType = CommandType.Unknown;
            this.sourceReferenceType = SourceReferenceType.Declaration;
            this.valueReferenceType = ValueReferenceType.Declaration;
        }

        private FormattingVisitor(
            TextWriter writer,
            CommandOptions options,
            int level,
            CommandType commandType,
            SourceReferenceType sourceType,
            ValueReferenceType projectionType)

        {
            this.writer = writer;
            this.options = options;
            this.level = level;
            this.commandType = commandType;
            this.sourceReferenceType = sourceType;
            this.valueReferenceType = projectionType;
        }

        private FormattingVisitor forSubCommand()
        {
            return new FormattingVisitor(writer, options, level + 1, commandType, sourceReferenceType, valueReferenceType);
        }

        private FormattingVisitor forCommandType(CommandType type)
        {
            return new FormattingVisitor(writer, options, level, type, sourceReferenceType, valueReferenceType);
        }

        private FormattingVisitor forSourceContext(SourceReferenceType type)
        {
            return new FormattingVisitor(writer, options, level, commandType, type, valueReferenceType);
        }

        private FormattingVisitor forValueContext(ValueReferenceType type)
        {
            return new FormattingVisitor(writer, options, level, commandType, sourceReferenceType, type);
        }

        /// <summary>
        /// Generates the text for an Addition builder.
        /// </summary>
        /// <param name="item">The Addition builder to generate the text for.</param>
        protected internal override void VisitAddition(Addition item)
        {
            visitArithmeticExpression(item, "+");
        }

        /// <summary>
        /// Generates the text for an AliasedSource.
        /// </summary>
        /// <param name="aliasedSource">The AliasedSource to generate the text for.</param>
        protected internal override void VisitAliasedSource(AliasedSource aliasedSource)
        {
            visitAliasedSource(aliasedSource);
        }

        /// <summary>
        /// Generates the text for an AllColumns builder.
        /// </summary>
        /// <param name="item">The AllColumns builder to generate the text for.</param>
        protected internal override void VisitAllColumns(AllColumns item)
        {
            if (item.Source != null)
            {
                forSourceContext(SourceReferenceType.Reference).visitAliasedSource(item.Source);
                writer.Write(".");
            }
            writer.Write("*");
        }

        /// <summary>
        /// Generates the text for a Batch builder.
        /// </summary>
        /// <param name="item">The BatchBuilder to generate the text for.</param>
        protected internal override void VisitBatch(BatchBuilder item)
        {
            foreach (var command in item.Commands())
            {
                command.Accept(this);
            }
            base.VisitBatch(item);
        }

        /// <summary>
        /// Generates the text for a BetweenFilter builder.
        /// </summary>
        /// <param name="item">The BetweenFilter builder to generate the text for.</param>
        protected internal override void VisitBetweenFilter(BetweenFilter item)
        {
            Action visitor = () =>
            {
                item.Value.Accept(forSubCommand());
                writer.Write(" ");
                if (item.Not)
                {
                    writer.Write("NOT ");
                }
                writer.Write("BETWEEN ");
                item.LowerBound.Accept(forSubCommand());
                writer.Write(" AND ");
                item.UpperBound.Accept(forSubCommand());
            };
            visitFilter(item, visitor);
        }

        /// <summary>
        /// Generates the text for a BetweenWindowFrame builder.
        /// </summary>
        /// <param name="item">The BetweenWindowFrame builder to generate the text for.</param>
        protected internal override void VisitBetweenWindowFrame(BetweenWindowFrame item)
        {
            visitWindowFrameType(item);
            writer.Write(" BETWEEN ");
            item.PrecedingFrame.Accept(forSubCommand());
            writer.Write(" AND ");
            item.FollowingFrame.Accept(forSubCommand());
        }

        /// <summary>
        /// Generates the text for a Column builder.
        /// </summary>
        /// <param name="item">The Column builder to generate the text for.</param>
        protected internal override void VisitColumn(Column item)
        {
            bool qualify = item.Source != null
                && (item.Qualify ?? (commandType == CommandType.Select
                || (commandType == CommandType.Insert && options.QualifyInsertColumns)
                || (commandType == CommandType.Update && options.QualifyUpdateColumns)
                || (commandType == CommandType.Delete && options.QualifyDeleteColumns)));
            if (qualify)
            {
                forSourceContext(SourceReferenceType.Reference).visitAliasedSource(item.Source);
                writer.Write(".");
            }
            writer.Write(item.Name);
        }

        /// <summary>
        /// Generates the text for a ConditionalCase builder.
        /// </summary>
        /// <param name="item">The ConditionCase builder to generate the text for.</param>
        protected internal override void VisitConditionalCase(ConditionalCase item)
        {
            if (!item.Branches.Any())
            {
                throw new SQLGenerationException("An attempt was made to create a case expression without any cases.");
            }
            writer.Write("CASE");
            foreach (ConditionalCaseBranch branch in item.Branches)
            {
                writer.Write(" WHEN ");
                branch.Condition.Accept(forSubCommand());
                writer.Write(" THEN ");
                branch.Value.Accept(forSubCommand());
            }
            if (item.Default != null)
            {
                writer.Write(" ELSE ");
                item.Default.Accept(forSubCommand());
            }
            writer.Write(" END");
        }

        /// <summary>
        /// Generates the text for a CrossJoin builder.
        /// </summary>
        /// <param name="item">The CrossJoin builder to generate the text for.</param>
        protected internal override void VisitCrossJoin(CrossJoin item)
        {
            visitBinaryJoin(item, "CROSS JOIN");
        }

        /// <summary>
        /// Generates the text for a CurrentRowFrame builder.
        /// </summary>
        /// <param name="item">The CurrentRowFrame builder to generate the text for.</param>
        protected internal override void VisitCurrentRowFrame(CurrentRowFrame item)
        {
            writer.Write("CURRENT ROW");
        }

        /// <summary>
        /// Generates the text for a Delete builder.
        /// </summary>
        /// <param name="item">The Delete builder to generate the text for.</param>
        protected internal override void VisitDelete(DeleteBuilder item)
        {
            forCommandType(CommandType.Delete).visitDelete(item);
        }

        private void visitDelete(DeleteBuilder item)
        {
            writer.Write("DELETE ");
            if (options.VerboseDeleteStatement)
            {
                writer.Write("FROM ");
            }
            forSourceContext(SourceReferenceType.Declaration).visitAliasedSource(item.Table);

            if (item.Output.Any())
            {
                writer.Write(" OUTPUT ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.Output);
            }

            if (item.WhereFilterGroup.HasFilters)
            {
                writer.Write(" WHERE ");
                IFilter filterGroup = item.WhereFilterGroup;
                filterGroup.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
            }
            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        /// <summary>
        /// Generates the text for a Division builder.
        /// </summary>
        /// <param name="item">The Division builder to generate the text for.</param>
        protected internal override void VisitDivision(Division item)
        {
            visitArithmeticExpression(item, "/");
        }

        /// <summary>
        /// Generates the text for an EqualToFilter builder.
        /// </summary>
        /// <param name="item">The EqualToFilter builder to generate the text for.</param>
        protected internal override void VisitEqualToFilter(EqualToFilter item)
        {
            visitOrderedFilter(item, "=");
        }

        /// <summary>
        /// Generates the text for an EqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The EqualToQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitEqualToQuantifierFilter(EqualToQuantifierFilter item)
        {
            visitQuantifiedFilter(item, "=");
        }

        /// <summary>
        /// Generates the text for an Except builder.
        /// </summary>
        /// <param name="item">The Except builder to generate the text for.</param>
        protected internal override void VisitExcept(Except item)
        {
            visitSelectCombiner(item, "EXCEPT");
        }

        /// <summary>
        /// Generates the text for an ExistsFilter builder.
        /// </summary>
        /// <param name="item">The ExistsFilter builder to generate the text for.</param>
        protected internal override void VisitExistsFilter(ExistsFilter item)
        {
            Action visitor = () =>
            {
                writer.Write("EXISTS");
                item.Select.Accept(forSubCommand());
            };
            visitFilter(item, visitor);
        }

        /// <summary>
        /// Generates the text for a FilterGroup builder.
        /// </summary>
        /// <param name="item">The FilterGroup builder to generate the text for.</param>
        protected internal override void VisitFilterGroup(FilterGroup item)
        {
            if (!item.Filters.Any())
            {
                throw new SQLGenerationException("An attempt was made to build an empty filter clause.");
            }
            bool wrapInParentheses = shouldWrapGroupInParentheses(item);
            if (wrapInParentheses)
            {
                writer.Write("(");
            }
            ConjunctionConverter converter = new ConjunctionConverter();
            string conjunction = " " + converter.ToString(item.Conjunction) + " ";
            join(conjunction, item.Filters);
            if (wrapInParentheses)
            {
                writer.Write(")");
            }
        }

        private bool shouldWrapGroupInParentheses(FilterGroup group)
        {
            // Add parenthesis when they are explicitly requested
            if (group.WrapInParentheses == true)
            {
                return true;
            }
            // Add parenthesis when precedence should be automatic
            if (options.WrapFiltersInParentheses && group.Conjunction == Conjunction.Or)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates the text for a FollowingBoundFrame builder.
        /// </summary>
        /// <param name="item">The FollowingBoundFrame builder to generate the text for.</param>
        protected internal override void VisitFollowingBoundFrame(FollowingBoundFrame item)
        {
            visitBoundFrame(item);
            writer.Write(" FOLLOWING");
        }

        /// <summary>
        /// Generates the text for a FollowingUnboundFrame builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitFollowingUnboundFrame(FollowingUnboundFrame item)
        {
            visitUnboundFrame(item);
            writer.Write(" FOLLOWING");
        }

        /// <summary>
        /// Generates the text for a FullOuterJoin builder.
        /// </summary>
        /// <param name="item">The FullOuterJin builder to generate the text for.</param>
        protected internal override void VisitFullOuterJoin(FullOuterJoin item)
        {
            StringBuilder result = new StringBuilder("FULL ");
            if (options.VerboseOuterJoin)
            {
                result.Append("OUTER ");
            }
            result.Append("JOIN");
            visitFilteredJoin(item, result.ToString());
        }

        /// <summary>
        /// Generates the text for a Function builder.
        /// </summary>
        /// <param name="item">The Function builder to generate the text for.</param>
        protected internal override void VisitFunction(Function item)
        {
            visitMultipartIdentifier(item.Qualifier, item.Name);
            writer.Write("(");
            if (item.Arguments.Any())
            {
                join(", ", item.Arguments);
            }
            writer.Write(")");
            if (item.FunctionWindow != null)
            {
                writer.Write(" ");
                IVisitableBuilder functionWindow = item.FunctionWindow;
                functionWindow.Accept(forSubCommand());
            }
        }

        /// <summary>
        /// Generates the text for a FunctionWindow builder.
        /// </summary>
        /// <param name="item">The FunctionWindow builder to generate the text for.</param>
        protected internal override void VisitFunctionWindow(FunctionWindow item)
        {
            writer.Write("OVER (");
            bool needsSpace = false;
            if (item.Partition.Any())
            {
                writer.Write("PARTITION BY ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.Partition);
                needsSpace = true;
            }
            if (item.OrderBy.Any())
            {
                if (needsSpace)
                {
                    writer.Write(" ");
                }
                writer.Write("ORDER BY ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.OrderBy);
                needsSpace = true;
            }
            if (item.Frame != null)
            {
                if (needsSpace)
                {
                    writer.Write(" ");
                }
                IVisitableBuilder frame = item.Frame;
                frame.Accept(forSubCommand());
            }
            writer.Write(")");
        }

        /// <summary>
        /// Generates the text for a GreaterThanEqualToFilter builder.
        /// </summary>
        /// <param name="item">The GreaterThanEqualToFilter builder to generate the text for.</param>
        protected internal override void VisitGreaterThanEqualToFilter(GreaterThanEqualToFilter item)
        {
            visitOrderedFilter(item, ">=");
        }

        /// <summary>
        /// Generates the text for a GreaterThanEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The GreaterThanEqualToQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitGreaterThanEqualToQuantifierFilter(GreaterThanEqualToQuantifierFilter item)
        {
            visitQuantifiedFilter(item, ">=");
        }

        /// <summary>
        /// Generates the text for a GreaterThanFilter builder.
        /// </summary>
        /// <param name="item">The GreaterThanFilter builder to generate the text for.</param>
        protected internal override void VisitGreaterThanFilter(GreaterThanFilter item)
        {
            visitOrderedFilter(item, ">");
        }

        /// <summary>
        /// Generates the text for a GreaterThanQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The GreaterThanQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitGreaterThanQuantifierFilter(GreaterThanQuantifierFilter item)
        {
            visitQuantifiedFilter(item, ">");
        }

        /// <summary>
        /// Generates the text for an InFilter builder.
        /// </summary>
        /// <param name="item">The InFilter builder to generate the text for.</param>
        protected internal override void VisitInFilter(InFilter item)
        {
            Action visitor = () =>
            {
                item.LeftHand.Accept(forSubCommand());
                writer.Write(" ");
                if (item.Not)
                {
                    writer.Write("NOT ");
                }
                writer.Write("IN ");
                item.Values.Accept(forSubCommand());
            };
            visitFilter(item, visitor);
        }

        /// <summary>
        /// Generates the text for an InnerJoin builder.
        /// </summary>
        /// <param name="item">The InnerJoin builder to generate the text for.</param>
        protected internal override void VisitInnerJoin(InnerJoin item)
        {
            StringBuilder joinBuilder = new StringBuilder();
            if (options.VerboseInnerJoin)
            {
                joinBuilder.Append("INNER ");
            }
            joinBuilder.Append("JOIN");
            visitFilteredJoin(item, joinBuilder.ToString());
        }

        /// <summary>
        /// Generates the text for an Insert builder.
        /// </summary>
        /// <param name="item">The Insert builder to generate the text for.</param>
        protected internal override void VisitInsert(InsertBuilder item)
        {
            forCommandType(CommandType.Insert).visitInsert(item);
        }

        private void visitInsert(InsertBuilder item)
        {
            writer.Write("INSERT INTO ");
            forSourceContext(SourceReferenceType.Declaration).visitAliasedSource(item.Table);
            if (item.Columns.Any())
            {
                writer.Write(" (");
                forValueContext(ValueReferenceType.Reference).join(", ", item.Columns);
                writer.Write(")");
            }
            writer.Write(" ");
            if (item.Output.Any())
            {
                writer.Write("OUTPUT ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.Output);
                writer.Write(" ");
            }
            if (item.Values.IsValueList)
            {
                writer.Write("VALUES");
            }
            item.Values.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        /// <summary>
        /// Generates the text for an Intersect builder.
        /// </summary>
        /// <param name="item">The Intersect builder to generate the text for.</param>
        protected internal override void VisitIntersect(Intersect item)
        {
            visitSelectCombiner(item, "INTERSECT");
        }

        /// <summary>
        /// Generates the text for a LeftOuterJoin builder.
        /// </summary>
        /// <param name="item">The LeftOuterJoin builder to generate the text for.</param>
        protected internal override void VisitLeftOuterJoin(LeftOuterJoin item)
        {
            StringBuilder joinBuilder = new StringBuilder("LEFT ");
            if (options.VerboseOuterJoin)
            {
                joinBuilder.Append("OUTER ");
            }
            joinBuilder.Append("JOIN");
            visitFilteredJoin(item, joinBuilder.ToString());
        }

        /// <summary>
        /// Generates the text for a LessThanEqualToFilter builder.
        /// </summary>
        /// <param name="item">The LessThanEqualToFilter builder to generate the text for.</param>
        protected internal override void VisitLessThanEqualToFilter(LessThanEqualToFilter item)
        {
            visitOrderedFilter(item, "<=");
        }

        /// <summary>
        /// Generates the text for a LessThanEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The LessThanEqualToQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitLessThanEqualToQuantifierFilter(LessThanEqualToQuantifierFilter item)
        {
            visitQuantifiedFilter(item, "<=");
        }

        /// <summary>
        /// Generates the text for a LessThanFilter builder.
        /// </summary>
        /// <param name="item">The LessThanFilter builder to generate the text for.</param>
        protected internal override void VisitLessThanFilter(LessThanFilter item)
        {
            visitOrderedFilter(item, "<");
        }

        /// <summary>
        /// Generates the text for a LessThanQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The LessThanQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitLessThanQuantifierFilter(LessThanQuantifierFilter item)
        {
            visitQuantifiedFilter(item, "<");
        }

        /// <summary>
        /// Generates the text for a LikeFilter builder.
        /// </summary>
        /// <param name="item">The LikeFilter builder to generate the text for.</param>
        protected internal override void VisitLikeFilter(LikeFilter item)
        {
            StringBuilder operationBuilder = new StringBuilder();
            if (item.Not)
            {
                operationBuilder.Append("NOT ");
            }
            operationBuilder.Append("LIKE");
            visitOrderedFilter(item, operationBuilder.ToString());
        }

        /// <summary>
        /// Generates the text for a MatchCase builder.
        /// </summary>
        /// <param name="item">The MatchCase builder to generate the text for.</param>
        protected internal override void VisitMatchCase(MatchCase item)
        {
            if (!item.Branches.Any())
            {
                throw new SQLGenerationException("An attempt was made to create a case expression without any cases.");
            }
            writer.Write("CASE ");
            item.Item.Accept(forSubCommand());
            writer.Write(" ");
            foreach (MatchCaseBranch branch in item.Branches)
            {
                writer.Write("WHEN ");
                branch.Option.Accept(forSubCommand());
                writer.Write(" THEN ");
                branch.Value.Accept(forSubCommand());
                writer.Write(" ");
            }
            if (item.Default != null)
            {
                writer.Write("ELSE ");
                item.Default.Accept(forSubCommand());
                writer.Write(" ");
            }
            writer.Write("END");
        }

        /// <summary>
        /// Generates the text for a Minus builder.
        /// </summary>
        /// <param name="item">The Minus builder to generate the text for.</param>
        protected internal override void VisitMinus(Minus item)
        {
            visitSelectCombiner(item, "MINUS");
        }

        /// <summary>
        /// Generates the text for a Modulus builder.
        /// </summary>
        /// <param name="item">The Minus builder to generate the text for.</param>
        protected internal override void VisitModulus(Modulus item)
        {
            visitArithmeticExpression(item, "%");
        }

        /// <summary>
        /// Generates the text for a Multiplication builder.
        /// </summary>
        /// <param name="item">The Multiplication builder to generate the text for.</param>
        protected internal override void VisitMultiplication(Multiplication item)
        {
            visitArithmeticExpression(item, "*");
        }

        /// <summary>
        /// Generates the text for a Negation builder.
        /// </summary>
        /// <param name="item">The Negation builder to generate the text for.</param>
        protected internal override void VisitNegation(Negation item)
        {
            writer.Write("-");
            bool wrapInParentheses = shouldWrapNegationInParentheses(item);
            if (wrapInParentheses)
            {
                writer.Write("(");
            }
            item.Item.Accept(forSubCommand());
            if (wrapInParentheses)
            {
                writer.Write(")");
            }
        }

        private bool shouldWrapNegationInParentheses(Negation item)
        {
            // if the item is a literal, we don't need to wrap it in parentheses
            ArithmeticExpression expression = item.Item as ArithmeticExpression;
            if (expression == null)
            {
                return false;
            }
            // if the item is an arithmetic expression, don't double wrap
            if (expression.WrapInParentheses ?? options.WrapArithmeticExpressionsInParentheses)
            {
                return false;
            }
            // otherwise, wrap to preserve precedence
            return true;
        }

        /// <summary>
        /// Generates the text for a NotEqualToFilter builder.
        /// </summary>
        /// <param name="item">The NotEqualToFilter builder to generate the text for.</param>
        protected internal override void VisitNotEqualToFilter(NotEqualToFilter item)
        {
            visitOrderedFilter(item, "<>");
        }

        /// <summary>
        /// Generates the text for a NotEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The NotEqualToQuantifierFilter builder to generate the text for.</param>
        protected internal override void VisitNotEqualToQuantifierFilter(NotEqualToQuantifierFilter item)
        {
            visitQuantifiedFilter(item, "<>");
        }

        /// <summary>
        /// Generates the text for a NotFilter builder.
        /// </summary>
        /// <param name="item">The NotFilter builder to generate the text for.</param>
        protected internal override void VisitNotFilter(NotFilter item)
        {
            Action visitor = () =>
            {
                writer.Write("NOT ");
                bool wrapInParenthesis = shouldWrapNotFilterInParenthesis(item);
                if (wrapInParenthesis)
                {
                    writer.Write("(");
                }
                item.Filter.Accept(forSubCommand());
                if (wrapInParenthesis)
                {
                    writer.Write(")");
                }
            };
            visitFilter(item, visitor);
        }

        private bool shouldWrapNotFilterInParenthesis(NotFilter item)
        {
            // if the child is a filter group with more than one child, wrap in parentheses
            // unless, it would result in double wrapping
            FilterGroup group = item.Filter as FilterGroup;
            if (group == null || group.Count == 1 || (group.WrapInParentheses ?? options.WrapFiltersInParentheses))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generates the text for a NullFilter builder.
        /// </summary>
        /// <param name="item">The NullFilter builder to generate the text for.</param>
        protected internal override void VisitNullFilter(NullFilter item)
        {
            Action visitor = () =>
            {
                item.LeftHand.Accept(forSubCommand());
                writer.Write(" IS ");
                if (item.Not)
                {
                    writer.Write("NOT ");
                }
                writer.Write("NULL");
            };
            visitFilter(item, visitor);
        }

        /// <summary>
        /// Generates the text for a NullLiteral builder.
        /// </summary>
        /// <param name="item">The NullLiteral builder to generate the text for.</param>
        protected internal override void VisitNullLiteral(NullLiteral item)
        {
            writer.Write("NULL");
        }

        /// <summary>
        /// Generates the text for a NumericLiteral builder.
        /// </summary>
        /// <param name="item">The NumericLiteral builder to generate the text for.</param>
        protected internal override void VisitNumericLiteral(NumericLiteral item)
        {
            if (item.Format == null)
            {
                writer.Write(item.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                writer.Write(item.Value.ToString(item.Format, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Generates the text for an OrderBy builder.
        /// </summary>
        /// <param name="item">The OrderBy builder to generate the text for.</param>
        protected internal override void VisitOrderBy(OrderBy item)
        {
            visitAliasedProjection(item.Projection);
            if (item.Order != Order.Default)
            {
                writer.Write(" ");
                OrderConverter converter = new OrderConverter();
                writer.Write(converter.ToString(item.Order));
            }
            if (item.NullPlacement != NullPlacement.Default)
            {
                writer.Write(" ");
                NullPlacementConverter converter = new NullPlacementConverter();
                writer.Write(converter.ToString(item.NullPlacement));
            }
            //分页
            if(!string.IsNullOrEmpty(item.Pagination))
            {
                writer.Write(" " + item.Pagination);
            }
        }

        /// <summary>
        /// Generates the text for a Placeholder builder.
        /// </summary>
        /// <param name="item">The Placeholder builder to generate the text for.</param>
        protected internal override void VisitPlaceholder(Placeholder item)
        {
            writer.Write(item.Value);
        }

        /// <summary>
        /// Generates the text for a PrecedingBoundFrame builder.
        /// </summary>
        /// <param name="item">The PrecedingBoundFrame builder to generate the text for.</param>
        protected internal override void VisitPrecedingBoundFrame(PrecedingBoundFrame item)
        {
            visitBoundFrame(item);
            writer.Write(" PRECEDING");
        }

        /// <summary>
        /// Generates the text for a PrecedingOnlyWindowFrame builder.
        /// </summary>
        /// <param name="item">The PrecedingOnlyWindowFrame builder to generate the text for.</param>
        protected internal override void VisitPrecedingOnlyWindowFrame(PrecedingOnlyWindowFrame item)
        {
            visitWindowFrameType(item);
            writer.Write(" ");
            item.PrecedingFrame.Accept(forSubCommand());
        }

        /// <summary>
        /// Generates the text for a PrecedingUnboundFrame builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitPrecedingUnboundFrame(PrecedingUnboundFrame item)
        {
            visitUnboundFrame(item);
            writer.Write(" PRECEDING");
        }

        /// <summary>
        /// Generates the text for a RightOuterJoin builder.
        /// </summary>
        /// <param name="item">The RightOuterJoin builder to generate the text for.</param>
        protected internal override void VisitRightOuterJoin(RightOuterJoin item)
        {
            StringBuilder joinBuilder = new StringBuilder("RIGHT ");
            if (options.VerboseOuterJoin)
            {
                joinBuilder.Append("OUTER ");
            }
            joinBuilder.Append("JOIN");
            visitFilteredJoin(item, joinBuilder.ToString());
        }

        /// <summary>
        /// Generates the text for a Select builder.
        /// </summary>
        /// <param name="item">The Select builder to generate the text for.</param>
        protected internal override void VisitSelect(SelectBuilder item)
        {
            forCommandType(CommandType.Select).visitSelect(item);
        }

        private void visitSelect(SelectBuilder item)
        {
            bool needsParentheses = level > 0;
            if (needsParentheses)
            {
                writer.Write("(");
            }
            writer.Write("SELECT ");
            if (item.Distinct != DistinctQualifier.Default)
            {
                DistinctQualifierConverter converter = new DistinctQualifierConverter();
                writer.Write(converter.ToString(item.Distinct));
                writer.Write(" ");
            }
            if (item.Top != null)
            {
                IVisitableBuilder top = item.Top;
                top.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
                writer.Write(" ");
            }
            forValueContext(ValueReferenceType.Declaration).join(", ", item.Projection);
            if (item.From.Any())
            {
                writer.Write(" FROM ");
                forSourceContext(SourceReferenceType.Declaration).join(", ", item.From);
            }
            if (item.WhereFilterGroup.HasFilters)
            {
                writer.Write(" WHERE ");
                IFilter where = item.WhereFilterGroup;
                where.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
            }
            if (item.GroupBy.Any())
            {
                writer.Write(" GROUP BY ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.GroupBy);
            }
            if (item.HavingFilterGroup.HasFilters)
            {
                writer.Write(" HAVING ");
                IFilter having = item.HavingFilterGroup;
                having.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
            }
            if (item.OrderBy.Any())
            {
                writer.Write(" ORDER BY ");
                forValueContext(ValueReferenceType.Alias).join(", ", item.OrderBy);
            }
            if (needsParentheses)
            {
                writer.Write(")");
            }
            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        /// <summary>
        /// Generates the text for a Setter builder.
        /// </summary>
        /// <param name="item">The Setter builder to generate the text for.</param>
        protected internal override void VisitSetter(Setter item)
        {
            IVisitableBuilder column = item.Column;
            column.Accept(forSubCommand());
            writer.Write(" = ");
            item.Value.Accept(forSubCommand());
        }

        /// <summary>
        /// Generates the text for a StringLiteral builder.
        /// </summary>
        /// <param name="item">The StringLiteral builder to generate the text for.</param>
        protected internal override void VisitStringLiteral(StringLiteral item)
        {
            writer.Write("'");
            if (item.Value != null)
            {
                string value = item.Value.Replace("'", "''");
                writer.Write(value);
            }
            writer.Write("'");
        }
        /// <summary>
        /// 参数sql 不带引号
        /// </summary>
        /// <param name="item"></param>
        protected internal override void VisitParameterLiteral(ParameterLiteral item)
        {
            if (item.Value != null)
            {
                string value = item.Value;
                writer.Write(value);
            }

        }
        /// <summary>
        /// Generates the text for a Subtraction builder.
        /// </summary>
        /// <param name="item">The Subtraction builder to generate the text for.</param>
        protected internal override void VisitSubtraction(Subtraction item)
        {
            visitArithmeticExpression(item, "-");
        }

        /// <summary>
        /// Generates the text for a Table builder.
        /// </summary>
        /// <param name="item">The Table builder to generate the text for.</param>
        protected internal override void VisitTable(Table item)
        {
            visitMultipartIdentifier(item.Qualifier, item.Name);
        }

        /// <summary>
        /// Generates the text for a Top builder.
        /// </summary>
        /// <param name="item">The Top builder to generate the text for.</param>
        protected internal override void VisitTop(Top item)
        {
            writer.Write("TOP ");
            item.Expression.Accept(forSubCommand());
            if (item.IsPercent)
            {
                writer.Write(" PERCENT");
            }
            if (item.WithTies)
            {
                writer.Write(" WITH TIES");
            }
        }

        /// <summary>
        /// Generates the text for a Union builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitUnion(Union item)
        {
            visitSelectCombiner(item, "UNION");
        }

        /// <summary>
        /// Generates the text for an Update builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitUpdate(UpdateBuilder item)
        {
            forCommandType(CommandType.Update).visitUpdate(item);
        }

        private void visitUpdate(UpdateBuilder item)
        {
            if (!item.Setters.Any())
            {
                throw new SQLGenerationException("An attempt was made to build an update statement without any setters.");
            }
            writer.Write("UPDATE ");
            forSourceContext(SourceReferenceType.Declaration).visitAliasedSource(item.Table);
            writer.Write(" SET ");
            forValueContext(ValueReferenceType.Reference).join(", ", item.Setters);

            if (item.Output.Any())
            {
                writer.Write(" OUTPUT ");
                forValueContext(ValueReferenceType.Reference).join(", ", item.Output);
            }
            if (item.WhereFilterGroup.HasFilters)
            {
                writer.Write(" WHERE ");
                IVisitableBuilder where = item.WhereFilterGroup;
                where.Accept(forSubCommand().forValueContext(ValueReferenceType.Reference));
            }
            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        /// <summary>
        /// Generates the text for a ValueList builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitValueList(ValueList item)
        {
            writer.Write("(");
            if (item.Values.Any())
            {
                join(", ", item.Values);
            }
            writer.Write(")");
        }

        private void visitArithmeticExpression(ArithmeticExpression item, string operationToken)
        {
            if (item.WrapInParentheses ?? options.WrapArithmeticExpressionsInParentheses)
            {
                writer.Write("(");
            }
            item.LeftHand.Accept(forSubCommand());
            writer.Write(" ");
            writer.Write(operationToken);
            writer.Write(" ");
            item.RightHand.Accept(forSubCommand());
            if (item.WrapInParentheses ?? options.WrapArithmeticExpressionsInParentheses)
            {
                writer.Write(")");
            }
        }

        private void visitAliasedSource(AliasedSource source)
        {
            if (sourceReferenceType == SourceReferenceType.Declaration)
            {
                visitAliasedSourceDeclaration(source);
            }
            else if (sourceReferenceType == SourceReferenceType.Reference)
            {
                visitAliasedSourceReference(source);
            }
        }

        private void visitAliasedSourceDeclaration(AliasedSource source)
        {
            source.Source.Accept(forSubCommand());
            if (!String.IsNullOrWhiteSpace(source.Alias))
            {
                if (options.AliasColumnSourcesUsingAs)
                {
                    writer.Write(" AS");
                }
                writer.Write(" ");
                writer.Write(source.Alias);
            }
        }

        private void visitAliasedSourceReference(AliasedSource source)
        {
            if (String.IsNullOrWhiteSpace(source.Alias))
            {
                if (source.Source.IsAliasRequired)
                {
                    throw new SQLGenerationException("A reference was made to a source without defining an alias for it.");
                }
                source.Source.Accept(forSubCommand());
            }
            else
            {
                writer.Write(source.Alias);
            }
        }

        private void visitAliasedProjection(AliasedProjection projection)
        {
            if (valueReferenceType == ValueReferenceType.Declaration)
            {
                visitAliasedProjectionDeclaration(projection);
            }
            else if (valueReferenceType == ValueReferenceType.Reference)
            {
                visitAliasedProjectionReference(projection);
            }
            else if (valueReferenceType == ValueReferenceType.Alias)
            {
                visitAliasedProjectionAlias(projection);
            }
        }

        private void visitAliasedProjectionDeclaration(AliasedProjection projection)
        {
            projection.ProjectionItem.Accept(forSubCommand());
            if (!String.IsNullOrWhiteSpace(projection.Alias) && !(projection.ProjectionItem is AllColumns))
            {
                writer.Write(" ");
                if (options.AliasProjectionsUsingAs)
                {
                    writer.Write("AS ");
                }
                writer.Write(projection.Alias);
            }
        }

        private void visitAliasedProjectionReference(AliasedProjection projection)
        {
            projection.ProjectionItem.Accept(forSubCommand());
        }

        private void visitAliasedProjectionAlias(AliasedProjection projection)
        {
            if (String.IsNullOrWhiteSpace(projection.Alias))
            {
                projection.ProjectionItem.Accept(forSubCommand());
            }
            else
            {
                writer.Write(projection.Alias);
            }
        }

        private void visitWindowFrameType(WindowFrame item)
        {
            FrameTypeConverter converter = new FrameTypeConverter();
            writer.Write(converter.ToString(item.FrameType));
        }

        private void visitOrderedFilter(OrderFilter item, string operationToken)
        {
            Action visitor = () =>
            {
                item.LeftHand.Accept(forSubCommand());
                writer.Write(" ");
                writer.Write(operationToken);
                writer.Write(" ");
                item.RightHand.Accept(forSubCommand());
            };
            visitFilter(item, visitor);

        }

        private void visitQuantifiedFilter(QuantifierFilter filter, string operationToken)
        {
            Action visitor = () =>
            {
                filter.LeftHand.Accept(forSubCommand());
                writer.Write(" ");
                writer.Write(operationToken);
                writer.Write(" ");
                QuantifierConverter converter = new QuantifierConverter();
                writer.Write(converter.ToString(filter.Quantifier));
                writer.Write(" ");
                filter.ValueProvider.Accept(forSubCommand());
            };
            visitFilter(filter, visitor);
        }

        private void visitFilter(IFilter item, Action visitor)
        {
            bool wrapInParentheses = shouldWrapInParentheses(item);
            if (wrapInParentheses)
            {
                writer.Write("(");
            }
            visitor();
            if (wrapInParentheses)
            {
                writer.Write(")");
            }
        }

        private bool shouldWrapInParentheses(IFilter filter)
        {
            return filter.WrapInParentheses ?? options.WrapFiltersInParentheses;
        }

        private void visitSelectCombiner(SelectCombiner combiner, string combinerToken)
        {
            bool needsParenthesis = level > 0;
            if (needsParenthesis)
            {
                writer.Write("(");
            }
            combiner.LeftHand.Accept(this);
            writer.Write(" ");
            writer.Write(combinerToken);
            writer.Write(" ");
            if (combiner.Distinct != DistinctQualifier.Default)
            {
                DistinctQualifierConverter converter = new DistinctQualifierConverter();
                writer.Write(converter.ToString(combiner.Distinct));
                writer.Write(" ");
            }
            combiner.RightHand.Accept(this);
            if (combiner.OrderBy.Any())
            {
                writer.Write(" ORDER BY ");
                join(", ", combiner.OrderBy);
            }
            if (needsParenthesis)
            {
                writer.Write(")");
            }
            if (combiner.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        private void visitBoundFrame(BoundFrame item)
        {
            IVisitableBuilder literal = new NumericLiteral(item.RowCount);
            literal.Accept(forSubCommand());
        }

        private void visitUnboundFrame(UnboundFrame item)
        {
            writer.Write("UNBOUNDED");
        }

        private void visitFilteredJoin(FilteredJoin item, string joinToken)
        {
            Action visitor = () =>
            {
                visitBinaryJoinInner(item, joinToken);
                writer.Write(" ON ");
                IVisitableBuilder filterGroup = item.OnFilterGroup;
                filterGroup.Accept(forSubCommand());
            };
            visitJoin(item, visitor);
        }

        private void visitBinaryJoin(BinaryJoin item, string joinToken)
        {
            Action visitor = () =>
            {
                visitBinaryJoinInner(item, joinToken);
            };
            visitJoin(item, visitor);
        }

        private void visitBinaryJoinInner(BinaryJoin item, string joinToken)
        {
            IJoinItem leftHand = item.LeftHand;
            leftHand.Accept(forSubCommand());
            writer.Write(" ");
            writer.Write(joinToken);
            writer.Write(" ");
            IJoinItem rightHand = item.RightHand;
            rightHand.Accept(forSubCommand());
        }

        private void visitJoin(Join join, Action visitor)
        {
            if (join.WrapInParentheses ?? options.WrapJoinsInParentheses)
            {
                writer.Write("(");
            }
            visitor();
            if (join.WrapInParentheses ?? options.WrapJoinsInParentheses)
            {
                writer.Write(")");
            }
        }

        private void visitMultipartIdentifier(Namespace item, string name)
        {
            List<string> parts = new List<string>();
            if (item != null)
            {
                parts.AddRange(item.Qualifiers);
            }
            parts.Add(name);
            string joined = String.Join(".", parts);
            writer.Write(joined);
        }

        private void join(string separator, IEnumerable<IVisitableBuilder> builders)
        {
            IVisitableBuilder first = builders.First();
            first.Accept(forSubCommand());
            foreach (IVisitableBuilder next in builders.Skip(1))
            {
                writer.Write(separator);
                next.Accept(forSubCommand());
            }
        }

        private void join(string separator, IEnumerable<AliasedProjection> projections)
        {
            AliasedProjection first = projections.First();
            visitAliasedProjection(first);
            foreach (AliasedProjection next in projections.Skip(1))
            {
                writer.Write(separator);
                visitAliasedProjection(next);
            }
        }

        private enum CommandType
        {
            Unknown,
            Select,
            Insert,
            Update,
            Delete,
            Create,
            Alter
        }

        private enum SourceReferenceType
        {
            Declaration,
            Reference
        }

        private enum ValueReferenceType
        {
            Declaration,
            Alias,
            Reference
        }

        #region DDL

        #region Create

        /// <summary>
        /// Generates the text for a Create builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitCreate(CreateBuilder item)
        {
            forCommandType(CommandType.Create).visitCreate(item);
        }

        private void visitCreate(CreateBuilder item)
        {
            writer.Write("CREATE ");

            item.CreateObject.Accept(this);

            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }
        }

        /// <summary>
        /// Generates the text for a Database builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDatabase(CreateDatabase item)
        {
            writer.Write("DATABASE ");
            writer.Write(item.Name);

            if (!string.IsNullOrWhiteSpace(item.Collation))
            {
                writer.Write(" COLLATE ");
                writer.Write(item.Collation);
            }
        }

        /// <summary>
        /// Generates the text for a TableDefinition builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitCreateTableDefinition(CreateTableDefinition item)
        {
            writer.Write("TABLE ");
            visitMultipartIdentifier(item.Qualifier, item.Name);

            if (item.Columns != null && item.Columns.Any())
            {
                writer.Write("(");

                join(", ", item.Columns);

                writer.Write(")");
            }
        }

        /// <summary>
        /// Generates the text for a DataType builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDataType(DataType item)
        {
            visitMultipartIdentifier(item.Qualifier, item.Name);

            bool hasArgs = item.Arguments != null && item.Arguments.Any();
            bool hasMax = item.HasMax;

            if (hasArgs || hasMax)
            {
                writer.Write("(");

                if (hasArgs)
                {
                    join(",", item.Arguments);
                }
                else if (hasMax)
                {
                    writer.Write("MAX");
                }
                writer.Write(")");
            }

            base.VisitDataType(item);
        }

        /// <summary>
        /// Generates the text for a AutoIncrement builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAutoIncrement(AutoIncrement item)
        {
            writer.Write("IDENTITY");
            if (item.Arguments != null && item.Arguments.Any())
            {
                writer.Write("(");
                if (item.Arguments.Any())
                {
                    join(",", item.Arguments);
                }
                writer.Write(")");
            }

            if (item.NotForReplication != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.NotForReplication).Accept(this);
            }
            base.VisitAutoIncrement(item);
        }

        /// <summary>
        /// Generates the text for a DefaultConstraint builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDefaultConstraint(DefaultConstraint item)
        {

            if (!string.IsNullOrWhiteSpace(item.ConstraintName))
            {
                writer.Write("CONSTRAINT ");
                writer.Write(item.ConstraintName);
                writer.Write(" ");
            }

            writer.Write("DEFAULT");
            if (item.Value != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.Value).Accept(this);
                return;
            }

            if (item.Function != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.Function).Accept(this);
                return;
            }

            base.VisitDefaultConstraint(item);
        }

        /// <summary>
        /// Generates the text for a PrimaryKeyConstraint builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitPrimaryKeyConstraint(PrimaryKeyConstraint item)
        {
            if (!string.IsNullOrWhiteSpace(item.ConstraintName))
            {
                writer.Write("CONSTRAINT ");
                writer.Write(item.ConstraintName);
                writer.Write(" ");
            }
            writer.Write("PRIMARY KEY");
            base.VisitPrimaryKeyConstraint(item);
        }

        /// <summary>
        /// Generates the text for a UniqueConstraint builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitUniqueConstraint(UniqueConstraint item)
        {
            if (!string.IsNullOrWhiteSpace(item.ConstraintName))
            {
                writer.Write("CONSTRAINT ");
                writer.Write(item.ConstraintName);
                writer.Write(" ");
            }
            writer.Write("UNIQUE");
            base.VisitUniqueConstraint(item);
        }

        /// <summary>
        /// Generates the text for a ForeignKeyConstraint builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitForeignKeyConstraint(ForeignKeyConstraint item)
        {
            if (!string.IsNullOrWhiteSpace(item.ConstraintName))
            {
                writer.Write("CONSTRAINT ");
                writer.Write(item.ConstraintName);
                writer.Write(" ");
            }
            writer.Write("FOREIGN KEY REFERENCES");
            if (item.ReferencedTable != null)
            {
                writer.Write(" ");
                this.VisitTable(item.ReferencedTable);
            }
            if (!string.IsNullOrWhiteSpace(item.ReferencedColumn))
            {
                writer.Write("(");
                writer.Write(item.ReferencedColumn);
                writer.Write(")");
            }
            if (item.OnDeleteAction != null)
            {
                writer.Write(" ON DELETE ");
                ((IVisitableBuilder)item.OnDeleteAction).Accept(this);
            }
            if (item.OnUpdateAction != null)
            {
                writer.Write(" ON UPDATE ");
                ((IVisitableBuilder)item.OnUpdateAction).Accept(this);
            }
            if (item.NotForReplication != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.NotForReplication).Accept(this);
            }

            base.VisitForeignKeyConstraint(item);
        }

        /// <summary>
        /// Generates the text for a CascadeAction builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitForeignKeyCascadeAction(CascadeAction item)
        {
            writer.Write("CASCADE");
            base.VisitForeignKeyCascadeAction(item);
        }

        /// <summary>
        /// Generates the text for a NoAction builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitForeignKeyNoAction(NoAction item)
        {
            writer.Write("NO ACTION");
            base.VisitForeignKeyNoAction(item);
        }

        /// <summary>
        /// Generates the text for a SetDefaultAction builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitForeignKeySetDefaultAction(SetDefaultAction item)
        {
            writer.Write("SET DEFAULT");
            base.VisitForeignKeySetDefaultAction(item);
        }

        /// <summary>
        /// Generates the text for a SetNullAction builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitForeignKeySetNullAction(SetNullAction item)
        {
            writer.Write("SET NULL");
            base.VisitForeignKeySetNullAction(item);
        }
        #endregion

        #region Alter

        /// <summary>
        /// Generates the text for a Alter builder.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAlter(AlterBuilder item)
        {
            forCommandType(CommandType.Alter).visitAlter(item);
        }

        private void visitAlter(AlterBuilder item)
        {
            writer.Write("ALTER ");

            item.AlterObject.Accept(this);

            if (item.HasTerminator)
            {
                writer.Write(options.Terminator);
            }

        }

        /// <summary>
        /// Generates the text for an Alter Database.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAlterDatabase(AlterDatabase item)
        {
            writer.Write("DATABASE ");
            if (!string.IsNullOrEmpty(item.Name))
            {
                writer.Write(item.Name);
            }
            else
            {
                if (item.CurrentDatabase)
                {
                    writer.Write("CURRENT");
                }
            }

            if (!string.IsNullOrWhiteSpace(item.NewDatabaseName))
            {
                writer.Write(" MODIFY NAME = ");
                writer.Write(item.NewDatabaseName);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(item.NewCollation))
                {
                    writer.Write(" COLLATE ");
                    writer.Write(item.NewCollation);
                }
            }

            base.VisitAlterDatabase(item);
        }

        /// <summary>
        /// Generates the text for an AlterTableDefinition.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAlterTableDefinition(AlterTableDefinition item)
        {
            writer.Write("TABLE ");
            writer.Write(item.Name);
            writer.Write(" ");

            if (item.Alteration != null)
            {
                ((IVisitableBuilder)item.Alteration).Accept(this);
            }

            base.VisitAlterTableDefinition(item);
        }

        /// <summary>
        /// Generates the text for an Alter Column.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAlterColumn(AlterColumn item)
        {
            writer.Write("ALTER COLUMN ");
            writer.Write(item.Name);

            if (item.DataType != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.DataType).Accept(this);
            }

            if (!string.IsNullOrEmpty(item.Collation))
            {
                writer.Write(" COLLATE ");
                writer.Write(item.Collation);
            }

            if (item.IsNullable.HasValue)
            {
                if (item.IsNullable.Value)
                {
                    writer.Write(" NULL");
                }
                else
                {
                    writer.Write(" NOT NULL");
                }
            }

            base.VisitAlterColumn(item);
        }

        /// <summary>
        /// Generates the text for an Alter Column Property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAlterColumnProperty(AlterColumnProperty item)
        {
            writer.Write("ALTER COLUMN ");
            writer.Write(item.Name);
            //  writer.Write(" ");

            switch (item.AlterType)
            {
                case AlterAction.Add:
                    writer.Write(" ADD ");
                    break;
                case AlterAction.Drop:
                    writer.Write(" DROP ");
                    break;
            }

            ((IVisitableBuilder)item.Property).Accept(this);
            base.VisitAlterColumnProperty(item);
        }

        /// <summary>
        /// Generates the text for a NotForReplicationColumnProperty column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitNotForReplicationColumnProperty(NotForReplicationColumnProperty item)
        {
            writer.Write("NOT FOR REPLICATION");
            base.VisitNotForReplicationColumnProperty(item);
        }

        /// <summary>
        /// Generates the text for a PERSISTED column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitPersistedColumnProperty(PersistedColumnProperty item)
        {
            writer.Write("PERSISTED");
            base.VisitPersistedColumnProperty(item);
        }

        /// <summary>
        /// Generates the text for a ROWGUIDCOL column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitRowGuidColumnProperty(RowGuidColumnProperty item)
        {
            writer.Write("ROWGUIDCOL");
            base.VisitRowGuidColumnProperty(item);
        }

        /// <summary>
        /// Generates the text for a Sparse column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitSparseColumnProperty(SparseColumnProperty item)
        {
            writer.Write("SPARSE");
            base.VisitSparseColumnProperty(item);
        }

        /// <summary>
        /// Generates the text for an Add Columns.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitAddColumns(AddColumns item)
        {
            if (item.Columns != null && item.Columns.Any())
            {
                writer.Write("ADD ");
                IVisitableBuilder first = item.Columns.First();
                first.Accept(this);

                foreach (IVisitableBuilder next in item.Columns.Skip(1))
                {
                    writer.Write(", ");
                    next.Accept(this);
                }
            }

            base.VisitAddColumns(item);
        }

        /// <summary>
        /// Generates the text for a DropItemsList column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDropItemsList(DropItemsList item)
        {
            if (item.Items != null && item.Items.Any())
            {
                writer.Write("DROP ");
                IVisitableBuilder first = item.Items.First();
                first.Accept(this);

                foreach (IVisitableBuilder next in item.Items.Skip(1))
                {
                    writer.Write(", ");
                    next.Accept(this);
                }
            }
            base.VisitDropItemsList(item);
        }

        /// <summary>
        /// Generates the text for a DropColumnsList column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDropColumnsList(DropColumnsList item)
        {
            if (item.Items != null && item.Items.Any())
            {
                writer.Write("COLUMN ");
                IVisitableBuilder first = item.Items.First();
                first.Accept(this);

                foreach (IVisitableBuilder next in item.Items.Skip(1))
                {
                    writer.Write(", ");
                    next.Accept(this);
                }
            }
            base.VisitDropColumnsList(item);
        }

        /// <summary>
        /// Generates the text for a DropColumn column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDropColumn(DropColumn item)
        {
            writer.Write(item.Name);
            base.VisitDropColumn(item);
        }

        /// <summary>
        /// Generates the text for a DropConstraintsList column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDropConstraintsList(DropConstraintsList item)
        {
            if (item.Items != null && item.Items.Any())
            {
                writer.Write("CONSTRAINT ");
                IVisitableBuilder first = item.Items.First();
                first.Accept(this);

                foreach (IVisitableBuilder next in item.Items.Skip(1))
                {
                    writer.Write(", ");
                    next.Accept(this);
                }
            }
            base.VisitDropConstraintsList(item);
        }

        /// <summary>
        /// Generates the text for a DropConstraint column property.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitDropConstraint(DropConstraint item)
        {
            writer.Write(item.Name);
            base.VisitDropConstraint(item);
        }

        #endregion

        /// <summary>
        /// Generates the text for a ColumnDefinition.
        /// </summary>
        /// <param name="item">The item to generate the text for.</param>
        protected internal override void VisitColumnDefinition(ColumnDefinition item)
        {
            writer.Write(item.Name);

            if (item.DataType != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.DataType).Accept(this);
            }

            if (!string.IsNullOrEmpty(item.Collation))
            {
                writer.Write(" COLLATE ");
                writer.Write(item.Collation);
            }

            if (item.IsNullable.HasValue)
            {
                if (item.IsNullable.Value)
                {
                    writer.Write(" NULL");
                }
                else
                {
                    writer.Write(" NOT NULL");
                }
            }

            if (item.Default != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.Default).Accept(this);
            }

            if (item.AutoIncrement != null)
            {
                writer.Write(" ");
                ((IVisitableBuilder)item.AutoIncrement).Accept(this);
            }

            if (item.IsRowGuid)
            {
                writer.Write(" ROWGUIDCOL");
            }

            // Now do the column constraint list. This will be primary key, unique, foreignkey, default.

            if (item.Constraints != null && item.Constraints.Any())
            {
                writer.Write(" ");
                join(" ", item.Constraints);
            }

            base.VisitColumnDefinition(item);
        }

        #endregion

    }
}
