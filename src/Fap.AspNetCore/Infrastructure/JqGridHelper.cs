using Fap.Core.Extensions;
using Fap.AspNetCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Fap.AspNetCore.Model;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DataAccess;

namespace Fap.AspNetCore.Infrastructure
{
    public class JqGridHelper
    {
        /// <summary>
        /// 构造条件描述
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<JqGridFilterDescViewModel> BuilderFilterDesc(string tableName, string filter,IDbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }
            JsonFilterToSql jfs = new JsonFilterToSql(dbContext);
            var filterDescModel = jfs.BuilderFilterDesc(tableName, filter);
            //Dictionary<string, List<JqGridFilterDescViewModel>> dicList = new Dictionary<string, List<JqGridFilterDescViewModel>>();
            List<JqGridFilterDescViewModel> sqlBuilder = new List<JqGridFilterDescViewModel>();

            if (filterDescModel != null)
            {
                filterDescModel.ForEach((m) =>
                {
                    sqlBuilder.Add(new JqGridFilterDescViewModel { FilterDesc = m.FilterDesc, FilterOper = m.FilterOper, FilterResult = m.FilterResult, Group = m.Group });
                });
            }
            return sqlBuilder;
        }

    
    }
}