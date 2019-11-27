using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;
using Fap.Core.MultiLanguage;
using System.Text.Encodings.Web;

namespace Fap.AspNetCore.Controls.DataForm
{
    public static class TagsHelper
    {
        public static string AsDate(this FapColumn fapColumn, string editAble,  string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            string dateFormat = "yyyy-mm-dd";
            if (fapColumn.DisplayFormat.IsPresent())
            {
                dateFormat = fapColumn.DisplayFormat;
            }
            StringBuilder sbDate = new StringBuilder();
            sbDate.AppendLine("<span class=\"input-icon input-icon-right\">");
            sbDate.AppendFormat("    <input class=\"form-control date-picker\" id=\"{0}\" " + editAble + " name=\"{0}\" ng-model=\"{1}\" value=\"{2}\" type=\"text\" data-dateformat=\"{3}\" />", ctrlName, ngModel, fieldValue, dateFormat).AppendLine();
            //sbFormGroup.AppendLine("                  <span class=\"input-group-addon\">");
            sbDate.AppendLine("                      <i class=\"ace-icon fa fa-calendar block\"></i>");
            sbDate.AppendLine("                 </span>");
            //sbFormGroup.AppendLine("             </div>");
            return sbDate.ToString();
        }
        public static string AsDateTime(this FapColumn fapColumn, string editAble,  string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbDateTime = new StringBuilder();
            sbDateTime.AppendLine("<span class=\"input-icon input-icon-right\">");
            sbDateTime.AppendFormat("                     <input class=\"form-control datetime-picker\"  id=\"{0}\" name=\"{0}\" " + editAble + " ng-model=\"{1}\" type=\"text\" value=\"{2}\"/>", ctrlName, ngModel, fieldValue).AppendLine();
            //sbFormGroup.AppendLine("                    <span class=\"input-group-addon\">");
            sbDateTime.AppendLine("                        <i class=\"ace-icon fa fa-clock-o block\"></i>");
            sbDateTime.AppendLine("                    </span>");
            // sbFormGroup.AppendLine("                </div>");
            return sbDateTime.ToString();
        }
        public static string AsCombobox(this FapColumn fapColumn,string editAble, IEnumerable<FapDict> dicList, List<string> tempList,string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbCombobox = new StringBuilder();
            if (fapColumn.MultiAble == 0)
            {
                sbCombobox.AppendFormat(" <select id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" " + editAble + " class=\"form-control \" data-placeholder=\"请选择\" >", ctrlName, ngModel).AppendLine();
                sbCombobox.AppendLine("<option value=''></option>");
                if (dicList.Any())
                {
                    foreach (FapDict dict in dicList)
                    {
                        if (dict.Code == fieldValue)
                        {
                            sbCombobox.AppendLine("<option value='" + dict.Code + "' selected>" + dict.Name + "</option>");
                        }
                        else
                        {
                            sbCombobox.AppendLine("<option value='" + dict.Code + "' >" + dict.Name + "</option>");
                        }
                    }
                }
                sbCombobox.AppendLine("</select>");
            }
            else
            {
                sbCombobox.AppendFormat("<div class=\"control-group form-horizontal checkboxlist\"  data-ctrlName='{0}'>", ctrlName);
                if (dicList.Any())
                {
                    foreach (FapDict dict in dicList)
                    {
                        if (tempList.Contains(dict.Code))
                        {
                            sbCombobox.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" checked class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                        }
                        else
                        {
                            sbCombobox.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                        }
                    }
                }
                sbCombobox.AppendFormat("</div>");
            }
            return sbCombobox.ToString();
        }
        public static string AsCheckboxList(this FapColumn fapColumn, string editAble,IEnumerable<FapDict> dlist,IList<string> tempList)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbCheckboxList = new StringBuilder();
            sbCheckboxList.AppendFormat("<div class=\"control-group form-horizontal checkboxlist\"  data-ctrlName='{0}'>", ctrlName);
            if (dlist.Any())
            {
                foreach (dynamic dict in dlist)
                {
                    if (tempList.Contains(dict.Code))
                    {
                        sbCheckboxList.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" checked class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                    }
                    else
                    {
                        sbCheckboxList.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                    }
                }
            }
            sbCheckboxList.AppendFormat("</div>");
            return sbCheckboxList.ToString();
        }

        public static string AsCheckbox(this FapColumn fapColumn,string editAble, IEnumerable<FapDict> dicList, string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbCheckbox = new StringBuilder();
            sbCheckbox.AppendFormat("<div class=\"control-group form-horizontal\" id=\"{0}\">", ctrlName).AppendLine();
            //内容动态添加
            string chk = "";
            if (string.IsNullOrWhiteSpace(fapColumn.RefTable))
            {
                if (fieldValue == "1")
                {
                    chk = "checked";
                }
                sbCheckbox.AppendLine("<label>");
                sbCheckbox.AppendFormat("<input name=\"{0}\" id=\"{0}\" value=\"1\" {1} {2} class=\"form-control ace\" type=\"checkbox\">", ctrlName, chk, editAble).AppendLine();
                sbCheckbox.AppendLine("<span class=\"lbl\"></span>");
                sbCheckbox.AppendLine("</label>");
            }
            else
            {
                if (dicList.Any())
                {
                    List<string> selCodes = new List<string>();
                    if (!string.IsNullOrWhiteSpace(fieldValue))
                    {
                        foreach (var strC in fieldValue.Split(','))
                        {
                            selCodes.Add(strC);
                        }
                    }
                    foreach (FapDict dict in dicList)
                    {
                        if (selCodes.Contains(dict.Code))
                        {
                            chk = "checked";
                        }
                        sbCheckbox.AppendLine("<label>");
                        sbCheckbox.AppendFormat("<input  name=\"{0}\" type=\"checkbox\" value=\"{1}\" {2} {3} class=\"form-control ace\" />", ctrlName, dict.Code, chk, editAble).AppendLine();
                        sbCheckbox.AppendFormat("<span class=\"lbl\"> {0}</span>", dict.Name);
                        sbCheckbox.AppendLine("</label>");
                    }
                }
            }
            sbCheckbox.AppendLine("</div>");
            return sbCheckbox.ToString();
        }

        public static string AsReference(this FapColumn fapColumn, string frmid, string editAble,  string fv, string fmc)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbReference = new StringBuilder();
            string ngModelMC = ngModel + "MC";
            string ctrlNameMC = frmid + ctrlName + "MC";
            sbReference.AppendLine("<span class=\"input-icon input-icon-right\">");
            sbReference.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"  class=\"form-control hidden\" />", ctrlName, ngModel, fv).AppendLine();
            sbReference.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" " + editAble + " value=\"{2}\"  class=\"form-control\" />", ctrlNameMC, ngModelMC, fmc).AppendLine();
            //sbFormGroup.AppendLine("   <span class=\"input-group-addon b\">");
            if (editAble.IsMissing())
            {
                sbReference.AppendLine("       <i class=\"ace-icon fa fa-search block\"></i>");
            }
            sbReference.AppendLine("    </span>");
            //sbFormGroup.AppendLine(" </div>");
            return sbReference.ToString();
        }
        public static string AsTextArea(this FapColumn fapColumn, string editAble,  string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            string rows = "5";
            if (fapColumn.DisplayFormat.IsPresent())
            {
                rows = fapColumn.DisplayFormat;
            }
            if (fapColumn.ColLength < 99999 && fapColumn.ColType == FapColumn.COL_TYPE_STRING)
            {
                return $"<textarea id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" { editAble } rows=\"{rows}\"  class=\"form-control autosize-transition limited\" maxlength=\"{fapColumn.ColLength}\">{fieldValue}</textarea>";
            }
            else
            {
                return $"<textarea id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" { editAble } rows=\"{rows}\"   class=\"form-control autosize-transition\">{fieldValue}</textarea>";
            }
        }
        public static string AsRichTextBox(this FapColumn fapColumn,  string fieldValue)
        {
            string ctrlName = fapColumn.ColName;
            string required = " data-nullable=1 ";
            if (fapColumn.NullAble == 0 && fapColumn.ShowAble == 1)
            {
                required = " data-nullable=0 ";
            }
            return $"<div class=\"form-control wysiwyg-editor\" name=\"{ctrlName}\" id=\"{ctrlName}\" {required}>{fieldValue}</div>";
        }

        public static string AsMultiLanguageTextBox(this FapColumn fapColumn, string editAble, string ctrlName,  string currentLangDesc, Dictionary<string, string> formData, string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            StringBuilder sbMultiLang = new StringBuilder();
            sbMultiLang.AppendLine("<span class=\"input-icon input-icon-right\">");
            sbMultiLang.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel,HtmlEncoder.Default.Encode(fieldValue.ToString())).AppendLine();
            sbMultiLang.AppendFormat("<i class=\"ace-icon fa fa-language green\" data-fid=\"{0}\"></i></span>", fapColumn.Fid);
            var langList = EnumExtensions.GetDictionary(typeof(MultiLanguageEnum));
            //style="top: 26px; left: 155.656px; display: block;"
            sbMultiLang.AppendFormat("<div class=\"popover popovermultilang fade right in \" role=\"tooltip\" id=\"{0}\" ><div class=\"arrow\" style=\"top: 50%;\"></div><h3 class=\"popover-title\"><i class=\"ace-icon fa fa-language green\"></i><button type=\"button\" class=\"multilangpopoverclose close red\" ><i class=\"ace-icon fa fa-times\"></i></button> 多语言</h3><div class=\"popover-content\">", fapColumn.Fid);
            foreach (var langField in langList)
            {
                if (langField.Description == currentLangDesc)
                {
                    continue;
                }
                string cname = ctrlName;
                if (langField.Value != MultiLanguageEnum.ZhCn.ToString())
                {
                    cname = ctrlName + langField.Value;
                }
                string value = formData[cname];
                sbMultiLang.AppendFormat("<input type=\"text\" id=\"{0}\" placeholder=\"{1}\" class=\"form-control col-xs-12 col-sm-12\" value=\"{2}\" style=\"margin:1px\" {3}/>", cname, langField.Description, value, editAble);
            }
            sbMultiLang.Append("</div></div>");
            return sbMultiLang.ToString();
        }

        public static string AsTextBox(this FapColumn fapColumn, string editAble, string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<input type=\"text\" {editAble}  class=\"form-control\" id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{HtmlEncoder.Default.Encode(fieldValue)}\"/>";
        }
        public static string AsPassword(this FapColumn fapColumn,string editAble,string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<input type=\"password\" {editAble} class=\"form-control\"  id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{fieldValue}\"/>";
        }
        public static string AsEmail(this FapColumn fapColumn, string editAble,  string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<input type=\"email\" class=\"form-control\" id=\"{ctrlName}\"  {editAble} name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{fieldValue}\"/>";
        }
        public static string AsFile(this FapColumn fapColumn, string editAble, string frmid, string fieldValue, string lableName)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbFile = new StringBuilder();
            sbFile.AppendFormat("<button id=\"file{0}{1}\" " + editAble + " class=\"btn btn-info\" type=\"button\"><i class=\"ace-icon fa fa-paperclip\"></i>附件</button>", frmid, ctrlName).AppendLine();
            sbFile.AppendFormat("<input type=\"text\" class=\"form-control attachment hidden\"  id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\" data-text=\"{3}\" data-nullable={4} data-editable={5} />", ctrlName, ngModel, fieldValue, lableName, fapColumn.NullAble, fapColumn.EditAble).AppendLine();
            sbFile.AppendFormat("<label class=\"text-muted\">支持文件格式({0})</label>", fapColumn.FileSuffix.IsMissing() ? "*.*" : fapColumn.FileSuffix).AppendLine();
            return sbFile.ToString();
        }
        public static string AsImage(this FapColumn fapColumn,  string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            StringBuilder sbImage = new StringBuilder();
            sbImage.AppendLine("<span class=\"profile-picture\">");
            sbImage.AppendFormat("       <img id=\"avatar{0}\" data-pk=\"{1}\" style=\"width:160px;\" class=\"editable img-responsive\"  src=\"{2}\" />", ctrlName, fieldValue, $"/Home/UserPhoto/{fieldValue}?v={DateTime.Now.Ticks}").AppendLine();
            sbImage.AppendLine("   </span>");
            sbImage.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"  class=\"form-control hidden\" />", ctrlName, ngModel, fieldValue).AppendLine();
            return sbImage.ToString();
        }
        public static string AsCity(this FapColumn fapColumn,string editAble,string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<input   type=\"text\" {editAble}  class=\"form-control\" id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{fieldValue}\">";
        }
        public static string AsNative(this FapColumn fapColumn,string editAble,string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<input  readonly type=\"text\" {editAble}  class=\"form-control\" id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{fieldValue}\">";
        }
        public static string AsRate(this FapColumn fapColumn,string editAble,string fieldValue)
        {
            string ngModel = "formData." + fapColumn.ColName;
            string ctrlName = fapColumn.ColName;
            return $"<div id=\"rat-{0}\" class=\"rating inline\"></div><input type=\"text\" {editAble}  class=\"form-control hide\" id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" value=\"{fieldValue}\">";
        }
    }
}
