using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Fap.Core.Exceptions;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.MultiLanguage;

namespace Fap.Core.Infrastructure.Interceptor
{
    [Service]
    public class FapTableDataInterceptor : DataInterceptorBase
    {
        private readonly IDbMetadataContext _metadataContext;
        public FapTableDataInterceptor(IServiceProvider provider, IDbContext dbContext, IDbMetadataContext metadataContext) : base(provider, dbContext)
        {
            _metadataContext = metadataContext;
        }
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            string tableName = fapDynamicData.Get(nameof(FapTable.TableName)).ToString();
            //根据table特性生成默认列元数据
            string tbFeature = fapDynamicData.Get(nameof(FapTable.TableFeature)).ToString();
            string tableLabel = fapDynamicData.Get(nameof(FapTable.TableComment)).ToString();
            AddColumns(tableName, tbFeature, tableLabel);
        }

        private void AddColumns(string tableName, string tbFeature, string tableLabel)
        {
            var columns = GetDefaultColumns(tableName);
            if (tbFeature.IsPresent())
            {
                var features = tbFeature.SplitSemicolon();
                foreach (var feature in features)
                {
                    var featureColumns = GetColumnsByFeature(feature.Split(':')[0].Trim());
                    if (featureColumns.Any())
                    {
                        var fcs = featureColumns.ToList();
                        fcs.ForEach(c => c.TableName = tableName);
                        columns = columns.Union(fcs);
                    }
                }
            }
            //多语
            _dbContext.Insert(new FapMultiLanguage { Qualifier = MultiLanguageOriginEnum.FapTable.ToString(), LangKey = tableName, LangValue = tableLabel });
            _dbContext.InsertBatch(columns);
        }

        public override void AfterEntityInsert(object entity)
        {
            FapTable tb = entity as FapTable;
            AddColumns(tb.TableName, tb.TableFeature, tb.TableComment);
        }
        public override void BeforeDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            FapTable fapTable = _dbContext.Get<FapTable>(fid);
            if (fapTable.IsSync == 1)
            {
                throw new FapException("表已经建立，不能再删除！");
            }
            _dbContext.DeleteExec(nameof(FapMultiLanguage), "Qualifier=@Qualifier and LangKey=@LangKey", new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.FapTable.ToString(), LangKey = fapTable.TableName }));
            _dbContext.DeleteExec(nameof(FapTable), "TableName=@TableName", new Dapper.DynamicParameters(new { TableName = fapTable.TableName }));
            _dbContext.DeleteExec(nameof(FapColumn), "TableName=@TableName", new Dapper.DynamicParameters(new { TableName = fapTable.TableName }));


        }
        public override void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            FapTable fapTable = _dbContext.Get<FapTable>(fid);
            string tbFeature = fapDynamicData.Get(nameof(FapTable.TableFeature)).ToString();
            if (fapTable.TableFeature.IsPresent() && fapTable.TableFeature != tbFeature)
            {
                var features = fapTable.TableFeature.SplitSemicolon();
                foreach (var feature in features)
                {
                    var featureColumns = GetColumnsByFeature(feature.Split(':')[0].Trim());
                    if (featureColumns.Any())
                    {
                        var fcs = featureColumns.ToList();
                        var cols = fcs.Select(c => c.ColName);
                        if (fapTable.IsSync == 1)
                        {
                            var columns = _dbContext.Query<FapColumn>("select * from FapColumn where TableName=@TableName and ColName in @Cols", new Dapper.DynamicParameters(new { TableName = fapTable.TableName, Cols = cols }));
                            try
                            {
                                foreach (var column in columns)
                                {
                                    _metadataContext.DropColumn(column);
                                }
                            }
                            catch (Exception)
                            {
                                throw new FapException("物理表删除列出错！");
                            }
                        }

                        _dbContext.DeleteExec(nameof(FapColumn), "TableName=@TableName and ColName in @Cols"
                            , new Dapper.DynamicParameters(new { TableName = fapTable.TableName, Cols = cols }));
                    }
                }
            }
            //更新多语
            string tableLabel = fapDynamicData.Get(nameof(FapTable.TableComment)).ToString();
            if (!fapTable.TableComment.EqualsWithIgnoreCase(tableLabel))
            {
                string updateMultisql = $"Update {nameof(FapMultiLanguage)} set {nameof(FapMultiLanguage.LangValue)}=@LangValue where Qualifier=@Qualifier and LangKey=@LangKey";
                var param = new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.FapTable.ToString(), LangKey = fapTable.TableName, LangValue = tableLabel });
                _dbContext.Execute(updateMultisql, param);
            }
            
            if (tbFeature.IsPresent())
            {
                var features = tbFeature.SplitSemicolon();
                foreach (var feature in features)
                {
                    if (fapTable.TableFeature.IsPresent()&&fapTable.TableFeature.IndexOf(feature) > -1)
                    {
                        continue;
                    }
                    var featureColumns = GetColumnsByFeature(feature.Split(':')[0].Trim());
                    if (featureColumns.Any())
                    {
                        var fcs = featureColumns.ToList();
                        fcs.ForEach(c => c.TableName = fapTable.TableName);
                        if (fapTable.IsSync == 1)
                        {
                            foreach (var column in fcs)
                            {
                                _metadataContext.AddColumn(column);
                            }
                        }
                        _dbContext.InsertBatch(fcs);
                    }
                }
            }
        }
        private IEnumerable<FapColumn> GetColumnsByFeature(string featureCode)
        {
            var feature = _dbContext.QueryFirstOrDefault<FapTableFeature>("select Data from FapTableFeature where Code=@Code", new Dapper.DynamicParameters(new { Code = featureCode }));
            if (feature != null && feature.Data.IsPresent())
            {
                return JsonConvert.DeserializeObject<IEnumerable<FapColumn>>(feature.Data);
            }
            return Enumerable.Empty<FapColumn>();
        }
        private IEnumerable<FapColumn> GetDefaultColumns(string tableName)
        {
            // 默认字段
            FapColumn field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_Id;
            field.ColType = FapColumn.COL_TYPE_PK;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "主键";
            field.ShowAble = 0;
            field.ColProperty = "0";
            field.ColOrder = 0;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_INT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_Fid;
            field.ColType = FapColumn.COL_TYPE_UID;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "唯一标识";
            field.ColLength = 20;
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 1;
            field.DisplayWidth = 20;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_OrgUid;
            field.ColType = FapColumn.COL_TYPE_UID;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "组织";
            field.ColLength = 20;
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 980;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_GroupUid;
            field.ColType = FapColumn.COL_TYPE_UID;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "集团";
            field.ColLength = 20;
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 981;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_EnableDate;
            field.ColType = FapColumn.COL_TYPE_DATETIME;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "有效开始时间";
            field.ColLength = 20;
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 982;
            field.DisplayWidth = 19;
            field.CtrlType = FapColumn.CTRL_TYPE_DATETIME;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_DisableDate;
            field.ColType = FapColumn.COL_TYPE_DATETIME;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "有效截止时间";
            field.ColLength = 20;
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 983;
            field.DisplayWidth = 19;
            field.CtrlType = FapColumn.CTRL_TYPE_DATETIME;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_Dr;
            field.ColType = FapColumn.COL_TYPE_BOOL;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "删除标记";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 984;
            field.ColLength = 10;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_CHECKBOX;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_Ts;
            field.ColType = FapColumn.COL_TYPE_LONG;
            field.NullAble = 0;
            field.IsDefaultCol = 1;
            field.ColComment = "时间戳";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 985;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_CreateBy;
            field.ColType = FapColumn.COL_TYPE_STRING;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "创建人";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 986;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_CreateName;
            field.ColType = FapColumn.COL_TYPE_STRING;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "创建人名称";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 987;
            field.ColLength = 100;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_CreateDate;
            field.ColType = FapColumn.COL_TYPE_DATETIME;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "创建时间";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 988;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_DATETIME;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_UpdateBy;
            field.ColType = FapColumn.COL_TYPE_STRING;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "更新人";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 989;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_UpdateName;
            field.ColType = FapColumn.COL_TYPE_STRING;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "更新人名称";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 990;
            field.ColLength = 100;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            yield return field;

            field = new FapColumn();
            field.TableName = tableName;
            field.ColName = FapDbConstants.FAPCOLUMN_FIELD_UpdateDate;
            field.ColType = FapColumn.COL_TYPE_DATETIME;
            field.NullAble = 1;
            field.IsDefaultCol = 1;
            field.ColComment = "更新时间";
            field.ShowAble = 0;
            field.ColProperty = "1";
            field.ColOrder = 991;
            field.ColLength = 20;
            field.DisplayWidth = 10;
            field.CtrlType = FapColumn.CTRL_TYPE_DATETIME;
            yield return field;
        }

    }
}
