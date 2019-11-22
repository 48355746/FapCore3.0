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
        public static List<JqGridFilterDescViewModel> BuilderFilterDesc(string tableName, string filter,IFapPlatformDomain domain,IDbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }
            JqgridJsonFilterToSql jfs = new JqgridJsonFilterToSql(dbContext,domain);
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

        public static HttpResponseMessage FormatJqGridData(PageDataResultView result)
        {
            try
            {
                string jsonData = "";
                if (result.Data == null || result.Data.Count() < 1)
                {
                    jsonData = new { total = 0, page = 0, records = 0, rows = ("") }.ToJsonIgnoreNullValue();
                }
                else
                {
                    List<dynamic> r = new List<dynamic>();
                    foreach (IDictionary<string, object> c in result.Data)
                    {
                        r.Add(new
                          {
                              id = c["Id"],
                              cell = c.Values.ToArray()
                          });
                    }
                    var jsonObj = new
                       {
                           total = result.TotalPage,
                           page = result.CurrentPageIndex,
                           records = result.TotalNum,
                           rows = r.ToArray()
                       };
                    jsonObj = null;
                    jsonData = jsonObj.ToJsonIgnoreNullValue();
                }

                HttpResponseMessage httpResMsg = new HttpResponseMessage { Content = new StringContent(jsonData, Encoding.GetEncoding("UTF-8"), "application/json") };
                return httpResMsg;
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}