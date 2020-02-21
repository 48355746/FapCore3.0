using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fap.AspNetCore.Controls.JqGrid.Extensions
{
    public static class FapColumnExtensions
    {
        public static string[] systemDefaultFields = { "EnableDate", "DisableDate", "CreateBy", "CreateName", "CreateDate", "UpdateBy", "UpdateName", "UpdateDate", "GroupUid", "OrgUid", "Dr", "Ts" };
        public static IEnumerable<Column> ToColumns(this IEnumerable<FapColumn> fapColumns, IDbContext dbContext, IMultiLangService multiLang)
        {
            if (fapColumns.Any())
            {
                if (fapColumns.Any())
                {
                    foreach (FapColumn fc in fapColumns)
                    {
                        if (systemDefaultFields.Contains(fc.ColName))
                            continue;
                        yield return fc.ToColumn( multiLang,dbContext);
                        if(fc.CtrlType==FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            FapColumn fcMC= (FapColumn)fc.Clone();
                            fcMC.ColName = fc.ColName + "MC";
                            fcMC.ShowAble = 0;
                            yield return fcMC.ToColumn(multiLang, dbContext);
                        }
                       
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fapColumn"></param>
        /// <param name="disCols">手动设置可见的字段，默认可能为不可见</param>
        /// <param name="hideCols">要隐藏的字段</param>
        /// <returns></returns>
        public static Column ToColumn(this FapColumn fapColumn, IMultiLangService multiLang,IDbContext dbContext)
        {
            //判断是否为隐藏。当disCols不包含且 ShowAble==0

            bool hideAble =fapColumn.ShowAble==0;
            string colName = fapColumn.ColName;
            //if (colName.EndsWith("MC") && fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            //{
            //    colName = colName.Substring(0, colName.Length - 2);
            //}
           
           
            //如果是参照字段，隐藏之，显示MC字段
            //if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && !hideAble && fapColumn.IsCustomColumn == 0)
            //{
            //    hideAble = true;
            //}
            string columnName = fapColumn.ColName;
            if (fapColumn.IsMultiLang == 1)
            {
                string langName = multiLang.CurrentLanguageName;
                if (langName != MultiLanguageEnum.ZhCn.ToString())
                {
                    columnName += langName;
                }
            }
            //fapColumn.CtrlType
            Column col = new Column(columnName);
            string label = multiLang.GetMultiLangValue(MultiLanguageOriginEnum.FapColumn, $"{fapColumn.TableName}_{fapColumn.ColName}");
            col.SetLabel(label);
            col.SetKey(fapColumn.ColType == FapColumn.COL_TYPE_PK);
            col.SetWidth(fapColumn.DisplayWidth);
            col.SetHidden(hideAble).SetHidelg(hideAble).SetColmenu(true);
            col.SetEditable(fapColumn.EditAble == 1);
            //编辑规则
            EditRules editRules = new EditRules();
            col.SetEditRules(editRules);
            if(fapColumn.NullAble==0)
            {
                editRules.Required = true;
            }
            if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
            {
                col.SetFormatter(Enums.Formatters.Checkbox);
                //编辑设置
                col.SetEditType(Enums.EditType.Checkbox);
                EditOptions eo = new EditOptions() { Value = "1:0" };
                col.SetEditOptions(eo);
                //查询设置
                col.SetSearchType(Enums.Searchtype.Select);
                col.SetSearchOption(Enums.SearchOptions.Equal);
                col.SetSearchOption(Enums.SearchOptions.NotEqual);
                IDictionary<string, string> searchTerms = new Dictionary<string, string>();
                searchTerms.Add("1", "是");
                searchTerms.Add("0", "否");
                col.SetSearchTerms(searchTerms);
                //默认ACE样式函数
                //col.SetUnFormat("aceSwitch");
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
            {
                if (fapColumn.MultiAble == 1)
                {
                    col.SetCustomFormatter("formatCheckboxList");
                    col.SetUnFormat("unformatCheckboxList");
                }
                else
                {
                    col.SetFormatter(Enums.Formatters.Select);
                    //编辑设置
                    col.SetEditType(Enums.EditType.Select);
                    IEnumerable<FapDict> list = dbContext.Dictionarys(fapColumn.ComboxSource);
                    if (list != null && list.Any())
                    {
                        string codeValues = string.Join(";", list.Select(c => c.Code + ":" + c.Name).ToList());
                        EditOptions eo = new EditOptions() { Value = codeValues };//" getCodeData('" + fapColumn.RefTable + "')"};
                        col.SetEditOptions(eo);
                        //查询设置
                        col.SetSearchType(Enums.Searchtype.Select);
                        col.SetSearchOption(Enums.SearchOptions.Equal);
                        col.SetSearchOption(Enums.SearchOptions.NotEqual);
                        IDictionary<string, string> searchTerms = new Dictionary<string, string>();
                        foreach (var item in list)
                        {
                            searchTerms.Add(item.Code, item.Name);
                        }
                        col.SetSearchTerms(searchTerms);
                    }
                }
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOXLIST)
            {
                col.SetCustomFormatter("formatCheckboxList");
                col.SetUnFormat("unformatCheckboxList");
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATE || fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
            {
                string fmt = fapColumn.DisplayFormat;
                col.SetEditType(Enums.EditType.Text);
                EditOptions editoptions = new EditOptions();
                col.SetEditOptions(editoptions);
                if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATE)
                {
                    if (fmt.EqualsWithIgnoreCase("yyyy"))
                    {
                        fmt = "Y";
                    }
                    else if (fmt.EqualsWithIgnoreCase("yyyy-mm"))
                    {
                        fmt = "Y-m";
                    }
                    else
                    {
                        fmt = "Y-m-d";
                    }
                    col.SetFormatter(Enums.Formatters.Date, "srcformat:\"" + fmt + "\",newformat:\"" + fmt + "\"");
                    //查询设置
                    col.SetSearchType(Enums.Searchtype.Datepicker);

                }
                else
                {
                    if (fmt.IsPresent()&&fmt.EqualsWithIgnoreCase("HH:mm"))
                    {
                        fmt = "H:i";
                    }
                    else
                    {
                        fmt = "Y-m-d H:i:s";
                    }
                    col.SetFormatter(Enums.Formatters.Date, "srcformat:\"" + fmt + "\",newformat:\"" + fmt + "\"");
                    //查询设置
                    col.SetSearchType(Enums.Searchtype.Datetimepicker);
                }
                //编辑设置
                editoptions.DataInit = @"function (element) {
                                $(element).datepicker({
                                    id: '" + fapColumn.ColName + @"_datePicker',
                                    dateFormat: '" + fmt + @"',
                                    //minDate: new Date(2010, 0, 1),
                                    maxDate: new Date(2030, 0, 1),
                                    showOn: 'focus'
                                });
                            }";
                //默认ACE样式函数
                //col.SetUnFormat("pickDate");
                //col.SetDataInit("initDateSearch");

                col.SetSearchDateFormat(fapColumn.DisplayFormat);
                col.SetSearchOption(Enums.SearchOptions.Equal);
                col.SetSearchOption(Enums.SearchOptions.NotEqual);
                col.SetSearchOption(Enums.SearchOptions.Greater);
                col.SetSearchOption(Enums.SearchOptions.GreaterOrEqual);
                col.SetSearchOption(Enums.SearchOptions.Less);
                col.SetSearchOption(Enums.SearchOptions.LessOrEqual);

            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || fapColumn.CtrlType == FapColumn.CTRL_TYPE_MONEY)
            {
                SetEditRulesNumber(fapColumn, editRules);
                //inline 编辑设置                
                col.SetEditType(Enums.EditType.Text);
                EditOptions editopt = new EditOptions();
                editopt.DataInit = @"function (element) {
                                $(element).TouchSpin({
                                step: 1,
                                decimals: 2,
                                boostat: 5,
                                maxboostedstep: 10               
                                });
                            }";
                //col.SetFormatter(Enums.Formatters.Currency);
                if (fapColumn.DisplayFormat == "%")
                {
                    col.SetFormatter(Enums.Formatters.Percent);
                }
                //col.SetFormatter(Enums.Formatters.Number, "thousandsSeparator: ','");
                col.SetSearchOption(Enums.SearchOptions.Equal);
                col.SetSearchOption(Enums.SearchOptions.NotEqual);
                col.SetSearchOption(Enums.SearchOptions.Greater);
                col.SetSearchOption(Enums.SearchOptions.GreaterOrEqual);
                col.SetSearchOption(Enums.SearchOptions.Less);
                col.SetSearchOption(Enums.SearchOptions.LessOrEqual);
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_INT)
            {
                SetEditRulesNumber(fapColumn, editRules);
                //inline 编辑设置                
                col.SetEditType(Enums.EditType.Text);
                EditOptions editopt = new EditOptions();
                editopt.DataInit = @"function (element) {
                                $(element).TouchSpin({
                                step: 1,
                                boostat: 5,
                                maxboostedstep: 10               
                                });
                            }";
                col.SetFormatter(Enums.Formatters.Integer);
                col.SetSearchOption(Enums.SearchOptions.Equal);
                col.SetSearchOption(Enums.SearchOptions.NotEqual);
                col.SetSearchOption(Enums.SearchOptions.Greater);
                col.SetSearchOption(Enums.SearchOptions.GreaterOrEqual);
                col.SetSearchOption(Enums.SearchOptions.Less);
                col.SetSearchOption(Enums.SearchOptions.LessOrEqual);
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_CITY)
            {
                //城市
                col.SetEditType(Enums.EditType.Text);
                EditOptions editoptions = new EditOptions();
                editoptions.DataInit = @"function (element) {
                                $(element).cityselect();
                            }";
                col.SetEditOptions(editoptions);

            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_NATIVE)
            {
                //籍贯
                col.SetEditType(Enums.EditType.Text);
                EditOptions editoptions = new EditOptions();
                editoptions.DataInit = @"function (element) {
                                $(element).citypicker();
                            }";
                col.SetEditOptions(editoptions);
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                col.SetSearchType(Enums.Searchtype.Text);
                SetTextSearchOption(col);
                col.SetCustomFormatter("formatReference");
                col.SetUnFormat("unformatReference");
                col.SetEditType(Enums.EditType.Custom);
                EditOptions editoptions = new EditOptions();
                editoptions.DataInit = @"function (element) {
                                $(element).find('input:text').on(ace.click_event,function(e){$(this).next().trigger(ace.click_event);e.preventDefault();});
                                $(element).find('input:text').next().on(ace.click_event, function(){ var $this=$(this).prev();
                                    openRefrenceWindow('" + fapColumn.ColComment + "选择','" + fapColumn.Fid + @"','" + fapColumn.RefType + @"',function(code,name){
                                        $this.val(name).data('value',code);
                                        $this.closest('td').next().find('input:text').val(name);
                                    },function(){
                                        $this.val('').data('value','');
                                   })
                                });
                            }";
                editoptions.Custom_element = "createReferenceEditElement";//创建元素
                editoptions.Custom_value = "getReferenceElementValue";//获取值
                col.SetEditOptions(editoptions);
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_FILE)
            {
                //附件
                col.SetCustomFormatter("attachmentInfo");
                col.SetUnFormat("unattachmentInfo");
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_IMAGE)
            {
                col.SetCustomFormatter("formatImage");
                col.SetUnFormat("unformatImage");
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_TEXT)
            {
                SetTextSearchOption(col);
            }
            else if (fapColumn.CtrlType == FapColumn.CTRL_TYPE_EMAIL)
            {
                SetTextSearchOption(col);
                editRules.Email = true;
            }
            else
            {
                SetTextSearchOption(col);
            }



            return col;
        }

        private static void SetEditRulesNumber(FapColumn fapColumn, EditRules editRules)
        {
            editRules.Number = true;
            if (fapColumn.MaxValue != 0|| fapColumn.MinValue!=0)
            {
                editRules.MaxValue = fapColumn.MaxValue;
                fapColumn.MinValue = fapColumn.MinValue;
            }
        }

        private static void SetTextSearchOption(Column col)
        {
            col.SetSearchOption(Enums.SearchOptions.Contains);
            col.SetSearchOption(Enums.SearchOptions.BeginsWith);
            col.SetSearchOption(Enums.SearchOptions.DoesNotBeginWith);
            col.SetSearchOption(Enums.SearchOptions.DoesNotContain);
            col.SetSearchOption(Enums.SearchOptions.DoesNotEndWith);
            col.SetSearchOption(Enums.SearchOptions.EndsWith);
            //不要加相等，因为参照用like判断是否加MC来查询
            //col.SetSearchOption(Enums.SearchOptions.Equal);
        }
    }
}
