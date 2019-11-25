using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Identifies tokens as SQL tokens.
    /// </summary>
    public class SqlTokenRegistry : TokenRegistry
    {
        /// <summary>
        /// Gets the identifier for alias indicators (AS).
        /// </summary>
        public const string As = "AliasIndicator";

        /// <summary>
        /// Gets the identifier for the addition operator.
        /// </summary>
        public const string PlusOperator = "PlusOperator";

        /// <summary>
        /// Gets the identifier for the subtraction operator.
        /// </summary>
        public const string MinusOperator = "MinusOperator";

        /// <summary>
        /// Gets the identifier for the multiplication operator.
        /// </summary>
        public const string MultiplicationOperator = "MultiplyOperator";

        /// <summary>
        /// Gets the identifier for the division operator.
        /// </summary>
        public const string DivisionOperator = "DivideOperator";

        /// <summary>
        /// Gets teh identifier for the modulus operator.
        /// </summary>
        public const string ModulusOperator = "ModulusOperator";

        /// <summary>
        /// Gets the identifier for the BETWEEN keyword.
        /// </summary>
        public const string Between = "Between";

        /// <summary>
        /// Gets the identifier for the comma separator.
        /// </summary>
        public const string Comma = "Comma";

        /// <summary>
        /// Gets the identifier for the AND keyword.
        /// </summary>
        public const string And = "And";

        /// <summary>
        /// Gets the identifier for the OR keyword.
        /// </summary>
        public const string Or = "Or";

        /// <summary>
        /// Gets the identifier for the DELETE keyword.
        /// </summary>
        public const string Delete = "Delete";

        /// <summary>
        /// Gets the identifier for the ALL keyword.
        /// </summary>
        public const string All = "All";

        /// <summary>
        /// Gets the identifier for the ANY keyword.
        /// </summary>
        public const string Any = "Any";

        /// <summary>
        /// Gets the identifier for the SOME keyword.
        /// </summary>
        public const string Some = "Some";

        /// <summary>
        /// Gets the identifier for the DISTINCT keyword.
        /// </summary>
        public const string Distinct = "Distinct";

        /// <summary>
        /// Gets the identifier for the dot separator.
        /// </summary>
        public const string Dot = "Dot";

        /// <summary>
        /// Gets the identifier for the FROM keyword.
        /// </summary>
        public const string From = "From";

        /// <summary>
        /// Gets the identifier for the GROUP BY keyword.
        /// </summary>
        public const string GroupBy = "GroupBy";

        /// <summary>
        /// Gets the identifier for the HAVING keyword.
        /// </summary>
        public const string Having = "Having";

        /// <summary>
        /// Gets the identifier for identifiers.
        /// </summary>
        public const string Identifier = "Identifier";

        /// <summary>
        /// Gets the identifier for the IN keyword.
        /// </summary>
        public const string In = "In";    
        /// <summary>
        /// Gets the identifier for the INSERT keyword.
        /// </summary>
        public const string Insert = "Insert";

        /// <summary>
        /// Gets the indentifier for the INTO keyword.
        /// </summary>
        public const string Into = "Into";

        /// <summary>
        /// Gets the identifier for the IS keyword.
        /// </summary>
        public const string Is = "Is";

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

        /// <summary>
        /// Gets the identifier for the CROSS JOIN keyword.
        /// </summary>
        public const string CrossJoin = "CrossJoin";

        /// <summary>
        /// Gets the identifier for a left parenthesis.
        /// </summary>
        public const string LeftParenthesis = "LeftParenthesis";

        /// <summary>
        /// Gets the identifier for the LIKE keyword.
        /// </summary>
        public const string Like = "Like";

        /// <summary>
        /// Gets the identifier for the NOT keyword.
        /// </summary>
        public const string Not = "Not";

        /// <summary>
        /// Gets the identifier for the NULL keyword.
        /// </summary>
        public const string Null = "Null";

        /// <summary>
        /// Gets the identifier for the NULLS FIRST keyword.
        /// </summary>
        public const string NullsFirst = "NullsFirst";

        /// <summary>
        /// Gets the identifier for the NULLS LAST keyword.
        /// </summary>
        public const string NullsLast = "NullsLast";

        /// <summary>
        /// Gets the idenifier for numeric literals.
        /// </summary>
        public const string Number = "Number";

        /// <summary>
        /// Gets the identifier for the ON keyword.
        /// </summary>
        public const string On = "On";

        /// <summary>
        /// Gets the identifier for the ORDER BY keyword.
        /// </summary>
        public const string OrderBy = "OrderBy";

        /// <summary>
        /// Gets the identifier for the DESC keyword.
        /// </summary>
        public const string Descending = "Descending";

        /// <summary>
        /// Gets the identifier for the ASC keyword.
        /// </summary>
        public const string Ascending = "Ascending";

        /// <summary>
        /// Gets the identifier for the PERCENT keyword.
        /// </summary>
        public const string Percent = "Percent";

        /// <summary>
        /// Gets the identifier for the right parenthesis.
        /// </summary>
        public const string RightParenthesis = "RightParenthesis";

        /// <summary>
        /// Gets the identifier for the SELECT keyword.
        /// </summary>
        public const string Select = "Select";

        /// <summary>
        /// Gets the identifier for the UNION keyword.
        /// </summary>
        public const string Union = "Union";

        /// <summary>
        /// Gets the identfiier for the INTERSECT keyword.
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

        /// <summary>
        /// Gets the identifier for the SET keyword.
        /// </summary>
        public const string Set = "Set";

        /// <summary>
        /// Gets the identifier for a string literal.
        /// </summary>
        public const string String = "String";

        /// <summary>
        /// Gets the identifier for the TOP keyword.
        /// </summary>
        public const string Top = "Top";

        /// <summary>
        /// Gets the identifier for the UPDATE keyword.
        /// </summary>
        public const string Update = "Update";

        /// <summary>
        /// Gets the identifier for the VALUES keyword.
        /// </summary>
        public const string Values = "Values";

        /// <summary>
        /// Gets the identifier for the WHERE keyword.
        /// </summary>
        public const string Where = "Where";

        /// <summary>
        /// Gets the identifier for the WITH TIES keyword.
        /// </summary>
        public const string WithTies = "WithTies";

        /// <summary>
        /// Gets the identifier for the equality operator.
        /// </summary>
        public const string EqualTo = "EqualTo";

        /// <summary>
        /// Gets the identifier for the inequality operator.
        /// </summary>
        public const string NotEqualTo = "NotEqualTo";

        /// <summary>
        /// Gets the identifier for the less than or equal to operator.
        /// </summary>
        public const string LessThanEqualTo = "LessThanEqualTo";

        /// <summary>
        /// Gets the identifier for the greater than or equal to operator.
        /// </summary>
        public const string GreaterThanEqualTo = "GreaterThanEqualTo";

        /// <summary>
        /// Gets the identifier for the less than operator.
        /// </summary>
        public const string LessThan = "less_than";

        /// <summary>
        /// Gets the identifier for the greater than operator.
        /// </summary>
        public const string GreaterThan = "greater_than";

        /// <summary>
        /// Gets the identifier for the EXISTS keyword.
        /// </summary>
        public const string Exists = "Exists";

        /// <summary>
        /// Gets the identifier for the OVER keyword.
        /// </summary>
        public const string Over = "Over";

        /// <summary>
        /// Gets the identifier for the PARTITION BY keyword.
        /// </summary>
        public const string PartitionBy = "PartitionBy";

        /// <summary>
        /// Gets the identifier for the ROWS keyword.
        /// </summary>
        public const string Rows = "Rows";

        /// <summary>
        /// Gets the identifier for the RANGE keyword.
        /// </summary>
        public const string Range = "Range";

        /// <summary>
        /// Gets the identifier for the UNBOUNDED keyword.
        /// </summary>
        public const string Unbounded = "Unbounded";

        /// <summary>
        /// Gets the identifier for the PRECEECING keyword.
        /// </summary>
        public const string Preceding = "Preceding";

        /// <summary>
        /// Gets the identifier for the FOLLOWING keyword.
        /// </summary>
        public const string Following = "Following";

        /// <summary>
        /// Gets the identifier for the CURRENT ROW keyword.
        /// </summary>
        public const string CurrentRow = "CurrentRow";

        /// <summary>
        /// Gets the identifier for the CASE keyword.
        /// </summary>
        public const string Case = "Case";

        /// <summary>
        /// Gets the identifier for the WHEN keyword.
        /// </summary>
        public const string When = "When";

        /// <summary>
        /// Gets the identfier for the THEN keyword.
        /// </summary>
        public const string Then = "Then";

        /// <summary>
        /// Gets the identifier for the ELSE keyword.
        /// </summary>
        public const string Else = "Else";

        /// <summary>
        /// Gets the identifier for the END keyword.
        /// </summary>
        public const string End = "End";

        /// <summary>
        /// Gets the identifier for the Line Terminator which can be used to seperate SQL statements in a batch.
        /// </summary>
        public const string LineTerminator = "LineTerminator";

        /// <summary>
        /// Gets the identifier for the Output keyword.
        /// </summary>
        public const string Output = "Output";

        /// <summary>
        /// Gets the identifier for the Offset_Fetch keyword.
        /// sql2012+分页
        /// </summary>
        public const string OffsetFetch = "OffsetFetch";
        /// <summary>
        /// Gets the identifier for the Limit keyword.
        /// </summary>
        public const string Limit = "Limit";


        #region DDL

        /// <summary>
        /// Gets the identifier for the CREATE keyword.
        /// </summary>
        public const string Create = "Create";

        /// <summary>
        /// Gets the identifier for the Database keyword.
        /// </summary>
        public const string Database = "Database";

        /// <summary>
        /// Gets the identifier for the Table keyword.
        /// </summary>
        public const string Table = "Table";

        /// <summary>
        /// Gets the identifier for the Primary keyword.
        /// </summary>
        public const string Primary = "Primary";

        /// <summary>
        /// Gets the identifier for the Key keyword.
        /// </summary>
        public const string Key = "Key";

        /// <summary>
        /// Gets the identifier for the Collate keyword.
        /// </summary>
        public const string Collate = "Collate";

        /// <summary>
        /// Gets the identifier for the Constraint keyword.
        /// </summary>
        public const string Constraint = "Constraint";

        /// <summary>
        /// Gets the identifier for the Constraint keyword.
        /// </summary>
        public const string Identity = "Identity";

        /// <summary>
        /// Gets the identifier for the Default keyword.
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// Gets the identifier for the RowGuidCol keyword.
        /// </summary>
        public const string RowGuidCol = "RowGuidCol";

        /// <summary>
        /// Gets the identifier for the Unique keyword.
        /// </summary>
        public const string Unique = "Unique";

        /// <summary>
        /// Gets the identifier for the Clustered keyword.
        /// </summary>
        public const string Clustered = "Clustered";

        /// <summary>
        /// Gets the identifier for the NonClustered keyword.
        /// </summary>
        public const string NonClustered = "NonClustered";

        /// <summary>
        /// Gets the identifier for the Foreign keyword.
        /// </summary>
        public const string Foreign = "Foreign";

        /// <summary>
        /// Gets the identifier for the References keyword.
        /// </summary>
        public const string References = "References";

        /// <summary>
        /// Gets the identifier for the No keyword.
        /// </summary>
        public const string No = "No";

        /// <summary>
        /// Gets the identifier for the Action keyword.
        /// </summary>
        public const string Action = "Action";

        /// <summary>
        /// Gets the identifier for the Cascade keyword.
        /// </summary>
        public const string Cascade = "Cascade";

        /// <summary>
        /// Gets the identifier for the For keyword.
        /// </summary>
        public const string For = "For";

        /// <summary>
        /// Gets the identifier for the Replication keyword.
        /// </summary>
        public const string Replication = "Replication";

        /// <summary>
        /// Gets the identifier for the Check keyword.
        /// </summary>
        public const string Check = "Check";

        /// <summary>
        /// Gets the identifier for the Alter keyword.
        /// </summary>
        public const string Alter = "Alter";

        /// <summary>
        /// Gets the identifier for the Modify keyword.
        /// </summary>
        public const string ModifyName = "ModifyName";

        /// <summary>
        /// Gets the identifier for the Current keyword.
        /// </summary>
        public const string Current = "Current";

        /// <summary>
        /// Gets the identifier for the Column keyword.
        /// </summary>
        public const string Column = "Column";

        /// <summary>
        /// Gets the identifier for the Add keyword.
        /// </summary>
        public const string Add = "Add";

        /// <summary>
        /// Gets the identifier for the Drop keyword.
        /// </summary>
        public const string Drop = "Drop";

        /// <summary>
        /// Gets the identifier for the Persisted keyword.
        /// </summary>
        public const string Persisted = "Persisted";

        /// <summary>
        /// Gets the identifier for the Sparse keyword.
        /// </summary>
        public const string Sparse = "Sparse";



        #endregion

        /// <summary>
        /// Initializes a new instance of a SqlTokenizer.
        /// </summary>
        public SqlTokenRegistry()
        {
            Define(Top, @"TOP\b", true);
            Define(Update, @"UPDATE\b", true);
            Define(Values, @"VALUES\b", true);
            Define(Where, @"WHERE\b", true);
            Define(WithTies, @"WITH\s+TIES\b", true);
            Define(Between, @"BETWEEN\b", true);
            Define(And, @"AND\b", true);
            Define(Or, @"OR\b", true);
            Define(Delete, @"DELETE\b", true);
            Define(All, @"ALL\b", true);
            Define(Any, @"ANY\b", true);
            Define(Some, @"SOME\b", true);
            Define(Distinct, @"DISTINCT\b", true);
            Define(From, @"FROM\b", true);
            Define(GroupBy, @"GROUP\s+BY\b", true);
            Define(Having, @"HAVING\b", true);
            Define(Insert, @"INSERT\b", true);
            Define(Into, @"INTO\b", true);
            Define(Is, @"IS\b", true);
            Define(FullOuterJoin, @"FULL\s+(OUTER\s+)?JOIN\b", true);
            Define(InnerJoin, @"(INNER\s+)?JOIN\b", true);
            Define(LeftOuterJoin, @"LEFT\s+(OUTER\s+)?JOIN\b", true);
            Define(RightOuterJoin, @"RIGHT\s+(OUTER\s+)?JOIN\b", true);
            Define(CrossJoin, @"CROSS\s+JOIN\b", true);
            Define(In, @"IN\b", true);
            Define(Like, @"LIKE\b", true);
            Define(Not, @"NOT\b", true);
            Define(NullsFirst, @"NULLS\s+FIRST\b", true);
            Define(NullsLast, @"NULLS\s+LAST\b", true);
            Define(Null, @"NULL\b", true);
            Define(OrderBy, @"ORDER\s+BY\b", true);
            Define(Ascending, @"ASC\b", true);
            Define(Descending, @"DESC\b", true);
            //sql2012以上分页语法
            Define(OffsetFetch, @"OFFSET\s+\d+\s+(ROWS|ROW)\s+FETCH\s+NEXT\s+\d+\s+(ROWS|ROW)\s+ONLY\b", true);
            Define(Limit, @"LIMIT\s+\d\s*,\s*\d\b", true);
            Define(Percent, @"PERCENT\b", true);
            Define(Select, @"SELECT\b", true);
            Define(Union, @"UNION\b", true);
            Define(Intersect, @"INTERSECT\b", true);
            Define(Except, @"EXCEPT\b", true);
            Define(Minus, @"MINUS\b", true);
            Define(Set, @"SET\b", true);
            Define(On, @"ON\b", true);
            Define(As, @"AS\b", true);
            Define(Exists, @"EXISTS\b", true);
            Define(Over, @"OVER\b", true);
            Define(PartitionBy, @"PARTITION\s+BY\b", true);
            Define(Rows, @"ROWS\b", true);
            Define(Range, @"RANGE\b", true);
            Define(Unbounded, @"UNBOUNDED\b", true);
            Define(Preceding, @"PRECEDING\b", true);
            Define(Following, @"FOLLOWING\b", true);
            Define(CurrentRow, @"CURRENT\s+ROW\b", true);
            Define(Case, @"CASE\b", true);
            Define(When, @"WHEN\b", true);
            Define(Then, @"THEN\b", true);
            Define(Else, @"ELSE\b", true);
            Define(End, @"END\b", true);
            Define(Output, @"OUTPUT\b", true);
            // DDL
            Define(Create, @"CREATE\b", true);
            Define(Database, @"DATABASE\b", true);
            Define(Table, @"TABLE\b", true);
            Define(Primary, @"PRIMARY\b", true);
            Define(Key, @"KEY\b", true);
            Define(Collate, @"COLLATE\b", true);
            Define(Constraint, @"CONSTRAINT\b", true);
            Define(Identity, @"IDENTITY\b", true);
            Define(Default, @"DEFAULT\b", true);
            Define(RowGuidCol, @"ROWGUIDCOL\b", true);
            Define(Unique, @"UNIQUE\b", true);
            Define(Clustered, @"CLUSTERED\b", true);
            Define(NonClustered, @"NONCLUSTERED\b", true);
            Define(Foreign, @"FOREIGN\b", true);
            Define(References, @"REFERENCES\b", true);
            Define(No, @"NO\b", true);
            Define(Action, @"ACTION\b", true);
            Define(Cascade, @"CASCADE\b", true);
            Define(For, @"FOR\b", true);
            Define(Replication, @"REPLICATION\b", true);
            Define(Check, @"CHECK\b", true);
            Define(Alter, @"ALTER\b", true);
            Define(ModifyName, @"MODIFY\s+NAME\b", true);
            Define(Current, @"CURRENT\b", true);
            Define(Column, @"COLUMN\b", true);
            Define(Add, @"ADD\b", true);
            Define(Drop, @"DROP\b", true);
            Define(Persisted, @"PERSISTED\b", true);
            Define(Sparse, @"SPARSE\b", true);

            Define(Identifier, @"([\p{L}:?@#_][\p{L}\p{N}@#$_]*)|(""(\.|"""")+"")|(\[[^\]]+\])");

            Define(PlusOperator, @"\+");
            Define(MinusOperator, @"-");
            Define(MultiplicationOperator, @"\*");
            Define(DivisionOperator, @"/");
            Define(ModulusOperator, @"%");
            Define(Comma, @",");
            Define(EqualTo, @"=");
            Define(NotEqualTo, @"<>");
            Define(LessThanEqualTo, @"<=");
            Define(GreaterThanEqualTo, @">=");
            Define(LessThan, @"<");
            Define(GreaterThan, @">");
            Define(Dot, @"\.");
            Define(LeftParenthesis, @"\(");
            Define(Number, @"[-+]?\d*\.?\d+([eE][-+]?\d+)?");
            Define(RightParenthesis, @"\)");
            Define(String, @"'([^']|'')*'");
            Define(LineTerminator, @"('(?:\\.|''|[^'])*(')?)|(;)");

        }
    }
}
