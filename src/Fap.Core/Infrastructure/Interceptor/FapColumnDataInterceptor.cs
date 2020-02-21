using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac.Model;
using System;

namespace Fap.Core.Infrastructure.Interceptor
{
    public class FapColumnDataInterceptor : DataInterceptorBase
    {
        private readonly IDbMetadataContext _metadataContext;
        public FapColumnDataInterceptor(IServiceProvider provider, IDbContext dbContext, IDbMetadataContext metadataContext) : base(provider, dbContext)
        {
            _metadataContext = metadataContext;
        }
        private FapColumn ToFapColumn(FapDynamicObject fapDynamicData) =>
        new FapColumn
        {

            TableName = fapDynamicData.Get(nameof(FapColumn.TableName)).ToString(),
            ColName = fapDynamicData.Get(nameof(FapColumn.ColName)).ToString(),
            ColType = fapDynamicData.Get(nameof(FapColumn.ColType)).ToString(),
            ColComment = fapDynamicData.Get(nameof(FapColumn.ColComment)).ToString(),
            ColLength = fapDynamicData.Get(nameof(FapColumn.ColLength)).ToString().ToInt(),
            ColPrecision = fapDynamicData.Get(nameof(FapColumn.ColPrecision)).ToString().ToInt(),
            IsMultiLang = fapDynamicData.Get(nameof(FapColumn.IsMultiLang)).ToString().ToInt()

        };

        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            string tableName = fapDynamicData.Get(nameof(FapColumn.TableName)).ToString();
            var table = _dbContext.QueryFirstOrDefault<FapTable>("select * from FapTable where TableName=@TableName", new Dapper.DynamicParameters(new { TableName = tableName }));
            var fapColumn= ToFapColumn(fapDynamicData);
            if (table.IsSync == 1)
            {
                try
                {
                    _metadataContext.AddColumn(fapColumn);
                }
                catch (Exception)
                {
                    throw new FapException("物理表增加列失败！");
                }
            }
            //多语            
            string langkey = $"{fapColumn.TableName}_{fapColumn.ColName}";
            _dbContext.Insert(new FapMultiLanguage { Qualifier = MultiLanguageOriginEnum.FapColumn.ToString(), LangKey = langkey, LangValue = fapColumn.ColComment });
        }
        public override void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            FapColumn newColumn = ToFapColumn(fapDynamicData);
            string fid = fapDynamicData.Get(nameof(FapColumn.Fid)).ToString();
            FapColumn oriColumn = _dbContext.Get<FapColumn>(fid);
            var table = _dbContext.QueryFirstOrDefault<FapTable>("select * from FapTable where TableName=@TableName", new Dapper.DynamicParameters(new { TableName = oriColumn.TableName }));
            if (table.IsSync == 1)
            {
                try
                {
                    if (!newColumn.ColName.EqualsWithIgnoreCase(oriColumn.ColName))
                    {
                        //修改名称，同时alter type
                        _metadataContext.RenameColumn(newColumn, oriColumn.ColName);
                    }
                    else if (!newColumn.ColType.EqualsWithIgnoreCase(oriColumn.ColType) || newColumn.ColLength != oriColumn.ColLength || newColumn.ColPrecision != oriColumn.ColPrecision)
                    {
                        _metadataContext.AlterColumn(newColumn);
                    }
                    //先处理多语字段，之前是多语，现在也是多语
                    if (oriColumn.IsMultiLang == 1 && newColumn.IsMultiLang == 1)
                    {
                        if (!newColumn.ColName.EqualsWithIgnoreCase(oriColumn.ColName))
                        {
                            _metadataContext.RenameMultiLangColumn(newColumn, oriColumn.ColName);
                        }
                        else if (!newColumn.ColType.EqualsWithIgnoreCase(oriColumn.ColType) || newColumn.ColLength != oriColumn.ColLength || newColumn.ColPrecision != oriColumn.ColPrecision)
                        {
                            _metadataContext.AlterMultiLangColumn(newColumn);
                        }
                    }
                    else if (oriColumn.IsMultiLang == 0 && newColumn.IsMultiLang == 1)
                    {
                        //之前不是多语现在是多语
                        _metadataContext.AddMultiLangColumn(newColumn);
                    }
                    if (oriColumn.IsMultiLang == 1 && newColumn.IsMultiLang == 0)
                    {
                        //之前是多语现在不是
                        _metadataContext.DropMultiLangColumn(newColumn);
                    }

                }
                catch (Exception)
                {
                    throw new FapException("物理表修改列失败！");
                }
            }

            //更新多语
            if (!oriColumn.ColComment.EqualsWithIgnoreCase(newColumn.ColComment))
            {
                string langkey = $"{newColumn.TableName}_{newColumn.ColName}";
                string updateMultisql = $"Update {nameof(FapMultiLanguage)} set {nameof(FapMultiLanguage.LangValue)}=@LangValue where Qualifier=@Qualifier and LangKey=@LangKey";
                var param = new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.FapColumn.ToString(), LangKey = langkey, LangValue = newColumn.ColComment });
                _dbContext.Execute(updateMultisql, param);
            }
        }
        public override void BeforeDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            FapColumn column = _dbContext.Get<FapColumn>(fid);
            var table = _dbContext.QueryFirstOrDefault<FapTable>("select * from FapTable where TableName=@TableName", new Dapper.DynamicParameters(new { TableName = column.TableName }));
            if (table.IsSync == 1)
            {
                try
                {
                    _metadataContext.DropColumn(column);
                }
                catch (Exception)
                {
                    throw new FapException("物理表删除列失败！");
                }
            }
            string langkey = $"{column.TableName}_{column.ColName}";
            _dbContext.DeleteExec(nameof(FapMultiLanguage), "Qualifier=@Qualifier and LangKey=@LangKey", new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.FapColumn.ToString(), LangKey = langkey }));
        }

    }
}
