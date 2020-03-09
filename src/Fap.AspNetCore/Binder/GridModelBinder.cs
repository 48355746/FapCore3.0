using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Binder
{
    public class GridModelBinder : IModelBinder
    {
        private readonly IDbContext _dbContext;
        public GridModelBinder(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Try to fetch the value of the argument by name
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue("tableName");

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            IEnumerable<IValueProvider> valueProviders = bindingContext.ValueProvider as IEnumerable<IValueProvider>;

            var formValueProviders = valueProviders.OfType<FormValueProvider>();
            if (formValueProviders == null)
            {
                return Task.CompletedTask;
            }
            bindingContext.ModelState.SetModelValue("TableName",
                valueProviderResult);

            var value = valueProviderResult.FirstValue;
            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            GridModel model = new GridModel();
            model.TableName = value;
            IEnumerable<FapDynamicObject> RowList = new List<FapDynamicObject>();
            var formValueProvider = formValueProviders.First();
            model.Rows = GetRows(formValueProvider, _dbContext.Columns(value));
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        private IEnumerable<FapDynamicObject> GetRows(FormValueProvider formValueProvider, IEnumerable<FapColumn> columns)
        {
            IDictionary<string, string> rowsDic = formValueProvider.GetKeysFromPrefix("rows");
            foreach (var row in rowsDic)
            {
                FapDynamicObject keyValues = new FapDynamicObject(columns);
                foreach (var cell in formValueProvider.GetKeysFromPrefix(row.Value))
                {
                    keyValues.SetValue(cell.Key, formValueProvider.GetValue(cell.Value));
                }
                yield return keyValues;
            }
        }
    }
}
