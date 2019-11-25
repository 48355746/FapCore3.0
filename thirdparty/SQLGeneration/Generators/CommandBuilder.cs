using System;
using System.Collections.Generic;
using System.Linq;
using SQLGeneration.Builders;
using SQLGeneration.Parsing;


namespace SQLGeneration.Generators
{
    /// <summary>
    /// Builds an ICommand from a SQL statement.
    /// </summary>
    public sealed class CommandBuilder : SqlGenerator
    {
        private SourceScope scope;
        private CommandBuilderOptions options;

        /// <summary>
        /// Initializes a new instance of a SimpleFormatter.
        /// </summary>
        /// <param name="registry">The token registry to use.</param>
        public CommandBuilder(SqlTokenRegistry registry)
            : this(new SqlGrammar(registry))
        {
        }

        /// <summary>
        /// Initializes a new instance of a SimpleFormatter.
        /// </summary>
        /// <param name="grammar">The grammar to use.</param>
        public CommandBuilder(SqlGrammar grammar = null)
            : base(grammar)
        {
        }

        /// <summary>
        /// Parses the given command text to build a command builder.
        /// </summary>
        /// <param name="commandText">The command text to parse.</param>
        /// <param name="options">Configures the behavior of the command builder.</param>
        /// <returns>The command that was parsed.</returns>
        public ICommand GetCommand(string commandText, CommandBuilderOptions options = null)
        {
            this.scope = new SourceScope();
            this.options = options ?? new CommandBuilderOptions();
            ITokenSource tokenSource = Grammar.TokenRegistry.CreateTokenSource(commandText);

            var batch = new BatchBuilder();
            int statementCount = 0;
            while (true)
            {
                MatchResult result = GetResult(tokenSource);
                if (result != null && result.IsMatch)
                {
                    statementCount++;
                    var command = BuildStart(result);
                    batch.AddCommand(command);
                }
                else
                {
                    break;
                }
            }

            if (statementCount == 0)
            {
                throw new SQLGenerationException($"sql 不标准或有错误。{commandText}");// InvalidOperationException();
            }
            else if (statementCount == 1)
            {
                return batch.GetCommand(0);
            }
            else
            {
                return batch;
            }
        }

        private void BuildTerminator(MatchResult result, ICommand builder)
        {
            MatchResult terminator = result.Matches[SqlGrammar.Start.Terminator];
            builder.HasTerminator = terminator.IsMatch;
            return;
        }

        private ICommand BuildStart(MatchResult result)
        {
            MatchResult select = result.Matches[SqlGrammar.Start.SelectStatement];
            if (select.IsMatch)
            {
                ICommand command = BuildSelectStatement(select);
                BuildTerminator(result, command);
                return command;
            }
            MatchResult insert = result.Matches[SqlGrammar.Start.InsertStatement];
            if (insert.IsMatch)
            {
                ICommand command = BuildInsertStatement(insert);
                BuildTerminator(result, command);
                return command;
            }
            MatchResult update = result.Matches[SqlGrammar.Start.UpdateStatement];
            if (update.IsMatch)
            {
                ICommand command = buildUpdateStatement(update);
                BuildTerminator(result, command);
                return command;
            }
            MatchResult delete = result.Matches[SqlGrammar.Start.DeleteStatement];
            if (delete.IsMatch)
            {
                ICommand command = buildDeleteStatement(delete);
                BuildTerminator(result, command);
                return command;
            }
            MatchResult create = result.Matches[SqlGrammar.Start.CreateStatement];
            if (create.IsMatch)
            {
                ICommand command = BuildCreateStatement(create);
                BuildTerminator(result, command);
                return command;
            }
            MatchResult alter = result.Matches[SqlGrammar.Start.AlterStatement];
            if (alter.IsMatch)
            {
                ICommand command = BuildAlterStatement(alter);
                BuildTerminator(result, command);
                return command;
            }

            throw new InvalidOperationException();
        }

        #region DDL

        #region Create

        private ICommand BuildCreateStatement(MatchResult result)
        {
            var createBuilder = new CreateBuilder();
            var createdbExpression = result.Matches[SqlGrammar.CreateStatement.CreateDatabaseExpressionName];
            if (createdbExpression.IsMatch)
            {
                CreateDatabase db = BuildCreateDatabase(createdbExpression);
                createBuilder.CreateObject = db;
                return createBuilder;
            }

            MatchResult createTableResult = result.Matches[SqlGrammar.CreateStatement.CreateTableExpressionName];
            if (createTableResult.IsMatch)
            {
                CreateTableDefinition tableDef = BuildCreateTableDefinition(createTableResult);
                createBuilder.CreateObject = tableDef;
                return createBuilder;
            }

            return createBuilder;
        }

        private CreateDatabase BuildCreateDatabase(MatchResult result)
        {
            CreateDatabase db = null;

            MatchResult databaseNameResult = result.Matches[SqlGrammar.CreateDatabaseStatement.DatabaseName];
            if (databaseNameResult.IsMatch)
            {
                var databaseName = GetToken(databaseNameResult);
                db = new CreateDatabase(databaseName);
                MatchResult collateResult = result.Matches[SqlGrammar.CreateDatabaseStatement.Collate.Name];
                if (collateResult.IsMatch)
                {
                    MatchResult collationNameResult = collateResult.Matches[SqlGrammar.CreateDatabaseStatement.Collate.Collation];
                    if (collationNameResult.IsMatch)
                    {
                        var collation = GetToken(collationNameResult);
                        db.Collation = collation;
                    }
                }

            }
            return db;
        }

        private CreateTableDefinition BuildCreateTableDefinition(MatchResult result)
        {
            CreateTableDefinition tableDef = null;

            MatchResult tableNameResult = result.Matches[SqlGrammar.CreateTableStatement.TableName];
            if (tableNameResult.IsMatch)
            {
                List<string> parts = new List<string>();
                BuildMultipartIdentifier(tableNameResult, parts);


                if (parts.Count > 1)
                {
                    Namespace qualifier = GetNamespace(parts.Take(parts.Count - 1));
                    string tableName = parts[parts.Count - 1];
                    // AliasedSource source = scope.GetSource(tableName);               
                    tableDef = new CreateTableDefinition(qualifier, tableName);
                }
                else
                {
                    string name = parts[0];
                    tableDef = new CreateTableDefinition(name);
                }

                // build the table definition
                MatchResult tableDefinitionResult = result.Matches[SqlGrammar.CreateTableStatement.TableDefinition.Name];
                if (tableDefinitionResult != null && tableDefinitionResult.IsMatch)
                {
                    MatchResult columnsDefinitionResult = tableDefinitionResult.Matches[SqlGrammar.CreateTableStatement.TableDefinition.ColumnsDefinitionList];
                    if (columnsDefinitionResult.IsMatch)
                    {
                        BuildColumnDefinitionsList(tableDef.Columns, columnsDefinitionResult);
                    }

                }
            }
            return tableDef;
        }

        #endregion

        #region Alter

        private ICommand BuildAlterStatement(MatchResult result)
        {
            var alterBuilder = new AlterBuilder();
            var alterdbExpression = result.Matches[SqlGrammar.AlterStatement.AlterDatabaseExpressionName];
            if (alterdbExpression.IsMatch)
            {
                AlterDatabase db = BuildAlterDatabase(alterdbExpression);
                alterBuilder.AlterObject = db;
                return alterBuilder;
            }

            var alterTableExpression = result.Matches[SqlGrammar.AlterStatement.AlterTableExpressionName];
            if (alterTableExpression.IsMatch)
            {
                AlterTableDefinition db = BuildAlterTable(alterTableExpression);
                alterBuilder.AlterObject = db;
                return alterBuilder;
            }

            return alterBuilder;
        }

        private AlterTableDefinition BuildAlterTable(MatchResult result)
        {
            AlterTableDefinition alterTable = null;
            MatchResult tableNameResult = result.Matches[SqlGrammar.AlterTableStatement.TableName];
            if (tableNameResult.IsMatch)
            {
                List<string> parts = new List<string>();
                BuildMultipartIdentifier(tableNameResult, parts);

                if (parts.Count > 1)
                {
                    Namespace qualifier = GetNamespace(parts.Take(parts.Count - 1));
                    string tableName = parts[parts.Count - 1];
                    // AliasedSource source = scope.GetSource(tableName);               
                    alterTable = new AlterTableDefinition(qualifier, tableName);
                }
                else
                {
                    string name = parts[0];
                    alterTable = new AlterTableDefinition(name);
                }

                // build the alter column definition
                MatchResult alterColumnResult = result.Matches[SqlGrammar.AlterTableStatement.AlterColumn.Name];
                if (alterColumnResult.IsMatch)
                {
                    ITableAlteration alterColumn = BuildAlterColumn(alterColumnResult);
                    alterTable.Alteration = alterColumn;
                }

                MatchResult addColumnsResult = result.Matches[SqlGrammar.AlterTableStatement.AddColumns.Name];
                if (addColumnsResult.IsMatch)
                {

                    ITableAlteration alterColumn = BuildAlterTableAddColumns(addColumnsResult);
                    alterTable.Alteration = alterColumn;
                }

                MatchResult dropTableItemsResult = result.Matches[SqlGrammar.AlterTableStatement.DropColumnsOrConstraints.Name];
                if (dropTableItemsResult.IsMatch)
                {
                    ITableAlteration dropItems = BuildDropTableItems(dropTableItemsResult);
                    alterTable.Alteration = dropItems;
                }

            }
            return alterTable;
            //  throw new NotImplementedException();
        }

        private ITableAlteration BuildDropTableItems(MatchResult result)
        {
            var dropItems = new DropItemsList();
            var dropListItemsResult = result.Matches[SqlGrammar.AlterTableStatement.DropColumnsOrConstraints.DropListExpressionName];

            //   MatchResult columnsDefinitionResult = result.Matches[SqlGrammar.CreateTableStatement.TableDefinition.ColumnsDefinitionList];
            if (dropListItemsResult.IsMatch)
            {
                buildDropItemsList(dropItems.Items, dropListItemsResult);
            }
            return dropItems;
            // throw new NotImplementedException();
        }

        private void buildDropItemsList(List<IDropTableItem> list, MatchResult result)
        {
            // First add column defintions
            MatchResult multiple = result.Matches[SqlGrammar.DropTableItemsList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.DropTableItemsList.Multiple.First];
                var dropTableItem = BuildDropItem(first);
                list.Add(dropTableItem);
                // ColumnDefinition column = buildColumnDefinition(first);
                //  columns.AddColumnDefinition(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.DropTableItemsList.Multiple.Remaining];
                buildDropItemsList(list, remaining);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.DropTableItemsList.Single];
            if (single.IsMatch)
            {
                var dropTableItem = BuildDropItem(single);
                list.Add(dropTableItem);
                //ColumnDefinition column = buildColumnDefinition(single);
                // columns.AddColumnDefinition(column);
                return;
            }
        }

        private IDropTableItem BuildDropItem(MatchResult result)
        {
            MatchResult dropConstraintExpression = result.Matches[SqlGrammar.DropTableItem.DropConstraintExpressionName];
            if (dropConstraintExpression.IsMatch)
            {
                var dropConstraintsListResult = dropConstraintExpression.Matches[SqlGrammar.DropTableItem.DropConstraintListExpressionName];
                if (dropConstraintsListResult.IsMatch)
                {
                    var constraints = new DropConstraintsList();
                    BuildDropTableConstraintsList(constraints, dropConstraintsListResult);
                    return constraints;
                }
            }

            MatchResult dropColumnExpression = result.Matches[SqlGrammar.DropTableItem.DropColumnExpressionName];
            if (dropColumnExpression.IsMatch)
            {

                var dropColumnsListResult = dropColumnExpression.Matches[SqlGrammar.DropTableItem.DropColumnListExpressionName];
                if (dropColumnsListResult.IsMatch)
                {
                    var cols = new DropColumnsList();
                    BuildDropTableColumnsList(cols, dropColumnsListResult);
                    return cols;
                }
            }

            return null;

        }

        private void BuildDropTableConstraintsList(DropConstraintsList list, MatchResult result)
        {
            // First add column defintions
            MatchResult multiple = result.Matches[SqlGrammar.DropTableConstraintList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.DropTableConstraintList.Multiple.First];
                DropConstraint item = BuildDropConstraint(first);
                list.Items.Add(item);
                // ColumnDefinition column = buildColumnDefinition(first);
                //  columns.AddColumnDefinition(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.DropTableConstraintList.Multiple.Remaining];
                BuildDropTableConstraintsList(list, remaining);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.DropTableConstraintList.Single];
            if (single.IsMatch)
            {
                DropConstraint item = BuildDropConstraint(single);
                list.Items.Add(item);
                //ColumnDefinition column = buildColumnDefinition(single);
                // columns.AddColumnDefinition(column);
                return;
            }
        }

        private DropConstraint BuildDropConstraint(MatchResult result)
        {
            var nameResult = result.Matches[SqlGrammar.DropTableConstraint.ConstraintName];
            var name = GetToken(nameResult);
            var dropConstraint = new DropConstraint(name);
            return dropConstraint;
        }

        private void BuildDropTableColumnsList(DropColumnsList list, MatchResult result)
        {
            // First add column defintions
            MatchResult multiple = result.Matches[SqlGrammar.DropTableColumnList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.DropTableColumnList.Multiple.First];
                DropColumn item = BuildDropColumn(first);
                list.Items.Add(item);
                // ColumnDefinition column = buildColumnDefinition(first);
                //  columns.AddColumnDefinition(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.DropTableColumnList.Multiple.Remaining];
                BuildDropTableColumnsList(list, remaining);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.DropTableColumnList.Single];
            if (single.IsMatch)
            {
                DropColumn item = BuildDropColumn(single);
                list.Items.Add(item);
                //ColumnDefinition column = buildColumnDefinition(single);
                // columns.AddColumnDefinition(column);
                return;
            }
        }

        private DropColumn BuildDropColumn(MatchResult result)
        {
            var colNameResult = result.Matches[SqlGrammar.DropTableColumn.ColumnName];
            var columnName = GetToken(colNameResult);
            var dropCol = new DropColumn(columnName);
            return dropCol;
        }

        private ITableAlteration BuildAlterTableAddColumns(MatchResult result)
        {
            var addColumns = new AddColumns();
            var addColumnsDefinitionListResult = result.Matches[SqlGrammar.AlterTableStatement.AddColumns.ColumnDefinitionListExpressionName];

            //   MatchResult columnsDefinitionResult = result.Matches[SqlGrammar.CreateTableStatement.TableDefinition.ColumnsDefinitionList];
            if (addColumnsDefinitionListResult.IsMatch)
            {
                BuildColumnDefinitionsList(addColumns.Columns, addColumnsDefinitionListResult);
            }
            return addColumns;
        }

        private ITableAlteration BuildAlterColumn(MatchResult result)
        {
            //  AlterColumn ac = null;
            string columnName = null;

            MatchResult columNameResult = result.Matches[SqlGrammar.AlterTableStatement.AlterColumn.ColumnName];
            if (columNameResult.IsMatch)
            {
                columnName = GetToken(columNameResult);
            }

            var columnDefinitionResult = result.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AlterColumnDataTypeExpressionName];
            if (columnDefinitionResult.IsMatch)
            {
                var ac = new AlterColumn(columnName);

                var colDataTypeResult = columnDefinitionResult.Matches[SqlGrammar.DataType.Name];
                var dataType = BuildDataType(colDataTypeResult);
                ac.DataType = dataType;

                var collationResult = columnDefinitionResult.Matches[SqlGrammar.Collate.Name];
                if (collationResult.IsMatch)
                {
                    var collation = BuildCollation(collationResult);
                    ac.Collation = collation;
                }

                var nullabilityResult = columnDefinitionResult.Matches[SqlGrammar.Nullability.Name];
                if (nullabilityResult.IsMatch)
                {
                    var isNullable = BuildIsNullable(nullabilityResult);
                    ac.IsNullable = isNullable;
                }

                return ac;
            }

            var addOrDropPropertyResult = result.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.Name];
            if (addOrDropPropertyResult.IsMatch)
            {

                AlterAction alterType;
                var addResult = addOrDropPropertyResult.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.AddKeyword];
                if (addResult.IsMatch)
                {
                    alterType = AlterAction.Add;
                }
                else
                {
                    alterType = AlterAction.Drop;
                }

                var ac = new AlterColumnProperty(alterType, columnName);

                IColumnProperty prop = null;
                var notForReplicationResult = addOrDropPropertyResult.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.NotForReplicationExpressionName];
                if (notForReplicationResult.IsMatch)
                {
                    prop = new NotForReplicationColumnProperty();
                    ac.Property = prop;
                    return ac;
                }

                var persistedResult = addOrDropPropertyResult.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.PersistedKeyword];
                if (persistedResult.IsMatch)
                {
                    prop = new PersistedColumnProperty();
                    ac.Property = prop;
                    return ac;
                }

                var rowGuidResult = addOrDropPropertyResult.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.RowGuidColKeyword];
                if (rowGuidResult.IsMatch)
                {
                    prop = new RowGuidColumnProperty();
                    ac.Property = prop;
                    return ac;
                }

                var sparseResult = addOrDropPropertyResult.Matches[SqlGrammar.AlterTableStatement.AlterColumn.AddOrDropColumnProperty.SparseKeyword];
                if (sparseResult.IsMatch)
                {
                    prop = new SparseColumnProperty();
                    ac.Property = prop;
                    return ac;
                }

            }

            throw new InvalidOperationException();

        }

        private AlterDatabase BuildAlterDatabase(MatchResult result)
        {
            AlterDatabase db = null;

            MatchResult databaseNameResult = result.Matches[SqlGrammar.AlterDatabaseStatement.DatabaseName];
            if (databaseNameResult.IsMatch)
            {
                var name = GetToken(databaseNameResult);
                db = new AlterDatabase(name);
            }
            else
            {
                MatchResult currentDatabase = result.Matches[SqlGrammar.AlterDatabaseStatement.CurrentKeyword];
                if (currentDatabase.IsMatch)
                {
                    db = new AlterDatabase(true);
                }
                else
                {
                    // A database name, or the CURRENT keyword must be used with an alter database statement.
                    throw new InvalidOperationException();
                }
            }

            // Are we modifying the name?
            MatchResult modifyNameResult = result.Matches[SqlGrammar.AlterDatabaseStatement.ModifyName.Name];
            if (modifyNameResult.IsMatch)
            {
                var newNameResult = modifyNameResult.Matches[SqlGrammar.AlterDatabaseStatement.ModifyName.NewDatabaseName];
                if (newNameResult.IsMatch)
                {
                    var newName = GetToken(newNameResult);
                    db.NewDatabaseName = newName;
                    return db;
                }
            }

            // Are we modifying the collation?
            MatchResult collateResult = result.Matches[SqlGrammar.Collate.Name];
            if (collateResult.IsMatch)
            {
                var collation = BuildCollation(collateResult);
                db.NewCollation = collation;
            }

            return db;
        }

        #endregion

        private ColumnDefinition BuildColumnDefinition(MatchResult result)
        {
            var columnNameResult = result.Matches[SqlGrammar.ColumnDefinition.ColumnName];
            var columnName = GetToken(columnNameResult);
            var columnDefinition = new ColumnDefinition(columnName);

            var collationResult = result.Matches[SqlGrammar.Collate.Name];
            if (collationResult.IsMatch)
            {
                string collation = BuildCollation(collationResult);
                columnDefinition.Collation = collation;
            }

            var dataTypeResult = result.Matches[SqlGrammar.DataType.Name];
            if (dataTypeResult.IsMatch)
            {
                DataType dataType = BuildDataType(dataTypeResult);
                columnDefinition.DataType = dataType;
            }

            var isNullableResult = result.Matches[SqlGrammar.Nullability.Name];
            if (isNullableResult.IsMatch)
            {
                bool isNullable = BuildIsNullable(isNullableResult);
                columnDefinition.IsNullable = isNullable;
            }

            var defaultConstraintResult = result.Matches[SqlGrammar.ColumnDefinition.Default.Name];
            if (defaultConstraintResult.IsMatch)
            {
                columnDefinition.Default = new DefaultConstraint();

                var constraintResult = defaultConstraintResult.Matches[SqlGrammar.ColumnDefinition.Constraint.Name];
                if (constraintResult.IsMatch)
                {
                    var constraintNameResult = constraintResult.Matches[SqlGrammar.ColumnDefinition.Constraint.ConstraintName];
                    if (constraintNameResult.IsMatch)
                    {
                        columnDefinition.Default.ConstraintName = GetToken(constraintNameResult);
                    }
                }

                var defaultValueResult = defaultConstraintResult.Matches[SqlGrammar.ColumnDefinition.Default.DefaultName];
                if (defaultValueResult.IsMatch)
                {

                    var defaultStringLiteralResult = defaultValueResult.Matches[SqlGrammar.ColumnDefinition.Default.DefaultStringLiteral];
                    if (defaultStringLiteralResult.IsMatch)
                    {
                        columnDefinition.Default.Value = BuildStringLiteral(defaultStringLiteralResult);
                    }
                    else
                    {
                        var defaultNumericLiteralResult = defaultValueResult.Matches[SqlGrammar.ColumnDefinition.Default.DefaultNumericLiteral];
                        if (defaultNumericLiteralResult.IsMatch)
                        {
                            columnDefinition.Default.Value = BuildNumericLiteral(defaultNumericLiteralResult);
                        }
                        else
                        {
                            var defaultFunctionResult = defaultValueResult.Matches[SqlGrammar.ColumnDefinition.Default.DefaultFunction];
                            if (defaultFunctionResult.IsMatch)
                            {
                                columnDefinition.Default.Function = BuildFunctionCall(defaultFunctionResult);
                            }
                        }
                    }

                }
            }

            var identityResult = result.Matches[SqlGrammar.ColumnDefinition.Identity.Name];
            if (identityResult.IsMatch)
            {
                AutoIncrement autoIncrement = new AutoIncrement();
                MatchResult identitySeedResult = identityResult.Matches[SqlGrammar.ColumnDefinition.Identity.IdentitySeed];
                if (identitySeedResult.IsMatch)
                {

                    MatchResult identitySeedValuesResult = identitySeedResult.Matches[SqlGrammar.ColumnDefinition.Identity.IdentitySeedValues];
                    if (identitySeedValuesResult.IsMatch)
                    {
                        ValueList arguments = new ValueList();
                        BuildValueList(identitySeedValuesResult, arguments);
                        foreach (NumericLiteral value in arguments.Values)
                        {
                            autoIncrement.AddArgument(value);
                        }
                    }
                }

                MatchResult notForReplicationResult = identityResult.Matches[SqlGrammar.ColumnDefinition.Identity.NotForReplicationExpressionName];
                if (notForReplicationResult.IsMatch)
                {
                    autoIncrement.NotForReplication = new NotForReplicationColumnProperty();
                }

                columnDefinition.AutoIncrement = autoIncrement;
            }

            var rowGuidResult = result.Matches[SqlGrammar.ColumnDefinition.RowGuidColKeyword];
            if (rowGuidResult.IsMatch)
            {
                columnDefinition.IsRowGuid = true;
            }

            // Column constraints
            MatchResult columnConstraintsResult = result.Matches[SqlGrammar.ColumnDefinition.ColumnConstraintListExpressionName];
            if (columnConstraintsResult.IsMatch)
            {
                BuildColumnConstraintList(columnDefinition, columnConstraintsResult);
            }

            return columnDefinition;
        }

        private void BuildColumnDefinitionsList(ColumnDefinitionList columns, MatchResult columnsDefinitionResult)
        {
            // First add column defintions
            MatchResult multiple = columnsDefinitionResult.Matches[SqlGrammar.ColumnDefinitionList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ColumnDefinitionList.Multiple.First];
                ColumnDefinition column = BuildColumnDefinition(first);
                columns.AddColumnDefinition(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.ColumnDefinitionList.Multiple.Remaining];
                BuildColumnDefinitionsList(columns, remaining);
                return;
            }
            MatchResult single = columnsDefinitionResult.Matches[SqlGrammar.ColumnDefinitionList.Single];
            if (single.IsMatch)
            {
                ColumnDefinition column = BuildColumnDefinition(single);
                columns.AddColumnDefinition(column);
                return;
            }
        }

        private void BuildColumnConstraintList(ColumnDefinition builder, MatchResult columnConstraintsResult)
        {
            // First add column defintions
            MatchResult multiple = columnConstraintsResult.Matches[SqlGrammar.ColumnConstraintList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ColumnConstraintList.Multiple.First];
                BuildColumnConstraint(builder, first);
                // builder.Columns.AddColumnDefinition(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.ColumnConstraintList.Multiple.Remaining];
                BuildColumnConstraintList(builder, remaining);
                return;
            }
            MatchResult single = columnConstraintsResult.Matches[SqlGrammar.ColumnConstraintList.Single];
            if (single.IsMatch)
            {
                BuildColumnConstraint(builder, single);
                // ColumnDefinition column = buildColumnDefinition(single);
                // builder.Columns.AddColumnDefinition(column);
                return;
            }
        }

        private void BuildColumnConstraint(ColumnDefinition builder, MatchResult result)
        {

            string constraintName = null;
            var constraintResult = result.Matches[SqlGrammar.ColumnDefinition.Constraint.Name];
            if (constraintResult.IsMatch)
            {
                var constraintNameResult = constraintResult.Matches[SqlGrammar.ColumnDefinition.Constraint.ConstraintName];
                if (constraintNameResult.IsMatch)
                {
                    constraintName = GetToken(constraintNameResult);
                }
            }

            //   var constraintResult = result.Matches[SqlGrammar.ColumnConstraint.Name];
            var pkOrUniqueResult = result.Matches[SqlGrammar.ColumnConstraint.PrimarKeyOrUniqueConstraint.Name];
            if (pkOrUniqueResult.IsMatch)
            {
                // which is it..
                var pkResult = pkOrUniqueResult.Matches[SqlGrammar.ColumnConstraint.PrimaryKey.Name];
                if (pkResult.IsMatch)
                {
                    // build pk                    
                    PrimaryKeyConstraint pk = new PrimaryKeyConstraint(constraintName);
                    builder.Constraints.AddConstraint(pk);
                }
                else
                {
                    var uniqueResult = pkOrUniqueResult.Matches[SqlGrammar.ColumnConstraint.Unique.Name];
                    if (uniqueResult.IsMatch)
                    {
                        UniqueConstraint un = new UniqueConstraint(constraintName);
                        builder.Constraints.AddConstraint(un);
                        // build unique
                    }
                }
            }
            else
            {
                // is it a fk?
                var fkResult = result.Matches[SqlGrammar.ColumnConstraint.ForeignKeyExpressionName];
                if (fkResult.IsMatch)
                {
                    // build fk
                    var fk = new ForeignKeyConstraint(constraintName);
                    var referencesResult = fkResult.Matches[SqlGrammar.ForeignKeyConstraint.References.Name];
                    if (referencesResult.IsMatch)
                    {
                        var referencedTableResult = referencesResult.Matches[SqlGrammar.ForeignKeyConstraint.ReferencedTable.Name];
                        if (referencedTableResult.IsMatch)
                        {
                            var table = BuildTable(referencedTableResult);
                            fk.ReferencedTable = table;
                        }

                        var referencedColumnResult = referencesResult.Matches[SqlGrammar.ForeignKeyConstraint.ReferencedColumn.Name];
                        if (referencedColumnResult.IsMatch)
                        {
                            var columnNameResult = referencedColumnResult.Matches[SqlGrammar.ForeignKeyConstraint.ReferencedColumn.ColumnName];
                            if (columnNameResult.IsMatch)
                            {
                                var colName = GetToken(columnNameResult);
                                fk.ReferencedColumn = colName;
                            }
                        }
                    }

                    // On Delete
                    var onDeleteResult = fkResult.Matches[SqlGrammar.ForeignKeyConstraint.On.Delete.Name];
                    if (onDeleteResult.IsMatch)
                    {
                        ForeignKeyAction action = null;
                        action = BuildForeignKeyAction(onDeleteResult);
                        fk.OnDeleteAction = action;
                    }

                    // On Update
                    var onUpdateResult = fkResult.Matches[SqlGrammar.ForeignKeyConstraint.On.Update.Name];
                    if (onUpdateResult.IsMatch)
                    {
                        ForeignKeyAction action = null;
                        action = BuildForeignKeyAction(onUpdateResult);
                        fk.OnUpdateAction = action;
                    }

                    // Not for replication
                    var notForReplicationResult = fkResult.Matches[SqlGrammar.ForeignKeyConstraint.NotForReplicationExpressionName];
                    if (notForReplicationResult.IsMatch)
                    {
                        fk.NotForReplication = new NotForReplicationColumnProperty();
                    }

                    builder.Constraints.AddConstraint(fk);
                }
                else
                {
                    // is it a check constraint?
                    var chkResult = result.Matches[SqlGrammar.ColumnConstraint.Check.Name];
                    if (chkResult.IsMatch)
                    {
                        // buils chk

                    }
                }
            }

            //  throw new NotImplementedException();
        }

        private ForeignKeyAction BuildForeignKeyAction(MatchResult onDeleteResult)
        {
            var noActionResult = onDeleteResult.Matches[SqlGrammar.ForeignKeyConstraint.On.NoAction.Name];
            if (noActionResult.IsMatch)
            {
                return new NoAction();
            }

            var cascadeResult = onDeleteResult.Matches[SqlGrammar.ForeignKeyConstraint.On.CascadeKeyword];
            if (cascadeResult.IsMatch)
            {
                return new CascadeAction();
            }

            var setNullResult = onDeleteResult.Matches[SqlGrammar.ForeignKeyConstraint.On.SetNull.Name];
            if (setNullResult.IsMatch)
            {
                return new SetNullAction();
            }

            var setDefaultResult = onDeleteResult.Matches[SqlGrammar.ForeignKeyConstraint.On.SetDefault.Name];
            if (setDefaultResult.IsMatch)
            {
                return new SetDefaultAction();
            }

            throw new NotSupportedException();
        }

        private bool BuildIsNullable(MatchResult isNullableResult)
        {
            bool isNullable = false;
            var isNotNullResult = isNullableResult.Matches[SqlGrammar.Nullability.NotKeyword];
            if (isNotNullResult.IsMatch)
            {
                isNullable = false;
            }
            else
            {
                isNullable = true;
            }
            return isNullable;
        }

        private string BuildCollation(MatchResult collationResult)
        {
            var collationNameResult = collationResult.Matches[SqlGrammar.Collate.Collation];
            var c = GetToken(collationNameResult);
            return c;
        }

        private DataType BuildDataType(MatchResult dataTypeResult)
        {
            DataType dataType = null;

            var dataTypeNameResult = dataTypeResult.Matches[SqlGrammar.DataType.DataTypeName];
            if (dataTypeNameResult.IsMatch)
            {
                List<string> parts = new List<string>();
                BuildMultipartIdentifier(dataTypeNameResult, parts);

                if (parts.Count > 1)
                {
                    Namespace qualifier = GetNamespace(parts.Take(parts.Count - 1));
                    string dataTypeName = parts[parts.Count - 1];
                    dataType = new DataType(qualifier, dataTypeName);

                }
                else
                {
                    string dataTypeName = parts[0];
                    dataType = new DataType(dataTypeName);
                }

                var columnSizeResult = dataTypeResult.Matches[SqlGrammar.DataType.ColumnSize];
                if (columnSizeResult.IsMatch)
                {
                    MatchResult argumentsResult = columnSizeResult.Matches[SqlGrammar.DataType.ColumnSizeArguments];
                    if (argumentsResult.IsMatch)
                    {
                        ValueList arguments = new ValueList();
                        BuildValueList(argumentsResult, arguments);

                        foreach (var value in arguments.Values)
                        {

                            var lit = value as Literal;
                            if (lit != null)
                            {
                                dataType.AddArgument(lit);
                            }
                            else
                            {

                                var placeHolder = value as Placeholder;
                                if (placeHolder != null)
                                {
                                    if (placeHolder.Value.ToLower() == "max")
                                    {
                                        dataType.HasMax = true;
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return dataType;

        }

        #endregion

        private ISelectBuilder BuildSelectStatement(MatchResult result)
        {
            MatchResult selectExpression = result.Matches[SqlGrammar.SelectStatement.SelectExpression];
            ISelectBuilder builder = BuildSelectExpression(selectExpression);
            MatchResult orderBy = result.Matches[SqlGrammar.SelectStatement.OrderBy.Name];
            if (orderBy.IsMatch)
            {
                scope.Push(builder.Sources);
                MatchResult orderByList = orderBy.Matches[SqlGrammar.SelectStatement.OrderBy.OrderByList];
                BuildOrderByList(orderByList, builder.OrderByList);
                scope.Pop();
            }
            return builder;
        }

        private ISelectBuilder BuildSelectExpression(MatchResult result)
        {
            MatchResult wrapped = result.Matches[SqlGrammar.SelectExpression.Wrapped.Name];
            if (wrapped.IsMatch)
            {
                MatchResult expression = wrapped.Matches[SqlGrammar.SelectExpression.Wrapped.SelectExpression];
                return BuildSelectExpression(expression);
            }
            MatchResult specification = result.Matches[SqlGrammar.SelectExpression.SelectSpecification];
            ISelectBuilder builder = BuildSelectSpecification(specification);
            MatchResult remaining = result.Matches[SqlGrammar.SelectExpression.Remaining.Name];
            if (remaining.IsMatch)
            {
                MatchResult expression = remaining.Matches[SqlGrammar.SelectExpression.Remaining.SelectExpression];
                ISelectBuilder rightHand = BuildSelectExpression(expression);
                MatchResult combinerResult = remaining.Matches[SqlGrammar.SelectExpression.Remaining.Combiner];
                SelectCombiner combiner = BuildSelectCombiner(combinerResult, builder, rightHand);
                MatchResult qualifierResult = remaining.Matches[SqlGrammar.SelectExpression.Remaining.DistinctQualifier];
                if (qualifierResult.IsMatch)
                {
                    combiner.Distinct = BuildDistinctQualifier(qualifierResult);
                }
                builder = combiner;
            }
            return builder;
        }

        private ISelectBuilder BuildSelectSpecification(MatchResult result)
        {
            SelectBuilder builder = new SelectBuilder();
            MatchResult distinctQualifier = result.Matches[SqlGrammar.SelectSpecification.DistinctQualifier];
            if (distinctQualifier.IsMatch)
            {
                builder.Distinct = BuildDistinctQualifier(distinctQualifier);
            }
            MatchResult top = result.Matches[SqlGrammar.SelectSpecification.Top.Name];
            if (top.IsMatch)
            {
                builder.Top = BuildTop(top, builder);
            }
            MatchResult from = result.Matches[SqlGrammar.SelectSpecification.From.Name];
            if (from.IsMatch)
            {
                MatchResult fromList = from.Matches[SqlGrammar.SelectSpecification.From.FromList];
                BuildFromList(fromList, builder);
            }
            scope.Push(builder.Sources);
            MatchResult projectionList = result.Matches[SqlGrammar.SelectSpecification.ProjectionList];
            BuildProjectionList(projectionList, builder);
            MatchResult where = result.Matches[SqlGrammar.SelectSpecification.Where.Name];
            if (where.IsMatch)
            {
                MatchResult filterList = where.Matches[SqlGrammar.SelectSpecification.Where.FilterList];
                IFilter innerFilter = BuildOrFilter(filterList);
                builder.WhereFilterGroup.AddFilter(innerFilter);
                builder.WhereFilterGroup.Optimize();
            }
            MatchResult groupBy = result.Matches[SqlGrammar.SelectSpecification.GroupBy.Name];
            if (groupBy.IsMatch)
            {
                MatchResult groupByList = groupBy.Matches[SqlGrammar.SelectSpecification.GroupBy.GroupByList];
                BuildGroupByList(groupByList, builder);
            }
            MatchResult having = result.Matches[SqlGrammar.SelectSpecification.Having.Name];
            if (having.IsMatch)
            {
                MatchResult filterList = having.Matches[SqlGrammar.SelectSpecification.Having.FilterList];
                IFilter innerFilter = BuildOrFilter(filterList);
                builder.HavingFilterGroup.AddFilter(innerFilter);
                builder.HavingFilterGroup.Optimize();
            }
            scope.Pop();
            return builder;
        }

        private DistinctQualifier BuildDistinctQualifier(MatchResult result)
        {
            DistinctQualifierConverter converter = new DistinctQualifierConverter();
            MatchResult distinct = result.Matches[SqlGrammar.DistinctQualifier.Distinct];
            if (distinct.IsMatch)
            {
                return DistinctQualifier.Distinct;
            }
            MatchResult all = result.Matches[SqlGrammar.DistinctQualifier.All];
            if (all.IsMatch)
            {
                return DistinctQualifier.All;
            }
            throw new InvalidOperationException();
        }

        private Top BuildTop(MatchResult result, SelectBuilder builder)
        {
            MatchResult expressionResult = result.Matches[SqlGrammar.SelectSpecification.Top.Expression];
            IProjectionItem expression = (IProjectionItem)BuildArithmeticItem(expressionResult);
            Top top = new Top(expression);
            MatchResult percentResult = result.Matches[SqlGrammar.SelectSpecification.Top.PercentKeyword];
            top.IsPercent = percentResult.IsMatch;
            MatchResult withTiesResult = result.Matches[SqlGrammar.SelectSpecification.Top.WithTiesKeyword];
            top.WithTies = withTiesResult.IsMatch;
            return top;
        }

        private void BuildFromList(MatchResult result, SelectBuilder builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.FromList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.FromList.Multiple.First];
                Join join = BuildJoin(first, false);
                AddJoinItem(builder, join);
                MatchResult remaining = multiple.Matches[SqlGrammar.FromList.Multiple.Remaining];
                BuildFromList(remaining, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.FromList.Single];
            if (single.IsMatch)
            {
                Join join = BuildJoin(single, false);
                AddJoinItem(builder, join);
                return;
            }
            throw new InvalidOperationException();
        }

        private Join BuildJoin(MatchResult result, bool wrap)
        {
            MatchResult wrapped = result.Matches[SqlGrammar.Join.Wrapped.Name];
            if (wrapped.IsMatch)
            {
                MatchResult joinResult = wrapped.Matches[SqlGrammar.Join.Wrapped.Join];
                Join first = BuildJoin(joinResult, true);
                first.WrapInParentheses = true;
                scope.Push(first.Sources);
                MatchResult joinPrime = wrapped.Matches[SqlGrammar.Join.Wrapped.JoinPrime];
                Join join = BuildJoinPrime(joinPrime, first);
                scope.Pop();
                return join;
            }
            MatchResult joined = result.Matches[SqlGrammar.Join.Joined.Name];
            if (joined.IsMatch)
            {
                string alias;
                MatchResult joinItemResult = joined.Matches[SqlGrammar.Join.Joined.JoinItem];
                IRightJoinItem first = BuildJoinItem(joinItemResult, out alias);
                Join start = Join.From(first, alias);
                scope.Push(start.Sources);
                MatchResult joinPrime = joined.Matches[SqlGrammar.Join.Joined.JoinPrime];
                Join join = BuildJoinPrime(joinPrime, start);
                scope.Pop();
                return join;
            }
            throw new InvalidOperationException();
        }

        private IRightJoinItem BuildJoinItem(MatchResult result, out string alias)
        {
            alias = null;
            MatchResult aliasExpression = result.Matches[SqlGrammar.JoinItem.AliasExpression.Name];
            if (aliasExpression.IsMatch)
            {
                MatchResult aliasResult = aliasExpression.Matches[SqlGrammar.JoinItem.AliasExpression.Alias];
                alias = GetToken(aliasResult);
            }
            MatchResult tableResult = result.Matches[SqlGrammar.JoinItem.Table];
            if (tableResult.IsMatch)
            {
                Table table = BuildTable(tableResult);
                return table;
            }
            MatchResult select = result.Matches[SqlGrammar.JoinItem.Select.Name];
            if (select.IsMatch)
            {
                MatchResult selectStatement = select.Matches[SqlGrammar.JoinItem.Select.SelectStatement];
                return BuildSelectStatement(selectStatement);
            }
            MatchResult functionCall = result.Matches[SqlGrammar.JoinItem.FunctionCall];
            if (functionCall.IsMatch)
            {
                return BuildFunctionCall(functionCall);
            }
            throw new InvalidOperationException();
        }

        private Join BuildJoinPrime(MatchResult result, Join join)
        {
            MatchResult filtered = result.Matches[SqlGrammar.JoinPrime.Filtered.Name];
            if (filtered.IsMatch)
            {
                MatchResult joinItemResult = filtered.Matches[SqlGrammar.JoinPrime.Filtered.JoinItem];
                string alias;
                IRightJoinItem joinItem = BuildJoinItem(joinItemResult, out alias);
                MatchResult joinTypeResult = filtered.Matches[SqlGrammar.JoinPrime.Filtered.JoinType];
                FilteredJoin filteredJoin = BuildFilteredJoin(joinTypeResult, join, joinItem, alias);
                scope.Push(filteredJoin.Sources);
                MatchResult onResult = filtered.Matches[SqlGrammar.JoinPrime.Filtered.On.Name];
                MatchResult filterListResult = onResult.Matches[SqlGrammar.JoinPrime.Filtered.On.FilterList];
                IFilter innerFilter = BuildOrFilter(filterListResult);
                filteredJoin.OnFilterGroup.AddFilter(innerFilter);
                filteredJoin.OnFilterGroup.Optimize();
                MatchResult joinPrimeResult = filtered.Matches[SqlGrammar.JoinPrime.Filtered.JoinPrime];
                Join prime = BuildJoinPrime(joinPrimeResult, filteredJoin);
                scope.Pop();
                return prime;
            }
            MatchResult cross = result.Matches[SqlGrammar.JoinPrime.Cross.Name];
            if (cross.IsMatch)
            {
                MatchResult joinItemResult = cross.Matches[SqlGrammar.JoinPrime.Cross.JoinItem];
                string alias;
                IRightJoinItem joinItem = BuildJoinItem(joinItemResult, out alias);
                Join crossJoin = join.CrossJoin(joinItem, alias);
                scope.Push(crossJoin.Sources);
                MatchResult joinPrimeResult = cross.Matches[SqlGrammar.JoinPrime.Cross.JoinPrime];
                Join prime = BuildJoinPrime(joinPrimeResult, crossJoin);
                scope.Pop();
                return prime;
            }
            MatchResult empty = result.Matches[SqlGrammar.JoinPrime.Empty];
            if (empty.IsMatch)
            {
                return join;
            }
            throw new InvalidOperationException();
        }

        private FilteredJoin BuildFilteredJoin(MatchResult result, Join join, IRightJoinItem joinItem, string alias)
        {
            MatchResult innerResult = result.Matches[SqlGrammar.FilteredJoinType.InnerJoin];
            if (innerResult.IsMatch)
            {
                return join.InnerJoin(joinItem, alias);
            }
            MatchResult leftResult = result.Matches[SqlGrammar.FilteredJoinType.LeftOuterJoin];
            if (leftResult.IsMatch)
            {
                return join.LeftOuterJoin(joinItem, alias);
            }
            MatchResult rightResult = result.Matches[SqlGrammar.FilteredJoinType.RightOuterJoin];
            if (rightResult.IsMatch)
            {
                return join.RightOuterJoin(joinItem, alias);
            }
            MatchResult fullResult = result.Matches[SqlGrammar.FilteredJoinType.FullOuterJoin];
            if (fullResult.IsMatch)
            {
                return join.FullOuterJoin(joinItem, alias);
            }
            throw new InvalidOperationException();
        }

        private void AddJoinItem(SelectBuilder builder, Join join)
        {
            JoinStart start = join as JoinStart;
            if (start == null)
            {
                builder.AddJoin(join);
                return;
            }
            AliasedSource source = start.Source;
            Table table = source.Source as Table;
            if (table != null)
            {
                builder.AddTable(table, source.Alias);
                return;
            }
            ISelectBuilder select = source.Source as SelectBuilder;
            if (select != null)
            {
                builder.AddSelect(select, source.Alias);
                return;
            }
            Function functionCall = source.Source as Function;
            if (functionCall != null)
            {
                builder.AddFunction(functionCall, source.Alias);
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildProjectionList(MatchResult result, SelectBuilder builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.ProjectionList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ProjectionList.Multiple.First];
                BuildProjectionItem(first, builder);
                MatchResult remaining = multiple.Matches[SqlGrammar.ProjectionList.Multiple.Remaining];
                BuildProjectionList(remaining, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.ProjectionList.Single];
            if (single.IsMatch)
            {
                BuildProjectionItem(single, builder);
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildProjectionItem(MatchResult result, SelectBuilder builder)
        {
            MatchResult expression = result.Matches[SqlGrammar.ProjectionItem.Expression.Name];
            if (expression.IsMatch)
            {
                MatchResult itemResult = expression.Matches[SqlGrammar.ProjectionItem.Expression.Item];
                IProjectionItem item = (IProjectionItem)BuildArithmeticItem(itemResult);
                string alias = null;
                MatchResult aliasExpression = expression.Matches[SqlGrammar.ProjectionItem.Expression.AliasExpression.Name];
                if (aliasExpression.IsMatch)
                {
                    MatchResult aliasResult = aliasExpression.Matches[SqlGrammar.ProjectionItem.Expression.AliasExpression.Alias];
                    alias = GetToken(aliasResult);
                }
                builder.AddProjection(item, alias);
                return;
            }
            MatchResult star = result.Matches[SqlGrammar.ProjectionItem.Star.Name];
            if (star.IsMatch)
            {
                AliasedSource source = null;
                MatchResult qualifier = star.Matches[SqlGrammar.ProjectionItem.Star.Qualifier.Name];
                if (qualifier.IsMatch)
                {
                    MatchResult columnSource = qualifier.Matches[SqlGrammar.ProjectionItem.Star.Qualifier.ColumnSource];
                    List<string> parts = new List<string>();
                    BuildMultipartIdentifier(columnSource, parts);
                    string sourceName = parts[parts.Count - 1];
                    source = scope.GetSource(sourceName);
                }
                AllColumns all = new AllColumns(source);
                builder.AddProjection(all);
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildGroupByList(MatchResult result, SelectBuilder builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.GroupByList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult firstResult = multiple.Matches[SqlGrammar.GroupByList.Multiple.First];
                IGroupByItem first = (IGroupByItem)BuildArithmeticItem(firstResult);
                builder.AddGroupBy(first);
                MatchResult remainingResult = multiple.Matches[SqlGrammar.GroupByList.Multiple.Remaining];
                BuildGroupByList(remainingResult, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.GroupByList.Single];
            if (single.IsMatch)
            {
                IGroupByItem item = (IGroupByItem)BuildArithmeticItem(single);
                builder.AddGroupBy(item);
                return;
            }
            throw new InvalidOperationException();
        }

        private IFilter BuildOrFilter(MatchResult result)
        {
            MatchResult multiple = result.Matches[SqlGrammar.OrFilter.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.OrFilter.Multiple.First];
                IFilter firstFilter = BuildAndFilter(first);
                MatchResult remaining = multiple.Matches[SqlGrammar.OrFilter.Multiple.Remaining];
                IFilter remainingFilter = BuildOrFilter(remaining);
                return new FilterGroup(Conjunction.Or, firstFilter, remainingFilter);
            }
            MatchResult single = result.Matches[SqlGrammar.OrFilter.Single];
            if (single.IsMatch)
            {
                return BuildAndFilter(single);
            }
            throw new InvalidOperationException();
        }

        private IFilter BuildAndFilter(MatchResult result)
        {
            MatchResult multiple = result.Matches[SqlGrammar.AndFilter.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.AndFilter.Multiple.First];
                IFilter firstFilter = BuildFilter(first);
                MatchResult remaining = multiple.Matches[SqlGrammar.AndFilter.Multiple.Remaining];
                IFilter remainingFilter = BuildOrFilter(remaining);
                return new FilterGroup(Conjunction.And, firstFilter, remainingFilter);
            }
            MatchResult single = result.Matches[SqlGrammar.AndFilter.Single];
            if (single.IsMatch)
            {
                return BuildFilter(single);
            }
            throw new InvalidOperationException();
        }

        private IFilter BuildFilter(MatchResult result)
        {
            MatchResult notResult = result.Matches[SqlGrammar.Filter.Not.Name];
            if (notResult.IsMatch)
            {
                MatchResult filterResult = notResult.Matches[SqlGrammar.Filter.Not.Filter];
                IFilter filter = BuildFilter(filterResult);
                return new NotFilter(filter);
            }
            MatchResult wrappedResult = result.Matches[SqlGrammar.Filter.Wrapped.Name];
            if (wrappedResult.IsMatch)
            {
                MatchResult filterResult = wrappedResult.Matches[SqlGrammar.Filter.Wrapped.Filter];
                FilterGroup nested = new FilterGroup();
                IFilter innerFilter = BuildOrFilter(filterResult);
                nested.AddFilter(innerFilter);
                nested.WrapInParentheses = true;
                return nested;
            }
            MatchResult quantifyResult = result.Matches[SqlGrammar.Filter.Quantify.Name];
            if (quantifyResult.IsMatch)
            {
                MatchResult expressionResult = quantifyResult.Matches[SqlGrammar.Filter.Quantify.Expression];
                IFilterItem filterItem = (IFilterItem)BuildArithmeticItem(expressionResult);
                MatchResult quantifierResult = quantifyResult.Matches[SqlGrammar.Filter.Quantify.Quantifier];
                Quantifier quantifier = BuildQuantifier(quantifierResult);
                IValueProvider valueProvider = null;
                MatchResult selectResult = quantifyResult.Matches[SqlGrammar.Filter.Quantify.SelectStatement];
                if (selectResult.IsMatch)
                {
                    valueProvider = BuildSelectStatement(selectResult);
                }
                MatchResult valueListResult = quantifyResult.Matches[SqlGrammar.Filter.Quantify.ValueList];
                if (valueListResult.IsMatch)
                {
                    ValueList values = new ValueList();
                    BuildValueList(valueListResult, values);
                    valueProvider = values;
                }
                MatchResult operatorResult = quantifyResult.Matches[SqlGrammar.Filter.Quantify.ComparisonOperator];
                return BuildQuantifierFilter(operatorResult, filterItem, quantifier, valueProvider);
            }
            MatchResult functionResult = result.Matches[SqlGrammar.Filter.Function.Name];
            if (functionResult.IsMatch)
            {
                MatchResult expressionResult = functionResult.Matches[SqlGrammar.Filter.Function.Expression];
                return BuildFunctionCall(expressionResult);
            }
            MatchResult orderResult = result.Matches[SqlGrammar.Filter.Order.Name];
            if (orderResult.IsMatch)
            {
                MatchResult leftResult = orderResult.Matches[SqlGrammar.Filter.Order.Left];
                IFilterItem left = (IFilterItem)BuildArithmeticItem(leftResult);
                MatchResult rightResult = orderResult.Matches[SqlGrammar.Filter.Order.Right];
                IFilterItem right = (IFilterItem)BuildArithmeticItem(rightResult);
                MatchResult operatorResult = orderResult.Matches[SqlGrammar.Filter.Order.ComparisonOperator];
                return BuildOrderFilter(operatorResult, left, right);
            }
            MatchResult betweenResult = result.Matches[SqlGrammar.Filter.Between.Name];
            if (betweenResult.IsMatch)
            {
                MatchResult expressionResult = betweenResult.Matches[SqlGrammar.Filter.Between.Expression];
                IFilterItem expression = (IFilterItem)BuildArithmeticItem(expressionResult);
                MatchResult lowerBoundResult = betweenResult.Matches[SqlGrammar.Filter.Between.LowerBound];
                IFilterItem lowerBound = (IFilterItem)BuildArithmeticItem(lowerBoundResult);
                MatchResult upperBoundResult = betweenResult.Matches[SqlGrammar.Filter.Between.UpperBound];
                IFilterItem upperBound = (IFilterItem)BuildArithmeticItem(upperBoundResult);
                BetweenFilter filter = new BetweenFilter(expression, lowerBound, upperBound);
                MatchResult betweenNotResult = betweenResult.Matches[SqlGrammar.Filter.Between.NotKeyword];
                filter.Not = betweenNotResult.IsMatch;
                return filter;
            }
            MatchResult likeResult = result.Matches[SqlGrammar.Filter.Like.Name];
            if (likeResult.IsMatch)
            {
                MatchResult leftResult = likeResult.Matches[SqlGrammar.Filter.Like.Left];
                IFilterItem left = (IFilterItem)BuildArithmeticItem(leftResult);
                MatchResult rightResult = likeResult.Matches[SqlGrammar.Filter.Like.Right];
                IFilterItem right = (IFilterItem)BuildArithmeticItem(rightResult);
                LikeFilter filter = new LikeFilter(left, right);
                MatchResult likeNotResult = likeResult.Matches[SqlGrammar.Filter.Like.NotKeyword];
                filter.Not = likeNotResult.IsMatch;
                return filter;
            }
            MatchResult isResult = result.Matches[SqlGrammar.Filter.Is.Name];
            if (isResult.IsMatch)
            {
                MatchResult expressionResult = isResult.Matches[SqlGrammar.Filter.Is.Expression];
                IFilterItem expression = (IFilterItem)BuildArithmeticItem(expressionResult);
                NullFilter filter = new NullFilter(expression);
                MatchResult isNotResult = isResult.Matches[SqlGrammar.Filter.Is.NotKeyword];
                filter.Not = isNotResult.IsMatch;
                return filter;
            }
            MatchResult inResult = result.Matches[SqlGrammar.Filter.In.Name];
            if (inResult.IsMatch)
            {
                MatchResult expressionResult = inResult.Matches[SqlGrammar.Filter.In.Expression];
                IFilterItem expression = (IFilterItem)BuildArithmeticItem(expressionResult);
                IValueProvider valueProvider = null;
                MatchResult parameterResult = inResult.Matches[SqlGrammar.Filter.In.Parameter.Name];
                if (parameterResult.IsMatch)
                {
                    string value = ((TokenResult)parameterResult.Matches[SqlGrammar.Filter.In.Parameter.Value].Context).Value;
                    ParameterLiteral sl = new ParameterLiteral(value);
                    ParameterValue pv = new ParameterValue(sl);

                    valueProvider = pv;
                }

                MatchResult valuesResult = inResult.Matches[SqlGrammar.Filter.In.Values.Name];
                if (valuesResult.IsMatch)
                {
                    MatchResult valueListResult = valuesResult.Matches[SqlGrammar.Filter.In.Values.ValueList];
                    ValueList values = new ValueList();
                    BuildValueList(valueListResult, values);
                    valueProvider = values;
                }
                MatchResult selectResult = inResult.Matches[SqlGrammar.Filter.In.Select.Name];
                if (selectResult.IsMatch)
                {
                    MatchResult selectStatementResult = selectResult.Matches[SqlGrammar.Filter.In.Select.SelectStatement];
                    valueProvider = BuildSelectStatement(selectStatementResult);
                }
                MatchResult functionCall = inResult.Matches[SqlGrammar.Filter.In.FunctionCall];
                if (functionCall.IsMatch)
                {
                    valueProvider = BuildFunctionCall(functionCall);
                }
                InFilter filter = new InFilter(expression, valueProvider);
                MatchResult inNotResult = inResult.Matches[SqlGrammar.Filter.In.NotKeyword];
                filter.Not = inNotResult.IsMatch;
                return filter;
            }
            MatchResult existsResult = result.Matches[SqlGrammar.Filter.Exists.Name];
            if (existsResult.IsMatch)
            {
                MatchResult selectExpressionResult = existsResult.Matches[SqlGrammar.Filter.Exists.SelectStatement];
                ISelectBuilder builder = BuildSelectStatement(selectExpressionResult);
                ExistsFilter filter = new ExistsFilter(builder);
                return filter;
            }
            throw new InvalidOperationException();
        }

        private Quantifier BuildQuantifier(MatchResult result)
        {
            MatchResult all = result.Matches[SqlGrammar.Quantifier.All];
            if (all.IsMatch)
            {
                return Quantifier.All;
            }
            MatchResult any = result.Matches[SqlGrammar.Quantifier.Any];
            if (any.IsMatch)
            {
                return Quantifier.Any;
            }
            MatchResult some = result.Matches[SqlGrammar.Quantifier.Some];
            if (some.IsMatch)
            {
                return Quantifier.Some;
            }
            throw new InvalidOperationException();
        }

        private IFilter BuildQuantifierFilter(MatchResult result, IFilterItem leftHand, Quantifier quantifier, IValueProvider valueProvider)
        {
            MatchResult equalToResult = result.Matches[SqlGrammar.ComparisonOperator.EqualTo];
            if (equalToResult.IsMatch)
            {
                return new EqualToQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            MatchResult notEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.NotEqualTo];
            if (notEqualToResult.IsMatch)
            {
                return new NotEqualToQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            MatchResult lessThanEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.LessThanEqualTo];
            if (lessThanEqualToResult.IsMatch)
            {
                return new LessThanEqualToQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            MatchResult greaterThanEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.GreaterThanEqualTo];
            if (greaterThanEqualToResult.IsMatch)
            {
                return new GreaterThanEqualToQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            MatchResult lessThanResult = result.Matches[SqlGrammar.ComparisonOperator.LessThan];
            if (lessThanResult.IsMatch)
            {
                return new LessThanQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            MatchResult greaterThanResult = result.Matches[SqlGrammar.ComparisonOperator.GreaterThan];
            if (greaterThanResult.IsMatch)
            {
                return new GreaterThanQuantifierFilter(leftHand, quantifier, valueProvider);
            }
            throw new InvalidOperationException();
        }

        private IFilter BuildOrderFilter(MatchResult result, IFilterItem left, IFilterItem right)
        {
            MatchResult equalToResult = result.Matches[SqlGrammar.ComparisonOperator.EqualTo];
            if (equalToResult.IsMatch)
            {
                return new EqualToFilter(left, right);
            }
            MatchResult notEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.NotEqualTo];
            if (notEqualToResult.IsMatch)
            {
                return new NotEqualToFilter(left, right);
            }
            MatchResult lessThanEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.LessThanEqualTo];
            if (lessThanEqualToResult.IsMatch)
            {
                return new LessThanEqualToFilter(left, right);
            }
            MatchResult greaterThanEqualToResult = result.Matches[SqlGrammar.ComparisonOperator.GreaterThanEqualTo];
            if (greaterThanEqualToResult.IsMatch)
            {
                return new GreaterThanEqualToFilter(left, right);
            }
            MatchResult lessThanResult = result.Matches[SqlGrammar.ComparisonOperator.LessThan];
            if (lessThanResult.IsMatch)
            {
                return new LessThanFilter(left, right);
            }
            MatchResult greaterThanResult = result.Matches[SqlGrammar.ComparisonOperator.GreaterThan];
            if (greaterThanResult.IsMatch)
            {
                return new GreaterThanFilter(left, right);
            }
            throw new InvalidOperationException();
        }

        private SelectCombiner BuildSelectCombiner(MatchResult result, ISelectBuilder leftHand, ISelectBuilder rightHand)
        {
            MatchResult union = result.Matches[SqlGrammar.SelectCombiner.Union];
            if (union.IsMatch)
            {
                return new Union(leftHand, rightHand);
            }
            MatchResult intersect = result.Matches[SqlGrammar.SelectCombiner.Intersect];
            if (intersect.IsMatch)
            {
                return new Intersect(leftHand, rightHand);
            }
            MatchResult except = result.Matches[SqlGrammar.SelectCombiner.Except];
            if (except.IsMatch)
            {
                return new Except(leftHand, rightHand);
            }
            MatchResult minus = result.Matches[SqlGrammar.SelectCombiner.Minus];
            if (minus.IsMatch)
            {
                return new Minus(leftHand, rightHand);
            }
            throw new InvalidOperationException();
        }

        private void BuildOrderByList(MatchResult result, List<OrderBy> orderByList)
        {
            MatchResult multiple = result.Matches[SqlGrammar.OrderByList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.OrderByList.Multiple.First];
                BuildOrderByItem(first, orderByList);
                MatchResult remaining = multiple.Matches[SqlGrammar.OrderByList.Multiple.Remaining];
                BuildOrderByList(remaining, orderByList);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.OrderByList.Single];
            if (single.IsMatch)
            {
                BuildOrderByItem(single, orderByList);
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildOrderByItem(MatchResult result, List<OrderBy> orderByList)
        {
            MatchResult expressionResult = result.Matches[SqlGrammar.OrderByItem.Expression];
            IProjectionItem expression = (IProjectionItem)BuildArithmeticItem(expressionResult);
            Order order = Order.Default;
            MatchResult directionResult = result.Matches[SqlGrammar.OrderByItem.OrderDirection];
            if (directionResult.IsMatch)
            {
                order = buildOrderDirection(directionResult);
            }
            NullPlacement placement = NullPlacement.Default;
            MatchResult placementResult = result.Matches[SqlGrammar.OrderByItem.NullPlacement];
            if (placementResult.IsMatch)
            {
                placement = buildNullPlacement(placementResult);
            }
            OrderBy orderBy = new OrderBy(expression, order, placement);
            //sql2012+分页
            MatchResult offsetFetchResult = result.Matches[SqlGrammar.OrderByItem.PageOffsetFetch];
            if (offsetFetchResult.IsMatch)
            {
                orderBy.OffsetFetch = ((TokenResult)offsetFetchResult.Matches[SqlGrammar.OrderByItem.PageOffsetFetch].Context).Value.ToUpper();
            }
            MatchResult limitResult = result.Matches[SqlGrammar.OrderByItem.PageLimit];
            if (limitResult.IsMatch)
            {
                orderBy.Limit = ((TokenResult)limitResult.Matches[SqlGrammar.OrderByItem.PageLimit].Context).Value.ToUpper();
            }
            orderByList.Add(orderBy);
        }

        private Order buildOrderDirection(MatchResult result)
        {
            MatchResult descending = result.Matches[SqlGrammar.OrderDirection.Descending];
            if (descending.IsMatch)
            {
                return Order.Descending;
            }
            MatchResult ascending = result.Matches[SqlGrammar.OrderDirection.Ascending];
            if (ascending.IsMatch)
            {
                return Order.Ascending;
            }
            throw new InvalidOperationException();
        }

        private NullPlacement buildNullPlacement(MatchResult result)
        {
            MatchResult nullsFirst = result.Matches[SqlGrammar.NullPlacement.NullsFirst];
            if (nullsFirst.IsMatch)
            {
                return NullPlacement.First;
            }
            MatchResult nullsLast = result.Matches[SqlGrammar.NullPlacement.NullsLast];
            if (nullsLast.IsMatch)
            {
                return NullPlacement.Last;
            }
            throw new InvalidOperationException();
        }

        private ICommand BuildInsertStatement(MatchResult result)
        {
            MatchResult tableResult = result.Matches[SqlGrammar.InsertStatement.Table];
            Table table = BuildTable(tableResult);
            IValueProvider valueProvider = null;
            MatchResult valuesResult = result.Matches[SqlGrammar.InsertStatement.Values.Name];
            if (valuesResult.IsMatch)
            {
                ValueList values = new ValueList();
                MatchResult valueListResult = valuesResult.Matches[SqlGrammar.InsertStatement.Values.ValueList];
                if (valueListResult.IsMatch)
                {
                    BuildValueList(valueListResult, values);
                }
                valueProvider = values;
            }
            MatchResult selectResult = result.Matches[SqlGrammar.InsertStatement.Select.Name];
            if (selectResult.IsMatch)
            {
                MatchResult selectStatementResult = selectResult.Matches[SqlGrammar.InsertStatement.Select.SelectStatement];
                ISelectBuilder selectBuilder = BuildSelectStatement(selectStatementResult);
                valueProvider = selectBuilder;
            }
            string alias = null;
            MatchResult aliasExpressionResult = result.Matches[SqlGrammar.InsertStatement.AliasExpression.Name];
            if (aliasExpressionResult.IsMatch)
            {
                MatchResult aliasResult = aliasExpressionResult.Matches[SqlGrammar.InsertStatement.AliasExpression.Alias];
                alias = GetToken(aliasResult);
            }
            InsertBuilder builder = new InsertBuilder(table, valueProvider, alias);
            SourceCollection collection = new SourceCollection();
            collection.AddSource(builder.Table.GetSourceName(), builder.Table);
            scope.Push(collection);
            MatchResult columnsResult = result.Matches[SqlGrammar.InsertStatement.Columns.Name];
            if (columnsResult.IsMatch)
            {
                MatchResult columnListResult = columnsResult.Matches[SqlGrammar.InsertStatement.Columns.ColumnList];
                BuildColumnsList(columnListResult, builder);
            }

            // within the context of a T-SQL output clause, INSERTED and DELETED are valid sources.
            collection.AddSource("INSERTED", new AliasedSource(new Table("INSERTED"), null));
            collection.AddSource("DELETED", new AliasedSource(new Table("DELETED"), null));
            MatchResult outputClauseResult = result.Matches[SqlGrammar.InsertStatement.Output.Name];
            if (outputClauseResult.IsMatch)
            {
                var columnListResult = outputClauseResult.Matches[SqlGrammar.Output.Columns.Name];
                BuildOutputProjectionList(columnListResult, builder);
            }
            collection.Remove("INSERTED");
            collection.Remove("DELETED");
            scope.Pop();


            return builder;
        }

        private void BuildColumnsList(MatchResult result, InsertBuilder builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.ColumnList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ColumnList.Multiple.First];
                Column column = BuildColumn(first);
                builder.AddColumn(column);
                MatchResult remaining = multiple.Matches[SqlGrammar.ColumnList.Multiple.Remaining];
                BuildColumnsList(remaining, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.ColumnList.Single];
            if (single.IsMatch)
            {
                Column column = BuildColumn(single);
                builder.AddColumn(column);
                return;
            }
            throw new InvalidOperationException();
        }

        //private void buildOutputColumnsList(MatchResult result, IOutputCommand builder)
        //{
        //    MatchResult multiple = result.Matches[SqlGrammar.ColumnList.Multiple.Name];
        //    if (multiple.IsMatch)
        //    {
        //        MatchResult first = multiple.Matches[SqlGrammar.ColumnList.Multiple.First];
        //        Column column = buildColumn(first);
        //        builder.AddOutputColumn(column);
        //        MatchResult remaining = multiple.Matches[SqlGrammar.ColumnList.Multiple.Remaining];
        //        buildOutputColumnsList(remaining, builder);
        //        return;
        //    }
        //    MatchResult single = result.Matches[SqlGrammar.ColumnList.Single];
        //    if (single.IsMatch)
        //    {
        //        Column column = buildColumn(single);
        //        builder.AddOutputColumn(column);
        //        return;
        //    }
        //    throw new InvalidOperationException();
        //}


        private void BuildOutputProjectionList(MatchResult result, IOutputCommand builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.ProjectionList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ProjectionList.Multiple.First];
                BuildOutputProjectionItem(first, builder);
                MatchResult remaining = multiple.Matches[SqlGrammar.ProjectionList.Multiple.Remaining];
                BuildOutputProjectionList(remaining, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.ProjectionList.Single];
            if (single.IsMatch)
            {
                BuildOutputProjectionItem(single, builder);
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildOutputProjectionItem(MatchResult result, IOutputCommand builder)
        {
            MatchResult expression = result.Matches[SqlGrammar.ProjectionItem.Expression.Name];
            if (expression.IsMatch)
            {
                MatchResult itemResult = expression.Matches[SqlGrammar.ProjectionItem.Expression.Item];
                IProjectionItem item = (IProjectionItem)BuildArithmeticItem(itemResult);
                string alias = null;
                MatchResult aliasExpression = expression.Matches[SqlGrammar.ProjectionItem.Expression.AliasExpression.Name];
                if (aliasExpression.IsMatch)
                {
                    MatchResult aliasResult = aliasExpression.Matches[SqlGrammar.ProjectionItem.Expression.AliasExpression.Alias];
                    alias = GetToken(aliasResult);
                }

                var p = builder.AddOutputProjection(item, alias);

                return;
            }
            MatchResult star = result.Matches[SqlGrammar.ProjectionItem.Star.Name];
            if (star.IsMatch)
            {
                AliasedSource source = null;
                MatchResult qualifier = star.Matches[SqlGrammar.ProjectionItem.Star.Qualifier.Name];
                if (qualifier.IsMatch)
                {
                    MatchResult columnSource = qualifier.Matches[SqlGrammar.ProjectionItem.Star.Qualifier.ColumnSource];
                    List<string> parts = new List<string>();
                    BuildMultipartIdentifier(columnSource, parts);
                    string sourceName = parts[parts.Count - 1];
                    source = scope.GetSource(sourceName);
                }
                AllColumns all = new AllColumns(source);
                builder.AddOutputProjection(all);
                return;
            }
            throw new InvalidOperationException();
        }

        private ICommand buildUpdateStatement(MatchResult result)
        {
            MatchResult tableResult = result.Matches[SqlGrammar.UpdateStatement.Table];
            Table table = BuildTable(tableResult);
            string alias = null;
            MatchResult aliasExpressionResult = result.Matches[SqlGrammar.UpdateStatement.AliasExpression.Name];
            if (aliasExpressionResult.IsMatch)
            {
                MatchResult aliasResult = aliasExpressionResult.Matches[SqlGrammar.UpdateStatement.AliasExpression.Alias];
                alias = GetToken(aliasResult);
            }
            UpdateBuilder builder = new UpdateBuilder(table, alias);
            SourceCollection collection = new SourceCollection();
            collection.AddSource(builder.Table.GetSourceName(), builder.Table);
            scope.Push(collection);
            MatchResult setterListResult = result.Matches[SqlGrammar.UpdateStatement.SetterList];
            BuildSetterList(setterListResult, builder);

            // within the context of a T-SQL output clause, INSERTED and DELETED are valid sources.
            collection.AddSource("INSERTED", new AliasedSource(new Table("INSERTED"), null));
            collection.AddSource("DELETED", new AliasedSource(new Table("DELETED"), null));

            MatchResult outputClauseResult = result.Matches[SqlGrammar.UpdateStatement.Output.Name];
            if (outputClauseResult.IsMatch)
            {
                var columnListResult = outputClauseResult.Matches[SqlGrammar.Output.Columns.Name];
                BuildOutputProjectionList(columnListResult, builder);
            }
            collection.Remove("INSERTED");
            collection.Remove("DELETED");

            MatchResult whereResult = result.Matches[SqlGrammar.UpdateStatement.Where.Name];
            if (whereResult.IsMatch)
            {
                MatchResult filterListResult = whereResult.Matches[SqlGrammar.UpdateStatement.Where.FilterList];
                IFilter innerFilter = BuildOrFilter(filterListResult);
                builder.WhereFilterGroup.AddFilter(innerFilter);
                builder.WhereFilterGroup.Optimize();
            }
            scope.Pop();
            return builder;
        }

        private void BuildSetterList(MatchResult result, UpdateBuilder builder)
        {
            MatchResult multiple = result.Matches[SqlGrammar.SetterList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.SetterList.Multiple.First];
                Setter setter = BuildSetter(first);
                builder.AddSetter(setter);
                MatchResult remaining = multiple.Matches[SqlGrammar.SetterList.Multiple.Remaining];
                BuildSetterList(remaining, builder);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.SetterList.Single];
            if (single.IsMatch)
            {
                Setter setter = BuildSetter(single);
                builder.AddSetter(setter);
                return;
            }
            throw new InvalidOperationException();
        }

        private Setter BuildSetter(MatchResult result)
        {
            MatchResult columnResult = result.Matches[SqlGrammar.Setter.Column];
            Column column = BuildColumn(columnResult);
            MatchResult valueResult = result.Matches[SqlGrammar.Setter.Value];
            IProjectionItem value = (IProjectionItem)BuildArithmeticItem(valueResult);
            Setter setter = new Setter(column, value);
            return setter;
        }

        private ICommand buildDeleteStatement(MatchResult result)
        {
            MatchResult tableResult = result.Matches[SqlGrammar.DeleteStatement.Table];
            Table table = BuildTable(tableResult);
            string alias = null;
            MatchResult aliasExpressionResult = result.Matches[SqlGrammar.DeleteStatement.AliasExpression.Name];
            if (aliasExpressionResult.IsMatch)
            {
                MatchResult aliasResult = aliasExpressionResult.Matches[SqlGrammar.DeleteStatement.AliasExpression.Alias];
                alias = GetToken(aliasResult);
            }
            DeleteBuilder builder = new DeleteBuilder(table, alias);
            SourceCollection collection = new SourceCollection();
            collection.AddSource(builder.Table.GetSourceName(), builder.Table);
            scope.Push(collection);

            // within the context of a T-SQL output clause, INSERTED and DELETED are valid sources.
            collection.AddSource("INSERTED", new AliasedSource(new Table("INSERTED"), null));
            collection.AddSource("DELETED", new AliasedSource(new Table("DELETED"), null));
            //scope.Push(collection);
            MatchResult outputClauseResult = result.Matches[SqlGrammar.DeleteStatement.Output.Name];
            if (outputClauseResult.IsMatch)
            {
                var columnListResult = outputClauseResult.Matches[SqlGrammar.Output.Columns.Name];
                BuildOutputProjectionList(columnListResult, builder);
            }
            collection.Remove("INSERTED");
            collection.Remove("DELETED");

            MatchResult whereResult = result.Matches[SqlGrammar.DeleteStatement.Where.Name];
            if (whereResult.IsMatch)
            {
                MatchResult filterListResult = whereResult.Matches[SqlGrammar.DeleteStatement.Where.FilterList];
                IFilter innerFilter = BuildOrFilter(filterListResult);
                builder.WhereFilterGroup.AddFilter(innerFilter);
                builder.WhereFilterGroup.Optimize();
            }
            scope.Pop();
            return builder;
        }

        private void BuildMultipartIdentifier(MatchResult result, List<string> parts)
        {
            MatchResult multiple = result.Matches[SqlGrammar.MultipartIdentifier.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.MultipartIdentifier.Multiple.First];
                parts.Add(GetToken(first));
                MatchResult remaining = multiple.Matches[SqlGrammar.MultipartIdentifier.Multiple.Remaining];
                BuildMultipartIdentifier(remaining, parts);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.MultipartIdentifier.Single];
            if (single.IsMatch)
            {
                parts.Add(GetToken(single));
                return;
            }
            throw new InvalidOperationException();
        }

        private object BuildArithmeticItem(MatchResult result)
        {
            MatchResult expression = result.Matches[SqlGrammar.ArithmeticItem.ArithmeticExpression];
            return BuildAdditiveExpression(expression, false);
        }

        private object BuildAdditiveExpression(MatchResult result, bool wrap)
        {
            MatchResult multiple = result.Matches[SqlGrammar.AdditiveExpression.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult firstResult = multiple.Matches[SqlGrammar.AdditiveExpression.Multiple.First];
                IProjectionItem first = (IProjectionItem)BuildMultiplicitiveExpression(firstResult, false);
                MatchResult remainingResult = multiple.Matches[SqlGrammar.AdditiveExpression.Multiple.Remaining];
                IProjectionItem remaining = (IProjectionItem)BuildAdditiveExpression(remainingResult, false);
                MatchResult operatorResult = multiple.Matches[SqlGrammar.AdditiveExpression.Multiple.Operator];
                return BuildAdditiveOperator(operatorResult, first, remaining, wrap);
            }
            MatchResult single = result.Matches[SqlGrammar.AdditiveExpression.Single];
            if (single.IsMatch)
            {
                return BuildMultiplicitiveExpression(single, wrap);
            }
            throw new InvalidOperationException();
        }

        private object BuildAdditiveOperator(MatchResult result, IProjectionItem leftHand, IProjectionItem rightHand, bool wrap)
        {
            MatchResult plusResult = result.Matches[SqlGrammar.AdditiveOperator.PlusOperator];
            if (plusResult.IsMatch)
            {
                Addition addition = new Addition(leftHand, rightHand);
                addition.WrapInParentheses = wrap;
                return addition;
            }
            MatchResult minusResult = result.Matches[SqlGrammar.AdditiveOperator.MinusOperator];
            if (minusResult.IsMatch)
            {
                Subtraction subtraction = new Subtraction(leftHand, rightHand);
                subtraction.WrapInParentheses = wrap;
                return subtraction;
            }
            throw new InvalidOperationException();
        }

        private object BuildMultiplicitiveExpression(MatchResult result, bool wrap)
        {
            MatchResult multiple = result.Matches[SqlGrammar.MultiplicitiveExpression.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult firstResult = multiple.Matches[SqlGrammar.MultiplicitiveExpression.Multiple.First];
                IProjectionItem first = (IProjectionItem)BuildWrappedItem(firstResult);
                MatchResult remainingResult = multiple.Matches[SqlGrammar.MultiplicitiveExpression.Multiple.Remaining];
                IProjectionItem remaining = (IProjectionItem)BuildMultiplicitiveExpression(remainingResult, false);
                MatchResult operatorResult = multiple.Matches[SqlGrammar.MultiplicitiveExpression.Multiple.Operator];
                return BuildMultiplicitiveOperator(operatorResult, first, remaining, wrap);
            }
            MatchResult single = result.Matches[SqlGrammar.MultiplicitiveExpression.Single];
            if (single.IsMatch)
            {
                return BuildWrappedItem(single);
            }
            throw new InvalidOperationException();
        }

        private object BuildMultiplicitiveOperator(MatchResult result, IProjectionItem leftHand, IProjectionItem rightHand, bool wrap)
        {
            MatchResult multiply = result.Matches[SqlGrammar.MultiplicitiveOperator.Multiply];
            if (multiply.IsMatch)
            {
                Multiplication multiplication = new Multiplication(leftHand, rightHand);
                multiplication.WrapInParentheses = wrap;
                return multiplication;
            }
            MatchResult divide = result.Matches[SqlGrammar.MultiplicitiveOperator.Divide];
            if (divide.IsMatch)
            {
                Division division = new Division(leftHand, rightHand);
                division.WrapInParentheses = wrap;
                return division;
            }
            MatchResult modulo = result.Matches[SqlGrammar.MultiplicitiveOperator.Modulus];
            if (modulo.IsMatch)
            {
                Modulus modulus = new Modulus(leftHand, rightHand);
                modulus.WrapInParentheses = wrap;
                return modulus;
            }
            throw new InvalidOperationException();
        }

        private object BuildWrappedItem(MatchResult result)
        {
            MatchResult negatedResult = result.Matches[SqlGrammar.WrappedItem.Negated.Name];
            if (negatedResult.IsMatch)
            {
                MatchResult expressionResult = negatedResult.Matches[SqlGrammar.WrappedItem.Negated.Item];
                IProjectionItem item = (IProjectionItem)BuildWrappedItem(expressionResult);
                return new Negation(item);
            }
            MatchResult wrappedResult = result.Matches[SqlGrammar.WrappedItem.Wrapped.Name];
            if (wrappedResult.IsMatch)
            {
                MatchResult expressionResult = wrappedResult.Matches[SqlGrammar.WrappedItem.Wrapped.AdditiveExpression];
                object expression = BuildAdditiveExpression(expressionResult, true);
                return expression;
            }
            MatchResult itemResult = result.Matches[SqlGrammar.WrappedItem.Item];
            if (itemResult.IsMatch)
            {
                return BuildItem(itemResult);
            }
            throw new InvalidOperationException();
        }

        private object BuildItem(MatchResult result)
        {
            MatchResult numberResult = result.Matches[SqlGrammar.Item.Number];
            if (numberResult.IsMatch)
            {
                return BuildNumericLiteral(numberResult);
            }
            MatchResult stringResult = result.Matches[SqlGrammar.Item.String];
            if (stringResult.IsMatch)
            {
                return BuildStringLiteral(stringResult);
            }
            MatchResult nullResult = result.Matches[SqlGrammar.Item.Null];
            if (nullResult.IsMatch)
            {
                return new NullLiteral();
            }
            MatchResult functionCallResult = result.Matches[SqlGrammar.Item.FunctionCall];
            if (functionCallResult.IsMatch)
            {
                return BuildFunctionCall(functionCallResult);
            }
            MatchResult columnResult = result.Matches[SqlGrammar.Item.Column];
            if (columnResult.IsMatch)
            {
                return BuildNamedItem(columnResult);
            }
            MatchResult matchCaseResult = result.Matches[SqlGrammar.Item.MatchCase];
            if (matchCaseResult.IsMatch)
            {
                return BuildMatchCase(matchCaseResult);
            }
            MatchResult conditionCaseResult = result.Matches[SqlGrammar.Item.ConditionCase];
            if (conditionCaseResult.IsMatch)
            {
                return BuildConditionalCase(conditionCaseResult);
            }
            MatchResult selectResult = result.Matches[SqlGrammar.Item.Select.Name];
            if (selectResult.IsMatch)
            {
                MatchResult selectExpressionResult = selectResult.Matches[SqlGrammar.Item.Select.SelectStatement];
                return BuildSelectStatement(selectExpressionResult);
            }
            throw new NotImplementedException();
        }

        private NumericLiteral BuildNumericLiteral(MatchResult result)
        {
            string numberString = GetToken(result);
            double value = Double.Parse(numberString);
            return new NumericLiteral(value);
        }

        private object BuildNamedItem(MatchResult result)
        {
            List<string> parts = new List<string>();
            BuildMultipartIdentifier(result, parts);
            if (parts.Count > 1)
            {
                // if is a period-separated, multiple-part identifier, it is a column
                Namespace qualifier = GetNamespace(parts.Take(parts.Count - 2));
                string tableName = parts[parts.Count - 2];
                AliasedSource source = scope.GetSource(tableName);
                string columnName = parts[parts.Count - 1];
                var column = source.Column(columnName);
                column.Qualify = true;
                return column;
            }
            string name = parts[0];
            if (options.PlaceholderPrefix != null && name.StartsWith(options.PlaceholderPrefix))
            {
                // if the identifier begins with the placeholder prefix, treat is as a placeholder
                Placeholder placeholder = new Placeholder(name);
                return placeholder;
            }
            AliasedSource singleSource;
            if (scope.HasSingleSource(out singleSource))
            {
                // there is only one source in the query, so assume a column
                Column column = singleSource.Column(name);
                column.Qualify = false;
                return column;
            }
            // otherwise, we have no idea what the name represents
            // in order for the SQL to roundtrip without alteration, just use a placeholder
            Placeholder unknown = new Placeholder(name);
            return unknown;
        }

        private Column BuildColumn(MatchResult result)
        {
            List<string> parts = new List<string>();
            BuildMultipartIdentifier(result, parts);
            if (parts.Count > 1)
            {
                Namespace qualifier = GetNamespace(parts.Take(parts.Count - 2));
                string tableName = parts[parts.Count - 2];
                AliasedSource source = scope.GetSource(tableName);
                string columnName = parts[parts.Count - 1];
                var col = source.Column(columnName);
                col.Qualify = true;
                return col;
            }
            else
            {
                string columnName = parts[0];
                Column column;
                AliasedSource source;
                if (scope.HasSingleSource(out source))
                {
                    column = source.Column(columnName);
                    column.Qualify = false;
                }
                else
                {
                    column = new Column(columnName);
                }
                return column;
            }
        }

        private StringLiteral BuildStringLiteral(MatchResult result)
        {
            string value = GetToken(result);
            value = value.Substring(1, value.Length - 2);
            value = value.Replace("''", "'");
            return new StringLiteral(value);
        }

        private Function BuildFunctionCall(MatchResult result)
        {
            MatchResult functionNameResult = result.Matches[SqlGrammar.FunctionCall.FunctionName];
            List<string> parts = new List<string>();
            BuildMultipartIdentifier(functionNameResult, parts);
            Namespace qualifier = GetNamespace(parts.Take(parts.Count - 1));
            string functionName = parts[parts.Count - 1];
            Function function = new Function(qualifier, functionName);
            MatchResult argumentsResult = result.Matches[SqlGrammar.FunctionCall.Arguments];
            if (argumentsResult.IsMatch)
            {
                ValueList arguments = new ValueList();
                BuildValueList(argumentsResult, arguments);
                foreach (IProjectionItem value in arguments.Values)
                {
                    function.AddArgument(value);
                }
            }
            MatchResult windowResult = result.Matches[SqlGrammar.FunctionCall.Window.Name];
            if (windowResult.IsMatch)
            {
                FunctionWindow window = new FunctionWindow();
                MatchResult partitioning = windowResult.Matches[SqlGrammar.FunctionCall.Window.Partitioning.Name];
                if (partitioning.IsMatch)
                {
                    MatchResult valueListResult = partitioning.Matches[SqlGrammar.FunctionCall.Window.Partitioning.ValueList];
                    ValueList valueList = new ValueList();
                    BuildValueList(valueListResult, valueList);
                    foreach (IProjectionItem value in valueList.Values)
                    {
                        window.AddPartition(value);
                    }
                }
                MatchResult ordering = windowResult.Matches[SqlGrammar.FunctionCall.Window.Ordering.Name];
                if (ordering.IsMatch)
                {
                    MatchResult orderByListResult = ordering.Matches[SqlGrammar.FunctionCall.Window.Ordering.OrderByList];
                    BuildOrderByList(orderByListResult, window.OrderByList);
                }
                MatchResult framing = windowResult.Matches[SqlGrammar.FunctionCall.Window.Framing.Name];
                if (framing.IsMatch)
                {
                    MatchResult precedingOnlyFrameResult = framing.Matches[SqlGrammar.FunctionCall.Window.Framing.PrecedingFrame];
                    if (precedingOnlyFrameResult.IsMatch)
                    {
                        IPrecedingFrame precedingFrame = BuildPrecedingFrame(precedingOnlyFrameResult);
                        window.Frame = new PrecedingOnlyWindowFrame(precedingFrame);
                    }
                    MatchResult betweenFrameResult = framing.Matches[SqlGrammar.FunctionCall.Window.Framing.BetweenFrame.Name];
                    if (betweenFrameResult.IsMatch)
                    {
                        MatchResult precedingFrameResult = betweenFrameResult.Matches[SqlGrammar.FunctionCall.Window.Framing.BetweenFrame.PrecedingFrame];
                        IPrecedingFrame precedingFrame = BuildPrecedingFrame(precedingFrameResult);
                        MatchResult followingFrameResult = betweenFrameResult.Matches[SqlGrammar.FunctionCall.Window.Framing.BetweenFrame.FollowingFrame];
                        IFollowingFrame followingFrame = BuildFollowingFrame(followingFrameResult);
                        window.Frame = new BetweenWindowFrame(precedingFrame, followingFrame);
                    }
                    MatchResult frameTypeResult = framing.Matches[SqlGrammar.FunctionCall.Window.Framing.FrameType];
                    window.Frame.FrameType = BuildFrameType(frameTypeResult);
                }
                function.FunctionWindow = window;
            }
            return function;
        }

        private FrameType BuildFrameType(MatchResult result)
        {
            MatchResult rows = result.Matches[SqlGrammar.FrameType.Rows];
            if (rows.IsMatch)
            {
                return FrameType.Row;
            }
            MatchResult range = result.Matches[SqlGrammar.FrameType.Range];
            if (range.IsMatch)
            {
                return FrameType.Range;
            }
            throw new InvalidOperationException();
        }

        private IPrecedingFrame BuildPrecedingFrame(MatchResult result)
        {
            MatchResult unbound = result.Matches[SqlGrammar.PrecedingFrame.UnboundedPreceding.Name];
            if (unbound.IsMatch)
            {
                return new PrecedingUnboundFrame();
            }
            MatchResult bound = result.Matches[SqlGrammar.PrecedingFrame.BoundedPreceding.Name];
            if (bound.IsMatch)
            {
                MatchResult rowCountResult = bound.Matches[SqlGrammar.PrecedingFrame.BoundedPreceding.Number];
                NumericLiteral rowCount = BuildNumericLiteral(rowCountResult);
                return new PrecedingBoundFrame((int)rowCount.Value);
            }
            MatchResult currentRow = result.Matches[SqlGrammar.PrecedingFrame.CurrentRow];
            if (currentRow.IsMatch)
            {
                return new CurrentRowFrame();
            }
            throw new InvalidOperationException();
        }

        private IFollowingFrame BuildFollowingFrame(MatchResult result)
        {
            MatchResult unbound = result.Matches[SqlGrammar.FollowingFrame.UnboundedFollowing.Name];
            if (unbound.IsMatch)
            {
                return new FollowingUnboundFrame();
            }
            MatchResult bound = result.Matches[SqlGrammar.FollowingFrame.BoundedFollowing.Name];
            if (bound.IsMatch)
            {
                MatchResult rowCountResult = bound.Matches[SqlGrammar.FollowingFrame.BoundedFollowing.Number];
                NumericLiteral rowCount = BuildNumericLiteral(rowCountResult);
                return new FollowingBoundFrame((int)rowCount.Value);
            }
            MatchResult currentRow = result.Matches[SqlGrammar.FollowingFrame.CurrentRow];
            if (currentRow.IsMatch)
            {
                return new CurrentRowFrame();
            }
            throw new InvalidOperationException();
        }

        private MatchCase BuildMatchCase(MatchResult result)
        {
            MatchResult expressionResult = result.Matches[SqlGrammar.MatchCase.Expression];
            IProjectionItem expression = (IProjectionItem)BuildArithmeticItem(expressionResult);
            MatchCase options = new MatchCase(expression);
            MatchResult matchListResult = result.Matches[SqlGrammar.MatchCase.MatchList];
            BuildMatchList(matchListResult, options);
            return options;
        }

        private void BuildMatchList(MatchResult result, MatchCase options)
        {
            MatchResult match = result.Matches[SqlGrammar.MatchList.Match];
            BuildMatch(match, options);
            MatchResult matchListPrime = result.Matches[SqlGrammar.MatchList.MatchListPrime];
            buildMatchListPrime(matchListPrime, options);
        }

        private void buildMatchListPrime(MatchResult result, MatchCase options)
        {
            MatchResult matchResult = result.Matches[SqlGrammar.MatchListPrime.Match.Name];
            if (matchResult.IsMatch)
            {
                MatchResult firstResult = matchResult.Matches[SqlGrammar.MatchListPrime.Match.First];
                BuildMatch(firstResult, options);
                MatchResult remainingResult = matchResult.Matches[SqlGrammar.MatchListPrime.Match.Remaining];
                buildMatchListPrime(remainingResult, options);
                return;
            }
            MatchResult elseResult = result.Matches[SqlGrammar.MatchListPrime.Else.Name];
            if (elseResult.IsMatch)
            {
                MatchResult valueResult = elseResult.Matches[SqlGrammar.MatchListPrime.Else.Value];
                options.Default = (IProjectionItem)BuildArithmeticItem(valueResult);
                return;
            }
            MatchResult emptyResult = result.Matches[SqlGrammar.MatchListPrime.Empty];
            if (emptyResult.IsMatch)
            {
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildMatch(MatchResult result, MatchCase options)
        {
            MatchResult expressionResult = result.Matches[SqlGrammar.Match.Expression];
            IProjectionItem expression = (IProjectionItem)BuildArithmeticItem(expressionResult);
            MatchResult valueResult = result.Matches[SqlGrammar.Match.Value];
            IProjectionItem value = (IProjectionItem)BuildArithmeticItem(valueResult);
            options.AddBranch(expression, value);
        }

        private ConditionalCase BuildConditionalCase(MatchResult result)
        {
            ConditionalCase options = new ConditionalCase();
            MatchResult conditionListResult = result.Matches[SqlGrammar.ConditionalCase.ConditionList];
            BuildConditionList(conditionListResult, options);
            return options;
        }

        private void BuildConditionList(MatchResult result, ConditionalCase options)
        {
            MatchResult condition = result.Matches[SqlGrammar.ConditionList.Condition];
            BuildCondition(condition, options);
            MatchResult conditionListPrime = result.Matches[SqlGrammar.ConditionList.ConditionListPrime];
            BuildConditionListPrime(conditionListPrime, options);
        }

        private void BuildConditionListPrime(MatchResult result, ConditionalCase options)
        {
            MatchResult conditionResult = result.Matches[SqlGrammar.ConditionListPrime.Condition.Name];
            if (conditionResult.IsMatch)
            {
                MatchResult firstResult = conditionResult.Matches[SqlGrammar.ConditionListPrime.Condition.First];
                BuildCondition(firstResult, options);
                MatchResult remainingResult = conditionResult.Matches[SqlGrammar.ConditionListPrime.Condition.Remaining];
                BuildConditionListPrime(remainingResult, options);
                return;
            }
            MatchResult elseResult = result.Matches[SqlGrammar.ConditionListPrime.Else.Name];
            if (elseResult.IsMatch)
            {
                MatchResult valueResult = elseResult.Matches[SqlGrammar.ConditionListPrime.Else.Value];
                options.Default = (IProjectionItem)BuildArithmeticItem(valueResult);
                return;
            }
            MatchResult emptyResult = result.Matches[SqlGrammar.ConditionListPrime.Empty];
            if (emptyResult.IsMatch)
            {
                return;
            }
            throw new InvalidOperationException();
        }

        private void BuildCondition(MatchResult result, ConditionalCase options)
        {
            MatchResult expressionResult = result.Matches[SqlGrammar.Condition.Filter];
            IFilter innerFilter = BuildOrFilter(expressionResult);
            MatchResult valueResult = result.Matches[SqlGrammar.Condition.Value];
            IProjectionItem value = (IProjectionItem)BuildArithmeticItem(valueResult);
            FilterGroup filterGroup = new FilterGroup(Conjunction.And, innerFilter);
            filterGroup.Optimize();
            options.AddBranch(filterGroup, value);
        }

        private void BuildValueList(MatchResult result, ValueList values)
        {
            MatchResult multiple = result.Matches[SqlGrammar.ValueList.Multiple.Name];
            if (multiple.IsMatch)
            {
                MatchResult first = multiple.Matches[SqlGrammar.ValueList.Multiple.First];
                IProjectionItem value = (IProjectionItem)BuildArithmeticItem(first);
                values.AddValue(value);
                MatchResult remaining = multiple.Matches[SqlGrammar.ValueList.Multiple.Remaining];
                BuildValueList(remaining, values);
                return;
            }
            MatchResult single = result.Matches[SqlGrammar.ValueList.Single];
            if (single.IsMatch)
            {
                IProjectionItem value = (IProjectionItem)BuildArithmeticItem(single);
                values.AddValue(value);
                return;
            }
            throw new NotImplementedException();
        }

        private Table BuildTable(MatchResult tableResult)
        {
            List<string> parts = new List<string>();
            BuildMultipartIdentifier(tableResult, parts);
            Namespace qualifier = GetNamespace(parts.Take(parts.Count - 1));
            string tableName = parts[parts.Count - 1];
            Table table = new Table(qualifier, tableName);
            return table;
        }

        private Namespace GetNamespace(IEnumerable<string> qualifiers)
        {
            if (!qualifiers.Any())
            {
                return null;
            }
            Namespace schema = new Namespace();
            foreach (string qualifier in qualifiers)
            {
                schema.AddQualifier(qualifier);
            }
            return schema;
        }

        private string GetToken(MatchResult result)
        {
            TokenResult tokenResult = (TokenResult)result.Context;
            return tokenResult.Value;
        }

        private sealed class SourceScope
        {
            private readonly List<SourceCollection> stack;

            public SourceScope()
            {
                stack = new List<SourceCollection>();

            }

            public void Push(SourceCollection collection)
            {
                stack.Add(collection);
            }

            public void Pop()
            {
                stack.RemoveAt(stack.Count - 1);
            }

            public AliasedSource GetSource(string sourceName)
            {
                int index = stack.Count;
                while (index != 0)
                {
                    --index;
                    SourceCollection collection = stack[index];
                    if (collection.Exists(sourceName))
                    {
                        return collection[sourceName];
                    }
                }
                string message = String.Format("An attempt was made to retrieve an column source: {0}.", sourceName);
                throw new SQLGenerationException(message);
            }

            public bool HasSingleSource(out AliasedSource source)
            {
                if (stack.Count == 0)
                {
                    source = null;
                    return false;
                }
                SourceCollection collection = stack[stack.Count - 1];
                if (collection.Count != 1)
                {
                    source = null;
                    return false;
                }
                source = collection.Sources.Single();
                return true;
            }
        }
    }
}
