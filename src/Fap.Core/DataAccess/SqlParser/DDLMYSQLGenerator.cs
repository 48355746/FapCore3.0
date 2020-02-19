using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Fap.Core.Extensions;

namespace Fap.Core.DataAccess.SqlParser
{
    class DDLMYSQLGenerator : IDDLSqlGenerator
    {
        public string AddColumnSql(FapColumn fapColumn)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"alter table {fapColumn.TableName} add column {CreateColumnSql(fapColumn)};");
            if (fapColumn.IsMultiLang == 1) //多语
            {
                var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                foreach (var lang in languageList)
                {
                    FapColumn column = (FapColumn)fapColumn.Clone();
                    column.ColName = fapColumn.ColName + lang.Value;
                    column.ColComment = fapColumn.ColComment + lang.Description;
                    column.IsMultiLang = 0;
                    stringBuilder.AppendLine($"alter table {column.TableName} add column {CreateColumnSql(column)}");
                }
            }
            return stringBuilder.ToString();
        }

        public string AlterColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"alter table {fapColumn.TableName} modify  column {CreateColumnSql(fapColumn)};");
         
            return builder.ToString();
        }
        public string AlterMultiLangColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var lang in languageList)
            {
                FapColumn column = (FapColumn)fapColumn.Clone();
                column.ColName = fapColumn.ColName + lang.Value;
                column.ColComment = fapColumn.ColComment + lang.Description;
                column.IsMultiLang = 0;
                builder.AppendLine($"alter table {fapColumn.TableName} alter column {CreateColumnSql(column)}");
            }
            return builder.ToString();
        }
        public string CreateColumnSql(FapColumn fapColumn)
        {
            StringBuilder sql = new StringBuilder();
            string colName = fapColumn.ColName;
            sql.AppendFormat("`{0}` ", colName);

            if (FapColumn.COL_TYPE_STRING.Equals(fapColumn.ColType)) //字符串字段需要处理多语情况
            {
                if (fapColumn.ColLength > 4000)
                {
                    sql.Append(" TEXT ");
                }
                else
                {
                    sql.Append(" VARCHAR(" + (fapColumn.ColLength > 0 ? fapColumn.ColLength : 32) + ") ");
                }
            }
            else if (FapColumn.COL_TYPE_PK.Equals(fapColumn.ColType))
            {
                sql.Append(" INT(5)  AUTO_INCREMENT NOT NULL ");
            }
            else if (FapColumn.COL_TYPE_INT.Equals(fapColumn.ColType))
            {
                sql.Append(" INT(5) NULL default 0 ");
            }
            else if (FapColumn.COL_TYPE_BOOL.Equals(fapColumn.ColType))
            {
                sql.Append(" TinyInt(5) NULL default 0 ");
            }
            else if (FapColumn.COL_TYPE_LONG.Equals(fapColumn.ColType))
            {
                sql.Append(" BIGINT(8) NULL default 0 ");
            }
            else if (FapColumn.COL_TYPE_DATETIME.Equals(fapColumn.ColType))
            {
                sql.Append(" VARCHAR(19) NULL ");
            }
            else if (FapColumn.COL_TYPE_DOUBLE.Equals(fapColumn.ColType))
            {
                sql.Append(" DECIMAL(20, " + (fapColumn.ColPrecision > 0 ? fapColumn.ColPrecision : 1) + ")  NULL  default 0.0 ");
            }
            else if (FapColumn.COL_TYPE_UID.Equals(fapColumn.ColType))
            {
                sql.Append(" VARCHAR(20) NULL ");
            }
            else if (FapColumn.COL_TYPE_BLOB.Equals(fapColumn.ColType))
            {
                //sql.Append(" BINARY");
                //sql.Append(" BLOB ");
                sql.Append(" BLOB ");
            }
            else if (FapColumn.COL_TYPE_CLOB.Equals(fapColumn.ColType))
            {
                sql.Append(" TEXT ");
            }
            else
            {
                sql.Append(" VARCHAR(" + (fapColumn.ColLength > 0 ? fapColumn.ColLength : 32) + ") ");
            }
            sql.AppendFormat(" comment '{0}' ", fapColumn.ColComment);
            return sql.ToString();
        }

        public string AddMultiLangColumnSql(FapColumn fapColumn)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var lang in languageList)
            {
                FapColumn column = (FapColumn)fapColumn.Clone();
                column.ColName = fapColumn.ColName + lang.Value;
                column.ColComment = fapColumn.ColComment + lang.Description;
                column.IsMultiLang = 0;
                sqlBuilder.Append($"alter table {column.TableName} add column {CreateColumnSql(column)};");
            }
            return sqlBuilder.ToString();
        }

        public string CreateTableSql(FapTable table, IEnumerable<FapColumn> columns)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("-- 创建表");
            sqlBuilder.AppendLine($"CREATE TABLE `{table.TableName}`( ");
            foreach (var fapColumn in columns.OrderBy(c => c.ColOrder))
            {
                sqlBuilder.Append(CreateColumnSql(fapColumn)).Append(",").AppendLine();
                if (fapColumn.IsMultiLang == 1) //多语
                {
                    string fname = fapColumn.ColName;
                    string description = fapColumn.ColComment;
                    var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                    foreach (var lang in languageList)
                    {
                        fapColumn.ColName = fname + lang.Value;
                        fapColumn.ColComment = description + lang.Description;
                        fapColumn.IsMultiLang = 0;
                        sqlBuilder.Append(CreateColumnSql(fapColumn)).Append(",").AppendLine(); 
                    }
                }
            }
            var pkField = columns.FirstOrDefault(f => f.ColType == FapColumn.COL_TYPE_PK);

            if (pkField != null)
            {
                sqlBuilder.Append("PRIMARY KEY (").Append(pkField.ColName).Append("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(")comment='{0}' ENGINE = InnoDB DEFAULT CHARSET = utf8mb4", table.TableComment).Append(Environment.NewLine);

            sqlBuilder.AppendLine();

            return sqlBuilder.ToString();
        }

        public string DropColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"alter table {fapColumn.TableName} drop column {fapColumn.ColName};");
            if (fapColumn.IsMultiLang == 1) //多语
            {
                var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                foreach (var lang in languageList)
                {
                    string colName = fapColumn.ColName + lang.Value;
                    builder.AppendLine($"alter table {fapColumn.TableName} drop column {colName};");
                }
            }
            return builder.ToString();
        }

        public string DropTableSql(FapTable fapTable)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-- 删除表");
            builder.AppendLine($"drop table if exists  {fapTable.TableName}");
            return builder.ToString();
        }

        public string PhysicalTableColumnSql(string tableName)
        {
            return "select  Table_name as TableName,COLUMN_NAME as ColName,DATA_TYPE as ColType,CHARACTER_MAXIMUM_LENGTH as ColLength  from Information_schema.columns  where table_Name ='" + tableName + "'";
        }

        public string RenameColumnSql(FapColumn newColumn, string oldName)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"alter table {newColumn.TableName} change  column {oldName} {CreateColumnSql(newColumn)};");
            if (newColumn.IsMultiLang == 1) //多语
            {
                var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                foreach (var lang in languageList)
                {
                    FapColumn column = (FapColumn)newColumn.Clone();
                    column.ColName = newColumn.ColName + lang.Value;
                    column.ColComment = newColumn.ColComment + lang.Description;
                    column.IsMultiLang = 0;
                    string oldLangName = oldName + lang.Value;
                    builder.AppendLine($"alter table {newColumn.TableName} change  column {oldLangName} {CreateColumnSql(column)};");
                }
            }
            return builder.ToString();
        }

        public string DropMultiLangColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var lang in languageList)
            {
                string colName = fapColumn.ColName + lang.Value;
                builder.AppendLine($"alter table {fapColumn.TableName} drop column {colName};");
            }
            return builder.ToString();
        }

        public string InsertSql(IDictionary<string, object> data, IEnumerable<FapColumn> columns)
        {
            string tableName = columns.First().TableName;
            StringBuilder sql = new StringBuilder($"INSERT INTO `{tableName}`(");
            StringBuilder sqlCol = new StringBuilder();
            StringBuilder sqlValue = new StringBuilder();
            foreach (var column in columns)
            {
                if (column.ColName.EqualsWithIgnoreCase("Id"))
                {
                    continue;
                }
                sqlCol.Append($"`{column.ColName}`,");
                if (data[column.ColName] == null)
                {
                    sqlValue.Append("NULL,");
                }
                else
                {
                    if (column.ColType.EqualsWithIgnoreCase("varchar")||column.ColType.EqualsWithIgnoreCase("text"))
                    {
                        sqlValue.Append($"'{data[column.ColName].ToString().Replace("'","''")}',");
                    }
                    else
                    {
                        sqlValue.Append($"{data[column.ColName]},");
                    }
                }
            }
            sql.Append(sqlCol.ToString().TrimEnd(',')).Append(") VALUES(")
                .Append(sqlValue.ToString().TrimEnd(',')).Append(")");
            return sql.ToString();
        }
    }
}
