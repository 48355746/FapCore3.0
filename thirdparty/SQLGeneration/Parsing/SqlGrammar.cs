using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Identifies the expressions making up a SQL statement.
    /// </summary>
    public class SqlGrammar : Grammar
    {
        private static SqlGrammar instance = new SqlGrammar();

        /// <summary>
        /// Initializes a new instance of a SqlGrammar.
        /// </summary>
        /// <param name="registry">The token registry to retrieve SQL tokens from.</param>
        public SqlGrammar(SqlTokenRegistry registry = null)
            : base(registry == null ? new SqlTokenRegistry() : registry)
        {
            defineStart();
            defineSelectStatement();
            defineSelectExpression();
            defineSelectCombiner();
            defineSelectSpecification();
            defineDistinctQualifier();
            defineOrderByList();
            defineOrderByItem();
            defineOrderDirection();
            //sql2012+分页
            defineOffsetFetch();
            DefineLimit();
            defineNullPlacement();
            defineAdditiveExpression();
            defineAdditiveOperator();
            defineMultiplicitiveExpression();
            defineMultiplicitiveOperator();
            defineProjectionList();
            defineProjectionItem();
            defineFromList();
            defineJoinItem();
            defineFunctionCall();
            defineFrameType();
            definePrecedingFrame();
            defineFollowingFrame();
            defineJoin();
            defineJoinPrime();
            defineFilteredJoinType();
            defineFilter();
            defineComparisonOperator();
            defineQuantifier();
            defineOrFilter();
            defineAndFilter();
            defineValueList();
            defineGroupByList();
            defineArithmeticItem();
            defineWrappedItem();
            defineItem();
            defineMatchCase();
            defineMatchList();
            defineMatchListPrime();
            defineMatch();
            defineConditionalCase();
            defineConditionList();
            defineConditionListPrime();
            defineCondition();
            defineOutput();
            defineInsertStatement();
            defineColumnList();
            defineUpdateStatement();
            defineSetterList();
            defineSetter();
            defineDeleteStatement();
            defineMultipartIdentifier();

            // ddl
            defineCreateStatement();
            defineCreateDatabase();
            defineCreateTable();
            defineColumnDefinitionsList();
            defineColumnDefinition();
            defineColumnConstraintList();
            defineColumnConstraint();
            defineForeignKeyConstraint();
            defineNotForReplication();
            defineAlterStatement();
            defineAlterDatabase();
            defineAlterTable();
            defineDataType();
            defineCollate();
            defineNullability();
            defineDropTableItemsList();
            defineDropTableItem();
            defineDropTableConstraintList();
            defineDropTableColumnList();
            defineDropTableColumn();
            defineDropTableConstraint();
        }

        /// <summary>
        /// Gets the default instance of the SqlGrammar.
        /// </summary>
        public static SqlGrammar Default
        {
            get { return instance; }
        }

        #region Start

        /// <summary>
        /// Describes the structure of the top-level SQL grammar.
        /// </summary>
        public static class Start
        {
            /// <summary>
            /// Gets the identifier representing the start expression.
            /// </summary>
            public const string Name = "Start";

            /// <summary>
            /// Gets the name for the SELECT statement option.
            /// </summary>
            public const string SelectStatement = "select_statement";

            /// <summary>
            /// Gets the name for the INSERT statement option.
            /// </summary>
            public const string InsertStatement = "insert_statement";

            /// <summary>
            /// Gets the name for the UPDATE statement option.
            /// </summary>
            public const string UpdateStatement = "update_statement";

            /// <summary>
            /// Gets the name for the DELETE statement option.
            /// </summary>
            public const string DeleteStatement = "delete_statement";

            /// <summary>
            /// Gets the name for the Terminator of a statement.
            /// </summary>
            public const string Terminator = "terminator";

            /// <summary>
            /// Gets the name for the CREATE statement option.
            /// </summary>
            public const string CreateStatement = "create_statement";

            /// <summary>
            /// Gets the name for the CREATE statement option.
            /// </summary>
            public const string AlterStatement = "alter_statement";


        }

        private void defineStart()
        {
            Define(Start.Name)
                .Add(true, Options()
                  .Add(Start.SelectStatement, Expression(SelectStatement.Name))
                  .Add(Start.InsertStatement, Expression(InsertStatement.Name))
                  .Add(Start.UpdateStatement, Expression(UpdateStatement.Name))
                  .Add(Start.DeleteStatement, Expression(DeleteStatement.Name))
                  .Add(Start.CreateStatement, Expression(CreateStatement.Name))
                  .Add(Start.AlterStatement, Expression(AlterStatement.Name))
                    )
                .Add(Start.Terminator, false, Token(SqlTokenRegistry.LineTerminator));
        }

        #endregion

        #region SelectStatement

        /// <summary>
        /// Describes the structure of the SELECT statement.
        /// </summary>
        public static class SelectStatement
        {
            /// <summary>
            /// Gets the identifier representing the SELECT statement.
            /// </summary>
            public const string Name = "SelectStatement";

            /// <summary>
            /// Gets the name of the SELECT expression.
            /// </summary>
            public const string SelectExpression = "select_expression";

            /// <summary>
            /// Describes the structure of the optional ORDER BY clause.
            /// </summary>
            public static class OrderBy
            {
                /// <summary>
                /// Gets the identifier representing the ORDER BY clause.
                /// </summary>
                public const string Name = "OrderBy";

                /// <summary>
                /// Gets the name representing the ORDER BY keyword.
                /// </summary>
                public const string OrderByKeyword = "order_by";

                /// <summary>
                /// Gets the name representing the ORDER BY list.
                /// </summary>
                public const string OrderByList = "order_by_list";
            }
        }

        private void defineSelectStatement()
        {
            Define(SelectStatement.Name)
                .Add(SelectStatement.SelectExpression, true, Expression(SelectExpression.Name))
                .Add(SelectStatement.OrderBy.Name, false, Define()
                    .Add(SelectStatement.OrderBy.OrderByKeyword, true, Token(SqlTokenRegistry.OrderBy))
                    .Add(SelectStatement.OrderBy.OrderByList, true, Expression(OrderByList.Name)));
        }

        #endregion

        #region SelectExpression

        /// <summary>
        /// Describes the structure of the SELECT expression.
        /// </summary>
        public static class SelectExpression
        {
            /// <summary>
            /// Gets the name identifying the SELECT expression.
            /// </summary>
            public const string Name = "SelectExpression";

            /// <summary>
            /// Describes the structure of the leading SELECT expression when it is surrounded by parenthesis.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the name identifying the leading SELECT expression when it is surrounded by parenthesis.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the left parenthesis identifier.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the SELECT expression identifier.
                /// </summary>
                public const string SelectExpression = "select_expression";

                /// <summary>
                /// Gets the right parenthesis identifier.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Gets the SELECT specification identifier.
            /// </summary>
            public const string SelectSpecification = "select_specification";

            /// <summary>
            /// Describes the structure of a compound SELECT statement.
            /// </summary>
            public static class Remaining
            {
                /// <summary>
                /// Gets the identifier for a compound SELECT statement.
                /// </summary>
                public const string Name = "Remaining";

                /// <summary>
                /// Gets the SELECT statement combiner identifier.
                /// </summary>
                public const string Combiner = "combiner";

                /// <summary>
                /// Gets the identifier for the distinct qualifier.
                /// </summary>
                public const string DistinctQualifier = "distinct_qualifier";

                /// <summary>
                /// Gets the SELECT expression identifier.
                /// </summary>
                public const string SelectExpression = "select_expression";
            }
        }

        private void defineSelectExpression()
        {
            Define(SelectExpression.Name)
                .Add(true, Options()
                    .Add(SelectExpression.Wrapped.Name, Define()
                        .Add(SelectExpression.Wrapped.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(SelectExpression.Wrapped.SelectExpression, true, Expression(SelectExpression.Name))
                        .Add(SelectExpression.Wrapped.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(SelectExpression.SelectSpecification, Expression(SelectSpecification.Name)))
                .Add(SelectExpression.Remaining.Name, false, Define()
                    .Add(SelectExpression.Remaining.Combiner, true, Expression(SelectCombiner.Name))
                    .Add(SelectExpression.Remaining.DistinctQualifier, false, Expression(DistinctQualifier.Name))
                    .Add(SelectExpression.Remaining.SelectExpression, true, Expression(SelectExpression.Name)));
        }

        #endregion

        #region SelectCombiner

        /// <summary>
        /// Describes the options for a SELECT combiner.
        /// </summary>
        public static class SelectCombiner
        {
            /// <summary>
            /// Gets the identifier indicating that the token is a SELECT combiner.
            /// </summary>
            public const string Name = "SelectCombiner";

            /// <summary>
            /// Gets the identifier for the UNION keyword.
            /// </summary>
            public const string Union = "Union";

            /// <summary>
            /// Gets the identifier for the INTERSECT keyword.
            /// </summary>
            public const string Intersect = "Intersect";

            /// <summary>
            /// Gets the identifier for the EXCEPT keyword.
            /// </summary>
            public const string Except = "Except";

            /// <summary>
            /// Gets the identifier for the MINUS keyword.
            /// </summary>
            public const string Minus = "Minus";
        }

        private void defineSelectCombiner()
        {
            Define(SelectCombiner.Name)
                .Add(true, Options()
                    .Add(SelectCombiner.Union, Token(SqlTokenRegistry.Union))
                    .Add(SelectCombiner.Intersect, Token(SqlTokenRegistry.Intersect))
                    .Add(SelectCombiner.Except, Token(SqlTokenRegistry.Except))
                    .Add(SelectCombiner.Minus, Token(SqlTokenRegistry.Minus)));
        }

        #endregion

        #region SelectSpecification

        /// <summary>
        /// Describes the structure of the SELECT specification.
        /// </summary>
        public static class SelectSpecification
        {
            /// <summary>
            /// Gets the name identifying the SELECT specification.
            /// </summary>
            public const string Name = "SelectSpecification";

            /// <summary>
            /// Gets the SELECT keyword identifier.
            /// </summary>
            public const string SelectKeyword = "select";

            /// <summary>
            /// Gets the distinct qualifier identifier.
            /// </summary>
            public const string DistinctQualifier = "distinct_qualifier";

            /// <summary>
            /// Describes the structure of the TOP clause.
            /// </summary>
            public static class Top
            {
                /// <summary>
                /// Gets the identifier for the TOP expression.
                /// </summary>
                public const string Name = "Top";

                /// <summary>
                /// Gets the TOP keyword identifier.
                /// </summary>
                public const string TopKeyword = "top";

                /// <summary>
                /// Gets the expression identifier.
                /// </summary>
                public const string Expression = "expression";

                /// <summary>
                /// Gets the PERCENT keyword identifier.
                /// </summary>
                public const string PercentKeyword = "percent";

                /// <summary>
                /// Gets the WITH TIES keyword identifier.
                /// </summary>
                public const string WithTiesKeyword = "with_ties";
            }

            /// <summary>
            /// Get the projection list identifier.
            /// </summary>
            public const string ProjectionList = "projection_list";

            /// <summary>
            /// Describes the structure of the FROM clause.
            /// </summary>
            public static class From
            {
                /// <summary>
                /// Gets the identifier for the FROM expression.
                /// </summary>
                public const string Name = "From";

                /// <summary>
                /// Gets the FROM keyword identifier.
                /// </summary>
                public const string FromKeyword = "from";

                /// <summary>
                /// Gets the from list identifier.
                /// </summary>
                public const string FromList = "from_list";
            }

            /// <summary>
            /// Describes the structure of the WHERE clause.
            /// </summary>
            public static class Where
            {
                /// <summary>
                /// Get the identifier for the WHERE clause.
                /// </summary>
                public const string Name = "Where";

                /// <summary>
                /// Gets the WHERE keyword identifier.
                /// </summary>
                public const string WhereKeyword = "where";

                /// <summary>
                /// Gets the filter list identifier.
                /// </summary>
                public const string FilterList = "filter_list";
            }

            /// <summary>
            /// Describes the structure of the GROUP BY clause.
            /// </summary>
            public static class GroupBy
            {
                /// <summary>
                /// Gets the identifier for the GROUP BY clause.
                /// </summary>
                public const string Name = "GroupBy";

                /// <summary>
                /// Gets the GROUP BY keyword identifier.
                /// </summary>
                public const string GroupByKeyword = "group_by";

                /// <summary>
                /// Gets the group by list identifier.
                /// </summary>
                public const string GroupByList = "group_by_list";
            }

            /// <summary>
            /// Describes the structure of the HAVING clause.
            /// </summary>
            public static class Having
            {
                /// <summary>
                /// Gets the identifier for the HAVING clause.
                /// </summary>
                public const string Name = "Having";

                /// <summary>
                /// Gets the HAVING keyword identifier.
                /// </summary>
                public const string HavingKeyword = "having";

                /// <summary>
                /// Gets the filter list identifier.
                /// </summary>
                public const string FilterList = "filter_list";
            }
        }

        private void defineSelectSpecification()
        {
            Define(SelectSpecification.Name)
                .Add(SelectSpecification.SelectKeyword, true, Token(SqlTokenRegistry.Select))
                .Add(SelectSpecification.DistinctQualifier, false, Expression(DistinctQualifier.Name))
                .Add(SelectSpecification.Top.Name, false, Define()
                    .Add(SelectSpecification.Top.TopKeyword, true, Token(SqlTokenRegistry.Top))
                    .Add(SelectSpecification.Top.Expression, true, Expression(ArithmeticItem.Name))
                    .Add(SelectSpecification.Top.PercentKeyword, false, Token(SqlTokenRegistry.Percent))
                    .Add(SelectSpecification.Top.WithTiesKeyword, false, Token(SqlTokenRegistry.WithTies)))
                .Add(SelectSpecification.ProjectionList, true, Expression(ProjectionList.Name))
                .Add(SelectSpecification.From.Name, false, Define()
                    .Add(SelectSpecification.From.FromKeyword, true, Token(SqlTokenRegistry.From))
                    .Add(SelectSpecification.From.FromList, true, Expression(FromList.Name)))
                .Add(SelectSpecification.Where.Name, false, Define()
                    .Add(SelectSpecification.Where.WhereKeyword, true, Token(SqlTokenRegistry.Where))
                    .Add(SelectSpecification.Where.FilterList, true, Expression(OrFilter.Name)))
                .Add(SelectSpecification.GroupBy.Name, false, Define()
                    .Add(SelectSpecification.GroupBy.GroupByKeyword, true, Token(SqlTokenRegistry.GroupBy))
                    .Add(SelectSpecification.GroupBy.GroupByList, true, Expression(GroupByList.Name)))
                .Add(SelectSpecification.Having.Name, false, Define()
                    .Add(SelectSpecification.Having.HavingKeyword, true, Token(SqlTokenRegistry.Having))
                    .Add(SelectSpecification.Having.FilterList, true, Expression(OrFilter.Name)));
        }

        #endregion

        #region DistinctQualifier

        /// <summary>
        /// Describes the options for a distinct qualifier.
        /// </summary>
        public static class DistinctQualifier
        {
            /// <summary>
            /// Gets the identifier indicating that the token is a distinct qualifier.
            /// </summary>
            public const string Name = "DistinctQualifier";

            /// <summary>
            /// Gets the identifier for the DISTINCT keyword.
            /// </summary>
            public const string Distinct = "distinct";

            /// <summary>
            /// Gets the identifier for the ALL keyword.
            /// </summary>
            public const string All = "all";
        }

        private void defineDistinctQualifier()
        {
            Define(DistinctQualifier.Name)
                .Add(true, Options()
                    .Add(DistinctQualifier.Distinct, Token(SqlTokenRegistry.Distinct))
                    .Add(DistinctQualifier.All, Token(SqlTokenRegistry.All)));
        }

        #endregion

        #region OrderByList

        /// <summary>
        /// Describes the structure of the ORDER BY list.
        /// </summary>
        public static class OrderByList
        {
            /// <summary>
            /// Gets the name identifying the ORDER BY list.
            /// </summary>
            public const string Name = "OrderByList";

            /// <summary>
            /// Describes the structure of an order by list containing multiple items.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier for the multiple option.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the first order by item identifier.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the order by list.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier for a single order by item.
            /// </summary>
            public const string Single = "single";
        }

        private void defineOrderByList()
        {
            Define(OrderByList.Name)
                .Add(true, Options()
                    .Add(OrderByList.Multiple.Name, Define()
                        .Add(OrderByList.Multiple.First, true, Expression(OrderByItem.Name))
                        .Add(OrderByList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(OrderByList.Multiple.Remaining, true, Expression(OrderByList.Name)))
                    .Add(OrderByList.Single, Expression(OrderByItem.Name)));
        }

        #endregion

        #region OrderByItem

        /// <summary>
        /// Describes the structure of the ORDER BY item.
        /// </summary>
        public static class OrderByItem
        {
            /// <summary>
            /// Gets the name identifying the ORDER BY item.
            /// </summary>
            public const string Name = "OrderByItem";

            /// <summary>
            /// Gets the identifier for the item being ordered.
            /// </summary>
            public const string Expression = "expression";

            /// <summary>
            /// Gets the identifier for the direction specifier.
            /// </summary>
            public const string OrderDirection = "order_direction";

            /// <summary>
            /// Gets the identifier for the null placement specifier.
            /// </summary>
            public const string NullPlacement = "null_placement";
            /// <summary>
            /// sql2012+分页
            /// </summary>
            public const string PageOffsetFetch = "page_offset_fetch";
            /// <summary>
            /// Gets the identifier for the limit specifier.
            /// </summary>
            public const string PageLimit = "page_limit";
        }

        private void defineOrderByItem()
        {
            Define(OrderByItem.Name)
                .Add(OrderByItem.Expression, true, Expression(ArithmeticItem.Name))
                .Add(OrderByItem.OrderDirection, false, Expression(OrderDirection.Name))
                .Add(OrderByItem.NullPlacement, false, Expression(NullPlacement.Name))
                .Add(OrderByItem.PageOffsetFetch, false, Expression(OffsetFetch.Name))
                .Add(OrderByItem.PageLimit,false,Expression(Limit.Name));
        }

        #endregion

        #region OrderDirection

        /// <summary>
        /// Describes the options for an ORDER BY direction.
        /// </summary>
        public static class OrderDirection
        {
            /// <summary>
            /// Gets the identifier indicating that the token is an ORDER direction.
            /// </summary>
            public const string Name = "OrderDirection";

            /// <summary>
            /// Gets the identifier for the DESC keyword.
            /// </summary>
            public const string Descending = "Descending";

            /// <summary>
            /// Gets the identifier for the ASC keyword.
            /// </summary>
            public const string Ascending = "Ascending";
        }

        private void defineOrderDirection()
        {
            Define(OrderDirection.Name)
                .Add(true, Options()
                    .Add(OrderDirection.Descending, Token(SqlTokenRegistry.Descending))
                    .Add(OrderDirection.Ascending, Token(SqlTokenRegistry.Ascending)));
        }

        #endregion

        #region OffsetFetch
        public static class OffsetFetch
        {
            public const string Name = "OffsetFetch";
            public const string OffsetFetchItem = "page_offset_fetch";
        }
        private void defineOffsetFetch()
        {
            Define(OffsetFetch.Name).Add(OffsetFetch.OffsetFetchItem, true, Token(SqlTokenRegistry.OffsetFetch));
        }
        #endregion

        #region Limit
        public static class Limit
        {
            public const string Name = "Limit";
            public const string LimitItem = "page_limit";
        }
        private void DefineLimit()
        {
            Define(Limit.Name).Add(Limit.LimitItem, true, Token(SqlTokenRegistry.Limit));
        }
        #endregion

        #region NullPlacement

        /// <summary>
        /// Describes the options for null placement.
        /// </summary>
        public static class NullPlacement
        {
            /// <summary>
            /// Gets the identifier indicating that the token is a null placement keyword.
            /// </summary>
            public const string Name = "NullPlacement";

            /// <summary>
            /// Gets the identifier for the NULLS FIRST keyword.
            /// </summary>
            public const string NullsFirst = "NullsFirst";

            /// <summary>
            /// Gets the identifier for the NULLS LAST keyword.
            /// </summary>
            public const string NullsLast = "NullsLast";
        }

        private void defineNullPlacement()
        {
            Define(NullPlacement.Name)
                .Add(true, Options()
                    .Add(NullPlacement.NullsFirst, Token(SqlTokenRegistry.NullsFirst))
                    .Add(NullPlacement.NullsLast, Token(SqlTokenRegistry.NullsLast)));
        }

        #endregion

        #region AdditiveExpression

        /// <summary>
        /// Describes the structure of an arithmetic expression adding or substracting two values.
        /// </summary>
        public static class AdditiveExpression
        {
            /// <summary>
            /// Gets the identifier indicating that the expression adds or substracts two values.
            /// </summary>
            public const string Name = "AdditiveExpression";

            /// <summary>
            /// Describes the structure of a additive expression adding or substracting multiple values.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that the expression is adding or subtracting multiple values.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first operand.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identfier for the operator.
                /// </summary>
                public const string Operator = "operator";

                /// <summary>
                /// Gets the identifier for the rest of the arithmetic expression.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that the expression is a single value, a multiplication or division.
            /// </summary>
            public const string Single = "single";
        }

        private void defineAdditiveExpression()
        {
            Define(AdditiveExpression.Name)
                .Add(true, Options()
                    .Add(AdditiveExpression.Multiple.Name, Define()
                        .Add(AdditiveExpression.Multiple.First, true, Expression(MultiplicitiveExpression.Name))
                        .Add(AdditiveExpression.Multiple.Operator, true, Expression(AdditiveOperator.Name))
                        .Add(AdditiveExpression.Multiple.Remaining, true, Expression(AdditiveExpression.Name)))
                    .Add(AdditiveExpression.Single, Expression(MultiplicitiveExpression.Name)));
        }

        #endregion

        #region AdditiveOperator

        /// <summary>
        /// Describes the structure of an arithmetic operator doing addition or subtraction.
        /// </summary>
        public static class AdditiveOperator
        {
            /// <summary>
            /// Gets the identifier indicating that the operator is addition or subtraction.
            /// </summary>
            public const string Name = "AdditiveOperator";

            /// <summary>
            /// Gets the identifier for the addition operator.
            /// </summary>
            public const string PlusOperator = "plus_operator";

            /// <summary>
            /// Gets the identifier for the subtraction operator.
            /// </summary>
            public const string MinusOperator = "minus_operator";
        }

        private void defineAdditiveOperator()
        {
            Define(AdditiveOperator.Name)
                .Add(true, Options()
                    .Add(AdditiveOperator.PlusOperator, Token(SqlTokenRegistry.PlusOperator))
                    .Add(AdditiveOperator.MinusOperator, Token(SqlTokenRegistry.MinusOperator)));
        }

        #endregion

        #region MultiplicitiveExpression

        /// <summary>
        /// Describes the structure of an arithmetic expression multiplying or dividing values.
        /// </summary>
        public static class MultiplicitiveExpression
        {
            /// <summary>
            /// Gets the identifier indicating that there are values being multiplied or divided.
            /// </summary>
            public const string Name = "MultiplicitiveExpression";

            /// <summary>
            /// Describes the structure of an expression when multiple values are being multiplied and divided together.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that multiple values are being multiplied or divided together.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first operand.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the operator.
                /// </summary>
                public const string Operator = "operator";

                /// <summary>
                /// Gets the identifier for the rest of the arithmetic expression.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier for a single item.
            /// </summary>
            public const string Single = "single";
        }

        private void defineMultiplicitiveExpression()
        {
            Define(MultiplicitiveExpression.Name)
                .Add(true, Options()
                    .Add(MultiplicitiveExpression.Multiple.Name, Define()
                        .Add(MultiplicitiveExpression.Multiple.First, true, Expression(WrappedItem.Name))
                        .Add(MultiplicitiveExpression.Multiple.Operator, true, Expression(MultiplicitiveOperator.Name))
                        .Add(MultiplicitiveExpression.Multiple.Remaining, true, Expression(MultiplicitiveExpression.Name)))
                    .Add(MultiplicitiveExpression.Single, Expression(WrappedItem.Name)));
        }

        #endregion

        #region MultiplicitiveOperator

        /// <summary>
        /// Describes the structure of a multiplicitive operator.
        /// </summary>
        public static class MultiplicitiveOperator
        {
            /// <summary>
            /// Gets the identifier indicating that the operator is multiplication or division.
            /// </summary>
            public const string Name = "MultiplicitiveOperator";

            /// <summary>
            /// Gets the identifier indicating that the operation is multiplication.
            /// </summary>
            public const string Multiply = "multiply";

            /// <summary>
            /// Gets the identifier indicating that the operation is division.
            /// </summary>
            public const string Divide = "divide";

            /// <summary>
            /// Gets the identfier indicating that the operation is modulus.
            /// </summary>
            public const string Modulus = "modulus";
        }

        private void defineMultiplicitiveOperator()
        {
            Define(MultiplicitiveOperator.Name)
                .Add(true, Options()
                    .Add(MultiplicitiveOperator.Multiply, Token(SqlTokenRegistry.MultiplicationOperator))
                    .Add(MultiplicitiveOperator.Divide, Token(SqlTokenRegistry.DivisionOperator))
                    .Add(MultiplicitiveOperator.Modulus, Token(SqlTokenRegistry.ModulusOperator)));
        }

        #endregion

        #region ProjectionList

        /// <summary>
        /// Describes the structure of the projection list.
        /// </summary>
        public static class ProjectionList
        {
            /// <summary>
            /// Gets the name identifying the projection list.
            /// </summary>
            public const string Name = "ProjectionList";

            /// <summary>
            /// Describes the structure of a projection list containing multiple items.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that the projection list contains multiple items.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first projection item.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the projection list.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that there is one item in the projection list.
            /// </summary>
            public const string Single = "single";
        }

        private void defineProjectionList()
        {
            Define(ProjectionList.Name)
                .Add(true, Options()
                    .Add(ProjectionList.Multiple.Name, Define()
                        .Add(ProjectionList.Multiple.First, true, Expression(ProjectionItem.Name))
                        .Add(ProjectionList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(ProjectionList.Multiple.Remaining, true, Expression(ProjectionList.Name)))
                    .Add(ProjectionList.Single, Expression(ProjectionItem.Name)));
        }

        #endregion

        #region ProjectionItem

        /// <summary>
        /// Describes the structure of the projection item.
        /// </summary>
        public static class ProjectionItem
        {
            /// <summary>
            /// Gets the name identifying the projection item.
            /// </summary>
            public const string Name = "ProjectionItem";

            /// <summary>
            /// Describes the structure of the projection item when it is a star (*).
            /// </summary>
            public static class Star
            {
                /// <summary>
                /// Gets the identifier indicating that the projection item is a star (*).
                /// </summary>
                public const string Name = "Star";

                /// <summary>
                /// Describes the structure of a star projection item (*) that is qualified.
                /// </summary>
                public static class Qualifier
                {
                    /// <summary>
                    /// Gets the identifier indicating that the star projection item (*) is qualified.
                    /// </summary>
                    public const string Name = "Qualifier";

                    /// <summary>
                    /// Gets the identifier for the qualifier.
                    /// </summary>
                    public const string ColumnSource = "column_source";

                    /// <summary>
                    /// Gets the identifier for the dot separator.
                    /// </summary>
                    public const string Dot = "dot";
                }

                /// <summary>
                /// Gets the identifier for the star (*) token.
                /// </summary>
                public const string StarToken = "star";
            }

            /// <summary>
            /// Describes the structure of a projection item when it is a column, function call, SELECT statement or arithmetic expression.
            /// </summary>
            public static class Expression
            {
                /// <summary>
                /// Gets the identifier indicating that the expression is a column, function call, SELECT statement of arithmetic expression.
                /// </summary>
                public const string Name = "Expression";

                /// <summary>
                /// Gets the identifier for the item.
                /// </summary>
                public const string Item = "item";

                /// <summary>
                /// Describes the structure of an alias of a projection item.
                /// </summary>
                public static class AliasExpression
                {
                    /// <summary>
                    /// Gets the identifier indicating that the projection item has an alias.
                    /// </summary>
                    public const string Name = "Alias";

                    /// <summary>
                    /// Gets the identifier for the AS token.
                    /// </summary>
                    public const string AliasIndicator = "alias_indicator";

                    /// <summary>
                    /// Gets the identifier for the alias.
                    /// </summary>
                    public const string Alias = "alias";
                }
            }
        }

        private void defineProjectionItem()
        {
            Define(ProjectionItem.Name)
                .Add(true, Options()
                    .Add(ProjectionItem.Star.Name, Define()
                        .Add(ProjectionItem.Star.Qualifier.Name, false, Define()
                            .Add(ProjectionItem.Star.Qualifier.ColumnSource, true, Expression(MultipartIdentifier.Name))
                            .Add(ProjectionItem.Star.Qualifier.Dot, true, Token(SqlTokenRegistry.Dot)))
                        .Add(ProjectionItem.Star.StarToken, true, Token(SqlTokenRegistry.MultiplicationOperator)))
                    .Add(ProjectionItem.Expression.Name, Define()
                        .Add(ProjectionItem.Expression.Item, true, Expression(ArithmeticItem.Name))
                        .Add(ProjectionItem.Expression.AliasExpression.Name, false, Define()
                            .Add(ProjectionItem.Expression.AliasExpression.AliasIndicator, false, Token(SqlTokenRegistry.As))
                            .Add(ProjectionItem.Expression.AliasExpression.Alias, true, Token(SqlTokenRegistry.Identifier)))));
        }

        #endregion

        #region FromList

        /// <summary>
        /// Describes the structure of the FROM list.
        /// </summary>
        public static class FromList
        {
            /// <summary>
            /// Gets the name identifying the FROM list.
            /// </summary>
            public const string Name = "FromList";

            /// <summary>
            /// Describes the structure of multiple sources in a FROM clause.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that the list contains multiple sources.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first source.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the sources.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that the list only has one source.
            /// </summary>
            public const string Single = "single";
        }

        private void defineFromList()
        {
            Define(FromList.Name)
                .Add(true, Options()
                    .Add(FromList.Multiple.Name, Define()
                        .Add(FromList.Multiple.First, true, Expression(Join.Name))
                        .Add(FromList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(FromList.Multiple.Remaining, true, Expression(FromList.Name)))
                    .Add(FromList.Single, Expression(Join.Name)));
        }

        #endregion

        #region JoinItem

        /// <summary>
        /// Describes the structure of the join item.
        /// </summary>
        public static class JoinItem
        {
            /// <summary>
            /// Gets the name identifying the join item.
            /// </summary>
            public const string Name = "JoinItem";

            /// <summary>
            /// Gets the identifier indicating that the join item is a table.
            /// </summary>
            public const string Table = "table";

            /// <summary>
            /// Gets the identifier indicating that the join item is a function call.
            /// </summary>
            public const string FunctionCall = "function_call";

            /// <summary>
            /// Describes the structure of a select statement as a table source.
            /// </summary>
            public static class Select
            {
                /// <summary>
                /// Gets the identifier indicating that the table source is a select statement.
                /// </summary>
                public const string Name = "Select";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the select statement.
                /// </summary>
                public const string SelectStatement = "select_statement";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of the join item alias.
            /// </summary>
            public static class AliasExpression
            {
                /// <summary>
                /// Gets the identifier indicating that the join item is aliased.
                /// </summary>
                public const string Name = "Alias";

                /// <summary>
                /// Gets the identifier for the alias indicator (AS).
                /// </summary>
                public const string AliasIndicator = "alias_indicator";

                /// <summary>
                /// Gets the identifier for the alias.
                /// </summary>
                public const string Alias = "alias";
            }
        }

        private void defineJoinItem()
        {
            Define(JoinItem.Name)
                .Add(true, Options()
                    .Add(JoinItem.FunctionCall, Expression(FunctionCall.Name))
                    .Add(JoinItem.Table, Expression(MultipartIdentifier.Name))
                    .Add(JoinItem.Select.Name, Define()
                        .Add(JoinItem.Select.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(JoinItem.Select.SelectStatement, true, Expression(SelectStatement.Name))
                        .Add(JoinItem.Select.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))))
                .Add(JoinItem.AliasExpression.Name, false, Define()
                    .Add(JoinItem.AliasExpression.AliasIndicator, false, Token(SqlTokenRegistry.As))
                    .Add(JoinItem.AliasExpression.Alias, true, Token(SqlTokenRegistry.Identifier)));
        }

        #endregion

        #region FunctionCall

        /// <summary>
        /// Describes the structure of the function call.
        /// </summary>
        public static class FunctionCall
        {
            /// <summary>
            /// Gets the name identifying the function call.
            /// </summary>
            public const string Name = "FunctionCall";

            /// <summary>
            /// Gets the identifier for the function name.
            /// </summary>
            public const string FunctionName = "function_name";

            /// <summary>
            /// Gets the identifier for the left parenthesis.
            /// </summary>
            public const string LeftParethesis = "left_parenthesis";

            /// <summary>
            /// Gets the identifier for the function arguments.
            /// </summary>
            public const string Arguments = "arguments";

            /// <summary>
            /// Gets the identifier for the right parenthesis.
            /// </summary>
            public const string RightParenthesis = "right_parenthesis";

            /// <summary>
            /// Describes the structure of an analytical function window specification.
            /// </summary>
            public static class Window
            {
                /// <summary>
                /// Gets the identifier indicating that function has windowing applied to it.
                /// </summary>
                public const string Name = "Window";

                /// <summary>
                /// Gets the identifier for the OVER keyword.
                /// </summary>
                public const string Over = "over";

                /// <summary>
                /// Gets the identifier for the left parenthesis surrounding the window specification.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Describes the structure of a partitioned window.
                /// </summary>
                public static class Partitioning
                {
                    /// <summary>
                    /// Gets the identifier indicating that the window is partitioned.
                    /// </summary>
                    public const string Name = "Partitioning";

                    /// <summary>
                    /// Gets the identifier for the PARTITION BY keyword.
                    /// </summary>
                    public const string PartitionBy = "partition_by";

                    /// <summary>
                    /// Gets the identifier for the value list.
                    /// </summary>
                    public const string ValueList = "value_list";
                }

                /// <summary>
                /// Describes the structure of an ordered window.
                /// </summary>
                public static class Ordering
                {
                    /// <summary>
                    /// Gets the identifier indicating that the window is ordered.
                    /// </summary>
                    public const string Name = "Ordering";

                    /// <summary>
                    /// Gets the identifier for the ORDER BY keyword.
                    /// </summary>
                    public const string OrderByKeyword = "order_by";

                    /// <summary>
                    /// Gets the identifier for the ORDER BY list.
                    /// </summary>
                    public const string OrderByList = "order_by_list";
                }

                /// <summary>
                /// Describes the structure of a window that frames its partitions.
                /// </summary>
                public static class Framing
                {
                    /// <summary>
                    /// Gets the identifier indicating that the window uses framed.
                    /// </summary>
                    public const string Name = "Framing";

                    /// <summary>
                    /// Gets the identifier for the frame type.
                    /// </summary>
                    public const string FrameType = "frame_type";

                    /// <summary>
                    /// Gets the identifier indicating that the frame is determined by preceding items only.
                    /// </summary>
                    public const string PrecedingFrame = "preceding_frame";

                    /// <summary>
                    /// Describes the structure of a window frame that exists within a range of rows.
                    /// </summary>
                    public static class BetweenFrame
                    {
                        /// <summary>
                        /// Gets the identifier indicating that the frame exists within a range of rows.
                        /// </summary>
                        public const string Name = "BetweenFrame";

                        /// <summary>
                        /// The identifier for the BETWEEN keyword.
                        /// </summary>
                        public const string BetweenKeyword = "between";

                        /// <summary>
                        /// Gets the identifier for the preceding frame.
                        /// </summary>
                        public const string PrecedingFrame = "preceding_frame";

                        /// <summary>
                        /// Gets the identifier for the AND keyword.
                        /// </summary>
                        public const string AndKeyword = "and";

                        /// <summary>
                        /// Gets the keyword for the following frame."
                        /// </summary>
                        public const string FollowingFrame = "following_frame";
                    }
                }

                /// <summary>
                /// Gets the identifier for the right parenthesis surrounding the window specification.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }
        }

        private void defineFunctionCall()
        {
            Define(FunctionCall.Name)
                .Add(FunctionCall.FunctionName, true, Expression(MultipartIdentifier.Name))
                .Add(FunctionCall.LeftParethesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                .Add(FunctionCall.Arguments, false, Expression(ValueList.Name))
                .Add(FunctionCall.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))
                .Add(FunctionCall.Window.Name, false, Define()
                    .Add(FunctionCall.Window.Over, true, Token(SqlTokenRegistry.Over))
                    .Add(FunctionCall.Window.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                    .Add(FunctionCall.Window.Partitioning.Name, false, Define()
                        .Add(FunctionCall.Window.Partitioning.PartitionBy, true, Token(SqlTokenRegistry.PartitionBy))
                        .Add(FunctionCall.Window.Partitioning.ValueList, true, Expression(ValueList.Name)))
                    .Add(FunctionCall.Window.Ordering.Name, false, Define()
                        .Add(FunctionCall.Window.Ordering.OrderByKeyword, true, Token(SqlTokenRegistry.OrderBy))
                        .Add(FunctionCall.Window.Ordering.OrderByList, true, Expression(OrderByList.Name)))
                    .Add(FunctionCall.Window.Framing.Name, false, Define()
                        .Add(FunctionCall.Window.Framing.FrameType, true, Expression(FrameType.Name))
                        .Add(true, Options()
                            .Add(FunctionCall.Window.Framing.PrecedingFrame, Expression(PrecedingFrame.Name))
                            .Add(FunctionCall.Window.Framing.BetweenFrame.Name, Define()
                                .Add(FunctionCall.Window.Framing.BetweenFrame.BetweenKeyword, true, Token(SqlTokenRegistry.Between))
                                .Add(FunctionCall.Window.Framing.BetweenFrame.PrecedingFrame, true, Expression(PrecedingFrame.Name))
                                .Add(FunctionCall.Window.Framing.BetweenFrame.AndKeyword, true, Token(SqlTokenRegistry.And))
                                .Add(FunctionCall.Window.Framing.BetweenFrame.FollowingFrame, true, Expression(FollowingFrame.Name)))))
                    .Add(FunctionCall.Window.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)));
        }

        #endregion

        #region FrameType

        /// <summary>
        /// Describes the options for a frame type.
        /// </summary>
        public static class FrameType
        {
            /// <summary>
            /// Gets the identifier for the frame type.
            /// </summary>
            public const string Name = "FrameType";

            /// <summary>
            /// Gets the identifier for the ROWS keyword.
            /// </summary>
            public const string Rows = "rows";

            /// <summary>
            /// Gets the identifier for the RANGE keyword.
            /// </summary>
            public const string Range = "range";
        }

        private void defineFrameType()
        {
            Define(FrameType.Name)
                .Add(true, Options()
                    .Add(FrameType.Rows, Token(SqlTokenRegistry.Rows))
                    .Add(FrameType.Range, Token(SqlTokenRegistry.Range)));
        }

        #endregion

        #region PrecedingFrame

        /// <summary>
        /// Describes the structure of a preceding frame in a windowed function.
        /// </summary>
        public static class PrecedingFrame
        {
            /// <summary>
            /// Gets the identifier indicating there's a preceding frame.
            /// </summary>
            public const string Name = "PrecedingFrame";

            /// <summary>
            /// Describes the structure of a preceding window without a bound.
            /// </summary>
            public static class UnboundedPreceding
            {
                /// <summary>
                /// Gets the identifier indicating that the preceding window is unbounded.
                /// </summary>
                public const string Name = "UnboundedPreceding";

                /// <summary>
                /// Gets the identifier for the UNBOUNDED keyword.
                /// </summary>
                public const string UnboundedKeyword = "unbounded";

                /// <summary>
                /// Gets the identifier for the PRECEDING keyword.
                /// </summary>
                public const string PrecedingKeyword = "preceding";
            }

            /// <summary>
            /// Describes the structure of a preceding window with a bound.
            /// </summary>
            public static class BoundedPreceding
            {
                /// <summary>
                /// Gets the identifier indicating that the preceding window is bounded.
                /// </summary>
                public const string Name = "BoundedPreceding";

                /// <summary>
                /// Gets the identifier for the number of items in the preceding window.
                /// </summary>
                public const string Number = "number";

                /// <summary>
                /// Gets the identifier for the PRECEDING keyword.
                /// </summary>
                public const string PrecedingKeyword = "preceding";
            }

            /// <summary>
            /// Gets the identifier indicating that the windowed function should apply to a range
            /// starting with the current row.
            /// </summary>
            public const string CurrentRow = "current_row";
        }

        private void definePrecedingFrame()
        {
            Define(PrecedingFrame.Name)
                .Add(true, Options()
                    .Add(PrecedingFrame.UnboundedPreceding.Name, Define()
                        .Add(PrecedingFrame.UnboundedPreceding.UnboundedKeyword, true, Token(SqlTokenRegistry.Unbounded))
                        .Add(PrecedingFrame.UnboundedPreceding.PrecedingKeyword, true, Token(SqlTokenRegistry.Preceding)))
                    .Add(PrecedingFrame.BoundedPreceding.Name, Define()
                        .Add(PrecedingFrame.BoundedPreceding.Number, true, Token(SqlTokenRegistry.Number))
                        .Add(PrecedingFrame.BoundedPreceding.PrecedingKeyword, true, Token(SqlTokenRegistry.Preceding)))
                    .Add(PrecedingFrame.CurrentRow, Token(SqlTokenRegistry.CurrentRow)));
        }

        #endregion

        #region FollowingFrame

        /// <summary>
        /// Describes the structure of a following frame in a windowed function.
        /// </summary>
        public static class FollowingFrame
        {
            /// <summary>
            /// Gets the identifier indicating there's a following frame.
            /// </summary>
            public const string Name = "FollowingFrame";

            /// <summary>
            /// Describes the structure of a following window without a bound.
            /// </summary>
            public static class UnboundedFollowing
            {
                /// <summary>
                /// Gets the identifier indicating that the following window is unbounded.
                /// </summary>
                public const string Name = "UnboundedFollowing";

                /// <summary>
                /// Gets the identifier for the UNBOUNDED keyword.
                /// </summary>
                public const string UnboundedKeyword = "unbounded";

                /// <summary>
                /// Gets the identifier for the FOLLOWING keyword.
                /// </summary>
                public const string FollowingKeyword = "following";
            }

            /// <summary>
            /// Describes the structure of a following window with a bound.
            /// </summary>
            public static class BoundedFollowing
            {
                /// <summary>
                /// Gets the identifier indicating that the following window is bounded.
                /// </summary>
                public const string Name = "BoundedFollowing";

                /// <summary>
                /// Gets the identifier for the number of items in the following window.
                /// </summary>
                public const string Number = "number";

                /// <summary>
                /// Gets the identifier for the FOLLOWING keyword.
                /// </summary>
                public const string FollowingKeyword = "following";
            }

            /// <summary>
            /// Gets the identifier indicating that the windowed function should apply to a range
            /// stoping with the current row.
            /// </summary>
            public const string CurrentRow = "current_row";
        }

        private void defineFollowingFrame()
        {
            Define(FollowingFrame.Name)
                .Add(true, Options()
                    .Add(FollowingFrame.UnboundedFollowing.Name, Define()
                        .Add(FollowingFrame.UnboundedFollowing.UnboundedKeyword, true, Token(SqlTokenRegistry.Unbounded))
                        .Add(FollowingFrame.UnboundedFollowing.FollowingKeyword, true, Token(SqlTokenRegistry.Following)))
                    .Add(FollowingFrame.BoundedFollowing.Name, Define()
                        .Add(FollowingFrame.BoundedFollowing.Number, true, Token(SqlTokenRegistry.Number))
                        .Add(FollowingFrame.BoundedFollowing.FollowingKeyword, true, Token(SqlTokenRegistry.Following)))
                    .Add(FollowingFrame.CurrentRow, Token(SqlTokenRegistry.CurrentRow)));
        }

        #endregion

        #region Join

        /// <summary>
        /// Describes the structure of the join.
        /// </summary>
        public static class Join
        {
            /// <summary>
            /// Gets the name identifying the join.
            /// </summary>
            public const string Name = "Join";

            /// <summary>
            /// Describes the structure of a join surrounded in parenthesis.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the identifier indicating that the join is wrapped in parenthesis.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the wrapped join.
                /// </summary>
                public const string Join = "join";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";

                /// <summary>
                /// Gets the identifier for the rest of the join statement.
                /// </summary>
                public const string JoinPrime = "join_prime";
            }

            /// <summary>
            /// Describes the structure of a join item potentially joined to another item.
            /// </summary>
            public static class Joined
            {
                /// <summary>
                /// Gets the identifier indicating that we have a join item and potentially a join.
                /// </summary>
                public const string Name = "Joined";

                /// <summary>
                /// Gets the identifier for the item being joined.
                /// </summary>
                public const string JoinItem = "join_item";

                /// <summary>
                /// Gets the identifier for the rest of the join statement.
                /// </summary>
                public const string JoinPrime = "join_prime";
            }
        }

        private void defineJoin()
        {
            Define(Join.Name)
                .Add(true, Options()
                    .Add(Join.Wrapped.Name, Define()
                        .Add(Join.Wrapped.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(Join.Wrapped.Join, true, Expression(Join.Name))
                        .Add(Join.Wrapped.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))
                        .Add(Join.Wrapped.JoinPrime, true, Expression(JoinPrime.Name)))
                    .Add(Join.Joined.Name, Define()
                        .Add(Join.Joined.JoinItem, true, Expression(JoinItem.Name))
                        .Add(Join.Joined.JoinPrime, true, Expression(JoinPrime.Name))));
        }

        #endregion

        #region JoinPrime

        /// <summary>
        /// Describes the structure of the join prime expression.
        /// </summary>
        public static class JoinPrime
        {
            /// <summary>
            /// Gets the name identifying the join prime expression.
            /// </summary>
            public const string Name = "JoinPrime";

            /// <summary>
            /// Describes the structure of a join with a filter.
            /// </summary>
            public static class Filtered
            {
                /// <summary>
                /// Gets the identifier indicating that there is another join item.
                /// </summary>
                public const string Name = "Joined";

                /// <summary>
                /// Gets the identifier for the next join type.
                /// </summary>
                public const string JoinType = "join_type";

                /// <summary>
                /// Gets the identifier for the next join item.
                /// </summary>
                public const string JoinItem = "join_item";

                /// <summary>
                /// Describes the structure of the join filter.
                /// </summary>
                public static class On
                {
                    /// <summary>
                    /// Gets the identifier indicating that there is a ON clause.
                    /// </summary>
                    public const string Name = "On";

                    /// <summary>
                    /// Gets the identifier for the ON keyword.
                    /// </summary>
                    public const string OnKeyword = "on";

                    /// <summary>
                    /// Gets the identifier for the filter list.
                    /// </summary>
                    public const string FilterList = "filter_list";
                }

                /// <summary>
                /// Gets the identifier for the next join in the series.
                /// </summary>
                public const string JoinPrime = "join_prime";
            }

            /// <summary>
            /// Describes the structure of a cross join.
            /// </summary>
            public static class Cross
            {
                /// <summary>
                /// Gets the identifier indicating that the join is a cross join.
                /// </summary>
                public const string Name = "Cross";

                /// <summary>
                /// Gets the identifier for the next join type.
                /// </summary>
                public const string JoinType = "join_type";

                /// <summary>
                /// Gets the identifier for the next join item.
                /// </summary>
                public const string JoinItem = "join_item";

                /// <summary>
                /// Gets the identifier for the next join in the series.
                /// </summary>
                public const string JoinPrime = "join_prime";
            }

            /// <summary>
            /// Gets the identifier indicating that there are no more joins.
            /// </summary>
            public const string Empty = "empty";
        }

        private void defineJoinPrime()
        {
            Define(JoinPrime.Name)
                .Add(true, Options()
                    .Add(JoinPrime.Filtered.Name, Define()
                        .Add(JoinPrime.Filtered.JoinType, true, Expression(FilteredJoinType.Name))
                        .Add(JoinPrime.Filtered.JoinItem, true, Expression(JoinItem.Name))
                        .Add(JoinPrime.Filtered.On.Name, false, Define()
                            .Add(JoinPrime.Filtered.On.OnKeyword, true, Token(SqlTokenRegistry.On))
                            .Add(JoinPrime.Filtered.On.FilterList, true, Expression(OrFilter.Name)))
                        .Add(JoinPrime.Filtered.JoinPrime, true, Expression(JoinPrime.Name)))
                    .Add(JoinPrime.Cross.Name, Define()
                        .Add(JoinPrime.Cross.JoinType, true, Token(SqlTokenRegistry.CrossJoin))
                        .Add(JoinPrime.Cross.JoinItem, true, Expression(JoinItem.Name))
                        .Add(JoinPrime.Cross.JoinPrime, true, Expression(JoinPrime.Name)))
                    .Add(JoinPrime.Empty, Define()));
        }

        #endregion

        #region FilteredJoinType

        /// <summary>
        /// Describes the options for a join type.
        /// </summary>
        public static class FilteredJoinType
        {
            /// <summary>
            /// Gets the identifier indicating that the token is a join type.
            /// </summary>
            public const string Name = "FilteredJoinType";

            /// <summary>
            /// Gets the identifier for the INNER JOIN keyword.
            /// </summary>
            public const string InnerJoin = "InnerJoin";

            /// <summary>
            /// Gets the identifier for the LEFT OUTER JOIN keyword.
            /// </summary>
            public const string LeftOuterJoin = "LeftOuterJoin";

            /// <summary>
            /// Gets the identifier for the RIGHT OUTER JOIN keyword.
            /// </summary>
            public const string RightOuterJoin = "RightOuterJoin";

            /// <summary>
            /// Gets the identifier for the FULL OUTER JOIN keyword.
            /// </summary>
            public const string FullOuterJoin = "FullOuterJoin";
        }

        private void defineFilteredJoinType()
        {
            Define(FilteredJoinType.Name)
                .Add(true, Options()
                    .Add(FilteredJoinType.InnerJoin, Token(SqlTokenRegistry.InnerJoin))
                    .Add(FilteredJoinType.LeftOuterJoin, Token(SqlTokenRegistry.LeftOuterJoin))
                    .Add(FilteredJoinType.RightOuterJoin, Token(SqlTokenRegistry.RightOuterJoin))
                    .Add(FilteredJoinType.FullOuterJoin, Token(SqlTokenRegistry.FullOuterJoin)));
        }

        #endregion

        #region Filter

        /// <summary>
        /// Describes the structure of the filter.
        /// </summary>
        public static class Filter
        {
            /// <summary>
            /// Gets the name identifying the filter.
            /// </summary>
            public const string Name = "Filter";

            /// <summary>
            /// Describes the structure of a filter wrapped in parenthesis.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the indentifier indicating that the filter is wrapped in parenthesis.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the identifier for the left parenthesis token.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the wrapped filter.
                /// </summary>
                public const string Filter = "filter";

                /// <summary>
                /// Gets the identifier for the right parenthesis token.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of a filter that is negated.
            /// </summary>
            public static class Not
            {
                /// <summary>
                /// Gets the identifier indicating that a filter is negated.
                /// </summary>
                public const string Name = "Not";

                /// <summary>
                /// Gets the identifier for the NOT keyword.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the negated filter.
                /// </summary>
                public const string Filter = "filter";
            }

            /// <summary>
            /// Describes the structure of filter that compares the order of two items.
            /// </summary>
            public static class Order
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is an order filter.
                /// </summary>
                public const string Name = "Order";

                /// <summary>
                /// Gets the identifier for the value on the left.
                /// </summary>
                public const string Left = "left";

                /// <summary>
                /// Gets the identifier for the comparison operator.
                /// </summary>
                public const string ComparisonOperator = "comparison_operator";

                /// <summary>
                /// Gets the identifier for the value on the right.
                /// </summary>
                public const string Right = "right";
            }

            /// <summary>
            /// Describes the structure of a filter checking that a value falls within a range.
            /// </summary>
            public static class Between
            {
                /// <summary>
                /// The identifier indicating that the filter is a BETWEEN filter.
                /// </summary>
                public const string Name = "Between";

                /// <summary>
                /// Gets the identifier for the value being checked.
                /// </summary>
                public const string Expression = "expression";

                /// <summary>
                /// Gets the identifier for whether or not to negate the filter.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the BETWEEN keyword.
                /// </summary>
                public const string BetweenKeyword = "between";

                /// <summary>
                /// Gets the identifier for the lower bound value.
                /// </summary>
                public const string LowerBound = "lower_bound";

                /// <summary>
                /// Gets the identifier for the AND keyword.
                /// </summary>
                public const string And = "between_and";

                /// <summary>
                /// Gets the identifier for the upper bound value.
                /// </summary>
                public const string UpperBound = "upper_bound";
            }

            /// <summary>
            /// Describes the structure of a filter doing a string comparison.
            /// </summary>
            public static class Like
            {
                /// <summary>
                /// Gets the identifier indicating whether the filter is doing a string comparison.
                /// </summary>
                public const string Name = "Like";

                /// <summary>
                /// Gets the identifier for the expression being compared.
                /// </summary>
                public const string Left = "left";

                /// <summary>
                /// Gets the identifier indicating whether to negate the results of the comparison.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the LIKE keyword.
                /// </summary>
                public const string LikeKeyword = "like";

                /// <summary>
                /// Gets the identifier for string literal being compared to.
                /// </summary>
                public const string Right = "right";
            }

            /// <summary>
            /// Describes the structure of a filter checking whether a value is null.
            /// </summary>
            public static class Is
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is checking whether a value is null or not.
                /// </summary>
                public const string Name = "Is";

                /// <summary>
                /// Gets the identifier for value being compared.
                /// </summary>
                public const string Expression = "expression";

                /// <summary>
                /// Gets the identifier for the IS keyword.
                /// </summary>
                public const string IsKeyword = "is";

                /// <summary>
                /// Gets the identifier for the NOT keyword.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the NULL keyword.
                /// </summary>
                public const string NullKeyword = "null";
            }

            /// <summary>
            /// Describes the structure of a filter checking whether a value exists in a list of values.
            /// </summary>
            public static class In
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is checking whether a value exists in a list of values.
                /// </summary>
                public const string Name = "In";

                /// <summary>
                /// Gets the identifier for the value being compared.
                /// </summary>
                public const string Expression = "expression";

                /// <summary>
                /// Gets the identifier for the NOT keyword.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the IN keyword.
                /// </summary>
                public const string InKeyword = "in";
                /// <summary>
                /// Describes the structure of a parameter variable.
                /// </summary>
                /// <remarks>wyf 用于dapper 中的 in @参数表达式</remarks>
                public static class Parameter
                {
                    public const string Name = "Parameter";
                    public const string Value = "value_parameter";
                }
                /// <summary>
                /// Describes the structure of a values list.
                /// </summary>
                public static class Values
                {
                    /// <summary>
                    /// Gets the identifier indicating that the source is a value list.
                    /// </summary>
                    public const string Name = "Values";

                    /// <summary>
                    /// Gets the identifier for the left parenthesis token.
                    /// </summary>
                    public const string LeftParenthesis = "left_parenthesis";

                    /// <summary>
                    /// Gets the identifier indicating that the values come from a list of values.
                    /// </summary>
                    public const string ValueList = "value_list";

                    /// <summary>
                    /// Gets the identifier for the right parenthesis.
                    /// </summary>
                    public const string RightParenthesis = "right_parenthesis";
                }

                /// <summary>
                /// Describes the structure a select source.
                /// </summary>
                public static class Select
                {
                    /// <summary>
                    /// Gets the identifier indicating that the source is a SELECT expression.
                    /// </summary>
                    public const string Name = "Select";

                    /// <summary>
                    /// Gets the identifier for the left parenthesis token.
                    /// </summary>
                    public const string LeftParenthesis = "left_parenthesis";

                    /// <summary>
                    /// Gets the identifier indicating that the values come from a SELECT expression.
                    /// </summary>
                    public const string SelectStatement = "select_statement";

                    /// <summary>
                    /// Gets the identifier for the right parenthesis.
                    /// </summary>
                    public const string RightParenthesis = "right_parenthesis";
                }

                /// <summary>
                /// Gets the identifier indicating that the values come from a function call.
                /// </summary>
                public const string FunctionCall = "function_call";
            }

            /// <summary>
            /// Describes the structure of an Exists filter.
            /// </summary>
            public static class Exists
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is an exists filter.
                /// </summary>
                public const string Name = "Exists";

                /// <summary>
                /// Gets the identifier for the EXISTS keyword.
                /// </summary>
                public const string ExistsKeyword = "exists";

                /// <summary>
                /// Gets the identfier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the SELECT expression.
                /// </summary>
                public const string SelectStatement = "select_statement";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of a existential or universal quantifier.
            /// </summary>
            public static class Quantify
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is a quantifier.
                /// </summary>
                public const string Name = "Quantify";

                /// <summary>
                /// Gets the identifier for the value being compared.
                /// </summary>
                public const string Expression = "expression";

                /// <summary>
                /// Gets the identifier for the comparison operator.
                /// </summary>
                public const string ComparisonOperator = "comparison_operator";

                /// <summary>
                /// Gets the identifier for the existential or universal quantifier.
                /// </summary>
                public const string Quantifier = "quantifier";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the select expression.
                /// </summary>
                public const string SelectStatement = "select_statement";

                /// <summary>
                /// Gets the identifier for the value list.
                /// </summary>
                public const string ValueList = "value_list";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of a
            /// </summary>
            public static class Function
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is a function call.
                /// </summary>
                public const string Name = "Function";

                /// <summary>
                /// Gets the identifier for the function being called.
                /// </summary>
                public const string Expression = "expression";
            }
        }

        private void defineFilter()
        {
            Define(Filter.Name)
                .Add(true, Options()
                    .Add(Filter.Not.Name, Define()
                        .Add(Filter.Not.NotKeyword, true, Token(SqlTokenRegistry.Not))
                        .Add(Filter.Not.Filter, true, Expression(Filter.Name)))
                    .Add(Filter.Wrapped.Name, Define()
                        .Add(Filter.Wrapped.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(Filter.Wrapped.Filter, true, Expression(OrFilter.Name))
                        .Add(Filter.Wrapped.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(Filter.Order.Name, Define()
                        .Add(Filter.Order.Left, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.Order.ComparisonOperator, true, Expression(ComparisonOperator.Name))
                        .Add(Filter.Order.Right, true, Expression(ArithmeticItem.Name)))
                    .Add(Filter.Quantify.Name, Define()
                        .Add(Filter.Quantify.Expression, true, Expression(SqlGrammar.ArithmeticItem.Name))
                        .Add(Filter.Quantify.ComparisonOperator, true, Expression(SqlGrammar.ComparisonOperator.Name))
                        .Add(Filter.Quantify.Quantifier, true, Expression(SqlGrammar.Quantifier.Name))
                        .Add(Filter.Quantify.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(true, Options()
                            .Add(Filter.Quantify.SelectStatement, Expression(SelectStatement.Name))
                            .Add(Filter.Quantify.ValueList, Expression(ValueList.Name)))
                        .Add(Filter.Quantify.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(Filter.Function.Name, Define()
                        .Add(Filter.Function.Expression, true, Expression(FunctionCall.Name)))
                    .Add(Filter.Between.Name, Define()
                        .Add(Filter.Between.Expression, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.Between.NotKeyword, false, Token(SqlTokenRegistry.Not))
                        .Add(Filter.Between.BetweenKeyword, true, Token(SqlTokenRegistry.Between))
                        .Add(Filter.Between.LowerBound, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.Between.And, true, Token(SqlTokenRegistry.And))
                        .Add(Filter.Between.UpperBound, true, Expression(ArithmeticItem.Name)))
                    .Add(Filter.Like.Name, Define()
                        .Add(Filter.Like.Left, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.Like.NotKeyword, false, Token(SqlTokenRegistry.Not))
                        .Add(Filter.Like.LikeKeyword, true, Token(SqlTokenRegistry.Like))
                        .Add(Filter.Like.Right, true, Expression(ArithmeticItem.Name)))
                    .Add(Filter.Is.Name, Define()
                        .Add(Filter.Is.Expression, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.Is.IsKeyword, true, Token(SqlTokenRegistry.Is))
                        .Add(Filter.Is.NotKeyword, false, Token(SqlTokenRegistry.Not))
                        .Add(Filter.Is.NullKeyword, true, Token(SqlTokenRegistry.Null)))
                    .Add(Filter.In.Name, Define()
                        .Add(Filter.In.Expression, true, Expression(ArithmeticItem.Name))
                        .Add(Filter.In.NotKeyword, false, Token(SqlTokenRegistry.Not))
                        .Add(Filter.In.InKeyword, true, Token(SqlTokenRegistry.In))
                        .Add(true, Options()//wyf 增加in后面加@参数，用于dapper语法
                            .Add(Filter.In.Parameter.Name,Define()
                                .Add(Filter.In.Parameter.Value,true,Token(SqlTokenRegistry.Identifier)))
                            .Add(Filter.In.Values.Name, Define()
                                .Add(Filter.In.Values.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                                .Add(Filter.In.Values.ValueList, false, Expression(ValueList.Name))
                                .Add(Filter.In.Values.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                            .Add(Filter.In.Select.Name, Define()
                                .Add(Filter.In.Select.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                                .Add(Filter.In.Select.SelectStatement, true, Expression(SelectStatement.Name))
                                .Add(Filter.In.Select.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                            .Add(Filter.In.FunctionCall, Expression(FunctionCall.Name))))
                    .Add(Filter.Exists.Name, Define()
                        .Add(Filter.Exists.ExistsKeyword, true, Token(SqlTokenRegistry.Exists))
                        .Add(Filter.Exists.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(Filter.Exists.SelectStatement, true, Expression(SqlGrammar.SelectStatement.Name))
                        .Add(Filter.Exists.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))));
        }

        #endregion

        #region ComparisonOperator

        /// <summary>
        /// Describes the structure of a comparison operator.
        /// </summary>
        public static class ComparisonOperator
        {
            /// <summary>
            /// Gets the identifier indicating that the token is a comparison operator.
            /// </summary>
            public const string Name = "ComparisonOperator";

            /// <summary>
            /// Gets the identifier for the equality operator.
            /// </summary>
            public const string EqualTo = "equal_to";

            /// <summary>
            /// Gets the identifier for the inequality operator.
            /// </summary>
            public const string NotEqualTo = "not_equal_to";

            /// <summary>
            /// Gets the identifier for the less than or equal to operator.
            /// </summary>
            public const string LessThanEqualTo = "less_than_equal_to";

            /// <summary>
            /// Gets the identifier for the greater than or equal to operator.
            /// </summary>
            public const string GreaterThanEqualTo = "greater_than_equal_to";

            /// <summary>
            /// Gets the identifier for the less than operator.
            /// </summary>
            public const string LessThan = "less_than";

            /// <summary>
            /// Gets the identifier for the greater than operator.
            /// </summary>
            public const string GreaterThan = "greater_than";
        }

        private void defineComparisonOperator()
        {
            Define(ComparisonOperator.Name)
                .Add(true, Options()
                    .Add(ComparisonOperator.EqualTo, Token(SqlTokenRegistry.EqualTo))
                    .Add(ComparisonOperator.NotEqualTo, Token(SqlTokenRegistry.NotEqualTo))
                    .Add(ComparisonOperator.LessThanEqualTo, Token(SqlTokenRegistry.LessThanEqualTo))
                    .Add(ComparisonOperator.GreaterThanEqualTo, Token(SqlTokenRegistry.GreaterThanEqualTo))
                    .Add(ComparisonOperator.LessThan, Token(SqlTokenRegistry.LessThan))
                    .Add(ComparisonOperator.GreaterThan, Token(SqlTokenRegistry.GreaterThan)));
        }

        #endregion

        #region Quantifier

        /// <summary>
        /// Describes the structure of a quantifier.
        /// </summary>
        public static class Quantifier
        {
            /// <summary>
            /// Gets the identifier indicating that the current token is a quantifier.
            /// </summary>
            public const string Name = "Quantifier";

            /// <summary>
            /// Gets the identifier for the ALL quantifier.
            /// </summary>
            public const string All = "all";

            /// <summary>
            /// Gets the identifier for the ANY quantifier.
            /// </summary>
            public const string Any = "any";

            /// <summary>
            /// Gets the identifier for the SOME quantifier.
            /// </summary>
            public const string Some = "some";
        }

        private void defineQuantifier()
        {
            Define(Quantifier.Name)
                .Add(true, Options()
                    .Add(Quantifier.All, Token(SqlTokenRegistry.All))
                    .Add(Quantifier.Any, Token(SqlTokenRegistry.Any))
                    .Add(Quantifier.Some, Token(SqlTokenRegistry.Some)));
        }

        #endregion

        #region OrFilter

        /// <summary>
        /// Describes the structure of two filters OR'd together.
        /// </summary>
        public static class OrFilter
        {
            /// <summary>
            /// Gets the identifier indicating that two filters are OR'd together.
            /// </summary>
            public const string Name = "OrFilter";

            /// <summary>
            /// Gets the structure of a filter wrapped in parentheses.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is wrapped in parenthesis.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the identifier for the NOT keyword.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the wrapped filter.
                /// </summary>
                public const string OrFilter = "or_filter";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of a filter joining two filters with an OR.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that filters are OR'd together.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first filter.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the OR keyword.
                /// </summary>
                public const string Or = "or";

                /// <summary>
                /// Gets the identifier for the rest of the filters.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier for a single filter.
            /// </summary>
            public const string Single = "single";
        }

        private void defineOrFilter()
        {
            Define(OrFilter.Name)
                .Add(true, Options()
                    .Add(OrFilter.Multiple.Name, Define()
                        .Add(OrFilter.Multiple.First, true, Expression(AndFilter.Name))
                        .Add(OrFilter.Multiple.Or, true, Token(SqlTokenRegistry.Or))
                        .Add(OrFilter.Multiple.Remaining, true, Expression(OrFilter.Name)))
                    .Add(OrFilter.Single, Expression(AndFilter.Name)));
        }

        #endregion

        #region AndFilter

        /// <summary>
        /// Describes the structure of two filters AND'd together.
        /// </summary>
        public static class AndFilter
        {
            /// <summary>
            /// Gets the identifier indicating that two filters are AND'd together.
            /// </summary>
            public const string Name = "AndFilter";

            /// <summary>
            /// Gets the identifier indicating that two filter are AND'd together.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that filters are AND'd together.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first filter.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the AND keyword.
                /// </summary>
                public const string And = "and";

                /// <summary>
                /// Gets the identifier for the rest of the filters.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the structure of a filter wrapped in parentheses.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the identifier indicating that the filter is wrapped in parenthesis.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the identifier for the NOT keyword.
                /// </summary>
                public const string NotKeyword = "not";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the wrapped filter.
                /// </summary>
                public const string AndFilter = "and_filter";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Gets the identifier for a single filter.
            /// </summary>
            public const string Single = "single";
        }

        private void defineAndFilter()
        {
            Define(AndFilter.Name)
                .Add(true, Options()
                    .Add(AndFilter.Multiple.Name, Define()
                        .Add(AndFilter.Multiple.First, true, Expression(Filter.Name))
                        .Add(AndFilter.Multiple.And, true, Token(SqlTokenRegistry.And))
                        .Add(AndFilter.Multiple.Remaining, true, Expression(OrFilter.Name)))
                    .Add(AndFilter.Single, Expression(Filter.Name)));
        }

        #endregion

        #region ValueList

        /// <summary>
        /// Describes the structure of the value list.
        /// </summary>
        public static class ValueList
        {
            /// <summary>
            /// Gets the name identifying the value list.
            /// </summary>
            public const string Name = "ValueList";

            /// <summary>
            /// Describes the structure of a value list containing multiple items.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that the value list has more than one item.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first value.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the values.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that there is a single value.
            /// </summary>
            public const string Single = "single";
        }

        private void defineValueList()
        {
            Define(ValueList.Name)
                .Add(true, Options()
                    .Add(ValueList.Multiple.Name, Define()
                        .Add(ValueList.Multiple.First, true, Expression(ArithmeticItem.Name))
                        .Add(ValueList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(ValueList.Multiple.Remaining, true, Expression(ValueList.Name)))
                    .Add(ValueList.Single, Expression(ArithmeticItem.Name)));
        }

        #endregion

        #region GroupByList

        /// <summary>
        /// Describes the structure of the GROUP BY list.
        /// </summary>
        public static class GroupByList
        {
            /// <summary>
            /// Gets the name identifying the GROUP BY list.
            /// </summary>
            public const string Name = "GroupByList";

            /// <summary>
            /// Describes the structure of multiple GROUP BY items.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that there are multiple GROUP BY items.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first GROUP BY item.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the GROUP BY items.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier for a single GROUP BY item.
            /// </summary>
            public const string Single = "single";
        }

        private void defineGroupByList()
        {
            Define(GroupByList.Name)
                .Add(true, Options()
                    .Add(SqlGrammar.GroupByList.Multiple.Name, Define()
                        .Add(SqlGrammar.GroupByList.Multiple.First, true, Expression(ArithmeticItem.Name))
                        .Add(SqlGrammar.GroupByList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(SqlGrammar.GroupByList.Multiple.Remaining, true, Expression(GroupByList.Name)))
                    .Add(SqlGrammar.GroupByList.Single, Expression(ArithmeticItem.Name)));
        }

        #endregion

        #region ArithmeticItem

        /// <summary>
        /// Describes the structure of an item that can be an arithmetic expression.
        /// </summary>
        public static class ArithmeticItem
        {
            /// <summary>
            /// The identifier indicating that the item can be an arithmetic expression.
            /// </summary>
            public const string Name = "ArithmeticItem";

            /// <summary>
            /// Gets the identifier indicating that the item is an arithmetic expression.
            /// </summary>
            public const string ArithmeticExpression = "arithmetic_expression";
        }

        private void defineArithmeticItem()
        {
            Define(ArithmeticItem.Name)
                .Add(ArithmeticItem.ArithmeticExpression, true, Expression(AdditiveExpression.Name));
        }

        #endregion

        #region WrappedItem

        /// <summary>
        /// Describes the structure of an item that is potentially wrapped by parenthesis.
        /// </summary>
        public static class WrappedItem
        {
            /// <summary>
            /// Gets the identifier indicating that the item is a wrapped item.
            /// </summary>
            public const string Name = "WrappedItem";

            /// <summary>
            /// Describes the structure of an negated expression.
            /// </summary>
            public static class Negated
            {
                /// <summary>
                /// Gets the identifier indicating that the expression is negated.
                /// </summary>
                public const string Name = "Negated";

                /// <summary>
                /// Gets the identifier for the minus sign.
                /// </summary>
                public const string Minus = "minus";

                /// <summary>
                /// Gets the identifier for the expression being negated.
                /// </summary>
                public const string Item = "item";
            }

            /// <summary>
            /// Describes the structure of an item that is wrapped in parentheses.
            /// </summary>
            public static class Wrapped
            {
                /// <summary>
                /// Gets the identifier indicating that the item is wrapped.
                /// </summary>
                public const string Name = "Wrapped";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the additive expression.
                /// </summary>
                public const string AdditiveExpression = "additive_expression";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Gets the identifier for an unwrapped item.
            /// </summary>
            public const string Item = "item";
        }

        private void defineWrappedItem()
        {
            Define(WrappedItem.Name)
                .Add(true, Options()
                    .Add(WrappedItem.Negated.Name, Define()
                        .Add(WrappedItem.Negated.Minus, true, Token(SqlTokenRegistry.MinusOperator))
                        .Add(WrappedItem.Negated.Item, true, Expression(WrappedItem.Name)))
                    .Add(WrappedItem.Wrapped.Name, Define()
                        .Add(WrappedItem.Wrapped.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(WrappedItem.Wrapped.AdditiveExpression, true, Expression(AdditiveExpression.Name))
                        .Add(WrappedItem.Wrapped.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(WrappedItem.Item, Expression(Item.Name)));
        }

        #endregion

        #region Item

        /// <summary>
        /// Describes the structure of the item.
        /// </summary>
        public static class Item
        {
            /// <summary>
            /// Gets the name identifying the item.
            /// </summary>
            public const string Name = "Item";

            /// <summary>
            /// Gets the identifier indicating that the item is a column.
            /// </summary>
            public const string Column = "column";

            /// <summary>
            /// Gets the identifier indicating that the item is a function call.
            /// </summary>
            public const string FunctionCall = "function_call";

            /// <summary>
            /// Gets the identifier indicating that the item is a select statement.
            /// </summary>
            public static class Select
            {
                /// <summary>
                /// Gets the identifier indicating that the item is a SELECT statement.
                /// </summary>
                public const string Name = "SelectStatement";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the SELECT statement.
                /// </summary>
                public const string SelectStatement = "select_statement";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Gets the identifier indicating that the item is a number.
            /// </summary>
            public const string Number = "number";

            /// <summary>
            /// Gets the identifier indicating that the item is a string.
            /// </summary>
            public const string String = "string";

            /// <summary>
            /// Gets the identifier indicating that the item is a null.
            /// </summary>
            public const string Null = "null";

            /// <summary>
            /// Gets the identifier indicating that the item is a case expression.
            /// </summary>
            public const string MatchCase = "match_case";

            /// <summary>
            /// Gets the identifier indicating that the item is a case expression.
            /// </summary>
            public const string ConditionCase = "conditional_case";
        }

        private void defineItem()
        {
            Define(Item.Name)
                .Add(true, Options()
                    .Add(Item.Number, Token(SqlTokenRegistry.Number))
                    .Add(Item.String, Token(SqlTokenRegistry.String))
                    .Add(Item.Null, Token(SqlTokenRegistry.Null))
                    .Add(Item.FunctionCall, Expression(FunctionCall.Name))
                    .Add(Item.Column, Expression(MultipartIdentifier.Name))
                    .Add(Item.Select.Name, Define()
                        .Add(Item.Select.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(Item.Select.SelectStatement, true, Expression(SelectStatement.Name))
                        .Add(Item.Select.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(Item.MatchCase, Expression(MatchCase.Name))
                    .Add(Item.ConditionCase, Expression(ConditionalCase.Name)));
        }

        #endregion

        #region MatchCase

        /// <summary>
        /// Describes the structure of a CASE expression.
        /// </summary>
        public static class MatchCase
        {
            /// <summary>
            /// Gets the identifier indicating that an item is a case statement.
            /// </summary>
            public const string Name = "MatchCase";

            /// <summary>
            /// Gets the identifier for the CASE keyword.
            /// </summary>
            public const string CaseKeyword = "case";

            /// <summary>
            /// Gets the identifier for the expression to match against the options.
            /// </summary>
            public const string Expression = "expression";

            /// <summary>
            /// Gets the identifier for the list of possible matches.
            /// </summary>
            public const string MatchList = "match_list";

            /// <summary>
            /// Gets the identifier for the END keyword.
            /// </summary>
            public const string EndKeyword = "end";
        }

        private void defineMatchCase()
        {
            Define(MatchCase.Name)
                .Add(MatchCase.CaseKeyword, true, Token(SqlTokenRegistry.Case))
                .Add(MatchCase.Expression, true, Expression(ArithmeticItem.Name))
                .Add(MatchCase.MatchList, true, Expression(MatchList.Name))
                .Add(MatchCase.EndKeyword, true, Token(SqlTokenRegistry.End));
        }

        #endregion

        #region MatchList

        /// <summary>
        /// Describes the structure of a case's match list.
        /// </summary>
        public static class MatchList
        {
            /// <summary>
            /// Gets the identifier indicating that there's a case's match list.
            /// </summary>
            public const string Name = "MatchList";

            /// <summary>
            /// Gets the identifier for the first match.
            /// </summary>
            public const string Match = "match";

            /// <summary>
            /// Gets the identifier for the rest of the matches.
            /// </summary>
            public const string MatchListPrime = "match_list_prime";
        }

        private void defineMatchList()
        {
            Define(MatchList.Name)
                .Add(MatchList.Match, true, Expression(Match.Name))
                .Add(MatchList.MatchListPrime, true, Expression(MatchListPrime.Name));
        }

        #endregion

        #region MatchListPrime

        /// <summary>
        /// Desribes the structure of the remaining matches in a CASE statement.
        /// </summary>
        public static class MatchListPrime
        {
            /// <summary>
            /// Gets the identifier indicating that there are potentially more tests.
            /// </summary>
            public const string Name = "MatchListPrime";

            /// <summary>
            /// Describes the structure if there are more matches.
            /// </summary>
            public static class Match
            {
                /// <summary>
                /// Gets the identifier indicating that there is a match.
                /// </summary>
                public const string Name = "Match";

                /// <summary>
                /// Gets the identifier for the next match.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the remaining matches.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Describes the structure of the default case.
            /// </summary>
            public static class Else
            {
                /// <summary>
                /// Gets the identifier indicating a default case.
                /// </summary>
                public const string Name = "Else";

                /// <summary>
                /// Gets the identifier for the ELSE keyword.
                /// </summary>
                public const string ElseKeyword = "else";

                /// <summary>
                /// Gets the identifier for the default value.
                /// </summary>
                public const string Value = "value";
            }

            /// <summary>
            /// Gets the identifier indicating that there are no more matches.
            /// </summary>
            public const string Empty = "empty";
        }

        private void defineMatchListPrime()
        {
            Define(MatchListPrime.Name)
                .Add(true, Options()
                    .Add(MatchListPrime.Match.Name, Define()
                        .Add(MatchListPrime.Match.First, true, Expression(Match.Name))
                        .Add(MatchListPrime.Match.Remaining, true, Expression(MatchListPrime.Name)))
                    .Add(MatchListPrime.Else.Name, Define()
                        .Add(MatchListPrime.Else.ElseKeyword, true, Token(SqlTokenRegistry.Else))
                        .Add(MatchListPrime.Else.Value, true, Expression(ArithmeticItem.Name)))
                    .Add(MatchListPrime.Empty, Define()));
        }

        #endregion

        #region Match

        /// <summary>
        /// Describes the structure of a match in a CASE statement.
        /// </summary>
        public static class Match
        {
            /// <summary>
            /// Gets the identifier indicating that there is a CASE test.
            /// </summary>
            public const string Name = "Match";

            /// <summary>
            /// Gets the identifier for the WHEN keyword.
            /// </summary>
            public const string WhenKeyword = "when";

            /// <summary>
            /// Gets the identifier for the value being matched against.
            /// </summary>
            public const string Expression = "expression";

            /// <summary>
            /// Gets the identifier for the THEN keyword.
            /// </summary>
            public const string ThenKeyword = "then";

            /// <summary>
            /// Gets the identifier for the value to return if there's a match.
            /// </summary>
            public const string Value = "value";
        }

        private void defineMatch()
        {
            Define(Match.Name)
                .Add(Match.WhenKeyword, true, Token(SqlTokenRegistry.When))
                .Add(Match.Expression, true, Expression(ArithmeticItem.Name))
                .Add(Match.ThenKeyword, true, Token(SqlTokenRegistry.Then))
                .Add(Match.Value, true, Expression(ArithmeticItem.Name));
        }

        #endregion

        #region ConditionalCase

        /// <summary>
        /// Describes the structure of a CASE expression.
        /// </summary>
        public static class ConditionalCase
        {
            /// <summary>
            /// Gets the identifier indicating that an item is a case statement.
            /// </summary>
            public const string Name = "ConditionalCase";

            /// <summary>
            /// Gets the identifier for the CASE keyword.
            /// </summary>
            public const string CaseKeyword = "case";

            /// <summary>
            /// Gets the identifier for the list of possible matches.
            /// </summary>
            public const string ConditionList = "condition_list";

            /// <summary>
            /// Gets the identifier for the END keyword.
            /// </summary>
            public const string EndKeyword = "end";
        }

        private void defineConditionalCase()
        {
            Define(ConditionalCase.Name)
                .Add(ConditionalCase.CaseKeyword, true, Token(SqlTokenRegistry.Case))
                .Add(ConditionalCase.ConditionList, true, Expression(ConditionList.Name))
                .Add(ConditionalCase.EndKeyword, true, Token(SqlTokenRegistry.End));
        }

        #endregion

        #region ConditionList

        /// <summary>
        /// Describes the structure of a case's match list.
        /// </summary>
        public static class ConditionList
        {
            /// <summary>
            /// Gets the identifier indicating that there's a case's match list.
            /// </summary>
            public const string Name = "ConditionList";

            /// <summary>
            /// Gets the identifier for the first match.
            /// </summary>
            public const string Condition = "condition";

            /// <summary>
            /// Gets the identifier for the rest of the matches.
            /// </summary>
            public const string ConditionListPrime = "condition_list_prime";
        }

        private void defineConditionList()
        {
            Define(ConditionList.Name)
                .Add(ConditionList.Condition, true, Expression(Condition.Name))
                .Add(ConditionList.ConditionListPrime, true, Expression(ConditionListPrime.Name));
        }

        #endregion

        #region ConditionListPrime

        /// <summary>
        /// Desribes the structure of the remaining matches in a CASE statement.
        /// </summary>
        public static class ConditionListPrime
        {
            /// <summary>
            /// Gets the identifier indicating that there are potentially more tests.
            /// </summary>
            public const string Name = "ConditionListPrime";

            /// <summary>
            /// Describes the structure if there are more matches.
            /// </summary>
            public static class Condition
            {
                /// <summary>
                /// Gets the identifier indicating that there is a match.
                /// </summary>
                public const string Name = "Condition";

                /// <summary>
                /// Gets the identifier for the next match.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the remaining matches.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Describes the structure of the default case.
            /// </summary>
            public static class Else
            {
                /// <summary>
                /// Gets the identifier indicating a default case.
                /// </summary>
                public const string Name = "Else";

                /// <summary>
                /// Gets the identifier for the ELSE keyword.
                /// </summary>
                public const string ElseKeyword = "else";

                /// <summary>
                /// Gets the identifier for the default value.
                /// </summary>
                public const string Value = "value";
            }

            /// <summary>
            /// Gets the identifier indicating that there are no more matches.
            /// </summary>
            public const string Empty = "empty";
        }

        private void defineConditionListPrime()
        {
            Define(ConditionListPrime.Name)
                .Add(true, Options()
                    .Add(ConditionListPrime.Condition.Name, Define()
                        .Add(ConditionListPrime.Condition.First, true, Expression(Condition.Name))
                        .Add(ConditionListPrime.Condition.Remaining, true, Expression(ConditionListPrime.Name)))
                    .Add(ConditionListPrime.Else.Name, Define()
                        .Add(ConditionListPrime.Else.ElseKeyword, true, Token(SqlTokenRegistry.Else))
                        .Add(ConditionListPrime.Else.Value, true, Expression(ArithmeticItem.Name)))
                    .Add(ConditionListPrime.Empty, Define()));
        }

        #endregion

        #region Condition

        /// <summary>
        /// Describes the structure of a condition in a CASE statement.
        /// </summary>
        public static class Condition
        {
            /// <summary>
            /// Gets the identifier indicating that there is a CASE test.
            /// </summary>
            public const string Name = "Condition";

            /// <summary>
            /// Gets the identifier for the WHEN keyword.
            /// </summary>
            public const string WhenKeyword = "when";

            /// <summary>
            /// Gets the identifier for the condition being tested.
            /// </summary>
            public const string Filter = "filter";

            /// <summary>
            /// Gets the identifier for the THEN keyword.
            /// </summary>
            public const string ThenKeyword = "then";

            /// <summary>
            /// Gets the identifier for the value to return if the condition is satisfied.
            /// </summary>
            public const string Value = "value";
        }

        private void defineCondition()
        {
            Define(Condition.Name)
                .Add(Condition.WhenKeyword, true, Token(SqlTokenRegistry.When))
                .Add(Condition.Filter, true, Expression(OrFilter.Name))
                .Add(Condition.ThenKeyword, true, Token(SqlTokenRegistry.Then))
                .Add(Condition.Value, true, Expression(ArithmeticItem.Name));
        }

        #endregion

        #region InsertStatement

        /// <summary>
        /// Describes the structure of the INSERT statement.
        /// </summary>
        public static class InsertStatement
        {
            /// <summary>
            /// Gets the name identifying the INSERT statement.
            /// </summary>
            public const string Name = "InsertStatement";

            /// <summary>
            /// Gets the identifier for the INSERT keyword.
            /// </summary>
            public const string InsertKeyword = "insert";

            /// <summary>
            /// Gets the identifier for the INTO keyword.
            /// </summary>
            public const string IntoKeyword = "into";

            /// <summary>
            /// Gets the identifier for the table name.
            /// </summary>
            public const string Table = "table";

            /// <summary>
            /// Describes the structure of the table alias.
            /// </summary>
            public static class AliasExpression
            {
                /// <summary>
                /// Gets the identifier indicating that the table has an alias.
                /// </summary>
                public const string Name = "AliasExpression";

                /// <summary>
                /// Gets the identifier for the AS keyword.
                /// </summary>
                public const string AliasIndicator = "alias_indicator";

                /// <summary>
                /// Gets the identiifier for the alias.
                /// </summary>
                public const string Alias = "alias";
            }

            /// <summary>
            /// Describes the structure of the columns list.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// Gets the identifier indicating that there is a column list.
                /// </summary>
                public const string Name = "ColumnList";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the column list.
                /// </summary>
                public const string ColumnList = "column_list";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of the values list.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// Gets the identifier indicating that a value list is used.
                /// </summary>
                public const string Name = "Values";

                /// <summary>
                /// Gets the identifier for the VALUES keyword.
                /// </summary>
                public const string ValuesKeyword = "values";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the value list.
                /// </summary>
                public const string ValueList = "value_list";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of the SELECT expression generating the values.
            /// </summary>
            public static class Select
            {
                /// <summary>
                /// Gets the identifier indicating that the values come from a SELECT statement.
                /// </summary>
                public const string Name = "SelectExpression";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the select statement.
                /// </summary>
                public const string SelectStatement = "select_statement";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

            /// <summary>
            /// Describes the structure of the output clause.
            /// </summary>
            public static class Output
            {
                /// <summary>
                /// Gets the identifier indicating that there is a column list.
                /// </summary>
                public const string Name = "OutputExpression";

            }
        }

        private void defineInsertStatement()
        {
            Define(InsertStatement.Name)
                .Add(SqlGrammar.InsertStatement.InsertKeyword, true, Token(SqlTokenRegistry.Insert))
                .Add(SqlGrammar.InsertStatement.IntoKeyword, false, Token(SqlTokenRegistry.Into))
                .Add(SqlGrammar.InsertStatement.Table, true, Expression(MultipartIdentifier.Name))
                .Add(SqlGrammar.InsertStatement.AliasExpression.Name, false, Define()
                    .Add(SqlGrammar.InsertStatement.AliasExpression.AliasIndicator, false, Token(SqlTokenRegistry.As))
                    .Add(SqlGrammar.InsertStatement.AliasExpression.Alias, true, Token(SqlTokenRegistry.Identifier)))
                .Add(SqlGrammar.InsertStatement.Columns.Name, false, Define()
                    .Add(SqlGrammar.InsertStatement.Columns.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                    .Add(SqlGrammar.InsertStatement.Columns.ColumnList, true, Expression(ColumnList.Name))
                    .Add(SqlGrammar.InsertStatement.Columns.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                .Add(SqlGrammar.InsertStatement.Output.Name, false, Expression(Output.Name))
                .Add(true, Options()
                    .Add(SqlGrammar.InsertStatement.Values.Name, Define()
                        .Add(SqlGrammar.InsertStatement.Values.ValuesKeyword, true, Token(SqlTokenRegistry.Values))
                        .Add(SqlGrammar.InsertStatement.Values.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(SqlGrammar.InsertStatement.Values.ValueList, false, Expression(ValueList.Name))
                        .Add(SqlGrammar.InsertStatement.Values.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                    .Add(SqlGrammar.InsertStatement.Select.Name, Define()
                        .Add(SqlGrammar.InsertStatement.Select.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(SqlGrammar.InsertStatement.Select.SelectStatement, true, Expression(SelectStatement.Name))
                        .Add(SqlGrammar.InsertStatement.Select.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))));

        }

        #endregion

        #region ColumnList

        /// <summary>
        /// Describes the structure of the column list.
        /// </summary>
        public static class ColumnList
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "ColumnList";

            /// <summary>
            /// Describes the structure of a column list with multiple columns.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple columns exist.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first column.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the remaining columns.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single column exists.
            /// </summary>
            public const string Single = "single";
        }

        private void defineColumnList()
        {
            Define(ColumnList.Name)
                .Add(true, Options()
                    .Add(ColumnList.Multiple.Name, Define()
                        .Add(ColumnList.Multiple.First, true, Expression(MultipartIdentifier.Name))
                        .Add(ColumnList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(ColumnList.Multiple.Remaining, true, Expression(ColumnList.Name)))
                    .Add(ColumnList.Single, Expression(MultipartIdentifier.Name)));
        }

        #endregion

        #region UpdateStatement

        /// <summary>
        /// Describes the structure of the UPDATE statement.
        /// </summary>
        public static class UpdateStatement
        {
            /// <summary>
            /// Gets the name identifying the UPDATE statement.
            /// </summary>
            public const string Name = "UpdateStatement";

            /// <summary>
            /// Gets the identifier for the UPDATE keyword.
            /// </summary>
            public const string UpdateKeyword = "update";

            /// <summary>
            /// Gets the identifier for the table.
            /// </summary>
            public const string Table = "table";

            /// <summary>
            /// Describes the structure of the table alias.
            /// </summary>
            public static class AliasExpression
            {
                /// <summary>
                /// Gets the identifier indicating that the table is aliased.
                /// </summary>
                public const string Name = "AliasExpression";

                /// <summary>
                /// Gets the identifier for the AS keyword.
                /// </summary>
                public const string AliasIndicator = "alias_indicator";

                /// <summary>
                /// Gets the identifier for the alias.
                /// </summary>
                public const string Alias = "alias";
            }

            /// <summary>
            /// Gets the identifier for the SET keyword.
            /// </summary>
            public const string SetKeyword = "set";

            /// <summary>
            /// Gets the identifier for the setter list.
            /// </summary>
            public const string SetterList = "setter_list";

            /// <summary>
            /// Describes the structure of the WHERE clause.
            /// </summary>
            public static class Where
            {
                /// <summary>
                /// Gets the identifier indicating that there is a WHERE clause.
                /// </summary>
                public const string Name = "Where";

                /// <summary>
                /// Gets the identifier for the WHERE keyword.
                /// </summary>
                public const string WhereKeyword = "where";

                /// <summary>
                /// Gets the identifier for the filter list.
                /// </summary>
                public const string FilterList = "filter_list";
            }

            /// <summary>
            /// Describes the structure of the output clause.
            /// </summary>
            public static class Output
            {
                /// <summary>
                /// Gets the identifier indicating that there is a column list.
                /// </summary>
                public const string Name = "OutputExpression";

            }

        }

        private void defineUpdateStatement()
        {
            Define(UpdateStatement.Name)
                .Add(UpdateStatement.UpdateKeyword, true, Token(SqlTokenRegistry.Update))
                .Add(UpdateStatement.Table, true, Expression(MultipartIdentifier.Name))
                .Add(UpdateStatement.AliasExpression.Name, false, Define()
                    .Add(UpdateStatement.AliasExpression.AliasIndicator, false, Token(SqlTokenRegistry.As))
                    .Add(UpdateStatement.AliasExpression.Alias, true, Token(SqlTokenRegistry.Identifier)))
                .Add(UpdateStatement.SetKeyword, true, Token(SqlTokenRegistry.Set))
                .Add(UpdateStatement.SetterList, true, Expression(SetterList.Name))
                .Add(UpdateStatement.Output.Name, false, Expression(Output.Name))
                .Add(UpdateStatement.Where.Name, false, Define()
                    .Add(UpdateStatement.Where.WhereKeyword, true, Token(SqlTokenRegistry.Where))
                    .Add(UpdateStatement.Where.FilterList, true, Expression(OrFilter.Name)));
        }

        #endregion

        #region SetterList

        /// <summary>
        /// Describes the structure of the setter list.
        /// </summary>
        public static class SetterList
        {
            /// <summary>
            /// Gets the name identifying the setter list.
            /// </summary>
            public const string Name = "SetterList";

            /// <summary>
            /// Describes the structure of a setter list when there is more than one item.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that there are multiple setters in the list.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first setter.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the rest of the setters in the list.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that the list only has one setter.
            /// </summary>
            public const string Single = "single";
        }

        private void defineSetterList()
        {
            Define(SetterList.Name)
                .Add(true, Options()
                    .Add(SetterList.Multiple.Name, Define()
                        .Add(SetterList.Multiple.First, true, Expression(Setter.Name))
                        .Add(SetterList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(SetterList.Multiple.Remaining, true, Expression(SetterList.Name)))
                    .Add(SetterList.Single, Expression(Setter.Name)));
        }

        #endregion

        #region Setter

        /// <summary>
        /// Describes the structure of a setter.
        /// </summary>
        public static class Setter
        {
            /// <summary>
            /// Gets the name identifying the setter.
            /// </summary>
            public const string Name = "Setter";

            /// <summary>
            /// Gets the identifier for the column being assigned.
            /// </summary>
            public const string Column = "column";

            /// <summary>
            /// Gets the identifier for the assignment operator.
            /// </summary>
            public const string Assignment = "assignment";

            /// <summary>
            /// Gets the identifier for the value the column is being assigned to.
            /// </summary>
            public const string Value = "item";
        }

        private void defineSetter()
        {
            Define(Setter.Name)
                .Add(Setter.Column, true, Expression(MultipartIdentifier.Name))
                .Add(Setter.Assignment, true, Token(SqlTokenRegistry.EqualTo))
                .Add(Setter.Value, true, Expression(ArithmeticItem.Name));
        }

        #endregion

        #region DeleteStatement

        /// <summary>
        /// Describes the structure of the DELETE statement.
        /// </summary>
        public static class DeleteStatement
        {
            /// <summary>
            /// Gets the name identifying the DELETE statement.
            /// </summary>
            public const string Name = "DeleteStatement";

            /// <summary>
            /// Gets the identifier for the DELETE keyword.
            /// </summary>
            public const string DeleteKeyword = "delete";

            /// <summary>
            /// Gets the identifier for the FROM keyword.
            /// </summary>
            public const string FromKeyword = "from";

            /// <summary>
            /// Gets the identifeir for the table name.
            /// </summary>
            public const string Table = "table";

            /// <summary>
            /// Describes the structure of the alias for the table.
            /// </summary>
            public static class AliasExpression
            {
                /// <summary>
                /// Gets the identifier indicating that the table is aliased.
                /// </summary>
                public const string Name = "AliasExpression";

                /// <summary>
                /// Gets the indentifier for the AS keyword.
                /// </summary>
                public const string AliasIndicator = "alias_indicator";

                /// <summary>
                /// Gets the identifier for the alias.
                /// </summary>
                public const string Alias = "alias";
            }

            /// <summary>
            /// Describes the structure of the WHERE clause.
            /// </summary>
            public static class Where
            {
                /// <summary>
                /// Gets the indentifier that indicates whether the WHERE clause is present.
                /// </summary>
                public const string Name = "Where";

                /// <summary>
                /// Gets the indentifier for the WHERE keyword.
                /// </summary>
                public const string WhereKeyword = "where";

                /// <summary>
                /// Gets the identifier for the filter list.
                /// </summary>
                public const string FilterList = "filter_list";
            }

            /// <summary>
            /// Describes the structure of the output clause.
            /// </summary>
            public static class Output
            {
                /// <summary>
                /// Gets the identifier indicating that there is a column list.
                /// </summary>
                public const string Name = "OutputExpression";

            }
        }

        private void defineDeleteStatement()
        {
            Define(DeleteStatement.Name)
                .Add(DeleteStatement.DeleteKeyword, true, Token(SqlTokenRegistry.Delete))
                .Add(DeleteStatement.FromKeyword, false, Token(SqlTokenRegistry.From))
                .Add(DeleteStatement.Table, true, Expression(MultipartIdentifier.Name))
                .Add(DeleteStatement.AliasExpression.Name, false, Define()
                    .Add(DeleteStatement.AliasExpression.AliasIndicator, false, Token(SqlTokenRegistry.As))
                    .Add(DeleteStatement.AliasExpression.Alias, true, Token(SqlTokenRegistry.Identifier)))
                .Add(DeleteStatement.Output.Name, false, Expression(Output.Name))
                .Add(DeleteStatement.Where.Name, false, Define()
                    .Add(DeleteStatement.Where.WhereKeyword, true, Token(SqlTokenRegistry.Where))
                    .Add(DeleteStatement.Where.FilterList, true, Expression(OrFilter.Name)));
        }

        #endregion

        #region MultipartIdentifier

        /// <summary>
        /// Describes the structure of a multi-part identifier.
        /// </summary>
        public static class MultipartIdentifier
        {
            /// <summary>
            /// Gets the name identifying the multi-part identifier.
            /// </summary>
            public const string Name = "MultipartIdentifier";

            /// <summary>
            /// Describes the structure of an identifier with multiple parts.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier indicating that there are multiple parts.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first identifier.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the dot separator.
                /// </summary>
                public const string Dot = "dot";

                /// <summary>
                /// Gets the identifier for the rest of the identifiers.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier indicating that there is a single identifier.
            /// </summary>
            public const string Single = "single";
        }

        private void defineMultipartIdentifier()
        {
            Define(MultipartIdentifier.Name)
                .Add(true, Options()
                    .Add(MultipartIdentifier.Multiple.Name, Define()
                        .Add(MultipartIdentifier.Multiple.First, true, Token(SqlTokenRegistry.Identifier))
                        .Add(MultipartIdentifier.Multiple.Dot, true, Token(SqlTokenRegistry.Dot))
                        .Add(MultipartIdentifier.Multiple.Remaining, true, Expression(MultipartIdentifier.Name)))
                    .Add(MultipartIdentifier.Single, Token(SqlTokenRegistry.Identifier)));
        }

        #endregion

        #region Output

        /// <summary>
        /// Describes the structure of an OutputExpression
        /// </summary>
        public static class Output
        {
            /// <summary>
            /// Gets the name identifying the output expression.
            /// </summary>
            public const string Name = "OutputClause";

            /// <summary>
            /// Gets the name identifying the output keyword.
            /// </summary>
            public const string OutputKeyword = "output";

            /// <summary>
            /// Describes the structure of the SELECT expression generating the output values.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// Gets the identifier indicating that the values come from a SELECT statement.
                /// </summary>
                public const string Name = "OutputColumnList";

                /// <summary>
                /// Gets the identifier for the select statement.
                /// </summary>
                public const string SelectStatement = "select_statement";

            }

        }

        private void defineOutput()
        {
            Define(Output.Name)
              .Add(Output.OutputKeyword, true, Token(SqlTokenRegistry.Output))
              .Add(Output.Columns.Name, true, Expression(ProjectionList.Name));
            // .Add(Output.Columns.Name, true, Expression(ColumnList.Name));
        }

        #endregion

        #region DDL

        #region Create

        /// <summary>
        /// Describes the structure of the Create statement.
        /// </summary>
        public static class CreateStatement
        {
            /// <summary>
            /// Gets the name identifying the CREATE statement.
            /// </summary>
            public const string Name = "CreateStatement";

            /// <summary>
            /// Gets the identifier for the CreateDatabaseExpression.
            /// </summary>
            public const string CreateDatabaseExpressionName = "CreateDatabaseExpression";

            /// <summary>
            /// Gets the identifier for the CreateTableExpression.
            /// </summary>
            public const string CreateTableExpressionName = "CreateTableExpression";

        }

        private void defineCreateStatement()
        {
            Define(CreateStatement.Name)
                .Add(true, Options()
                    .Add(CreateStatement.CreateDatabaseExpressionName, Expression(CreateDatabaseStatement.Name))
                    .Add(CreateStatement.CreateTableExpressionName, Expression(CreateTableStatement.Name)));
        }

        /// <summary>
        /// Describes the structure of the Create Database statement.
        /// </summary>
        public static class CreateDatabaseStatement
        {
            /// <summary>
            /// Gets the name identifying the CREATE statement.
            /// </summary>
            public const string Name = "CreateDatabaseStatement";

            /// <summary>
            /// Gets the identifier for the CREATE keyword.
            /// </summary>
            public const string CreateKeyword = "create";

            /// <summary>
            /// Gets the identifier for the DATABASE keyword.
            /// </summary>
            public const string DatabaseKeyword = "database";

            /// <summary>
            /// Gets the identifier for the table name.
            /// </summary>
            public const string DatabaseName = "databasename";

            /// <summary>
            /// Describes the structure of the Collate statement.
            /// </summary>
            public static class Collate
            {

                /// <summary>
                /// Gets the identifier for the Name keyword.
                /// </summary>
                public const string Name = "collate";

                /// <summary>
                /// Gets the identifier for the DATABASE keyword.
                /// </summary>
                public const string CollateKeyword = "collate";

                /// <summary>
                /// Gets the identifier for the collation.
                /// </summary>
                public const string Collation = "collation";
            }
        }

        private void defineCreateDatabase()
        {
            Define(CreateDatabaseStatement.Name)
                .Add(CreateDatabaseStatement.CreateKeyword, true, Token(SqlTokenRegistry.Create))
                .Add(CreateDatabaseStatement.DatabaseKeyword, true, Token(SqlTokenRegistry.Database))
                .Add(CreateDatabaseStatement.DatabaseName, true, Token(SqlTokenRegistry.Identifier))
                .Add(CreateDatabaseStatement.Collate.Name, false, Define()
                    .Add(CreateDatabaseStatement.Collate.CollateKeyword, true, Token(SqlTokenRegistry.Collate))
                    .Add(CreateDatabaseStatement.Collate.Collation, true, Token(SqlTokenRegistry.Identifier)));
        }

        #endregion

        #region CreateTable

        /// <summary>
        /// Describes the structure of the Create Table statement.
        /// </summary>
        public static class CreateTableStatement
        {
            /// <summary>
            /// Gets the name identifying the CREATE TABLE statement.
            /// </summary>
            public const string Name = "CreateTable";

            /// <summary>
            /// Gets the identifier for the CREATE keyword.
            /// </summary>
            public const string CreateKeyword = "create";

            /// <summary>
            /// Gets the identifier for the TABLE keyword.
            /// </summary>
            public const string TableKeyword = "table";

            /// <summary>
            /// Gets the identifier for the table name.
            /// </summary>
            public const string TableName = "tablename";

            /// <summary>
            /// Describes the structure of the table.
            /// </summary>
            public static class TableDefinition
            {
                /// <summary>
                /// Gets the identifier indicating that there is a columns definition list.
                /// </summary>
                public const string Name = "TableDefinition";

                /// <summary>
                /// Gets the identifier for the left parenthesis.
                /// </summary>
                public const string LeftParenthesis = "left_parenthesis";

                /// <summary>
                /// Gets the identifier for the columns definition list.
                /// </summary>
                public const string ColumnsDefinitionList = "tabledefinition_columnslist";

                /// <summary>
                /// Gets the identifier for the right parenthesis.
                /// </summary>
                public const string RightParenthesis = "right_parenthesis";
            }

        }

        private void defineCreateTable()
        {

            Define(CreateTableStatement.Name)
                .Add(CreateTableStatement.CreateKeyword, true, Token(SqlTokenRegistry.Create))
                .Add(CreateTableStatement.TableKeyword, true, Token(SqlTokenRegistry.Table))
                .Add(CreateTableStatement.TableName, true, Expression(MultipartIdentifier.Name))
                .Add(false, Options()
                    .Add(CreateTableStatement.TableDefinition.Name, Define()
                    .Add(CreateTableStatement.TableDefinition.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                    .Add(CreateTableStatement.TableDefinition.ColumnsDefinitionList, true, Expression(ColumnDefinitionList.Name))
                    .Add(CreateTableStatement.TableDefinition.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))));

        }

        #endregion

        #region ColumnConstraintList

        /// <summary>
        /// Describes the structure of the column constraint list.
        /// </summary>
        public static class ColumnConstraintList
        {
            /// <summary>
            /// Gets the name identifying the column constraint list.
            /// </summary>
            public const string Name = "ColumnConstraintList";

            /// <summary>
            /// Describes the structure of a column constraint list with multiple columns.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple column definitions exist.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first column.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the remaining columns.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single column definition exists.
            /// </summary>
            public const string Single = "single";

        }

        private void defineColumnConstraintList()
        {
            Define(ColumnConstraintList.Name)
                .Add(true, Options()
                    .Add(ColumnConstraintList.Multiple.Name, Define()
                        .Add(ColumnConstraintList.Multiple.First, true, Expression(ColumnConstraint.Name))
                        .Add(ColumnConstraintList.Multiple.Remaining, true, Expression(ColumnConstraintList.Name)))
                    .Add(ColumnConstraintList.Single, Expression(ColumnConstraint.Name)));
        }

        #endregion

        #region ColumnConstraint

        /// <summary>
        /// Describes the structure of the column constraint.
        /// </summary>
        public static class ColumnConstraint
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "ColumnConstraint";

            /// <summary>
            /// 
            /// </summary>
            public static class PrimaryKey
            {

                /// <summary>
                /// Gets the name identifying PrimaryKey Syntax.
                /// </summary>
                public const string Name = "PrimaryKey";

                /// <summary>
                /// Gets the name identifying Primary Keyword.
                /// </summary>
                public const string PrimaryKeyword = "PrimaryKeyword";

                /// <summary>
                /// Gets the name identifying Key Keyword.
                /// </summary>
                public const string KeyKeyword = "KeyKeyword";
            }

            /// <summary>
            /// Describes the structure of the Unique constraint syntax.
            /// </summary>
            public static class Unique
            {

                /// <summary>
                /// Gets the name identifying Unique Syntax.
                /// </summary>
                public const string Name = "Unique";

                /// <summary>
                /// Gets the name identifying Primary Keyword.
                /// </summary>
                public const string UniqueKeyword = "UniqueKeyword";

            }

            /// <summary>
            /// Describes the structure of the PrimarKey Or UniqueConstraint syntax.
            /// </summary>
            public static class PrimarKeyOrUniqueConstraint
            {

                /// <summary>
                /// Gets the name identifying Constraint.
                /// </summary>
                public const string Name = "PrimarKeyOrUniqueConstraint";


            }

            /// <summary>
            /// Gets the name identifying the Clustered Keyword.
            /// </summary>
            public const string ClusteredKeyword = "ClusteredKeyword";

            /// <summary>
            /// Gets the name identifying the Non Clustered Keyword.
            /// </summary>
            public const string NonClusteredKeyword = "NonClusteredKeyword";

            /// <summary>
            /// Gets the name identifying the Foreign Key.
            /// </summary>
            public const string ForeignKeyExpressionName = "ForeignKeyExpression";


            /// <summary>
            /// Describes the structure of the Unique constraint syntax.
            /// </summary>
            public static class Check
            {

                /// <summary>
                /// Gets the name identifying Unique Syntax.
                /// </summary>
                public const string Name = "Check";

                /// <summary>
                /// Gets the name identifying Check Keyword.
                /// </summary>
                public const string CheckKeyword = "CheckKeyword";

                /// <summary>
                /// Gets the name identifying the Not For Replication Expression.
                /// </summary>
                public const string NotForReplicationExpressionName = "NotForReplicationExpression";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string LeftParenthesis = "LeftParenthesis";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string RightParenthesis = "RightParenthesis";

            }

        }

        private void defineColumnConstraint()
        {

            /*
               <column_constraint> ::= 
       [ CONSTRAINT constraint_name ] 
       {     { PRIMARY KEY | UNIQUE } 
               [ CLUSTERED | NONCLUSTERED ] 
               [ 
                   WITH FILLFACTOR = fillfactor  
                 | WITH ( < index_option > [ , ...n ] ) 
               ] 
               [ ON { partition_scheme_name ( partition_column_name ) 
                   | filegroup | "default" } ]

         | [ FOREIGN KEY ] 
               REFERENCES [ schema_name . ] referenced_table_name [ ( ref_column ) ] 
               [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
               [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
               [ NOT FOR REPLICATION ] 

         | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
       } 
       */

            // TODO: ADD SUPPORT FOR 'logical_expression' OF THE CHECK CONSTRAINT
            // TODO: ADD SUPPORT FOR 'WITH FILLFACTOR = fillfactor'
            // TODO: ADD SUPPORT FOR '< index_option >'
            // TODO: ADD SUPPORT FOR 'ON { partition_scheme_name ( partition_column_name ) | filegroup | "default" } '

            Define(ColumnConstraint.Name)
                .Add(ColumnDefinition.Constraint.Name, false, Define()
                    .Add(ColumnDefinition.Constraint.ConstraintKeyword, true, Token(SqlTokenRegistry.Constraint))
                    .Add(ColumnDefinition.Constraint.ConstraintName, true, Token(SqlTokenRegistry.Identifier)))
                .Add(true, Options()
                    .Add(ColumnConstraint.PrimarKeyOrUniqueConstraint.Name, Define()
                        .Add(true, Options()
                            .Add(ColumnConstraint.PrimaryKey.Name, Define()
                                .Add(ColumnConstraint.PrimaryKey.PrimaryKeyword, true, Token(SqlTokenRegistry.Primary))
                                .Add(ColumnConstraint.PrimaryKey.KeyKeyword, true, Token(SqlTokenRegistry.Key)))
                            .Add(ColumnConstraint.Unique.Name, Define()
                                .Add(ColumnConstraint.Unique.UniqueKeyword, true, Token(SqlTokenRegistry.Unique))))
                        .Add(false, Options()
                            .Add(ColumnConstraint.ClusteredKeyword, Token(SqlTokenRegistry.Clustered))
                            .Add(ColumnConstraint.NonClusteredKeyword, Token(SqlTokenRegistry.NonClustered))))
                    .Add(ColumnConstraint.ForeignKeyExpressionName, Expression(ForeignKeyConstraint.Name))
                    .Add(ColumnConstraint.Check.Name, Define()
                        .Add(ColumnConstraint.Check.CheckKeyword, true, Token(SqlTokenRegistry.Check))
                        .Add(ColumnConstraint.Check.NotForReplicationExpressionName, false, Expression(NotForReplication.Name))
                        .Add(ColumnConstraint.Check.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        //TODO: Need to figure out how to define a 'logical_expression' (an expression which returns true or false..
                        .Add(ColumnConstraint.Check.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))));

        }

        #endregion

        #region ForeignKeyConstraint

        /// <summary>
        /// Describes the structure of the column constraint.
        /// </summary>
        public static class ForeignKeyConstraint
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "ForeignKeyConstraint";

            /// <summary>
            /// Gets the name identifying the Not For Replication Expression.
            /// </summary>
            public const string NotForReplicationExpressionName = "NotForReplicationExpression";

            /// <summary>
            /// Foreign Key
            /// </summary>
            public static class ForeignKey
            {

                /// <summary>
                /// Gets the name identifying PrimaryKey Syntax.
                /// </summary>
                public const string Name = "ForeignKey";

                /// <summary>
                /// Gets the name identifying the Foreign Keyword.
                /// </summary>
                public const string ForeignKeyword = "ForeignKeyword";

                /// <summary>
                /// Gets the name identifying the Key Keyword.
                /// </summary>
                public const string KeyKeyword = "KeyKeyword";

            }

            /// <summary>
            /// References
            /// </summary>
            public static class References
            {

                /// <summary>
                /// Gets the name identifying References Syntax.
                /// </summary>
                public const string Name = "References";

                /// <summary>
                /// Gets the name identifying the References Keyword.
                /// </summary>
                public const string ReferencesKeyword = "ReferencesKeyword";

            }

            /// <summary>
            /// ReferencedTable
            /// </summary>
            public static class ReferencedTable
            {
                /// <summary>
                /// Gets the name identifying ReferencedTable Syntax.
                /// </summary>
                public const string Name = "ReferencedTable";
            }

            /// <summary>
            /// ReferencedColumn
            /// </summary>
            public static class ReferencedColumn
            {
                /// <summary>
                /// Gets the name identifying ReferencedColumn Syntax.
                /// </summary>
                public const string Name = "ReferencedColumn";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string LeftParenthesis = "LeftParenthesis";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string RightParenthesis = "RightParenthesis";

                /// <summary>
                /// Gets the name identifying ColumnName.
                /// </summary>
                public const string ColumnName = "ColumnName";


            }

            /// <summary>
            /// On
            /// </summary>
            public static class On
            {
                /// <summary>
                /// Gets the name identifying On Syntax.
                /// </summary>
                public const string Name = "On";

                /// <summary>
                /// Gets the name identifying On Keyword.
                /// </summary>
                public const string OnKeyword = "OnKeyword";

                /// <summary>
                /// Delete
                /// </summary>
                public static class Delete
                {
                    /// <summary>
                    /// Gets the name Delete Syntax.
                    /// </summary>
                    public const string Name = "Delete";

                    /// <summary>
                    /// Gets the name identifying Delete Keyword.
                    /// </summary>
                    public const string DeleteKeyword = "DeleteKeyword";
                }

                /// <summary>
                /// Update
                /// </summary>
                public static class Update
                {
                    /// <summary>
                    /// Gets the name Delete Syntax.
                    /// </summary>
                    public const string Name = "Update";

                    /// <summary>
                    /// Gets the name identifying Update Keyword.
                    /// </summary>
                    public const string UpdateKeyword = "UpdateKeyword";
                }

                /// <summary>
                /// NoAction
                /// </summary>
                public static class NoAction
                {
                    /// <summary>
                    /// Gets the name Delete Syntax.
                    /// </summary>
                    public const string Name = "NoAction";

                    /// <summary>
                    /// Gets the name identifying No Keyword.
                    /// </summary>
                    public const string NoKeyword = "NoKeyword";

                    /// <summary>
                    /// Gets the name identifying Action Keyword.
                    /// </summary>
                    public const string ActionKeyword = "ActionKeyword";
                }

                /// <summary>
                /// Gets the name identifying Cascade Keyword 
                /// </summary>
                public const string CascadeKeyword = "CascadeKeyword";

                /// <summary>
                /// SetNull
                /// </summary>
                public static class SetNull
                {
                    /// <summary>
                    /// Gets the name Set Null Syntax.
                    /// </summary>
                    public const string Name = "SetNull";

                    /// <summary>
                    /// Gets the name identifying Set Keyword.
                    /// </summary>
                    public const string SetKeyword = "SetKeyword";

                    /// <summary>
                    /// Gets the name identifying Null Keyword.
                    /// </summary>
                    public const string NullKeyword = "NullKeyword";
                }

                /// <summary>
                /// SetDefault
                /// </summary>
                public static class SetDefault
                {
                    /// <summary>
                    /// Gets the name Set Default Syntax.
                    /// </summary>
                    public const string Name = "SetDefault";

                    /// <summary>
                    /// Gets the name identifying Set Keyword.
                    /// </summary>
                    public const string SetKeyword = "SetKeyword";

                    /// <summary>
                    /// Gets the name identifying Default Keyword.
                    /// </summary>
                    public const string DefaultKeyword = "DefaultKeyword";
                }
            }

        }

        private void defineForeignKeyConstraint()
        {
            /*
           [ FOREIGN KEY ] 
        REFERENCES [ schema_name . ] referenced_table_name [ ( ref_column ) ] 
        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ NOT FOR REPLICATION ] 
            */
            Define(ForeignKeyConstraint.Name)
                .Add(ForeignKeyConstraint.ForeignKey.Name, false, Define()
                    .Add(ForeignKeyConstraint.ForeignKey.ForeignKeyword, true, Token(SqlTokenRegistry.Foreign))
                    .Add(ForeignKeyConstraint.ForeignKey.KeyKeyword, true, Token(SqlTokenRegistry.Key)))
                .Add(ForeignKeyConstraint.References.Name, true, Define()
                    .Add(ForeignKeyConstraint.References.ReferencesKeyword, true, Token(SqlTokenRegistry.References))
                    .Add(ForeignKeyConstraint.ReferencedTable.Name, true, Expression(MultipartIdentifier.Name))
                    .Add(ForeignKeyConstraint.ReferencedColumn.Name, false, Define()
                        .Add(ForeignKeyConstraint.ReferencedColumn.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                        .Add(ForeignKeyConstraint.ReferencedColumn.ColumnName, true, Token(SqlTokenRegistry.Identifier))
                        .Add(ForeignKeyConstraint.ReferencedColumn.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis))))
                .Add(ForeignKeyConstraint.On.Delete.Name, false, Define()
                    .Add(ForeignKeyConstraint.On.OnKeyword, true, Token(SqlTokenRegistry.On))
                    .Add(ForeignKeyConstraint.On.Delete.DeleteKeyword, true, Token(SqlTokenRegistry.Delete))
                    .Add(true, Options()
                        .Add(ForeignKeyConstraint.On.NoAction.Name, Define()
                            .Add(ForeignKeyConstraint.On.NoAction.NoKeyword, true, Token(SqlTokenRegistry.No))
                            .Add(ForeignKeyConstraint.On.NoAction.ActionKeyword, true, Token(SqlTokenRegistry.Action)))
                        .Add(ForeignKeyConstraint.On.CascadeKeyword, Token(SqlTokenRegistry.Cascade))
                        .Add(ForeignKeyConstraint.On.SetNull.Name, Define()
                            .Add(ForeignKeyConstraint.On.SetNull.SetKeyword, true, Token(SqlTokenRegistry.Set))
                            .Add(ForeignKeyConstraint.On.SetNull.NullKeyword, true, Token(SqlTokenRegistry.Null)))
                        .Add(ForeignKeyConstraint.On.SetDefault.Name, Define()
                            .Add(ForeignKeyConstraint.On.SetDefault.SetKeyword, true, Token(SqlTokenRegistry.Set))
                            .Add(ForeignKeyConstraint.On.SetDefault.DefaultKeyword, true, Token(SqlTokenRegistry.Default)))))
                .Add(ForeignKeyConstraint.On.Update.Name, false, Define()
                    .Add(ForeignKeyConstraint.On.OnKeyword, true, Token(SqlTokenRegistry.On))
                    .Add(ForeignKeyConstraint.On.Update.UpdateKeyword, true, Token(SqlTokenRegistry.Update))
                    .Add(true, Options()
                        .Add(ForeignKeyConstraint.On.NoAction.Name, Define()
                             .Add(ForeignKeyConstraint.On.NoAction.NoKeyword, true, Token(SqlTokenRegistry.No))
                             .Add(ForeignKeyConstraint.On.NoAction.ActionKeyword, true, Token(SqlTokenRegistry.Action)))
                        .Add(ForeignKeyConstraint.On.CascadeKeyword, Token(SqlTokenRegistry.Cascade))
                        .Add(ForeignKeyConstraint.On.SetNull.Name, Define()
                             .Add(ForeignKeyConstraint.On.SetNull.SetKeyword, true, Token(SqlTokenRegistry.Set))
                             .Add(ForeignKeyConstraint.On.SetNull.NullKeyword, true, Token(SqlTokenRegistry.Null)))
                        .Add(ForeignKeyConstraint.On.SetDefault.Name, Define()
                             .Add(ForeignKeyConstraint.On.SetDefault.SetKeyword, true, Token(SqlTokenRegistry.Set))
                             .Add(ForeignKeyConstraint.On.SetDefault.DefaultKeyword, true, Token(SqlTokenRegistry.Default)))))
                .Add(ForeignKeyConstraint.NotForReplicationExpressionName, false, Expression(NotForReplication.Name));

        }

        #endregion

        #region NotForReplication

        /// <summary>
        /// NotForReplication
        /// </summary>
        public static class NotForReplication
        {
            /// <summary>
            /// Gets the name Delete Syntax.
            /// </summary>
            public const string Name = "NotForReplication";

            /// <summary>
            /// Gets the name identifying Not Keyword.
            /// </summary>
            public const string NotKeyword = "NotKeyword";

            /// <summary>
            /// Gets the name identifying For Keyword.
            /// </summary>
            public const string ForKeyword = "ForKeyword";

            /// <summary>
            /// Gets the name identifying Replication Keyword.
            /// </summary>
            public const string ReplicationKeyword = "ReplicationKeyword";
        }

        private void defineNotForReplication()
        {
            /*        
                 NOT FOR REPLICATION 
            */
            Define(NotForReplication.Name)
                 .Add(NotForReplication.NotKeyword, true, Token(SqlTokenRegistry.Not))
                 .Add(NotForReplication.ForKeyword, true, Token(SqlTokenRegistry.For))
                 .Add(NotForReplication.ReplicationKeyword, true, Token(SqlTokenRegistry.Replication));

        }

        #endregion

        #region Alter

        /// <summary>
        /// Describes the structure of the Alter statement.
        /// </summary>
        public static class AlterStatement
        {
            /// <summary>
            /// Gets the name identifying the Alter statement.
            /// </summary>
            public const string Name = "AlterStatement";

            /// <summary>
            /// Gets the identifier for the AlterDatabaseExpression.
            /// </summary>
            public const string AlterDatabaseExpressionName = "AlterDatabaseExpression";

            /// <summary>
            /// Gets the identifier for the AlterTableExpression.
            /// </summary>
            public const string AlterTableExpressionName = "AlterTableExpression";

        }

        private void defineAlterStatement()
        {
            Define(AlterStatement.Name)
                .Add(true, Options()
                      .Add(AlterStatement.AlterDatabaseExpressionName, Expression(AlterDatabaseStatement.Name))
                      .Add(AlterStatement.AlterTableExpressionName, Expression(AlterTableStatement.Name))
            );
            // .Add(AlterStatement.AlterTableExpressionName, Expression(AlterTableStatement.Name))
            // );
        }

        /// <summary>
        /// Describes the structure of the Alter Database statement.
        /// </summary>
        public static class AlterDatabaseStatement
        {
            /// <summary>
            /// Gets the name identifying the Alter Database statement.
            /// </summary>
            public const string Name = "AlterDatabaseStatement";

            /// <summary>
            /// Gets the identifier for the CREATE keyword.
            /// </summary>
            public const string AlterKeyword = "alter";

            /// <summary>
            /// Gets the identifier for the DATABASE keyword.
            /// </summary>
            public const string DatabaseKeyword = "database";

            /// <summary>
            /// Gets the identifier for the Current keyword.
            /// </summary>
            public const string CurrentKeyword = "current";

            /// <summary>
            /// Gets the identifier for the database name.
            /// </summary>
            public const string DatabaseName = "databasename";

            /// <summary>
            /// Describes the structure of the ModifyName statement.
            /// </summary>
            public static class ModifyName
            {

                /// <summary>
                /// Gets the identifier for the ModifyName statement.
                /// </summary>
                public const string Name = "modifyname";

                /// <summary>
                /// Gets the identifier for the MODIFY keyword.
                /// </summary>
                public const string ModifyKeyword = "modify";

                /// <summary>
                /// Gets the identifier for the EqualTo Keyword.
                /// </summary>
                public const string EqualToKeyword = "equalto";

                /// <summary>
                /// Gets the identifier for the NameKeyword.
                /// </summary>
                public const string NameKeyword = "name";

                /// <summary>
                /// Gets the identifier for the New Database Name.
                /// </summary>
                public const string NewDatabaseName = "newdatabasename";
            }

        }

        private void defineAlterDatabase()
        {
            Define(AlterDatabaseStatement.Name)
                .Add(AlterDatabaseStatement.AlterKeyword, true, Token(SqlTokenRegistry.Alter))
                .Add(AlterDatabaseStatement.DatabaseKeyword, true, Token(SqlTokenRegistry.Database))
                .Add(true, Options()
                    .Add(AlterDatabaseStatement.CurrentKeyword, Token(SqlTokenRegistry.Current))
                    .Add(AlterDatabaseStatement.DatabaseName, Token(SqlTokenRegistry.Identifier)))
                .Add(true, Options()
                    .Add(AlterDatabaseStatement.ModifyName.Name, Define()
                        .Add(AlterDatabaseStatement.ModifyName.ModifyKeyword, true, Token(SqlTokenRegistry.ModifyName))
                        .Add(AlterDatabaseStatement.ModifyName.EqualToKeyword, true, Token(SqlTokenRegistry.EqualTo))
                        .Add(AlterDatabaseStatement.ModifyName.NewDatabaseName, true, Token(SqlTokenRegistry.Identifier))
                      )
                    .Add(Collate.Name, Expression(Collate.Name)));
        }

        /// <summary>
        /// Describes the structure of the Alter Table statement.
        /// </summary>
        public static class AlterTableStatement
        {
            /// <summary>
            /// Gets the name identifying the ALTER Database statement.
            /// </summary>
            public const string Name = "AlterTableStatement";

            /// <summary>
            /// Gets the identifier for the CREATE keyword.
            /// </summary>
            public const string AlterKeyword = "alter";

            /// <summary>
            /// Gets the identifier for the TABLE keyword.
            /// </summary>
            public const string TableKeyword = "table";

            /// <summary>
            /// Gets the identifier for the table name.
            /// </summary>
            public const string TableName = "tablename";

            /// <summary>
            /// Describes the structure of the AlterColumn statement.
            /// </summary>
            public static class AlterColumn
            {

                /// <summary>
                /// Gets the identifier for the ALTER keyword.
                /// </summary>
                public const string AlterKeyword = "alterkeyword";

                /// <summary>
                /// Gets the identifier for the ModifyName statement.
                /// </summary>
                public const string Name = "altercolumn";

                /// <summary>
                /// Gets the identifier for the NameKeyword.
                /// </summary>
                public const string ColumnKeyword = "column";

                /// <summary>
                /// Gets the identifier for the ColumnName.
                /// </summary>
                public const string ColumnName = "columnname";

                /// <summary>
                /// Gets the identifier for the alter column datatype expression.
                /// </summary>
                public const string AlterColumnDataTypeExpressionName = "altercolumndatatype";

                /// <summary>
                /// Describes the structure of the Add / Drop column clause. 
                /// </summary>
                public class AddOrDropColumnProperty
                {

                    /// <summary>
                    /// Gets the identifier for the addordropproperty statement.
                    /// </summary>
                    public const string Name = "addordropproperty";

                    /// <summary>
                    /// Gets the identifier for the Add Keyword.
                    /// </summary>
                    public const string AddKeyword = "add";

                    /// <summary>
                    /// Gets the identifier for the Drop Keyword.
                    /// </summary>
                    public const string DropKeyword = "drop";

                    /// <summary>
                    /// Gets the name identifying RowGuidCol Keyword.
                    /// </summary>
                    public const string RowGuidColKeyword = "RowGuidColKeyword";

                    /// <summary>
                    /// Gets the name identifying Persisted Keyword.
                    /// </summary>
                    public const string PersistedKeyword = "PersistedKeyword";

                    /// <summary>
                    /// Gets the name identifying NotForReplicationExpression.
                    /// </summary>
                    public const string NotForReplicationExpressionName = "NotForReplicationExpression";

                    /// <summary>
                    /// Gets the name identifying SparseKeyword.
                    /// </summary>
                    public const string SparseKeyword = "SparseKeyword";


                }




            }

            /// <summary>
            /// Describes the structure of the AlterColumn statement.
            /// </summary>
            public static class AddColumns
            {

                /// <summary>
                /// Gets the identifier for the ModifyName statement.
                /// </summary>
                public const string Name = "addcolumns";

                /// <summary>
                /// Gets the identifier for the ADD keyword.
                /// </summary>
                public const string AddKeyword = "addkeyword";

                /// <summary>
                /// Gets the identifier for the ColumnDefinitionListExpressionName expression.
                /// </summary>
                public const string ColumnDefinitionListExpressionName = "ColumnDefinitionListExpressionName";

            }

            /// <summary>
            /// Describes the structure of the DropColumnsOrConstraints statement.
            /// </summary>
            public static class DropColumnsOrConstraints
            {

                /// <summary>
                /// Gets the identifier for the ModifyName statement.
                /// </summary>
                public const string Name = "DropColumnsOrConstraints";

                /// <summary>
                /// Gets the identifier for the ADD keyword.
                /// </summary>
                public const string DropKeyword = "DropKeyword";

                /// <summary>
                /// Gets the identifier for the ColumnNameListExpressionName expression.
                /// </summary>
                public const string DropListExpressionName = "DropListExpressionName";

            }

        }

        private void defineAlterTable()
        {
            Define(AlterTableStatement.Name)
                .Add(AlterTableStatement.AlterKeyword, true, Token(SqlTokenRegistry.Alter))
                .Add(AlterTableStatement.TableKeyword, true, Token(SqlTokenRegistry.Table))
                .Add(AlterTableStatement.TableName, true, Expression(MultipartIdentifier.Name))
                .Add(true, Options()
                    .Add(AlterTableStatement.AlterColumn.Name, Define()
                        .Add(AlterTableStatement.AlterColumn.AlterKeyword, true, Token(SqlTokenRegistry.Alter))
                        .Add(AlterTableStatement.AlterColumn.ColumnKeyword, true, Token(SqlTokenRegistry.Column))
                        .Add(AlterTableStatement.AlterColumn.ColumnName, true, Token(SqlTokenRegistry.Identifier))
                        .Add(true, Options()
                            .Add(AlterTableStatement.AlterColumn.AlterColumnDataTypeExpressionName, Define()
                                .Add(DataType.Name, true, Expression(DataType.Name))
                                .Add(Collate.Name, false, Expression(Collate.Name))
                                .Add(Nullability.Name, false, Expression(Nullability.Name)))
                            .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.Name, Define()
                                .Add(true, Options()
                                     .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.AddKeyword, Token(SqlTokenRegistry.Add))
                                     .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.DropKeyword, Token(SqlTokenRegistry.Drop))
                                     )
                                .Add(true, Options()
                                    .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.RowGuidColKeyword, Token(SqlTokenRegistry.RowGuidCol))
                                    .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.PersistedKeyword, Token(SqlTokenRegistry.Persisted))
                                    .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.NotForReplicationExpressionName, Expression(NotForReplication.Name))
                                    .Add(AlterTableStatement.AlterColumn.AddOrDropColumnProperty.SparseKeyword, Token(SqlTokenRegistry.Sparse))))))
                    .Add(AlterTableStatement.AddColumns.Name, Define()
                        .Add(AlterTableStatement.AddColumns.AddKeyword, true, Token(SqlTokenRegistry.Add))
                        .Add(AlterTableStatement.AddColumns.ColumnDefinitionListExpressionName, true, Expression(ColumnDefinitionList.Name)))
                    .Add(AlterTableStatement.DropColumnsOrConstraints.Name, Define()
                        .Add(AlterTableStatement.DropColumnsOrConstraints.DropKeyword, true, Token(SqlTokenRegistry.Drop))
                        .Add(AlterTableStatement.DropColumnsOrConstraints.DropListExpressionName, true, Expression(DropTableItemsList.Name)))
                        );
        }

        #region DropTableItemsList

        /// <summary>
        /// Describes the structure of the DropItemsList.
        /// </summary>
        public static class DropTableItemsList
        {
            /// <summary>
            /// Gets the name identifying the DropTableItemsList.
            /// </summary>
            public const string Name = "DropTableItemsList";

            /// <summary>
            /// Describes the structure of a column definitions list with multiple columns.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple column definitions exist.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first column.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the remaining columns.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single DropTableItem.
            /// </summary>
            public const string Single = "single";
        }

        private void defineDropTableItemsList()
        {
            Define(DropTableItemsList.Name)
                .Add(true, Options()
                    .Add(DropTableItemsList.Multiple.Name, Define()
                        .Add(DropTableItemsList.Multiple.First, true, Expression(DropTableItem.Name))
                        .Add(DropTableItemsList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(DropTableItemsList.Multiple.Remaining, true, Expression(DropTableItemsList.Name)))
                    .Add(DropTableItemsList.Single, Expression(DropTableItem.Name)));

        }

        #endregion

        #region DropTableItem

        /// <summary>
        /// Describes the structure of the DropItem.
        /// </summary>
        public static class DropTableItem
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "DropTableItem";

            /// <summary>
            /// Gets the name identifying RowGuidCol Keyword.
            /// </summary>
            public const string ColumnKeyword = "ColumnKeyword";

            /// <summary>
            /// Gets the name identifying RowGuidCol Keyword.
            /// </summary>
            public const string ConstraintKeyword = "ConstraintKeyword";

            /// <summary>
            /// Gets the name identifying DropConstraintListExpression.
            /// </summary>
            public const string DropConstraintListExpressionName = "DropConstraintListExpression";

            /// <summary>
            /// Gets the name identifying DropColumnListExpression.
            /// </summary>
            public const string DropColumnListExpressionName = "DropColumnListExpression";

            /// <summary>
            /// Gets the name identifying DropConstraintExpression.
            /// </summary>
            public const string DropConstraintExpressionName = "DropConstraintExpression";

            /// <summary>
            /// Gets the name identifying DropColumnExpression.
            /// </summary>
            public const string DropColumnExpressionName = "DropColumnExpression";


        }

        private void defineDropTableItem()
        {
            Define(DropTableItem.Name)
                .Add(true, Options()
                     .Add(DropTableItem.DropConstraintExpressionName, Define()
                        .Add(DropTableItem.ConstraintKeyword, false, Token(SqlTokenRegistry.Constraint))
                        .Add(DropTableItem.DropConstraintListExpressionName, true, Expression(DropTableConstraintList.Name)))
                     .Add(DropTableItem.DropColumnExpressionName, Define()
                        .Add(DropTableItem.ColumnKeyword, true, Token(SqlTokenRegistry.Column))
                        .Add(DropTableItem.DropColumnListExpressionName, true, Expression(DropTableColumnList.Name)))
                        );
        }

        #endregion

        #region DropTableConstraintList

        /// <summary>
        /// Describes the structure of the DropTableConstraintList.
        /// </summary>
        public static class DropTableConstraintList
        {
            /// <summary>
            /// Gets the name identifying the DropTableConstraintList.
            /// </summary>
            public const string Name = "DropTableConstraintList";

            /// <summary>
            /// Describes the structure of a DropTableConstraint list with multiple constraints.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple constraints.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first constraint.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the remaining constraints.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single constraint exists.
            /// </summary>
            public const string Single = "single";
        }

        private void defineDropTableConstraintList()
        {
            Define(DropTableConstraintList.Name)
                .Add(true, Options()
                    .Add(DropTableConstraintList.Multiple.Name, Define()
                        .Add(DropTableConstraintList.Multiple.First, true, Expression(DropTableConstraint.Name))
                        .Add(DropTableConstraintList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(DropTableConstraintList.Multiple.Remaining, true, Expression(DropTableConstraintList.Name)))
                    .Add(DropTableConstraintList.Single, Expression(DropTableConstraint.Name)));

        }

        #endregion

        #region DropTableColumnList

        /// <summary>
        /// Describes the structure of the DropColumnList.
        /// </summary>
        public static class DropTableColumnList
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "DropColumnList";

            /// <summary>
            /// Describes the structure of a column definitions list with multiple columns.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple column definitions exist.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first column.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the remaining columns.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single column definition exists.
            /// </summary>
            public const string Single = "single";
        }

        private void defineDropTableColumnList()
        {
            Define(DropTableColumnList.Name)
                .Add(true, Options()
                    .Add(DropTableColumnList.Multiple.Name, Define()
                        .Add(DropTableColumnList.Multiple.First, true, Expression(DropTableColumn.Name))
                        .Add(DropTableColumnList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(DropTableColumnList.Multiple.Remaining, true, Expression(DropTableColumnList.Name)))
                    .Add(DropTableColumnList.Single, Expression(DropTableColumn.Name)));

        }

        #endregion

        #region DropTableColumn

        /// <summary>
        /// Describes the structure of the DropColumn.
        /// </summary>
        public static class DropTableColumn
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "DropColumn";

            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string ColumnName = "ColumnName";

            ///// <summary>
            ///// Gets the name identifying RowGuidCol Keyword.
            ///// </summary>
            //public const string ColumnKeyword = "ColumnKeyword";

        }

        private void defineDropTableColumn()
        {
            Define(DropTableColumn.Name)
                .Add(DropTableColumn.ColumnName, true, Token(SqlTokenRegistry.Identifier));
        }

        #endregion

        #region DropTableConstraint

        /// <summary>
        /// Describes the structure of the TableDropItem.
        /// </summary>
        public static class DropTableConstraint
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "DropTableConstraint";

            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string ConstraintName = "ConstraintName";

            ///// <summary>
            ///// Gets the name identifying RowGuidCol Keyword.
            ///// </summary>
            //public const string ConstraintKeyword = "ConstraintKeyword";                 

        }

        private void defineDropTableConstraint()
        {
            Define(DropTableConstraint.Name)
                .Add(DropTableConstraint.ConstraintName, true, Token(SqlTokenRegistry.Identifier));

        }

        #endregion

        #endregion

        /// <summary>
        /// Describes the structure of the Collate statement.
        /// </summary>
        public static class Collate
        {

            /// <summary>
            /// Gets the identifier for the Name keyword.
            /// </summary>
            public const string Name = "collate";

            /// <summary>
            /// Gets the identifier for the DATABASE keyword.
            /// </summary>
            public const string CollateKeyword = "collate";

            /// <summary>
            /// Gets the identifier for the collation.
            /// </summary>
            public const string Collation = "collation";
        }

        private void defineCollate()
        {
            Define(Collate.Name)
             .Add(Collate.CollateKeyword, true, Token(SqlTokenRegistry.Collate))
             .Add(Collate.Collation, true, Token(SqlTokenRegistry.Identifier));
        }

        /// <summary>
        /// Describes the structure of the DataType syntax.
        /// </summary>
        public static class DataType
        {

            /// <summary>
            /// Gets the name identifying the Column Data Type expression.
            /// </summary>
            public const string Name = "ColumnDataType";

            /// <summary>
            /// Gets the name identifying the Column DataTypeName.
            /// </summary>
            public const string DataTypeName = "DataTypeName";

            /// <summary>
            /// Gets the name identifying the column size.
            /// </summary>
            public const string ColumnSize = "ColumnSize";

            /// <summary>
            /// Gets the name identifying LeftParethesis.
            /// </summary>
            public const string LeftParenthesis = "LeftParenthesis";

            /// <summary>
            /// Gets the name identifying LeftParethesis.
            /// </summary>
            public const string RightParenthesis = "RightParenthesis";

            /// <summary>
            /// Gets the name identifying LeftParethesis.
            /// </summary>
            public const string ColumnSizeArguments = "ColumnSizeArguments";
        }

        private void defineDataType()
        {
            Define(DataType.Name)
                .Add(DataType.DataTypeName, true, Expression(MultipartIdentifier.Name))
                .Add(DataType.ColumnSize, false, Define()
                      .Add(DataType.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                      .Add(DataType.ColumnSizeArguments, true, Expression(ValueList.Name))
                      .Add(DataType.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)));
        }

        /// <summary>
        /// Describes the structure of the Nullable syntax.
        /// </summary>
        public static class Nullability
        {
            /// <summary>
            /// Gets the name identifying Column Nullable.
            /// </summary>
            public const string Name = "ColumnNullable";

            /// <summary>
            /// Gets the name identifying ColumnNull.
            /// </summary>
            public const string NullKeyword = "NullKeyword";

            /// <summary>
            /// Gets the name identifying Not.
            /// </summary>
            public const string NotKeyword = "NotKeyword";
        }

        private void defineNullability()
        {
            Define(Nullability.Name)
                    .Add(Nullability.NotKeyword, false, Token(SqlTokenRegistry.Not))
                    .Add(Nullability.NullKeyword, true, Token(SqlTokenRegistry.Null));
        }

        /// <summary>
        /// Describes the structure of the column definitions list.
        /// </summary>
        public static class ColumnDefinition
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "ColumnDefinition";

            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string ColumnName = "ColumnName";

            /// <summary>
            /// Gets the name identifying RowGuidCol Keyword.
            /// </summary>
            public const string RowGuidColKeyword = "RowGuidColKeyword";

            /// <summary>
            /// Describes the structure of the Default syntax.
            /// </summary>
            public static class Default
            {

                /// <summary>
                /// Gets the name identifying the 'Default' syntax.
                /// </summary>
                public const string Name = "Default";

                /// <summary>
                /// Gets the name identifying the 'Default' part of the syntax.
                /// </summary>
                public const string DefaultName = "Default";

                /// <summary>
                /// Gets the name identifying Default Keyword .
                /// </summary>
                public const string DefaultKeyword = "DefaultKeyword";

                /// <summary>
                /// Gets the name identifying Default Expression.
                /// </summary>
                public const string DefaultStringLiteral = "DefaultStringLiteral";

                /// <summary>
                /// Gets the name identifying Default Numeric Literal.
                /// </summary>
                public const string DefaultNumericLiteral = "DefaultNumericLiteral";

                /// <summary>
                /// Gets the name identifying Default Function.
                /// </summary>
                public const string DefaultFunction = "DefaultFunction";
            }

            /// <summary>
            /// Describes the structure of the Constraint syntax.
            /// </summary>
            public static class Constraint
            {

                /// <summary>
                /// Gets the name identifying Constraint.
                /// </summary>
                public const string Name = "Constraint";

                /// <summary>
                /// Gets the name identifying Constraint Keyword.
                /// </summary>
                public const string ConstraintKeyword = "ConstraintKeyword";

                /// <summary>
                /// Gets the name identifying Constraint Name.
                /// </summary>
                public const string ConstraintName = "ConstraintName";
            }

            /// <summary>
            /// Describes the structure of the Identity syntax.
            /// </summary>
            public static class Identity
            {
                /// <summary>
                /// Gets the name identifying Identity.
                /// </summary>
                public const string Name = "Identity";

                /// <summary>
                /// Gets the name identifying Identity Keyword.
                /// </summary>
                public const string IdentityKeyword = "IdentityKeyword";

                /// <summary>
                /// Gets the name identifying Identity Seed.
                /// </summary>
                public const string IdentitySeed = "IdentitySeed";

                /// <summary>
                /// Gets the name identifying Identity Seed Values.
                /// </summary>
                public const string IdentitySeedValues = "IdentitySeedValues";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string LeftParenthesis = "LeftParenthesis";

                /// <summary>
                /// Gets the name identifying LeftParethesis.
                /// </summary>
                public const string RightParenthesis = "RightParenthesis";

                /// <summary>
                /// Gets the name identifying NotForReplication Expression
                /// </summary>
                public const string NotForReplicationExpressionName = "NotForReplicationExpression";

            }

            /// <summary>
            /// Gets the name identifying Column Constraint List Expression
            /// </summary>
            public const string ColumnConstraintListExpressionName = "ColumnConstraintListExpression";

        }

        private void defineColumnDefinition()
        {
            Define(ColumnDefinition.Name)
                  .Add(ColumnDefinition.ColumnName, true, Token(SqlTokenRegistry.Identifier))
                  .Add(DataType.Name, true, Expression(DataType.Name))
                  .Add(Collate.Name, false, Expression(Collate.Name))
                  .Add(Nullability.Name, false, Expression(Nullability.Name))
                  .Add(false, Options()
                      .Add(ColumnDefinition.Default.Name, Define()
                          .Add(ColumnDefinition.Constraint.Name, false, Define()
                              .Add(ColumnDefinition.Constraint.ConstraintKeyword, true, Token(SqlTokenRegistry.Constraint))
                              .Add(ColumnDefinition.Constraint.ConstraintName, true, Token(SqlTokenRegistry.Identifier)))
                          .Add(ColumnDefinition.Default.DefaultName, true, Define()
                              .Add(ColumnDefinition.Default.DefaultKeyword, true, Token(SqlTokenRegistry.Default))
                              .Add(true, Options()
                                  .Add(ColumnDefinition.Default.DefaultStringLiteral, Token(SqlTokenRegistry.String))
                                  .Add(ColumnDefinition.Default.DefaultNumericLiteral, Token(SqlTokenRegistry.Number))
                                  .Add(ColumnDefinition.Default.DefaultFunction, Expression(FunctionCall.Name)))))
                      .Add(ColumnDefinition.Identity.Name, Define()
                          .Add(ColumnDefinition.Identity.IdentityKeyword, true, Token(SqlTokenRegistry.Identity))
                          .Add(ColumnDefinition.Identity.IdentitySeed, false, Define()
                              .Add(ColumnDefinition.Identity.LeftParenthesis, true, Token(SqlTokenRegistry.LeftParenthesis))
                              .Add(ColumnDefinition.Identity.IdentitySeedValues, true, Expression(ValueList.Name))
                              .Add(ColumnDefinition.Identity.RightParenthesis, true, Token(SqlTokenRegistry.RightParenthesis)))
                          .Add(ColumnDefinition.Identity.NotForReplicationExpressionName, false, Expression(NotForReplication.Name))))
                  .Add(ColumnDefinition.RowGuidColKeyword, false, Token(SqlTokenRegistry.RowGuidCol))
                  .Add(ColumnDefinition.ColumnConstraintListExpressionName, false, Expression(ColumnConstraintList.Name));

            // Then we can have an optional column index.

            /*              <column_index> ::=
              INDEX index_name
            { [ NONCLUSTERED ] HASH WITH (BUCKET_COUNT = bucket_count) | [ NONCLUSTERED ] }                       
            */


        }

        #region ColumnDefinitionList

        /// <summary>
        /// Describes the structure of the column definitions list.
        /// </summary>
        public static class ColumnDefinitionList
        {
            /// <summary>
            /// Gets the name identifying the column list.
            /// </summary>
            public const string Name = "ColumnDefinitionList";

            /// <summary>
            /// Describes the structure of a column definitions list with multiple columns.
            /// </summary>
            public static class Multiple
            {
                /// <summary>
                /// Gets the identifier used to indicate that multiple column definitions exist.
                /// </summary>
                public const string Name = "Multiple";

                /// <summary>
                /// Gets the identifier for the first column.
                /// </summary>
                public const string First = "first";

                /// <summary>
                /// Gets the identifier for the comma separator.
                /// </summary>
                public const string Comma = "comma";

                /// <summary>
                /// Gets the identifier for the remaining columns.
                /// </summary>
                public const string Remaining = "remaining";
            }

            /// <summary>
            /// Gets the identifier used to indicate that a single column definition exists.
            /// </summary>
            public const string Single = "single";
        }

        private void defineColumnDefinitionsList()
        {
            Define(ColumnDefinitionList.Name)
                .Add(true, Options()
                    .Add(ColumnDefinitionList.Multiple.Name, Define()
                        .Add(ColumnDefinitionList.Multiple.First, true, Expression(ColumnDefinition.Name))
                        .Add(ColumnDefinitionList.Multiple.Comma, true, Token(SqlTokenRegistry.Comma))
                        .Add(ColumnDefinitionList.Multiple.Remaining, true, Expression(ColumnDefinitionList.Name)))
                    .Add(ColumnDefinitionList.Single, Expression(ColumnDefinition.Name)));

        }

        #endregion

        #endregion
    }
}
