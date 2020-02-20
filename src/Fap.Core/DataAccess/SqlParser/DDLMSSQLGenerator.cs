using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.DataAccess.SqlParser
{
    class DDLMSSQLGenerator : IDDLSqlGenerator
    {
        public string CreateTableSql(FapTable table, IEnumerable<FapColumn> columns)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"CREATE TABLE [{table.TableName}](").AppendLine();

            foreach (var fapColumn in columns.OrderBy(c => c.ColOrder))
            {
                sqlBuilder.Append(CreateColumnSql(fapColumn)).Append(",").AppendLine();
                if (fapColumn.IsMultiLang == 1) //多语
                {
                    string fname = fapColumn.ColName;
                    string description = fapColumn.ColComment;
                    var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                    foreach (var item in languageList)
                    {
                        FapColumn mf = (FapColumn)fapColumn.Clone();
                        mf.ColName = fname + item.Value;
                        mf.ColComment = description + item.Description;
                        mf.IsMultiLang = 0;
                        sqlBuilder.Append(CreateColumnSql(mf)).Append(",").AppendLine();
                    }
                }
            }
            var pkField = columns.FirstOrDefault(f => f.ColType == FapColumn.COL_TYPE_PK);

            if (pkField != null)
            {
                sqlBuilder.Append("PRIMARY KEY (").Append(pkField.ColName).Append("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(");").Append(Environment.NewLine);
            //默认值
            //foreach (var field in columns)
            //{
            //    if (HasDefaultValueConstraint(field))
            //    {
            //        sqlBuilder.AppendLine(string.Format("ALTER TABLE {0} ADD CONSTRAINT DF_{0}_{1} DEFAULT ('{2}') FOR {1};", field.TableName, field.ColName, field.ColDefault));
            //    }
            //}
            //表注释
            MakeTableCommentSql(table, sqlBuilder);
            foreach (var field in columns.OrderBy(c => c.ColOrder))
            {
                //字段注释
                sqlBuilder.Append(MakeColumnCommentSql(field)).AppendLine();
                if (field.IsMultiLang == 1)
                {
                    var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                    foreach (var lang in languageList)
                    {
                        FapColumn column = (FapColumn)field.Clone();
                        column.ColName = field.ColName + lang.Value;
                        column.ColComment = field.ColComment + lang.Description;
                        //多余字段注释
                        sqlBuilder.Append(MakeColumnCommentSql(column)).AppendLine();
                    }
                }
            }
            return sqlBuilder.ToString();
            void MakeTableCommentSql(FapTable table, StringBuilder sqlBuilder)
            {
                if (table.TableComment.IsPresent())
                {
                    sqlBuilder.Append("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'");
                    sqlBuilder.Append(table.TableComment);
                    sqlBuilder.Append("' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'");
                    sqlBuilder.Append(table.TableName);
                    sqlBuilder.Append("';").Append(Environment.NewLine);
                }
            }

        }

        public string AddMultiLangColumnSql(FapColumn fapColumn)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var item in languageList)
            {
                FapColumn mf = (FapColumn)fapColumn.Clone();
                mf.ColName = fapColumn.ColName + item.Value;
                mf.ColComment = fapColumn.ColComment + item.Description;
                mf.IsMultiLang = 0;
                sqlBuilder.AppendLine($"alter table {mf.TableName} add {CreateColumnSql(mf)} ;");
                //if (HasDefaultValueConstraint(mf))
                //{
                //    sqlBuilder.AppendLine(string.Format("ALTER TABLE {0} ADD CONSTRAINT DF_{0}_{1} DEFAULT ('{2}') FOR {1};", mf.TableName, mf.ColName, mf.ColDefault));
                //}
                sqlBuilder.AppendLine(MakeColumnCommentSql(mf));
            }

            return sqlBuilder.ToString();
        }

        private string MakeColumnCommentSql(FapColumn fapColumn)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            if (fapColumn.ColComment.IsPresent())
            {
                sqlBuilder.Append("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'");
                sqlBuilder.Append(fapColumn.ColComment);
                sqlBuilder.Append("', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'");
                sqlBuilder.Append(fapColumn.TableName);
                sqlBuilder.Append("', @level2type=N'COLUMN',@level2name=N'");
                sqlBuilder.Append(fapColumn.ColName);
                sqlBuilder.Append("';").Append(Environment.NewLine);
            }
            return sqlBuilder.ToString();

        }

        /// <summary>
        /// 生成字段默认值 约束
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        //private bool HasDefaultValueConstraint(FapColumn field)
        //{
        //    if (field.ColType == FapColumn.COL_TYPE_INT || field.ColType == FapColumn.COL_TYPE_DOUBLE || field.ColType == FapColumn.COL_TYPE_LONG || field.ColType == FapColumn.COL_TYPE_BOOL)
        //    {
        //        if (field.ColDefault.IsMissing())
        //        {
        //            field.ColDefault = "0";
        //        }
        //        return true;                
        //    }
        //    return false;
        //}

        public string PhysicalTableColumnSql(string tableName)
        {
            return "select a.name as TableName,b.name as ColName,c.name as ColType,b.length as ColLength from sysobjects a,syscolumns b, systypes c where a.id = b.id and a.name ='"+tableName+"' and a.xtype = 'U' and b.xtype = c.xtype ";

        }

        public string CreateColumnSql(FapColumn fapColumn)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("[{0}] ", fapColumn.ColName);
            if (FapColumn.COL_TYPE_STRING.Equals(fapColumn.ColType)) //字符串字段需要处理多语情况
            {
                if (fapColumn.ColLength > 4000)
                {
                    sqlBuilder.Append(" VARCHAR(MAX)");
                }
                else
                {
                    sqlBuilder.Append(" VARCHAR(" + (fapColumn.ColLength > 0 ? fapColumn.ColLength : 32) + ")");
                }
            }
            else if (FapColumn.COL_TYPE_PK.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" INT NOT NULL IDENTITY(1,1)");
            }
            else if (FapColumn.COL_TYPE_INT.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" INT");
            }
            else if (FapColumn.COL_TYPE_LONG.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" BIGINT");
            }
            else if (FapColumn.COL_TYPE_DATETIME.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" VARCHAR(19)");
            }
            else if (FapColumn.COL_TYPE_DOUBLE.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" DECIMAL(20," + (fapColumn.ColPrecision > 0 ? fapColumn.ColPrecision : 1) + ")");
            }
            else if (FapColumn.COL_TYPE_BOOL.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" INT");
            }
            else if (FapColumn.COL_TYPE_UID.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" VARCHAR(20)");
            }
            else if (FapColumn.COL_TYPE_BLOB.Equals(fapColumn.ColType))
            {
                //sqlBuilder.Append(" BINARY");
                sqlBuilder.Append(" VARBINARY(MAX)");
            }
            else if (FapColumn.COL_TYPE_CLOB.Equals(fapColumn.ColType))
            {
                sqlBuilder.Append(" TEXT");
            }
            else
            {
                sqlBuilder.Append(" VARCHAR(" + (fapColumn.ColLength > 0 ? fapColumn.ColLength : 32) + ")");
            }
            return sqlBuilder.ToString();
        }

        public string AddColumnSql(FapColumn fapColumn)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"alter table {fapColumn.TableName} add {CreateColumnSql(fapColumn)} ;");
            //if (HasDefaultValueConstraint(fapColumn))
            //{
            //    stringBuilder.AppendLine(string.Format("ALTER TABLE {0} ADD CONSTRAINT DF_{0}_{1} DEFAULT ('{2}') FOR {1};", fapColumn.TableName, fapColumn.ColName, fapColumn.ColDefault));
            //}
            stringBuilder.AppendLine(MakeColumnCommentSql(fapColumn));
            if (fapColumn.IsMultiLang == 1) //多语
            {
                var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                foreach (var item in languageList)
                {
                    FapColumn mf = (FapColumn)fapColumn.Clone();
                    mf.ColName = fapColumn.ColName + item.Value;
                    mf.ColComment = fapColumn.ColComment + item.Description;
                    mf.IsMultiLang = 0;
                    stringBuilder.AppendLine($"alter table {mf.TableName} add {CreateColumnSql(mf)} ;");                   
                    stringBuilder.Append(MakeColumnCommentSql(mf)).AppendLine();
                }
            }
            return stringBuilder.ToString();

        }

        public string AlterColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            //if (HasDefaultValueConstraint(fapColumn))
            //{
            //    builder.AppendLine($"ALTER TABLE {fapColumn.TableName} DROP CONSTRAINT  DF_{fapColumn.TableName}_{fapColumn.ColName};");
            //}
            builder.AppendLine($"alter table {fapColumn.TableName} alter column {CreateColumnSql(fapColumn)};");
          
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
                builder.AppendLine($"alter table {fapColumn.TableName} alter column {CreateColumnSql(column)};");
            }
            return builder.ToString();
        }

        public string DropColumnSql(FapColumn fapColumn)
        {
            StringBuilder builder = new StringBuilder();
            //if (HasDefaultValueConstraint(fapColumn))
            //{
            //    builder.AppendLine($"ALTER TABLE {fapColumn.TableName} DROP CONSTRAINT  DF_{fapColumn.TableName}_{fapColumn.ColName};");
            //}
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
            builder.AppendLine($"if exists(select * from sysobjects where id = object_id(N'[{fapTable.TableName}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)");
            builder.AppendLine($"drop table {fapTable.TableName}");            
            return builder.ToString();
        }

        public string RenameColumnSql(FapColumn newColumn, string oldName)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"EXEC sp_rename '{newColumn.TableName}.{oldName}', '{newColumn.ColName}' , 'COLUMN';");
            builder.AppendLine(AlterColumnSql(newColumn));
            return builder.ToString();
        }
        public string RenameMultilangColumnSql(FapColumn newColumn, string oldName)
        {
            StringBuilder builder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var lang in languageList)
            {
                FapColumn column = (FapColumn)newColumn.Clone();
                column.ColName = newColumn.ColName + lang.Value;
                column.ColComment = newColumn.ColComment + lang.Description;
                column.IsMultiLang = 0;
                string oldLangName = oldName + lang.Value;
                builder.AppendLine($"EXEC sp_rename '{column.TableName}.{oldLangName}', '{column.ColName}' , 'COLUMN';");
                builder.AppendLine(AlterColumnSql(column));
            }
            return builder.ToString();
        }
        public string DropMultiLangColumnSql(FapColumn fapColumn)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            foreach (var lang in languageList)
            {
                string colName = fapColumn.ColName + lang.Value;
                sqlBuilder.AppendLine($"alter table {fapColumn.TableName} drop column {colName};");
            }

            return sqlBuilder.ToString();
        }

        public string InsertSql(IDictionary<string, object> data, IEnumerable<FapColumn> columns)
        {
            string tableName = columns.First().TableName;
            StringBuilder sql = new StringBuilder($"INSERT INTO [{tableName}](");
            StringBuilder sqlCol = new StringBuilder();
            StringBuilder sqlValue = new StringBuilder();
            foreach (var column in columns)
            {
                if (column.ColName.EqualsWithIgnoreCase("Id"))
                {
                    continue;
                }
                sqlCol.Append($"[{column.ColName}],");
                if (data[column.ColName] == null)
                {
                    sqlValue.Append("NULL,");
                }
                else
                {
                    if (column.ColType.EqualsWithIgnoreCase("varchar") || column.ColType.EqualsWithIgnoreCase("text"))
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
                .Append(sqlValue.ToString().TrimEnd(',')).Append(");");
            return sql.ToString();
        }
    }
}
