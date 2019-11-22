using Fap.Core.Metadata;
using Ganss.XSS;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;

namespace Fap.AspNetCore.Extensions
{
    public static class FormCollectionExtentions
    {
        /// <summary>
        /// JObject对象转换成FapDynamicObject对象
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="dynamciObj">FapDynamicObject对象</param>
        /// <param name="excludeKeys">指定字段，排除要赋值的字段</param>
        /// <returns></returns>
        public static dynamic ToDynamicObject(this IFormCollection fcs, IEnumerable<FapColumn> columnList, params string[] excludeKeys)
        {
            dynamic dynamciObj  = new FapDynamicObject(columnList.First().TableName);
            ICollection<string> formKeys = fcs.Keys;
            foreach (var frmKey in formKeys)
            {
                if (excludeKeys.Contains(frmKey)) continue;

                if ("id".Equals(frmKey))
                {
                    if (!(string.IsNullOrEmpty(fcs[frmKey]) || "_empty".Equals(fcs[frmKey])))
                    {
                        dynamciObj.Add("Id", fcs[frmKey].ToString());
                    }
                }
                else
                {
                    var sanitizer = new HtmlSanitizer();
                    FapColumn column = columnList.Where(c => c.ColName == frmKey).FirstOrDefault();
                    if (column != null)
                    {
                        if (column.IsIntType()) //整型
                        {
                            dynamciObj.Add(frmKey, fcs[frmKey][0].ToInt());
                        }
                        else if (column.IsLongType()) //长整型
                        {
                            dynamciObj.Add(frmKey, fcs[frmKey][0].ToLong());
                        }
                        else if (column.IsDoubleType()) //浮点型
                        {
                            dynamciObj.Add(frmKey, fcs[frmKey][0].ToDecimal());
                        }
                        else if (column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
                        {
                            //富文本防止XSS
                            dynamciObj.Add(frmKey, sanitizer.Sanitize(fcs[frmKey].ToString()));
                        }
                        else //字符串
                        {
                            dynamciObj.Add(frmKey, fcs[frmKey].ToString());
                        }
                    }
                    else
                    {
                        dynamciObj.Add(frmKey, fcs[frmKey].ToString());
                    }
                }
            }
            return dynamciObj;
        }

      
    
    }
}
