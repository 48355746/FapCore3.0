using Fap.Core.DataAccess.SqlParser;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using Dapper;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Extensions;

namespace Fap.Core.DataAccess
{
    [Service]
    public class DbMetadataContext : IDbMetadataContext
    {
        private IDbSession _dbSession;
        public DbMetadataContext(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }
        private IDDLSqlGenerator GetSqlGenerator()
        {
            return _dbSession.DatabaseDialect switch
            {
                DatabaseDialectEnum.MSSQL => new DDLMSSQLGenerator(),
                DatabaseDialectEnum.MYSQL => new DDLMYSQLGenerator(),
                _ => throw new NotImplementedException()
            };
        }
        public void CreateTable(int id)
        {
            _dbSession.TransactionProxy((connection, transaction) =>
            {
                FapTable table = connection.QueryFirstOrDefault<FapTable>("select * from FapTable where Id=@Id", new { Id = id },transaction);
                if (table != null)
                {
                    var columns = connection.Query<FapColumn>("select * from FapColumn where TableName=@TableName", new { TableName = table.TableName },transaction);
                    var sql = GetSqlGenerator().CreateTableSql(table, columns);
                    connection.Execute(sql, null, transaction);
                    connection.Execute("Update FapTable set IsSync=1 where Id=@Id",new { Id=id}, transaction);
                }
            });
        }
        public void AlterTable(int id)
        {
            //_dbSession.TransactionProxy((connection, transaction) =>
            //{
            //    FapTable table = connection.QueryFirstOrDefault<FapTable>("select * from FapTable where Id=@Id", new { Id = id }, transaction);
            //    if (table != null)
            //    {
            //        var columnList_metadata = connection.Query<FapColumn>("select * from FapColumn where TableName=@TableName", new { TableName = table.TableName }, transaction);
            //        var sql = GetSqlGenerator().GetPhysicalTableColumnSql();
            //        var columnList_database = connection.Query<FapColumn>(sql, new { TableName = table.TableName },transaction);



            //        List<string> columnList_nohave = new List<string>(); //没有创建过的字段
            //        List<string> columnList_deleted = new List<string>(); //要删除的字段
            //        List<Tuple<string, string>> columnList_changed = new List<Tuple<string, string>>(); //有变化的字段

            //        List<string> columnNameList_database = new List<string>();
            //        List<string> columnNameList_metadata = new List<string>();
            //        List<Tuple<string, bool>> columnNameList_metadata_with_mutliLang = new List<Tuple<string, bool>>();
            //        foreach (var column in columnList_metadata)
            //        {
            //            columnNameList_metadata.Add(column.ColName.Trim());
            //            columnNameList_metadata_with_mutliLang.Add(new Tuple<string, bool>(column.ColName, column.IsMultiLang == 1));
            //        }
            //        var languageList = typeof(MultiLanguage.MultiLanguageEnum).EnumItems();
            //        foreach (dynamic column in columnList_database)
            //        {
            //            if (!MultiLangHelper.IsMultiLanguageField(column.ColumnName)) //排除多语字段
            //            {
            //                columnNameList_database.Add(column.ColumnName);
            //            }
            //        }
            //        //没有创建过的字段
            //        columnList_nohave = columnNameList_metadata.Except(columnNameList_database).ToList<string>();
            //        //要删除的字段
            //        columnList_deleted = columnNameList_database.Except(columnNameList_metadata).ToList<string>();
            //        //有变化的字段
            //        foreach (var column in columnList_metadata)
            //        {
            //            dynamic columnInfoInDB = this.EntityDao.GetColumnInDB(table.TableName, column.ColName);
            //            if (columnInfoInDB != null)
            //            {
            //                if (!this.EntityDao.MatchColumnType(column.ColType, columnInfoInDB.ColumnType))
            //                {
            //                    columnList_changed.Add(new Tuple<string, string>(column.ColName, "type_change")); //类型变化
            //                }

            //                if (column.ColType == FapColumn.COL_TYPE_STRING && column.ColLength != columnInfoInDB.ColumnLength) //字段长度目前只针对STRING类型
            //                {
            //                    columnList_changed.Add(new Tuple<string, string>(column.ColName, "length_change")); //长度变化
            //                }

            //                bool isMultiLangColumnInDB = this.EntityDao.IsMultiLangColumnInDB(column.TableName, column.ColName);
            //                if (column.IsMultiLang == 1 && !isMultiLangColumnInDB)
            //                {
            //                    columnList_changed.Add(new Tuple<string, string>(column.ColName, "ml_necessary")); //多语字段缺失，要新增
            //                }
            //                else if (column.IsMultiLang == 0 && isMultiLangColumnInDB)
            //                {
            //                    columnList_changed.Add(new Tuple<string, string>(column.ColName, "ml_unnecessary")); //多语字段多余，要删除
            //                }
            //            }
            //        }

            //        //表元数据的Map， 用于方便查找
            //        Dictionary<string, FapColumn> columnList_metadata_map = new Dictionary<string, FapColumn>();
            //        foreach (var column in columnList_metadata)
            //        {
            //            columnList_metadata_map.Add(column.ColName, column);
            //        }
            //        //新增字段
            //        foreach (var columnName_nohave in columnList_nohave)
            //        {
            //            bool result = this.TableDao.AddColumnInDB(columnList_metadata_map[columnName_nohave], tableName);
            //            if (!result)
            //            {
            //                msgBuilder.Append("字段[" + columnName_nohave + "]新增失败" + Environment.NewLine);
            //            }
            //        }
            //        //删除字段
            //        foreach (var columnName_deleted in columnList_deleted)
            //        {
            //            this.TableDao.DeleteMultiLangColumnConstraintInDB(columnName_deleted, tableName);
            //            bool result = this.TableDao.DeleteColumnInDB(columnName_deleted, tableName);
            //            if (!result)
            //            {
            //                msgBuilder.Append("字段[" + columnName_deleted + "]删除失败" + Environment.NewLine);
            //            }
            //        }
            //        //更新字段
            //        foreach (var columnName_changed in columnList_changed)
            //        {
            //            if (columnName_changed.Item2 == "type_change" || columnName_changed.Item2 == "length_change")
            //            {
            //                bool result = this.TableDao.AlterColumnInDB(columnList_metadata_map[columnName_changed.Item1], tableName);
            //                if (!result)
            //                {
            //                    msgBuilder.Append("字段[" + columnName_changed + "]更新失败" + Environment.NewLine);
            //                }
            //            }
            //            else if (columnName_changed.Item2 == "ml_necessary")
            //            {
            //                bool result = this.TableDao.AddMultiLangColumnInDB(columnList_metadata_map[columnName_changed.Item1], tableName);
            //                if (!result)
            //                {
            //                    msgBuilder.Append("多语字段[" + columnName_changed + "]新增失败" + Environment.NewLine);
            //                }
            //            }
            //            else if (columnName_changed.Item2 == "ml_unnecessary")
            //            {
            //                bool result = this.TableDao.DeleteMultiLangColumnInDB(columnList_metadata_map[columnName_changed.Item1]);
            //                if (!result)
            //                {
            //                    msgBuilder.Append("多语字段[" + columnName_changed + "]删除失败" + Environment.NewLine);
            //                }
            //            }

            //        }
            //    }
            //});
        }
    }
}
