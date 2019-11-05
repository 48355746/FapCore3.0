using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Provides a base class for visitors that navigate the builder objects.
    /// </summary>
    public class BuilderVisitor
    {
        /// <summary>
        /// Visits the given builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        public void Visit(IVisitableBuilder item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            item.Accept(this);
        }

        /// <summary>
        /// Visits an Addition builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitAddition(Addition item)
        {
        }

        /// <summary>
        /// Visits an AllColumns builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitAllColumns(AllColumns item)
        {
        }

        /// <summary>
        /// Visits a AliasedSource.
        /// </summary>
        /// <param name="aliasedSource">The item to visit.</param>
        protected internal virtual void VisitAliasedSource(AliasedSource aliasedSource)
        {

        }

        /// <summary>
        /// Visits a Batch builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitBatch(BatchBuilder item)
        {
        }

        /// <summary>
        /// Visits a BetweenFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitBetweenFilter(BetweenFilter item)
        {
        }

        /// <summary>
        /// Visits a BetweenWindowsFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitBetweenWindowFrame(BetweenWindowFrame item)
        {
        }

        /// <summary>
        /// Visits a Column builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitColumn(Column item)
        {
        }

        /// <summary>
        /// Visits a ConditionalCase builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitConditionalCase(ConditionalCase item)
        {
        }

        /// <summary>
        /// Visits a CrossJoin builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitCrossJoin(CrossJoin item)
        {
        }

        /// <summary>
        /// Visits a CurrentRowFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitCurrentRowFrame(CurrentRowFrame item)
        {
        }

        /// <summary>
        /// Visits a Delete builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitDelete(DeleteBuilder item)
        {
        }

        /// <summary>
        /// Visits a Division builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitDivision(Division item)
        {
        }

        /// <summary>
        /// Visits an EqualToFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitEqualToFilter(EqualToFilter item)
        {
        }

        /// <summary>
        /// Visits an EqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitEqualToQuantifierFilter(EqualToQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits an Except builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitExcept(Except item)
        {
        }

        /// <summary>
        /// Visits an ExistsFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitExistsFilter(ExistsFilter item)
        {
        }

        /// <summary>
        /// Visits a FilterGroup builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFilterGroup(FilterGroup item)
        {
        }

        /// <summary>
        /// Visits a FollowingBoundFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFollowingBoundFrame(FollowingBoundFrame item)
        {
        }

        /// <summary>
        /// Visits a FollowingUnboundFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFollowingUnboundFrame(FollowingUnboundFrame item)
        {
        }

        /// <summary>
        /// Visits a FullOuterJoin builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFullOuterJoin(FullOuterJoin item)
        {
        }

        /// <summary>
        /// Visits a Function builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFunction(Function item)
        {
        }

        /// <summary>
        /// Visits a FunctionWindow builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitFunctionWindow(FunctionWindow item)
        {
        }

        /// <summary>
        /// Visits a GreaterThanEqualToFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitGreaterThanEqualToFilter(GreaterThanEqualToFilter item)
        {
        }

        /// <summary>
        /// Visits a GreaterThanEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitGreaterThanEqualToQuantifierFilter(GreaterThanEqualToQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits a GreaterThanFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitGreaterThanFilter(GreaterThanFilter item)
        {
        }

        /// <summary>
        /// Visits a GreaterThanQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitGreaterThanQuantifierFilter(GreaterThanQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits an InFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitInFilter(InFilter item)
        {
        }

        /// <summary>
        /// Visits an InnerJoin builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitInnerJoin(InnerJoin item)
        {
        }

        /// <summary>
        /// Visits an Insert builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitInsert(InsertBuilder item)
        {
        }

        /// <summary>
        /// Visits an Intersect builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitIntersect(Intersect item)
        {
        }

        /// <summary>
        /// Visits a LeftOuterJoin builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLeftOuterJoin(LeftOuterJoin item)
        {
        }

        /// <summary>
        /// Visits a LessThanEqualToFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLessThanEqualToFilter(LessThanEqualToFilter item)
        {
        }

        /// <summary>
        /// Visits a LessThanEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLessThanEqualToQuantifierFilter(LessThanEqualToQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits a LessThanFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLessThanFilter(LessThanFilter item)
        {
        }

        /// <summary>
        /// Visits a LessThanQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLessThanQuantifierFilter(LessThanQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits a LikeFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitLikeFilter(LikeFilter item)
        {
        }

        /// <summary>
        /// Visits a MatchCase builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitMatchCase(MatchCase item)
        {
        }

        /// <summary>
        /// Visits a Minus builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitMinus(Minus item)
        {
        }

        /// <summary>
        /// Visits a Modulus builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitModulus(Modulus item)
        {
        }

        /// <summary>
        /// Visits a Multiplication builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitMultiplication(Multiplication item)
        {
        }

        /// <summary>
        /// Visits a Negation builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNegation(Negation item)
        {
        }

        /// <summary>
        /// Visits a NotEqualToFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNotEqualToFilter(NotEqualToFilter item)
        {
        }

        /// <summary>
        /// Visits a NotEqualToQuantifierFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNotEqualToQuantifierFilter(NotEqualToQuantifierFilter item)
        {
        }

        /// <summary>
        /// Visits a NotFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNotFilter(NotFilter item)
        {
        }

        /// <summary>
        /// Visits a NullFilter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNullFilter(NullFilter item)
        {
        }

        /// <summary>
        /// Visits a NullLiteral builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNullLiteral(NullLiteral item)
        {
        }

        /// <summary>
        /// Visits a NumericLiteral builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitNumericLiteral(NumericLiteral item)
        {
        }

        /// <summary>
        /// Visits a OrderBy builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitOrderBy(OrderBy item)
        {
        }

        /// <summary>
        /// Visits a Placeholder builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitPlaceholder(Placeholder item)
        {
        }

        /// <summary>
        /// Visits a PrecedingOnlyWindowFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitPrecedingOnlyWindowFrame(PrecedingOnlyWindowFrame item)
        {
        }

        /// <summary>
        /// Visits a PrecedingBoundFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitPrecedingBoundFrame(PrecedingBoundFrame item)
        {
        }

        /// <summary>
        /// Visits a PrecedingUnboundFrame builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitPrecedingUnboundFrame(PrecedingUnboundFrame item)
        {
        }

        /// <summary>
        /// Visits a RightOuterJoin builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitRightOuterJoin(RightOuterJoin item)
        {
        }

        /// <summary>
        /// Visits a SelectBuilder builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitSelect(SelectBuilder item)
        {
        }

        /// <summary>
        /// Visits a Setter builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitSetter(Setter item)
        {
        }

        /// <summary>
        /// Visits a StringLiteral builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitStringLiteral(StringLiteral item)
        {
        }
        /// <summary>
        /// Visits a ParameterLiteral builder
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitParameterLiteral(ParameterLiteral item)
        {

        }
        /// <summary>
        /// Visits a Subtraction builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitSubtraction(Subtraction item)
        {
        }

        /// <summary>
        /// Visits a Table builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitTable(Table item)
        {
        }

        /// <summary>
        /// Visits a Top builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitTop(Top item)
        {
        }

        /// <summary>
        /// Visits a Union builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitUnion(Union item)
        {
        }

        /// <summary>
        /// Visits a Update builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitUpdate(UpdateBuilder item)
        {
        }

        /// <summary>
        /// Visits a ValueList builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitValueList(ValueList item)
        {
        }

        /// <summary>
        /// Visits a Create builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitCreate(CreateBuilder item)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a Database builder.
        /// </summary>
        /// <param name="item">The item to visit.</param>
        protected internal virtual void VisitDatabase(CreateDatabase item)
        {
        }

        /// <summary>
        /// Visits a TableDefinition builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitCreateTableDefinition(CreateTableDefinition item)
        {
        }

        /// <summary>
        /// Visits a ColumnDefinition builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitColumnDefinition(ColumnDefinition item)
        {
        }

        /// <summary>
        /// Visits a DataType builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDataType(DataType item)
        {
        }

        /// <summary>
        /// Visits a AutoIncrement builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAutoIncrement(AutoIncrement item)
        {
        }

        /// <summary>
        /// Visits a DefaultConstraint builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDefaultConstraint(DefaultConstraint item)
        {
        }

        /// <summary>
        /// Visits a PrimaryKeyConstraint builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitPrimaryKeyConstraint(PrimaryKeyConstraint item)
        {

        }

        /// <summary>
        /// Visits a UniqueConstraint builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitUniqueConstraint(UniqueConstraint item)
        {
        }

        /// <summary>
        /// Visits a ForeignKeyConstraint builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitForeignKeyConstraint(ForeignKeyConstraint item)
        {
        }

        /// <summary>
        /// Visits a CascadeAction builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitForeignKeyCascadeAction(CascadeAction item)
        {
        }

        /// <summary>
        /// Visits a SetDefaultAction builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitForeignKeySetDefaultAction(SetDefaultAction item)
        {
        }

        /// <summary>
        /// Visits a SetNullAction builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitForeignKeySetNullAction(SetNullAction item)
        {
        }

        /// <summary>
        /// Visits a NoAction builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitForeignKeyNoAction(NoAction item)
        {
        }

        /// <summary>
        /// Visits a Alter builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAlter(AlterBuilder item)
        {
        }

        /// <summary>
        /// Visits a Alter database builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAlterDatabase(AlterDatabase item)
        {
        }

        /// <summary>
        /// Visits a Alter table builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAlterTableDefinition(AlterTableDefinition item)
        {
        }

        /// <summary>
        /// Visits a AlterColumn builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAlterColumn(AlterColumn item)
        {
        }

        /// <summary>
        /// Visits a AlterColumnProperty builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAlterColumnProperty(AlterColumnProperty item)
        {
        }
        /// <summary>
        /// Visits a RowGuidColumnProperty builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitRowGuidColumnProperty(RowGuidColumnProperty item)
        {

        }
        /// <summary>
        /// Visits a NotForReplicationColumnProperty builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitNotForReplicationColumnProperty(NotForReplicationColumnProperty item)
        {

        }
        /// <summary>
        /// Visits a SparseColumnProperty builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitSparseColumnProperty(SparseColumnProperty item)
        {

        }
        /// <summary>
        /// Visits a PersistedColumnProperty builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitPersistedColumnProperty(PersistedColumnProperty item)
        {

        }

        /// <summary>
        /// Visits a AddColumns builder.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitAddColumns(AddColumns item)
        {

        }
        /// <summary>
        /// Visits a DropColumn.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDropColumn(DropColumn item)
        {
        }
        /// <summary>
        /// Visits a DropConstraint.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDropConstraint(DropConstraint item)
        {
        }
        /// <summary>
        /// Visits a DropItemsList.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDropItemsList(DropItemsList item)
        {
        }

        /// <summary>
        /// Visits a DropColumnsList.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDropColumnsList(DropColumnsList item)
        {

        }

        /// <summary>
        /// Visits a DropConstraintsList.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void VisitDropConstraintsList(DropConstraintsList item)
        {
        }
    }
}
