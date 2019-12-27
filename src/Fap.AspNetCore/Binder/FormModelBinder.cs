using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Serivce;
using Fap.Core.DataAccess;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.AspNetCore.Binder
{
    public class FormModelBinder : IModelBinder
    {
        private readonly IDbContext _dbContext;
        public FormModelBinder(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            FormModel formModel = new FormModel();
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            // Try to fetch the value of the argument by name
            var tableNameProviderResult = bindingContext.ValueProvider.GetValue(FapWebConstants.FORM_TABLENAME);

            if (tableNameProviderResult == ValueProviderResult.None)
            {
                tableNameProviderResult = bindingContext.ValueProvider.GetValue(FapWebConstants.QUERY_TABLENAME);
                if (tableNameProviderResult == ValueProviderResult.None)
                {
                    return Task.CompletedTask;
                }
            }
            var operProviderResult = bindingContext.ValueProvider.GetValue(FapWebConstants.OPERATOR);
            if (operProviderResult == null)
            {
                return Task.CompletedTask;
            }
            var avoidDuplicateKey = bindingContext.ValueProvider.GetValue(FapWebConstants.AVOID_REPEAT_TOKEN);
            if (avoidDuplicateKey == null)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue("TableName", tableNameProviderResult);

            var tableName = tableNameProviderResult.FirstValue;
            // Check if the argument value is null or empty
            if (tableName.IsMissing())
            {
                return Task.CompletedTask;
            }
            formModel.TableName = tableName;
            formModel.Oper = (OperEnum)Enum.Parse(typeof(OperEnum), operProviderResult.FirstValue);
            formModel.AvoidDuplicateKey = avoidDuplicateKey.FirstValue;
            formModel.Ids = bindingContext.ValueProvider.GetValue(FapWebConstants.IDS).FirstValue;
            IEnumerable<IValueProvider> valueProviders = bindingContext.ValueProvider as IEnumerable<IValueProvider>;

            var formValueProviders = valueProviders.OfType<FormValueProvider>();
            if (formValueProviders == null)
            {
                return Task.CompletedTask;
            }
            var (main, child) = BuilderData(tableName, formValueProviders.First());
            formModel.MainData = main;
            formModel.ChildDataList = child;
            bindingContext.Result = ModelBindingResult.Success(formModel);
            return Task.CompletedTask;
        }
        private (FapDynamicObject mainData, Dictionary<string, IEnumerable<FapDynamicObject>> childsData) BuilderData(string tableName, FormValueProvider formValueProviders)
        {
            var columnList = _dbContext.Columns(tableName);
            var mainDic = formValueProviders.GetKeysFromPrefix("mainData");
            FapDynamicObject mainObject = new FapDynamicObject(columnList);
            foreach (var cell in mainDic)
            {
                mainObject.SetValue(cell.Key, formValueProviders.GetValue(cell.Value));
            }
            var childDicList = formValueProviders.GetKeysFromPrefix("childDataList");
            Dictionary<string, IEnumerable<FapDynamicObject>> childDataDic = null;

            childDataDic = new Dictionary<string, IEnumerable<FapDynamicObject>>();
            foreach (var row in childDicList)
            {
                IList<FapDynamicObject> list = new List<FapDynamicObject>();
                var tables = formValueProviders.GetKeysFromPrefix(row.Value);

                string tn = formValueProviders.GetValue(tables["tn"]).FirstValue;
                var dicData = formValueProviders.GetKeysFromPrefix(tables["data"]);
                foreach (var data in dicData)
                {
                    FapDynamicObject cData = new FapDynamicObject(_dbContext.Columns(tn));
                    foreach (var dataDic in formValueProviders.GetKeysFromPrefix(data.Value))
                    {
                        cData.SetValue(dataDic.Key, formValueProviders.GetValue(dataDic.Value));
                    }
                    list.Add(cData);
                }
                childDataDic.TryAdd(tn, list);

            }
            return (mainObject, childDataDic);
        }
    }
}
