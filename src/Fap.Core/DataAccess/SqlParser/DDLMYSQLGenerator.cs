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
        public string CreateTableSql(FapTable table, IEnumerable<FapColumn> columns)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("CREATE TABLE ").Append(table.TableName).Append("(").AppendLine();
            foreach (var field in columns.OrderBy(c => c.ColOrder))
            {
                MakeFieldTypeSql(field, sqlBuilder);               
            }
            var pkField = columns.FirstOrDefault(f => f.ColType == FapColumn.COL_TYPE_PK);

            if (pkField != null)
            {
                sqlBuilder.Append("PRIMARY KEY (").Append(pkField.ColName).Append("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(")comment='{0}' ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;", table.TableComment).Append(Environment.NewLine);

            sqlBuilder.AppendLine("GO").AppendLine();

            return sqlBuilder.ToString();
        }

        public string GetPhysicalTableColumnSql()
        {
            return "select  Table_name as TableName,COLUMN_NAME as ColumnName,DATA_TYPE as ColumnType,CHARACTER_MAXIMUM_LENGTH as ColumnLength  from Information_schema.columns  where table_Name =@TableName";
        }

        /// <summary>
        /// 生成createTable中每个字段的SQL
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sqlBuilder"></param>
        private void MakeFieldTypeSql(FapColumn field, StringBuilder sql)
        {
            if (field == null)
            {
                return;
            }
            string colName = field.ColName;
            sql.AppendFormat("`{0}` ", colName);

            if (FapColumn.COL_TYPE_STRING.Equals(field.ColType)) //字符串字段需要处理多语情况
            {
                if (field.ColLength > 4000)
                {
                    sql.Append(" TEXT ");
                }
                else
                {
                    sql.Append(" VARCHAR(" + (field.ColLength > 0 ? field.ColLength : 32) + ") ");
                }
            }
            else if (FapColumn.COL_TYPE_PK.Equals(field.ColType))
            {
                sql.Append(" INT(5) NOT NULL AUTO_INCREMENT ");
            }
            else if (FapColumn.COL_TYPE_INT.Equals(field.ColType))
            {
                sql.Append(" INT(5) NULL default 0");
            }
            else if(FapColumn.COL_TYPE_BOOL.Equals(field.ColType))
            {
                sql.Append(" TinyInt(5) NULL default 0");                
            }
            else if (FapColumn.COL_TYPE_LONG.Equals(field.ColType))
            {
                sql.Append(" BIGINT(8) NULL default 0");
            }
            else if (FapColumn.COL_TYPE_DATETIME.Equals(field.ColType))
            {
                sql.Append(" VARCHAR(19) NULL");
            }
            else if (FapColumn.COL_TYPE_DOUBLE.Equals(field.ColType))
            {
                sql.Append(" DECIMAL(20, " + (field.ColPrecision > 0 ? field.ColPrecision : 1) + ")  NULL  default 0.0");
            }
            else if (FapColumn.COL_TYPE_UID.Equals(field.ColType))
            {
                sql.Append(" VARCHAR(20) NULL");
            }
            else if (FapColumn.COL_TYPE_BLOB.Equals(field.ColType))
            {
                //sql.Append(" BINARY");
                //sql.Append(" BLOB ");
                sql.Append(" BLOB");
            }
            else if (FapColumn.COL_TYPE_CLOB.Equals(field.ColType))
            {
                sql.Append(" TEXT");
            }
            else
            {
                sql.Append(" VARCHAR(" + (field.ColLength > 0 ? field.ColLength : 32) + ") ");
            }
            sql.AppendFormat(" comment '{0}',", field.ColComment);
            sql.AppendLine();
            if (field.IsMultiLang == 1) //多语
            {
                string fname = field.ColName;
                string description = field.ColComment;
                var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                foreach (var lang in languageList)
                {
                    field.ColName= fname+ lang.Value;
                    field.ColComment=description+ lang.Description;
                    field.IsMultiLang = 0;
                    MakeFieldTypeSql(field, sql);
                }
            }
        }
    }
}
