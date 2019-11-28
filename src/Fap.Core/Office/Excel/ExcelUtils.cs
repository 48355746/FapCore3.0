using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Fap.Core.Office.Excel
{
    public class ExcelUtils
    {
        private static List<string> defaultFieldNameList = new List<string>();
        static ExcelUtils()
        {
            // 默认字段
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_Id);
            //defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_Fid);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_OrgUid);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_GroupUid);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_EnableDate);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_DisableDate);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_Dr);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_Ts);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_CreateBy);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_CreateName);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_CreateDate);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_UpdateBy);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_UpdateName);
            defaultFieldNameList.Add(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate);
        }
        public static object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell != null && cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date Type的数据CellType是Numeric
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Blank:
                            value = "";
                            break;
                        case CellType.Error:
                            value = "";
                            break;
                        case CellType.Formula:
                            value = "";
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;

                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
                else
                {
                    value = "";
                }
            }
            catch (Exception)
            {
                value = "";
            }

            return value;
        }

        /// <summary>
        /// 根据字段类型，获取单元格值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static object GetCellValue(ICell cell, string format)
        {
            object value = null;
            try
            {
                if (cell != null)
                {
                    if (FapColumn.COL_TYPE_STRING == format
                        || FapColumn.COL_TYPE_UID == format
                        || FapColumn.CTRL_TYPE_COMBOBOX == format)
                    {
                        value = cell.StringCellValue;
                    }
                    else if (FapColumn.COL_TYPE_DATETIME == format)
                    {
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            DateTime datevalue = cell.DateCellValue;
                            value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", datevalue);
                        }
                        else
                        {
                            value = cell.StringCellValue;
                        }
                    }
                    else if (FapColumn.COL_TYPE_BOOL == format)
                    {
                        if (cell.CellType == CellType.Boolean)
                        {
                            bool boolvalue = cell.BooleanCellValue;
                            value = boolvalue ? 1 : 0;
                        }
                        else
                        {
                            object objvalue = cell.StringCellValue;
                            value = objvalue.ToBool() ? 1 : 0;
                        }
                    }
                    else if (FapColumn.COL_TYPE_DOUBLE == format)
                    {
                        if (cell.CellType == CellType.Numeric)
                        {
                            value = cell.NumericCellValue;
                        }
                        else
                        {
                            value = cell.StringCellValue.ToDouble();
                        }
                    }
                    else if (FapColumn.COL_TYPE_INT == format)
                    {
                        if (cell.CellType == CellType.Numeric)
                        {
                            value = cell.NumericCellValue;
                        }
                        else
                        {
                            value = cell.StringCellValue.ToInt();
                        }
                    }
                    else if (FapColumn.COL_TYPE_LONG == format)
                    {
                        if (cell.CellType == CellType.Numeric)
                        {
                            value = cell.NumericCellValue;
                        }
                        else
                        {
                            value = cell.StringCellValue.ToLong();
                        }
                    }



                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date Type的数据CellType是Numeric
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
                else
                {
                    value = "";
                }
            }
            catch (Exception)
            {
                value = "";
            }

            return value;
        }

        public static T XmlDeserialize<T>(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                return default(T);
            }
            using (StringReader reader = new StringReader(xmlString))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                object obj = xmlSerializer.Deserialize(reader);
                return (T)obj;
            }
        }
        /// <summary>  
        /// 对象序列化成 XML String  
        /// </summary>  
        public static string XmlSerialize<T>(T obj)
        {
            using (StringWriter writer = new StringWriter())
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                //XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
        /// <summary>
        /// 获取审计字段
        /// </summary>
        /// <returns></returns>
        public static List<string> DefaultFieldNameList
        {
            get
            {
                return defaultFieldNameList;
            }
        }
    }
}
