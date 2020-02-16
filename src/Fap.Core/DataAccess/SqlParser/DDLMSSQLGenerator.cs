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
            sqlBuilder.Append($"CREATE TABLE {table.TableName}(").AppendLine();

            foreach (var column in columns.OrderBy(c => c.ColOrder))
            {
                MakeFieldTypeSql(column, sqlBuilder);
            }
            var pkField = columns.FirstOrDefault(f => f.ColType == FapColumn.COL_TYPE_PK);

            if (pkField != null)
            {
                sqlBuilder.Append("PRIMARY KEY (").Append(pkField.ColName).Append("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(");").Append(Environment.NewLine);
            //默认值
            foreach (var field in columns)
            {
                string tempSql = MakeFieldDefaultValueSql(field);
                if (tempSql != "")
                {
                    sqlBuilder.AppendLine(tempSql);
                }
            }
            //表注释
            MakeTableCommentSql(table, sqlBuilder);
            //字段注释
            foreach (var field in columns.OrderBy(c => c.ColOrder))
            {
                if (field.ColComment.IsPresent())
                {
                    sqlBuilder.Append("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'");
                    sqlBuilder.Append(field.ColComment);
                    sqlBuilder.Append("', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'");
                    sqlBuilder.Append(field.TableName);
                    sqlBuilder.Append("', @level2type=N'COLUMN',@level2name=N'");
                    sqlBuilder.Append(field.ColName);
                    sqlBuilder.Append("';").Append(Environment.NewLine);
                    if (field.IsMultiLang == 1)
                    {
                        var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                        foreach (var item in languageList)
                        {
                            sqlBuilder.Append("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'");
                            sqlBuilder.Append(field.ColComment + item.Description);
                            sqlBuilder.Append("', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'");
                            sqlBuilder.Append(field.TableName);
                            sqlBuilder.Append("', @level2type=N'COLUMN',@level2name=N'");
                            sqlBuilder.Append(field.ColName + item.Value);
                            sqlBuilder.Append("';").Append(Environment.NewLine);
                        }
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
            /// <summary>
            /// 生成createTable中每个字段的SQL
            /// </summary>
            /// <param name="field"></param>
            /// <param name="sqlBuilder"></param>
            void MakeFieldTypeSql(FapColumn field, StringBuilder sqlBuilder)
            {
                if (field == null)
                {
                    return;
                }
                if (FapColumn.COL_TYPE_STRING.Equals(field.ColType)) //字符串字段需要处理多语情况
                {
                    if (field.ColLength > 4000)
                    {
                        sqlBuilder.Append(field.ColName).Append(" VARCHAR(MAX),");
                    }
                    else
                    {
                        sqlBuilder.Append(field.ColName).Append(" VARCHAR(" + (field.ColLength > 0 ? field.ColLength : 32) + "),");
                    }
                }
                else if (FapColumn.COL_TYPE_PK.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" INT NOT NULL IDENTITY(1,1), ");
                }
                else if (FapColumn.COL_TYPE_INT.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" INT, ");
                }
                else if (FapColumn.COL_TYPE_LONG.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" BIGINT, ");
                }
                else if (FapColumn.COL_TYPE_DATETIME.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" VARCHAR(19),");
                }
                else if (FapColumn.COL_TYPE_DOUBLE.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" DECIMAL(20, " + (field.ColPrecision > 0 ? field.ColPrecision : 1) + "),");
                }
                else if (FapColumn.COL_TYPE_BOOL.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" INT,");
                }
                else if (FapColumn.COL_TYPE_UID.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" VARCHAR(20),");
                }
                else if (FapColumn.COL_TYPE_BLOB.Equals(field.ColType))
                {
                    //sqlBuilder.Append(" BINARY");
                    sqlBuilder.Append(" VARBINARY(MAX),");
                }
                else if (FapColumn.COL_TYPE_CLOB.Equals(field.ColType))
                {
                    sqlBuilder.Append(field.ColName).Append(" TEXT,");
                }
                else
                {
                    sqlBuilder.Append(field.ColName).Append(" VARCHAR(" + (field.ColLength > 0 ? field.ColLength : 32) + "),");
                }
                sqlBuilder.AppendLine();
                if (field.IsMultiLang == 1) //多语
                {
                    string fname = field.ColName;
                    string description = field.ColComment;
                    var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
                    foreach (var item in languageList)
                    {
                        FapColumn mf= (FapColumn)field.Clone();
                        mf.ColName = fname + item.Value;
                        mf.ColComment = description+item.Description;
                        mf.IsMultiLang = 0;
                        MakeFieldTypeSql(mf, sqlBuilder);
                    }

                }
            }
            /// <summary>
            /// 生成字段默认值
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            string MakeFieldDefaultValueSql(FapColumn field)
            {
                if (field.ColType == FapColumn.COL_TYPE_INT || field.ColType == FapColumn.COL_TYPE_DOUBLE || field.ColType == FapColumn.COL_TYPE_LONG || field.ColType == FapColumn.COL_TYPE_BOOL)
                {
                    if (field.ColDefault.IsMissing())
                    {
                        field.ColDefault = "0";
                    }
                    return string.Format("ALTER TABLE {0} ADD CONSTRAINT DF_{0}_{1} DEFAULT ('{2}') FOR {1};", field.TableName, field.ColName, field.ColDefault);
                }
                return "";
            }
        }

        public string GetPhysicalTableColumnSql()
        {
           return "select a.name as TableName,b.name as ColumnName,c.name as ColumnType,b.length as ColumnLength from sysobjects a,syscolumns b, systypes c where a.id = b.id and a.name =@TableName and a.xtype = 'U' and b.xtype = c.xtype ";
            
        }
    }
}
