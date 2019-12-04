using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Ardalis.GuardClauses;
using System.Text.Encodings.Web;

namespace Fap.AspNetCore.Controls.DataForm
{
    public class FapField
    {
        private FapColumn _fapColumn;
        private IDbContext _dataAccessor;
        private IMultiLangService _multiLangService;
        public FapField(IDbContext dataAccessor,IMultiLangService multiLangService)
        {
            _dataAccessor = dataAccessor;
            _multiLangService = multiLangService;
        }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string FieldComment { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object FieldValue { get; set; }
        /// <summary>
        /// 字段显示值
        /// </summary>
        public object FieldMCValue { get; set; }
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// 可编辑,优先级大于ReadOnly
        /// </summary>
        private bool _isEnabled = false;
        /// <summary>
        /// 可编辑
        /// </summary>
        public bool IsEnable { get { return _isEnabled; } set { _isEnabled = value; } }
        /// <summary>
        /// 字段分组
        /// </summary>
        public string FieldGroup { get; set; }
        public FapColumn CurrentColumn
        {
            get { return _fapColumn; }
            set { _fapColumn = value; }
        }
        /// <summary>
        /// 表单数据
        /// </summary>
        public dynamic FormData { get; set; }
        /// <summary>
        /// 生成控件，切记获取值得控件要有 class='form-control'
        /// </summary>
        /// <returns></returns>
        public string BuilderForm(string frmid, int _colCount, bool isColspan, FormStatus formStatus = FormStatus.Add)
        {
            string lableName = _multiLangService.GetLangColumnComent(_fapColumn);
            string ctrlName = _fapColumn.ColName;

            //是否可编辑
            string editAble = string.Empty;
            if (!IsEnable && (_fapColumn.EditAble == 0 || ReadOnly))
            {
                editAble = "disabled='disabled'";
            }
            StringBuilder sbFormGroup = new StringBuilder();
            string labelCol = " col-sm-2 col-xs-12";
            if (formStatus == FormStatus.View)
            {
                labelCol = " col-sm-2 col-xs-2";
            }
            string ctrlOneCol = " col-xs-12 col-sm-10";
            string ctrlTwoCol = " col-xs-12 col-sm-4 ";
            if (formStatus == FormStatus.View)
            {
                ctrlOneCol = " col-xs-10 col-sm-10";
                ctrlTwoCol = " col-xs-4 col-sm-4 ";
            }
            if (_colCount == 1)
            {
                labelCol = " col-sm-3 col-xs-12";
                ctrlTwoCol = ctrlOneCol = " col-xs-12 col-sm-9";
                if (formStatus == FormStatus.View)
                {
                    labelCol = " col-sm-3 col-xs-3";
                    ctrlTwoCol = ctrlOneCol = " col-xs-9 col-sm-9";
                }
            }

            //一个控件占一行textarea,richtextbox
            if (isColspan)
            {
                //sbFormGroup.AppendLine("<div class=\"form-group col-xs-12 col-sm-12\">");

                sbFormGroup.AppendFormat("<label class=\"" + labelCol + " control-label no-padding-right\" for=\"{0}\">{1}", ctrlName, lableName);
                if (_fapColumn.NullAble == 0 && formStatus != FormStatus.View)
                {
                    sbFormGroup.Append("<i class='red'>*</i>");
                }
                if (formStatus == FormStatus.View)
                {
                    sbFormGroup.Append(":");
                }
                sbFormGroup.Append("</label>");

                sbFormGroup.AppendLine("<div class=\"ctrlcontainer " + ctrlOneCol + "\">");

            }
            else
            {
                //sbFormGroup.AppendLine(" <div class=\"form-group col-xs-12 col-sm-6\">");

                sbFormGroup.AppendFormat("<label class=\"" + labelCol + " control-label no-padding-right\" for=\"{0}\">{1}", ctrlName, lableName);

                if (_fapColumn.NullAble == 0 && formStatus != FormStatus.View)
                {
                    sbFormGroup.Append("<i class='red'>*</i>");
                }
                if (formStatus == FormStatus.View)
                {
                    sbFormGroup.Append(":");
                }
                sbFormGroup.Append("</label>");

                sbFormGroup.AppendLine("<div class=\"ctrlcontainer " + ctrlTwoCol + "\">");

            }

            if (formStatus == FormStatus.View)
            {
                sbFormGroup.AppendLine("<div class=\"text-info control-label align-left\">" + $"<label id=\"{ctrlName}\" >{BuilderFormFieldText()} </label>" + "</div>");
            }
            else
            {
                sbFormGroup.AppendLine("<div class='clearfix'>");
                BuilderDataFormField(frmid, lableName,  editAble, sbFormGroup);
                sbFormGroup.AppendLine("</div>");
            }
            sbFormGroup.AppendLine("</div>");
            //sbFormGroup.AppendLine("</div>");

            return sbFormGroup.ToString();
        }

        private void BuilderDataFormField(string frmid, string lableName, string editAble, StringBuilder sbFormGroup)
        {
            //angularjs的时候使用，目前没用

            if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATE)
            {
                if (FieldValue.ToString().Length > 10)
                {
                    FieldValue = FieldValue.ToString().Substring(0, 10);
                }
                sbFormGroup.AppendLine(_fapColumn.AsDate(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
            {
                sbFormGroup.AppendLine(_fapColumn.AsDateTime(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
            {
                if (_fapColumn.RefTable.IsMissing())
                {
                    Guard.Against.Null(_fapColumn.RefTable, "字典未设置");
                }
                List<string> tempList = new List<string>();
                if (_fapColumn.MultiAble == 1)
                {
                    if (FieldValue != null && FieldValue.ToString().Length > 0)
                    {
                        //获取多选项的值
                        GetMultiSelValues(tempList);
                    }
                }
                IEnumerable<FapDict> dicList =_dataAccessor.Dictionarys(_fapColumn.RefTable);
                sbFormGroup.AppendLine(_fapColumn.AsCombobox(editAble, dicList, tempList, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOXLIST)
            {
                List<string> tempList = new List<string>();

                if (FieldValue != null && FieldValue.ToString().Length > 0)
                {
                    //获取多选项的值
                    GetMultiSelValues(tempList);
                }
                string where = "";
                if (_fapColumn.RefCondition.IsPresent())
                {
                    where = " where " + _fapColumn.RefCondition;
                }
                string sql = string.Format("select {0} Code,{1} Name from {2} {3}", _fapColumn.RefID, _fapColumn.RefName, _fapColumn.RefTable, where);
                var dlist = _dataAccessor.Query<FapDict>(sql);
                sbFormGroup.AppendLine(_fapColumn.AsCheckboxList(editAble, dlist, tempList));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                sbFormGroup.AppendLine(_fapColumn.AsReference(frmid, editAble, FieldValue.ToString(), FieldMCValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_MEMO)
            {

                sbFormGroup.AppendLine(_fapColumn.AsTextArea(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
            {
                sbFormGroup.AppendLine(_fapColumn.AsRichTextBox(FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
            {
                IEnumerable<FapDict> dicList = new List<FapDict>();
                if (!string.IsNullOrWhiteSpace(_fapColumn.RefTable))
                {
                    dicList = _dataAccessor.Dictionarys(_fapColumn.RefTable);
                }
                sbFormGroup.AppendLine(_fapColumn.AsCheckbox(editAble, dicList, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_EMAIL)
            {
                sbFormGroup.AppendFormat(_fapColumn.AsEmail(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_FILE)
            {
                sbFormGroup.AppendLine(_fapColumn.AsFile(editAble, frmid, FieldValue.ToString(), lableName));

            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_IMAGE)
            {
                sbFormGroup.AppendLine(_fapColumn.AsImage( FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_PASSWORD)
            {
                sbFormGroup.AppendLine(_fapColumn.AsPassword(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_INT || _fapColumn.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || _fapColumn.CtrlType == FapColumn.CTRL_TYPE_MONEY)
            {
                sbFormGroup.AppendLine(_fapColumn.AsTextBox(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_PHONE)
            {
                sbFormGroup.AppendLine(_fapColumn.AsTextBox(editAble, FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_RADIO)
            {
                throw new NotSupportedException("暂无实现，请用其他控件代替");
                //sbFormGroup.AppendFormat("<div class=\"control-group\" id=\"{0}\" name=\"{0}\">", ctrlName, ngModel).AppendLine();
                ////内容动态添加
                //sbFormGroup.AppendLine("</div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_TEXT)
            {
                if (_fapColumn.IsMultiLang == 1)
                {
                    //多語字段處理
                    string ctrlName = _fapColumn.ColName;
                    string currCtrlName = ctrlName;
                    if (_multiLangService.CurrentLanguage != MultiLanguageEnum.ZhCn)
                    {
                        currCtrlName = ctrlName + _multiLangService.CurrentLanguageName;
                        FieldValue = FormData.Get(_fapColumn.TableName + "_" + currCtrlName);
                        if (FieldValue == null)
                        {
                            FieldValue = "";
                        }
                    }
                    string currentLangDesc = _multiLangService.CurrentLanguage.GetDescription();
                    var langList = EnumExtensions.GetDictionary(typeof(MultiLanguageEnum));
                    Dictionary<string, string> dicLangValue = new Dictionary<string, string>();
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
                        string value = FormData.Get(_fapColumn.TableName + "_" + cname);
                        dicLangValue.Add(cname, value);
                    }
                    sbFormGroup.AppendLine(_fapColumn.AsMultiLanguageTextBox(editAble, currCtrlName,  currentLangDesc, dicLangValue, FieldValue.ToString()));

                }
                else
                {

                    sbFormGroup.AppendLine(_fapColumn.AsTextBox(editAble,  FieldValue.ToString()));
                }
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CITY)
            {
                //城市
                sbFormGroup.AppendFormat(_fapColumn.AsCity(editAble,FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_NATIVE)
            {
                //籍贯
                sbFormGroup.AppendFormat(_fapColumn.AsNative(editAble,FieldValue.ToString()));
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_STAR)
            {
                //评星级
                sbFormGroup.AppendLine(_fapColumn.AsRate(editAble, FieldValue.ToString()));
            }
            else
            {
                sbFormGroup.AppendLine(_fapColumn.AsTextBox(editAble, FieldValue.ToString()));
            }
        }

        public string BuilderFreeForm(string frmid, FormStatus frmStatus)
        {
            if (frmStatus == FormStatus.View)
            {
                string ctrlName = _fapColumn.ColName;
                return $"<label id=\"{ctrlName}\" >{BuilderFormFieldText()} </label>";
            }
            else
            {
                string lableName = _multiLangService.GetLangColumnComent(_fapColumn);
                string ctrlName = _fapColumn.ColName;
                //angularjs的时候使用，目前没用
                string ngModel = "formData." + _fapColumn.ColName;
                //是否可编辑
                string editAble = string.Empty;
                if (!IsEnable && (_fapColumn.EditAble == 0 || ReadOnly))
                {
                    editAble = "disabled='disabled'";
                }
                string notnullclass = "";
                if (_fapColumn.NullAble == 0)
                {
                    notnullclass = "has-error";
                }
                StringBuilder sbFormGroup = new StringBuilder();
                sbFormGroup.AppendFormat("<div class=\"ctrlcontainer form-group {0}\" style=\"position: relative;margin:2px 0px\">", notnullclass);
                BuilderDataFormField(frmid, lableName, editAble, sbFormGroup);
                sbFormGroup.AppendLine("</div>");
                //return BuilderFreeFormFieldControl(frmid);
                return sbFormGroup.ToString();
            }
        }
        /// <summary>
        /// 构造字段文本
        /// </summary>
        /// <returns></returns>
        private string BuilderFormFieldText()
        {
            if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
            {
                var dicts = _dataAccessor.Dictionarys(_fapColumn.RefTable);
                if (_fapColumn.MultiAble == 1)
                {
                    //多选
                    List<string> sels = new List<string>();
                    GetMultiSelTexts(sels);
                    return string.Join(",", sels);
                }
                else
                {
                    var dic = dicts.FirstOrDefault(d => d.Code == FieldValue.ToString());
                    if (dic != null)
                    {
                        return dic.Name;
                    }
                    //单选
                    return "";
                }
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOXLIST)
            {
                //多选
                List<string> sels = new List<string>();
                GetMultiSelTexts(sels);
                return string.Join(",", sels);
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                return FieldMCValue.ToStringOrEmpty();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_IMAGE)
            {
                StringBuilder img = new StringBuilder();
                //img.AppendLine("<span class=\"profile-picture\">");
                img.AppendFormat("       <img   class=\"img-responsive\"  src=\"{0}\" />", "/Home/UserPhoto/" + FieldValue).AppendLine();
                //img.AppendLine("   </span>");
                return img.ToString();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_FILE)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Bid", FieldValue);
                var attList = _dataAccessor.QueryWhere<FapAttachment>("bid=@Bid", param);
                if (attList != null && attList.Any())
                {
                    string li = string.Join("", attList.Select(a => $"<li class=\"text-info\"><a href=\"/PublicCtrl/DownloadFile/{a.Fid}\" title=\"{a.FileName}\"><i class=\"ace-icon fa fa-paperclip blue bigger-110\"></i>{ a.FileName}</a></li>"));
                    return $"<ul class=\"list-unstyled list-inline\">{li}</ul>";
                }
                return "";
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
            {
                string _value = FieldValue.ToStringOrEmpty();
                return _value == "1" ? "<i class=\"fa fa-check\"></i>" : "<i class=\"fa fa-times\"></i>";
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATE)
            {
                string _value = FieldValue.ToStringOrEmpty();
                return _value.Length > 10 ? _value.Substring(0, 10) : _value;
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
            {
                string _value = FieldValue.ToStringOrEmpty();
                return _value.Length > 19 ? _value.Substring(0, 19) : _value;
            }
            else
            {
                return FieldValue.ToStringOrEmpty();
            }
        }
        /// <summary>
        /// 构造自由表单字段控件
        /// </summary>
        /// <param name="frmid"></param>
        /// <returns></returns>
        [Obsolete("作废方法，并入BuilderDataFormField")]
        private string BuilderFreeFormFieldControl(string frmid)
        {
            string ctrlName = _fapColumn.ColName;
            //angularjs的时候使用，目前没用
            string ngModel = "formData." + _fapColumn.ColName;
            //是否可编辑
            string editAble = string.Empty;
            if (_fapColumn.EditAble == 0 || ReadOnly)
            {
                editAble = "disabled='disabled'";
            }
            string notnullclass = "";
            if (_fapColumn.NullAble == 0)
            {
                notnullclass = "has-error";
            }
            StringBuilder sbFormGroup = new StringBuilder();
            sbFormGroup.AppendFormat("<div class=\"ctrlcontainer form-group {0}\" style=\"position: relative;margin:2px 0px\">", notnullclass);
            if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATE)
            {
                string dateFormat = "yyyy-mm-dd";
                if (_fapColumn.DisplayFormat.IsPresent())
                {
                    dateFormat = _fapColumn.DisplayFormat;
                }
                if (FieldValue.ToString().Length > 10)
                {
                    FieldValue = FieldValue.ToString().Substring(0, 10);
                }
                sbFormGroup.AppendLine("<span class=\"input-icon input-icon-right\">");
                sbFormGroup.AppendFormat("    <input class=\"form-control date-picker\" id=\"{0}\" " + editAble + " name=\"{0}\" ng-model=\"{1}\" value=\"{2}\" type=\"text\" data-dateformat=\"{3}\" />", ctrlName, ngModel, FieldValue, dateFormat).AppendLine();
                sbFormGroup.AppendLine("                      <i class=\"ace-icon fa fa-calendar block\" style=\"right: 5px;\"></i>");
                sbFormGroup.AppendLine("                 </span>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
            {
                sbFormGroup.AppendLine("<span class=\"input-icon input-icon-right\">");
                sbFormGroup.AppendFormat("                     <input class=\"form-control datetime-picker\"  id=\"{0}\" name=\"{0}\" " + editAble + " ng-model=\"{1}\" type=\"text\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
                //sbFormGroup.AppendLine("                    <span class=\"input-group-addon\">");
                sbFormGroup.AppendLine("                        <i class=\"ace-icon fa fa-clock-o block\" style=\"right: 5px;\"></i>");
                sbFormGroup.AppendLine("                    </span>");
                // sbFormGroup.AppendLine("                </div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
            {
                //if (string.IsNullOrWhiteSpace(_fapColumn.RefTable))
                //{
                //    throw new FapException("下拉框表没设置！");
                //}
                //sbFormGroup.AppendFormat(" <select id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" " + editAble + " class=\"form-control\">", ctrlName, ngModel).AppendLine();
                //sbFormGroup.AppendLine("<option value=''>--请选择--</option>");
                //DataAccessor coms = new DataAccessor();
                //IEnumerable<FapDict> dicList = coms.GetDictList(_fapColumn.RefTable);

                //if (dicList.Any())
                //{
                //    foreach (FapDict dict in dicList)
                //    {
                //        if (dict.Code == FieldValue.ToString())
                //        {
                //            sbFormGroup.AppendLine("<option value='" + dict.Code + "' selected>" + dict.Name + "</option>");
                //        }
                //        else
                //        {
                //            sbFormGroup.AppendLine("<option value='" + dict.Code + "' >" + dict.Name + "</option>");
                //        }
                //    }
                //}
                //sbFormGroup.AppendLine("</select>");
                if (string.IsNullOrWhiteSpace(_fapColumn.RefTable))
                {
                    Guard.Against.Null(_fapColumn.RefTable, "字典未设置");
                }
                List<string> tempList = new List<string>();
                if (_fapColumn.MultiAble == 1)
                {
                    if (FieldValue != null && FieldValue.ToString().Length > 0)
                    {
                        //获取多选项的值
                        GetMultiSelValues(tempList);
                    }
                }
                IEnumerable<FapDict> dicList = _dataAccessor.Dictionarys(_fapColumn.RefTable);
                if (_fapColumn.MultiAble == 0)
                {
                    sbFormGroup.AppendFormat(" <select id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" " + editAble + " class=\"form-control \" data-placeholder=\"请选择\" >", ctrlName, ngModel).AppendLine();
                    sbFormGroup.AppendLine("<option value=''></option>");
                    if (dicList.Any())
                    {
                        foreach (FapDict dict in dicList)
                        {
                            if (dict.Code == FieldValue.ToString())
                            {
                                sbFormGroup.AppendLine("<option value='" + dict.Code + "' selected>" + dict.Name + "</option>");
                            }
                            else
                            {
                                sbFormGroup.AppendLine("<option value='" + dict.Code + "' >" + dict.Name + "</option>");
                            }
                        }
                    }
                    sbFormGroup.AppendLine("</select>");
                }
                else
                {
                    sbFormGroup.AppendFormat("<div class=\"control-group form-horizontal checkboxlist\"  data-ctrlName='{0}'>", ctrlName);
                    if (dicList.Any())
                    {
                        foreach (FapDict dict in dicList)
                        {
                            if (tempList.Contains(dict.Code))
                            {
                                sbFormGroup.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" checked class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                            }
                            else
                            {
                                sbFormGroup.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                            }
                        }
                    }
                    sbFormGroup.AppendFormat("</div>");
                }
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOXLIST)
            {
                sbFormGroup.AppendFormat("<div class=\"control-group form-horizontal checkboxlist\"  data-ctrlName='{0}'>", ctrlName);
                List<string> tempList = new List<string>();

                if (FieldValue != null && FieldValue.ToString().Length > 0)
                {
                    //获取多选项的值
                    GetMultiSelValues(tempList);
                }
                string where = "";
                if (_fapColumn.RefCondition.IsPresent())
                {
                    where = " where " + _fapColumn.RefCondition;
                }
                string sql = string.Format("select {0} Code,{1} Name from {2} {3}", _fapColumn.RefID, _fapColumn.RefName, _fapColumn.RefTable, where);
                var dlist = _dataAccessor.Query(sql);
                if (dlist.Any())
                {
                    foreach (dynamic dict in dlist)
                    {
                        if (tempList.Contains(dict.Code))
                        {
                            sbFormGroup.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" checked class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                        }
                        else
                        {
                            sbFormGroup.AppendFormat(@"<label>
														<input name='{0}' type='checkbox' ng-model='{1}' value='{2}' data-text='{3}' " + editAble + @" class='ace' />
														<span class='lbl'> {3}</span>
													</label>", ctrlName, ngModel, dict.Code, dict.Name);
                        }
                    }
                }
                sbFormGroup.AppendFormat("</div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                string ngModelMC = ngModel + "MC";
                string ctrlNameMC = frmid + ctrlName + "MC";
                sbFormGroup.AppendLine("<span class=\"input-icon input-icon-right\">");
                sbFormGroup.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"  class=\"form-control hidden\" />", ctrlName, ngModel, FieldValue).AppendLine();
                sbFormGroup.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" " + editAble + " value=\"{2}\"  class=\"form-control\" />", ctrlNameMC, ngModelMC, FieldMCValue).AppendLine();
                //sbFormGroup.AppendLine("   <span class=\"input-group-addon b\">");
                if (editAble.IsMissing())
                {
                    sbFormGroup.AppendLine("       <i class=\"ace-icon fa fa-search block\" style=\"right: 5px;\"></i>");
                }
                sbFormGroup.AppendLine("    </span>");
                //sbFormGroup.AppendLine(" </div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_MEMO)
            {
                string rows = "8";
                if (_fapColumn.DisplayFormat.IsPresent())
                {
                    rows = _fapColumn.DisplayFormat;
                }
                if (_fapColumn.ColLength < 99999 && _fapColumn.ColType == FapColumn.COL_TYPE_STRING)
                {
                    sbFormGroup.AppendLine($"<textarea id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" { editAble } rows=\"{rows}\"  class=\"form-control autosize-transition limited\" maxlength=\"{_fapColumn.ColLength}\">{FieldValue}</textarea>");
                }
                else
                {
                    sbFormGroup.AppendLine($"<textarea id=\"{ctrlName}\" name=\"{ctrlName}\" ng-model=\"{ngModel}\" { editAble } rows=\"{rows}\"   class=\"form-control autosize-transition\">{FieldValue}</textarea>");
                }
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
            {
                string required = " data-nullable=1 ";
                if (_fapColumn.NullAble == 0 && _fapColumn.ShowAble == 1)
                {
                    required = " data-nullable=0 ";
                }
                sbFormGroup.AppendFormat("<div class=\"form-control wysiwyg-editor\" name=\"{0}\" id=\"{0}\" {1}>{2}</div>", ctrlName, required, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
            {
                sbFormGroup.AppendFormat("<div class=\"control-group form-horizontal\" id=\"{0}\">", ctrlName).AppendLine();
                //内容动态添加
                string chk = "";
                if (string.IsNullOrWhiteSpace(_fapColumn.RefTable))
                {
                    if (FieldValue.ToString() == "1")
                    {
                        chk = "checked";
                    }
                    sbFormGroup.AppendLine("<label>");
                    sbFormGroup.AppendFormat("<input name=\"{0}\" id=\"{0}\" value=\"1\" {1} {2} class=\"form-control ace\" type=\"checkbox\">", ctrlName, chk, editAble).AppendLine();
                    sbFormGroup.AppendLine("<span class=\"lbl\"></span>");
                    sbFormGroup.AppendLine("</label>");
                }
                else
                {
                    IEnumerable<FapDict> dicList = _dataAccessor.Dictionarys(_fapColumn.RefTable);
                    if (dicList.Any())
                    {
                        List<string> selCodes = new List<string>();
                        if (!string.IsNullOrWhiteSpace(FieldValue.ToString()))
                        {
                            foreach (var strC in FieldValue.ToString().Split(','))
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
                            sbFormGroup.AppendLine("<label>");
                            sbFormGroup.AppendFormat("<input  name=\"{0}\" type=\"checkbox\" value=\"{1}\" {2} {3} class=\"form-control ace\" />", ctrlName, dict.Code, chk, editAble).AppendLine();
                            sbFormGroup.AppendFormat("<span class=\"lbl\"> {0}</span>", dict.Name);
                            sbFormGroup.AppendLine("</label>");
                        }
                    }
                }
                sbFormGroup.AppendLine("</div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_EMAIL)
            {
                sbFormGroup.AppendFormat("<input type=\"email\" class=\"form-control\" id=\"{0}\" " + editAble + " name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_FILE)
            {
                //dialog js中生成
                //sbFormGroup.AppendLine("<div id=\"dialog-file"+ctrlName+"\" class=\"hide\">");
                //sbFormGroup.AppendLine("   <div class=\"row\">  <div  class=\"col-lg-12\">");
                //sbFormGroup.AppendFormat("<input type=\"file\" class=\"file-loading\"  id=\"{0}\" name=\"{0}\" multiple   />", ctrlName + "-FILE").AppendLine();
                //sbFormGroup.AppendLine("   </div></div>");
                //sbFormGroup.AppendLine("</div>");
                sbFormGroup.AppendFormat("<button id=\"file{0}{1}\" " + editAble + " class=\"btn btn-info\" type=\"button\"><i class=\"ace-icon fa fa-paperclip\"></i>附件</button>", frmid, ctrlName).AppendLine();
                sbFormGroup.AppendFormat("<input type=\"text\" class=\"form-control attachment hidden\"  id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\" data-text=\"{3}\" data-nullable={4} data-editable={5} />", ctrlName, ngModel, FieldValue, FieldComment, _fapColumn.NullAble, _fapColumn.EditAble).AppendLine();
                sbFormGroup.AppendFormat("<label class=\"text-muted\">支持文件格式({0})</label>", _fapColumn.FileSuffix.IsMissing()? "*.*" : _fapColumn.FileSuffix).AppendLine();

            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_IMAGE)
            {
                //sbFormGroup.AppendFormat("<button id=\"file{0}{1}\" " + editAble + " class=\"btn btn-info\" type=\"button\"><i class=\"ace-icon fa fa-paperclip\"></i>附件</button>", frmid, ctrlName).AppendLine();
                //sbFormGroup.AppendFormat("<input type=\"text\" class=\"form-control attachment hidden\"  id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\" data-text=\"{3}\" data-nullable={4} data-editable={5} />", ctrlName, ngModel, FieldValue, FieldComment, _fapColumn.NullAble, _fapColumn.EditAble).AppendLine();
                //sbFormGroup.AppendFormat("<label class=\"text-muted\">支持文件格式({0})</label>", _fapColumn.FileSuffix.IsNullOrEmpty() ? "*.*" : _fapColumn.FileSuffix).AppendLine();
                sbFormGroup.AppendLine("<span class=\"profile-picture\">");
                sbFormGroup.AppendFormat("       <img id=\"avatar{0}\" data-pk=\"{1}\" style=\"width:160px;\" class=\"editable img-responsive\"  src=\"{2}\" />", ctrlName, FieldValue, "/Home/UserPhoto/" + FieldValue).AppendLine();
                sbFormGroup.AppendLine("   </span>");
                sbFormGroup.AppendFormat("    <input type=\"text\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"  class=\"form-control hidden\" />", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_PASSWORD)
            {
                sbFormGroup.AppendFormat("<input type=\"password\" " + editAble + " class=\"form-control\"  id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_INT || _fapColumn.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || _fapColumn.CtrlType == FapColumn.CTRL_TYPE_MONEY)
            {
                sbFormGroup.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
            }
            //else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_DOUBLE)
            //{
            //    sbFormGroup.AppendFormat("<input type=\"text\" class=\"form-control input-mini-int\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\" />", ctrlName, ngModel, FieldValue).AppendLine();
            //}
            //else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_MONEY)
            //{
            //    sbFormGroup.AppendFormat("<input type=\"text\" class=\"form-control input-mini-int\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
            //}
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_PHONE)
            {
                sbFormGroup.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_RADIO)
            {
                sbFormGroup.AppendFormat("<div class=\"control-group\" id=\"{0}\" name=\"{0}\">", ctrlName, ngModel).AppendLine();
                //内容动态添加
                sbFormGroup.AppendLine("</div>");
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_TEXT)
            {
                if (_fapColumn.IsMultiLang == 1)
                {
                    //多語字段處理
                    string currCtrlName = ctrlName;
                    if (_multiLangService.CurrentLanguage != MultiLanguageEnum.ZhCn)
                    {
                        currCtrlName = ctrlName + _multiLangService.CurrentLanguageName;
                        FieldValue = FormData.Get(_fapColumn.TableName + "_" + currCtrlName);
                        if (FieldValue == null)
                        {
                            FieldValue = "";
                        }
                    }
                    sbFormGroup.AppendLine("<span class=\"input-icon input-icon-right\">");
                    sbFormGroup.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", currCtrlName, ngModel,HtmlEncoder.Default.Encode(FieldValue.ToString())).AppendLine();
                    sbFormGroup.AppendFormat("<i class=\"ace-icon fa fa-language green\" data-fid=\"{0}\"></i></span>", _fapColumn.Fid);
                    var langList = EnumExtensions.GetDictionary(typeof(MultiLanguageEnum));
                    //style="top: 26px; left: 155.656px; display: block;"
                    sbFormGroup.AppendFormat("<div class=\"popover popovermultilang fade right in \" role=\"tooltip\" id=\"{0}\" ><div class=\"arrow\" style=\"top: 50%;\"></div><h3 class=\"popover-title\"><i class=\"ace-icon fa fa-language green\"></i><button type=\"button\" class=\"multilangpopoverclose close red\" ><i class=\"ace-icon fa fa-times\"></i></button> 多语言</h3><div class=\"popover-content\">", _fapColumn.Fid);
                    foreach (var langField in langList)
                    {
                        if (langField.Description == _multiLangService.CurrentLanguage.GetDescription())
                        {
                            continue;
                        }
                        string cname = ctrlName;
                        if (langField.Value != MultiLanguageEnum.ZhCn.ToString())
                        {
                            cname = ctrlName + langField.Value;
                        }
                        string value = FormData.Get(_fapColumn.TableName + "_" + cname);
                        sbFormGroup.AppendFormat("<input type=\"text\" id=\"{0}\" placeholder=\"{1}\" class=\"form-control col-xs-12 col-sm-12\" value=\"{2}\" style=\"margin:1px\" {3}/>", cname, langField.Description, value, editAble);
                    }
                    sbFormGroup.Append("</div></div>");
                }
                else
                {
                    sbFormGroup.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel,HtmlEncoder.Default.Encode(FieldValue.ToString())).AppendLine();
                }
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_CITY)
            {
                //城市
                sbFormGroup.AppendFormat("<input   type=\"text\" " + editAble + "  class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\">", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_NATIVE)
            {
                //籍贯
                sbFormGroup.AppendFormat("<input  readonly type=\"text\" " + editAble + "  class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\">", ctrlName, ngModel, FieldValue).AppendLine();
            }
            else if (_fapColumn.CtrlType == FapColumn.CTRL_TYPE_STAR)
            {
                //评星级
                sbFormGroup.AppendFormat("<div id=\"rat-{0}\" class=\"rating inline\"></div><input type=\"text\" " + editAble + "  class=\"form-control hide\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\">", ctrlName, ngModel, FieldValue);
            }
            else
            {
                sbFormGroup.AppendFormat("<input type=\"text\" " + editAble + " class=\"form-control\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\" value=\"{2}\"/>", ctrlName, ngModel,HtmlEncoder.Default.Encode(FieldValue.ToString())).AppendLine();
            }
            sbFormGroup.AppendLine("</div>");


            return sbFormGroup.ToString();
        }

        private void GetMultiSelValues(List<string> tempList)
        {
            //demo data:id:name;id:name;id:name...
            string[] datas = FieldValue.ToString().Split(';');
            if (datas.Length > 0)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    string[] data = datas[i].Split(':');
                    if (data.Length > 0)
                    {
                        tempList.Add(data[0]);
                    }
                }
            }
        }
        private void GetMultiSelTexts(List<string> tempList)
        {
            //demo data:id:name;id:name;id:name...
            string[] datas = FieldValue.ToString().Split(';');
            if (datas.Length > 0)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    string[] data = datas[i].Split(':');
                    if (data.Length > 1)
                    {
                        tempList.Add(data[1]);
                    }
                }
            }
        }
    }
}
