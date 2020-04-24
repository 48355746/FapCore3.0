using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Extensions;
using Dapper;
using System.Linq;
using Fap.Core.Utility;
using System.Text.RegularExpressions;
using System.Web;
using Fap.AspNetCore.Model;
using System.IO;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;

namespace Fap.AspNetCore.Controls.DataForm
{
    internal abstract class BaseForm : HtmlString
    {
        protected IWebHostEnvironment _env;
        //表单formid
        protected string FormId { get; set; }
        //data fid
        protected string FidValue { get; set; }
        //表单数据
        protected FapDynamicObject FormData { get; set; }
        protected FapTable _fapTable;
        protected IEnumerable<FapColumn> _fapColumns;
        protected List<FapField> formFields = new List<FapField>();
        #region childtables 
        //存在模板上的字段
        protected List<FapField> _existTemplateFields = new List<FapField>();
        //子表是否只读
        protected bool childGridReadOnly { get; set; }
        //模板上的子表
        protected Dictionary<string, FapTable> _childTableList = new Dictionary<string, FapTable>();
        //子表默认值集合
        protected IEnumerable<SubTableDefaultValue> _subTableListDefaultData;
        #endregion
        //设置自定义默认值
        protected Dictionary<string, string> _cutomDefault = new Dictionary<string, string>();
        //是否为单据
        protected bool IsDocument = false;
        protected FormStatus _formStatus = FormStatus.Add;
        protected IDbContext _dbContext;
        protected IFapApplicationContext _applicationContext;
        protected IMultiLangService _multiLangService;
        protected IRbacService _rbacService;
        protected IServiceProvider _serviceProvider;
        protected string _formTemplate = string.Empty;
        public BaseForm(IServiceProvider serviceProvider, string id, FormStatus formStatus = FormStatus.Add) : base("")
        {
            _dbContext = serviceProvider.GetService<IDbContext>();
            _formStatus = formStatus;
            _multiLangService = serviceProvider.GetService<IMultiLangService>();
            _rbacService = serviceProvider.GetService<IRbacService>(); ;
            _applicationContext = serviceProvider.GetService<IFapApplicationContext>();
            _serviceProvider = serviceProvider;
            _env = serviceProvider.GetService<IWebHostEnvironment>();
            FormId = id;
        }
        /// <summary>
        /// 设置子表是否只读
        /// </summary>
        /// <param name="gridReadonly"></param>
        public void SetGridReadonly(bool gridReadonly)
        {
            childGridReadOnly = gridReadonly;
        }
        public BaseForm SetFormStatus(FormStatus formStatus)
        {
            _formStatus = formStatus;
            return this;
        }
        public BaseForm SetQueryOption(QuerySet querySet)
        {
            _fapTable = _dbContext.Table(querySet.TableName);
            DynamicParameters parameters = new DynamicParameters();
            querySet.Parameters.ForEach(q => parameters.Add(q.ParamKey, q.ParamValue));
            var frmData = _dbContext.QueryFirstOrDefault(querySet.ToString(), parameters, true);
            _fapColumns = _dbContext.Columns(querySet.TableName);
            if (!querySet.QueryCols.EqualsWithIgnoreCase("*"))
            {
                var queryColList = querySet.QueryCols.ToLower().SplitComma();
                _fapColumns = _dbContext.Columns(querySet.TableName).Where(c => queryColList.Contains(c.ColName.ToLower()));
            }
            if (frmData != null)
            {
                FormData = (frmData as IDictionary<string, object>).ToFapDynamicObject(_fapColumns);
                if (_formStatus != FormStatus.View)
                {
                    _formStatus = FormStatus.Edit;
                }
            }
            else
            {
                FormData = _dbContext.GetDefualtData(querySet.TableName);
                _formStatus = FormStatus.Add;
            }
            if (_fapTable.TableFeature != null && _fapTable.TableFeature.Contains("BillFeature"))
            {
                IsDocument = true;
            }
            FidValue = FormData.Get("Fid").ToString();
            if (_fapColumns.Any())
            {
                SetFapClumns(querySet);
            }
            return this;
        }
        /// <summary>
        /// 设置列，包括权限部分
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public BaseForm SetFapClumns(QuerySet querySet)
        {
            if (_fapColumns.Any())
            {
                #region 权限
                //只读字段              
                string[] readOnlyCols = Array.Empty<string>();
                if (querySet.ReadOnlyCols.IsPresent())
                {
                    readOnlyCols = querySet.ReadOnlyCols.Split(',');
                }
                #endregion

                foreach (var col in _fapColumns.OrderBy(c => c.ColOrder))
                {
                    //参照MC为自定义列
                    if (col.IsCustomColumn == 1)
                    {
                        continue;
                    }
                    string key = col.ColName;
                    var fv = FormData.Get(key);
                    object fvc = null;
                    if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                    {
                        fvc = FormData.Get(key + "MC");
                    }
                    //设置自定义默认值
                    if (_cutomDefault.Any() && _cutomDefault.ContainsKey(col.ColName))
                    {
                        fv = _cutomDefault[col.ColName];
                    }
                    if (_cutomDefault.Any() && _cutomDefault.ContainsKey(col.ColName + "MC"))
                    {
                        fvc = _cutomDefault[col.ColName + "MC"];
                    }
                    FapField frmField = new FapField(_dbContext, _multiLangService) { FormData = FormData, FieldName = col.ColName, FieldComment = col.ColComment, CurrentColumn = col, FieldGroup = col.ColGroup.IsMissing() ? "默认分组" : col.ColGroup, FieldValue = (fv == null ? "" : fv), FieldMCValue = (fvc == null ? "" : fvc) };
                    #region 权限（只读可编辑）判断
                    if (readOnlyCols.Any() && readOnlyCols.Contains(col.ColName, new Fap.Core.Utility.FapStringEqualityComparer()))
                    {
                        frmField.ReadOnly = true;
                    }
                    #endregion
                    formFields.Add(frmField);

                }
            }
            return this;
        }
        /// <summary>
        /// 设置表单模板
        /// </summary>
        /// <param name="formTemplate"></param>
        /// <returns></returns>
        public BaseForm SetFromTemplate(string formTemplate)
        {
            _formTemplate = formTemplate;
            return this;
        }
        /// <summary>
        /// 这个要写在前面，否则不起作用
        /// </summary>
        /// <param name="dicCustomDefaultData"></param>
        /// <returns></returns>
        public BaseForm SetCustomDefaultData(Dictionary<string, string> dicCustomDefaultData)
        {
            _cutomDefault = dicCustomDefaultData;
            return this;
        }
        public BaseForm SetSubTableListDefualtData(IEnumerable<SubTableDefaultValue> subTableListDefualtData)
        {
            _subTableListDefaultData = subTableListDefualtData;
            return this;
        }
        public string CreateHiddenControl(string ctrlName, string fieldValue)
        {
            string ngModel = "formData." + ctrlName;
            return string.Format("<input type=\"text\" class=\"form-control hidden\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"/>", ctrlName, ngModel, fieldValue);
        }
        protected string RenderJavascript()
        {
            StringBuilder script = new StringBuilder();
            script.Append(" $(function () {").AppendLine();
            //多语弹出框关闭
            script.AppendLine(" $(\".multilangpopoverclose\").on(ace.click_event, function () {     $(this).closest(\".popover\").css(\"display\",\"none\");  });");
            //参照弹出事件、上传附件、数值控件
            //开启textarea 校验
            bool hasTextArea = false;
            //开启jqueryvalidate校验的条件
            bool needValidate = false;
            foreach (FapField field in formFields)
            {
                FapColumn column = field.CurrentColumn;
                if (column.ColName == "Id" || column.ColName == "Fid" || column.ShowAble == 0)
                {
                    continue;
                }
                if (!hasTextArea && column.CtrlType == FapColumn.CTRL_TYPE_MEMO)
                {
                    hasTextArea = true;
                }
                if (!needValidate && ((column.NullAble == 0 && column.ShowAble == 1) || column.RemoteChkURL.IsPresent()))
                {
                    needValidate = true;
                }
                #region 日期
                if (column.CtrlType == FapColumn.CTRL_TYPE_DATE)
                {
                    string model = "0";
                    string dateFormat = column.DisplayFormat;
                    string minDate = "1900-1-1";
                    string maxDate = "2999-12-12";
                    if (dateFormat.IsMissing())
                    {
                        dateFormat = "yyyy-mm-dd";
                        model = "0";
                    }
                    else if (dateFormat.Length > 4 && dateFormat.Length < 10)
                    {
                        dateFormat = "yyyy-mm";
                        model = "1";
                    }
                    else
                    {
                        dateFormat = "yyyy";
                        model = "2";
                    }
                    if (column.MinValue != 0)
                    {
                        minDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MinValue));
                    }
                    if (column.MaxValue != 0)
                    {
                        maxDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MaxValue));
                    }
                    if (dateFormat == "yyyy-mm-dd")
                    {
                        script.AppendLine(" $(\"###formid## #" + column.ColName + "\").scroller('destroy').scroller($.extend({preset:'date', minDate:moment('" + minDate + "').toDate(),maxDate:moment('" + maxDate + "').toDate()},{ theme: 'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                    else
                    {
                        script.AppendLine("$(\"###formid## #" + column.ColName + "\").datePicker({  followOffset: [0, 24],onselect:function(date){ var formatDate = DatePicker.formatDate(date, '" + dateFormat + "');this.shell.val(formatDate).change();  return false;},minDate:'" + minDate + "',maxDate:'" + maxDate + "', altFormat:'" + dateFormat + "',showMode:" + model + " }).next().on(ace.click_event, function () {$(this).prev().focus(); });");

                    }
                }
                else if (column.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
                {
                    //moment.js的格式
                    //string format = column.DisplayFormat;
                    //string startDate = "1900-1-1";
                    //string endDate = "2999-12-12";
                    //string startView = "2";
                    //if (column.MinValue.HasValue)
                    //{
                    //    startDate = DateTime.Now.AddDays((column.MinValue).ToDouble()).ToString(PublicUtils.DateFormat);
                    //}
                    //if (column.MaxValue.HasValue)
                    //{
                    //    endDate = DateTime.Now.AddDays((column.MaxValue).ToDouble()).ToString(PublicUtils.DateFormat);
                    //}
                    //if (format.IsMissing())
                    //{
                    //    format = "yyyy-mm-dd hh:ii:ss";
                    //}
                    //else if (format.EqualsWithIgnoreCase("HH:mm"))
                    //{
                    //    format = "hh:ii";
                    //    startView = "0";
                    //}


                    //script.AppendLine(" $(\"#" + column.ColName + "\").datetimepicker({ format:\"" + format + "\",startDate:'" + startDate + "',endDate:'" + endDate + "',startView:" + startView + ",todayBtn:true,todayHighlight:true, language: \"zh-CN\" }).next().on(ace.click_event, function () {            $(this).prev().focus();        });");

                    string format = column.DisplayFormat;
                    string startDate = "1900-1-1";
                    string endDate = "2999-12-12";

                    if (column.MinValue != 0)
                    {
                        startDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MinValue));
                    }
                    if (column.MaxValue != 0)
                    {
                        endDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MaxValue));
                    }
                    if (format.IsMissing())
                    {
                        //format = "datetime";
                        //script.AppendLine("opt.datetime = { preset : 'datetime', minDate:moment('" + startDate + "').toDate(), maxDate: minDate:moment('" + endDate + "').toDate() , stepMinute: 5  };");
                        script.AppendLine(" $(\"###formid## #" + column.ColName + "\").scroller('destroy').scroller($.extend({ preset:'datetime', minDate:moment('" + startDate + "').toDate(), maxDate:moment('" + endDate + "').toDate() , stepMinute: 5  }, { theme:'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                    else if (format.EqualsWithIgnoreCase("HH:mm"))
                    {
                        format = "time";
                        //script.AppendLine("opt.time = {preset : 'time'};");
                        script.AppendLine(" $(\"###formid## #" + column.ColName + "\").scroller('destroy').scroller($.extend({preset:'time'}, { theme: 'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                }
                #endregion

                #region 参照
                else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && !field.ReadOnly)
                {
                    //去除自定义列
                    if (column.IsCustomColumn == 1)
                        continue;
                    //编码已经和地址一样了
                    string refUrl = column.RefType;
                    //if (column.RefType == "GRID")
                    //{
                    //    refUrl = "GridReference";
                    //}
                    //else if (column.RefType == "TREE")
                    //{
                    //    refUrl = "TreeReference";
                    //}
                    //else
                    //{
                    //    refUrl = "TreeGridReference";
                    //}
                    string dispalyName = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.FapColumn, $"{column.TableName}_{column.ColName}");
                    script.AppendLine("$(\"###formid## #" + column.ColName + "MC\").next().on(ace.click_event, function(){");
                    //script.AppendLine("//不可编辑字段不能弹出");
                    script.AppendLine(" if($(this).prev().attr(\"disabled\")==\"disabled\"){return;}");
                    //扩展参考值，参照参数用
                    script.AppendLine("var extra=[];");

                    //针对某些参照要用表单上的控件数据
                    if (column.RefCondition.IsPresent())
                    {
                        //DeptUid='${DeptUid}',@后面为表单上的控件
                        string fieldName = "";
                        string pattern = FapPlatformConstants.VariablePattern;
                        Regex regex = new Regex(pattern);
                        var mat = regex.Matches(column.RefCondition);
                        foreach (Match item in mat)
                        {
                            int length = item.ToString().Length - 3;
                            fieldName = item.ToString().Substring(2, length);
                            //fieldName = item.Groups[1].ToString();

                            FapColumn col = _fapColumns.FirstOrDefault(f => f.ColName.EqualsWithIgnoreCase(fieldName));
                            if (col != null)
                            {
                                script.AppendLine("var conv=$('#" + fieldName + "').val();if(conv==''){bootbox.alert('【" + _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.FapColumn, $"{col.TableName}_{col.ColName}") + "】为空，请先设置。');return;}");
                                script.AppendLine("extra.push('" + fieldName + "='+conv)");
                            }
                        }
                    }
                    script.AppendLine("loadRefMessageBox('" + dispalyName + "','##formid##','" + column.Fid + "','" + column.ColName + "','" + refUrl + "',extra)");
                    script.AppendLine("});");
                    script.AppendLine("$(\"###formid## #" + column.ColName + "MC\").on(ace.click_event,function(e){$(this).next().trigger(ace.click_event);e.preventDefault();});");
                }
                #endregion

                #region 附件

                else if (column.CtrlType == FapColumn.CTRL_TYPE_FILE)
                {
                    script.AppendLine("$(\"###formid## #file" + FormId + column.ColName + "\").on(ace.click_event, function () {");
                    string tempFid = UUIDUtils.Fid;
                    if (field.FieldValue.ToString().IsMissing())
                    {
                        field.FieldValue = tempFid;
                    }
                    script.AppendLine("loadFileMessageBox('" + tempFid + "','##formid##',initFile" + FormId.Replace('-', '_') + tempFid + ");");

                    script.AppendLine("});");
                    string allowExt = string.Empty;
                    if (column.FileSuffix.IsPresent())
                    {
                        List<string> suffix = column.FileSuffix.SplitComma();
                        if (suffix.Any())
                        {
                            allowExt = string.Join(",", suffix.Select(s => "'" + s + "'").ToList());
                        }
                    }
                    //建立初始化附件控件js函数
                    script.AppendLine("var initFile" + FormId.Replace('-', '_') + tempFid + "=function(){");
                    script.AppendLine("$(\"###formid##" + tempFid + "-FILE\").fileinput({");
                    script.AppendLine("language: language,");
                    script.AppendLine($"uploadUrl:\"{ _applicationContext.BaseUrl }/Component/UploadFile/{ field.FieldValue }\",");
                    //script.AppendLine("deleteUrl:\"http://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  + "/Core/Api/deletefile\",");
                    if (allowExt.IsPresent())
                    {
                        script.AppendLine($"allowedFileExtensions : [{ allowExt }],");
                    }
                    if (column.FileSize > 0)
                    {
                        script.AppendLine($"maxFileSize: {column.FileSize},");
                    }
                    script.AppendLine("uploadExtraData:{fid:'" + field.FieldValue + "'},");
                    script.AppendLine("allowedPreviewTypes: ['image', 'text'],");
                    script.AppendLine($"maxFileCount:{(column.FileCount == 0 ? 1 : column.FileCount)},");
                    script.AppendLine("showUpload: true,");
                    //script.AppendLine("showCaption: false,");
                    //script.AppendLine("overwriteInitial: false,");                                 
                    script.AppendLine("slugCallback: function(filename) {");
                    script.AppendLine(" return filename.replace('(', '_').replace(']', '_');");
                    script.AppendLine("},");
                    //浏览按钮样式
                    //script.AppendLine("browseClass: \"btn btn-primary btn-block\",");
                    //浏览按钮图标
                    script.AppendLine("previewFileIcon: \"<i class='glyphicon glyphicon-king'></i>\"})");
                    //script.AppendLine(@".on('fileloaded', function(event, file, previewId, index, reader) {
                    //    var files =$(this).fileinput('getFileStack');
                    //    $(this).fileinput('uploadSingle',index,files,false);})");
                    script.AppendLine(@".on('fileuploaded', function (event, data, previewId, index) {  if(data.response.success==false){bootbox.alert(data.response.msg);}else{loadFileList('" + FormId + "', '" + column.ColName + "', '" + field.FieldValue.ToString() + "');                } });       ");
                    script.AppendLine("}");
                    if (_formStatus == FormStatus.Edit)
                    {
                        string bid = field.FieldValue.ToString();
                        script.AppendLine("loadFileList('" + FormId + "','" + column.ColName + "','" + bid + "');");
                    }
                }
                #endregion

                #region 图片头像
                else if (column.CtrlType == FapColumn.CTRL_TYPE_IMAGE && !field.ReadOnly)
                {
                    if (field.FieldValue.ToString().IsMissing())
                    {
                        field.FieldValue = UUIDUtils.Fid;
                    }
                    script.AppendLine("loadImageControl('avatar" + column.ColName + "')");
                }
                #endregion

                #region 富文本控件
                else if (column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
                {
                    script.AppendLine("var wysiwyg_" + column.ColName + " =$(\"###formid## #" + column.ColName + ".wysiwyg-editor\").ace_wysiwyg({" + @"
						toolbar:
						[
                            'font',
			                null,
			                'fontSize',
			                null,
							'bold',
							'italic',
							'strikethrough',
							'underline',
							null,
                            'insertunorderedlist',
                            'insertorderedlist',
                            'outdent',
                            'indent', 
                            null,
							'justifyleft',
							'justifycenter',
							'justifyright',
							null,
							'createLink',
							'unlink',
							null,
                            'insertImage',
                            null,
                            'foreColor',
		                	null,
							'undo',
							'redo',
                            null,
						'viewSource'
						]
					}).prev().addClass('wysiwyg-style1');");
                    if (column.EditAble == 0)
                    {
                        script.AppendLine("$('#"+ column.ColName+"').attr('contenteditable', false);");
                    }
                    //script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + "\").html('" + StringUtil.TextToHtml(field.FieldValue.ToString()) + "');");
                }
                #endregion

                #region 数值控件
                else if (column.CtrlType == FapColumn.CTRL_TYPE_INT || column.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || column.CtrlType == FapColumn.CTRL_TYPE_MONEY)
                {
                    if (column.EditAble != 1)
                        continue;
                    int min = column.MinValue;
                    int max = column.MaxValue;
                    if (min == 0 && max == 0)
                    {
                        min = int.MinValue;
                        max = int.MaxValue;
                    }
                    string unit = "";
                    if (column.CtrlType == FapColumn.CTRL_TYPE_MONEY)
                    {
                        unit = " postfix: '￥'";
                    }
                    string step = "1";
                    int precision = column.ColPrecision;
                    if (precision > 0)
                    {
                        step = "0." + "1".PadLeft(precision, '0');
                    }
                    script.AppendLine(" $(\"###formid## input[name='" + column.ColName + "']\").TouchSpin({");
                    script.AppendLine(@"
                        min: " + min + @",
                        max: " + max + @",
                        step: " + step + @",
                        decimals: " + precision + @",
                        boostat: 5,
                        maxboostedstep: 10,
                     
                        " + unit + @"
                    });
                   ");

                }
                #endregion

                #region 数值范围
                else if (column.CtrlType == FapColumn.CTRL_TYPE_RANGE)
                {
                    StringBuilder sr = new StringBuilder("[");
                    for (int i = column.MinValue; i <= column.MaxValue; i++)
                    {
                        sr.Append($"{i},");
                    }
                    string r = sr.ToString().TrimEnd(',');
                    r += "]";
                    script.AppendLine(@"$('###formid## #" + column.ColName + @"').jRange({
                        from: " + column.MinValue + @",
                        to: " + column.MaxValue + @",
                        step: 1,
                        scale: " + r + @",
                        format: '%s',
                        width: 680,
                        disable:" + ((column.EditAble == 0 || field.ReadOnly) ? "true," : "false,") + @"
                        theme:'theme-blue',
                        showLabels: true,
                        isRange: true
                    });");
                }
                #endregion

                #region 籍贯

                else if (column.CtrlType == FapColumn.CTRL_TYPE_NATIVE)
                {
                    //籍贯
                    script.AppendLine("$(\"###formid## #" + column.ColName + "\").citypicker();");
                }

                #endregion

                #region 城市

                else if (column.CtrlType == FapColumn.CTRL_TYPE_CITY)
                {
                    //城市
                    script.AppendLine("$(\"###formid## #" + column.ColName + "\").cityselect();");
                }

                #endregion
                #region 评星级
                else if (column.CtrlType == FapColumn.CTRL_TYPE_STAR)
                {
                    if (field.FieldValue?.ToString().IsMissing() ?? true)
                    {
                        field.FieldValue = "0";
                    }
                    //评星级
                    script.AppendLine("if(!$(\"###formid## #" + column.ColName + "\").prop(\"disabled\")){ $(\"###formid## #rat-" + column.ColName + "\").raty({number: 5,score:" + field.FieldValue +
                    @", cancel: true,  'starType' : 'i',
                    'click': function(score,evt) {" +
                        "$(\"###formid## #" + column.ColName + "\").val(score);" +
                    @"},					
                    })}else{" + "$(\"###formid## #rat-" + column.ColName + "\").raty({number: 5,score:" + field.FieldValue +
                    @",  'starType' : 'i',readOnly: true			
                    })}");
                }
                #endregion
                #region 多語
                else if (column.CtrlType == FapColumn.CTRL_TYPE_TEXT && column.IsMultiLang == 1)
                {
                    string oriCtrl = column.ColName;

                    string ctrmultiLang = oriCtrl + _multiLangService.CurrentLanguageName;

                    script.AppendLine("$(\"###formid## #" + oriCtrl + "\").on(\"blur\",function(){$(\"###formid## #" + ctrmultiLang + "\").val($(this).val())}).next().on(ace.click_event, function(){");
                    script.AppendLine(" document.addEventListener(\"mousedown\", onMultiLangPoverMouseDown, false);");
                    script.AppendLine("var fid=$(this).data(\"fid\");");
                    script.AppendLine("var X1 = $(\"###formid## #" + oriCtrl + "\").offset().top-55;var Y1 =$(\"###formid## #" + oriCtrl + "\").offset().left;");
                    script.AppendLine("var bg=$(\"#\"+fid).closest(\".modal-lg\");var top=X1;var left=Y1");
                    script.AppendLine("if(bg){ var bgo=bg.offset();   top=X1-bgo.top;left=Y1-bgo.left;}");
                    script.AppendLine("$(\"#\"+fid).css({\"position\": \"fixed\",\"display\":\"inline-grid\",\"top\":top+'px',\"left\":left+'px'});");
                    script.AppendLine("})");
                }
                #endregion
            }
            #region 多语公共js
            if (formFields.Exists(f => f.CurrentColumn.IsMultiLang == 1))
            {
                //关闭按钮事件
                script.AppendLine(" $(\".multilangpopoverclose\").on(ace.click_event, function () {     $(this).closest(\".popover\").css(\"display\",\"none\");  });");
                script.AppendLine(@"function multiLangPoverClose() {" +
                                        @"$('.popovermultilang').css('display','none');" +
                                        "document.removeEventListener(\"mousedown\", onMultiLangPoverMouseDown, false);}" +
                                    "function onMultiLangPoverMouseDown(event) {" +
                                        "if (!(event.target.className.indexOf(\"popovermultilang\")>0 || $(event.target).parents(\".popovermultilang\").length > 0)) {" +
                                            "multiLangPoverClose();" +
                                    "}}");
            }
            #endregion

            #region TextArea
            if (hasTextArea)
            {
                script.AppendLine(@"$('textarea.limited').inputlimiter({
					remText: '%n 字符剩余...',
					limitText: '最大字符数 : %n.'
				});");
            }
            #endregion

            #region 表单校验

            if (needValidate)
            {
                //校验
                script.AppendLine("$('###formid##').validate({");
                script.AppendLine("		errorElement: 'div',");
                script.AppendLine("		errorClass: 'error',");
                script.AppendLine("		focusInvalid: false,");
                script.AppendLine("		ignore: \"\",");
                script.AppendLine("		rules: {");
                foreach (FapColumn col in _fapColumns)
                {
                    //非空可见
                    if ((col.NullAble == 0 && col.ShowAble == 1) || col.RemoteChkURL.IsPresent())
                    {
                        if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            script.AppendLine("                " + col.ColName + "MC" + ": {");
                        }
                        else
                        {
                            script.AppendLine("             " + col.ColName + ": {");
                        }
                        if (col.NullAble == 0 && col.ShowAble == 1)
                        {
                            script.AppendLine("				required: true,");
                        }
                        if (col.RemoteChkURL.IsPresent())
                        {
                            string oriValue = FormData.Get(col.ColName).ToString();
                            script.AppendLine("				remote: '" + _applicationContext.BaseUrl + col.RemoteChkURL + "&fid=" + HttpUtility.UrlEncode(FidValue) + "&orivalue=" + HttpUtility.UrlEncode(oriValue) + "&currcol=" + HttpUtility.UrlEncode(col.ColName) + "',");
                        }
                        script.AppendLine("			},");
                    }

                }


                script.AppendLine("			},");

                script.AppendLine("		messages: {");

                foreach (FapColumn col in _fapColumns)
                {
                    //非空可见
                    if ((col.NullAble == 0 && col.ShowAble == 1) || col.RemoteChkURL.IsPresent())
                    {
                        if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            script.AppendLine("                " + col.ColName + "MC" + ": {");
                        }
                        else
                        {
                            script.AppendLine("                " + col.ColName + ": {");
                        }
                        if (col.NullAble == 0 && col.ShowAble == 1)
                        {
                            script.AppendLine("				required: \"[" + col.ColComment + "]必须填写！\",");
                        }
                        if (col.RemoteChkURL.IsPresent())
                        {
                            string msg = col.RemoteChkMsg;
                            if (msg.IsMissing())
                            {
                                msg = "[" + col.ColComment + "]此项值已经存在，请更换";
                            }
                            script.AppendLine("				remote: \"" + msg + "\",");
                        }
                        script.AppendLine("			},");
                    }

                }
                script.AppendLine("		},");
                //校验容器
                script.AppendLine("errorLabelContainer: $(\"###formid## div.error\"),");
                script.AppendLine("		highlight: function (e) {");
                script.AppendLine("			$(e).closest('.form-group').removeClass('has-info').addClass('has-error');");
                script.AppendLine("		},");

                script.AppendLine("		success: function (e) {");
                script.AppendLine("			$(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');");
                script.AppendLine("			$(e).remove();");
                script.AppendLine("		},");

                script.AppendLine("		errorPlacement: function (error, element) {");
                script.AppendLine("			if(element.is('input[type=checkbox]') || element.is('input[type=radio]')) {");
                script.AppendLine("				var controls = element.closest('div[class*=\"col-\"]');");
                script.AppendLine("				if(controls.find(':checkbox,:radio').length > 1) controls.append(error);");
                script.AppendLine("				else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));");
                script.AppendLine("			}");
                //script.AppendLine("			else if(element.is('.select2')) {");
                //script.AppendLine("				error.insertAfter(element.siblings('[class*=\"select2-container\"]:eq(0)'));");
                //script.AppendLine("			}");
                script.AppendLine("			else if(element.is('.chosen-select')) {");
                script.AppendLine("				error.insertAfter(element.siblings('[class*=\"chosen-container\"]:eq(0)'));");
                script.AppendLine("			}");
                script.AppendLine("			else error.insertAfter(element.parent());");
                script.AppendLine("		},");

                script.AppendLine("		submitHandler: function (form) {");
                script.AppendLine("		},");
                script.AppendLine("		invalidHandler: function (form) {");
                script.AppendLine("		}");
                script.AppendLine("	});");

            }
            #endregion

            #region 表单联动脚本
            DynamicParameters pm = new DynamicParameters();
            pm.Add("TableName", _fapTable.TableName);
            var formInjections = _dbContext.QueryWhere<FapFormInjection>("TableName=@TableName and IsEnabled=1", pm);
            if (formInjections != null && formInjections.Any())
            {
                foreach (var inject in formInjections)
                {
                    var changCol = _fapColumns.FirstOrDefault(f => f.ColName == inject.ChangeColumn);
                    //可见可编辑
                    if (changCol != null && changCol.EditAble == 1 && changCol.ShowAble == 1)
                    {
                        string ctrlName = changCol.ColName;

                        if (changCol.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            ctrlName = changCol.ColName + "MC";
                        }
                        script.AppendLine("$('#" + ctrlName + "').change(function(){");
                        string jsonData = "{'Fid':'" + inject.Fid + "','" + changCol.ColName + "':$('#" + changCol.ColName + "').val()";
                        if (inject.ParamColumns.IsPresent())
                        {
                            var paramCols = inject.ParamColumns.SplitComma();
                            foreach (var pc in paramCols)
                            {
                                jsonData += ",'" + pc + "':$('#" + pc + "').val()";
                            }
                        }
                        jsonData += "}";
                        script.AppendLine("$.post(basePath+'/Core/Api/frminjection'," + jsonData + ",function(result){");
                        script.AppendLine("$.each(result,function(name,value) {");
                        script.AppendLine("$('#'+name).val(value)");
                        script.AppendLine("});");
                        script.AppendLine("});");
                        script.AppendLine("})");

                    }
                }
            }

            #endregion

            #region 注入script脚本
            //元数据表注入
            if (_fapTable.ScriptInjection.IsPresent())
            {
                script.AppendLine(_fapTable.ScriptInjection);
            }
            //js文件注入
            string jsfilePath = Path.Combine(new string[] { Directory.GetCurrentDirectory(), "wwwroot", "Scripts", "FapFormPlugin", $"frm.plugin.{_fapTable.TableName}.js" });
            if (File.Exists(jsfilePath))
            {
                script.AppendLine(File.ReadAllText(jsfilePath, Encoding.UTF8));
            }
            #endregion

            script.AppendLine(" });");
            #region 子表js注入
            //存在子表时注入js
            if (_childTableList.Any() && _formStatus != FormStatus.View && !childGridReadOnly)
            {
                foreach (var childtb in _childTableList)
                {
                    jsfilePath = Path.Combine(new string[] { Directory.GetCurrentDirectory(), "wwwroot", "Scripts", "FapFormPlugin", $"frm.plugin.grid.{childtb.Value.TableName}.js" });
                    if (File.Exists(jsfilePath))
                    {
                        script.AppendLine(File.ReadAllText(jsfilePath, Encoding.UTF8));
                    }
                }
            }

            #endregion

            return script.ToString();
        }

        protected string RenderBaseControl()
        {
            //id,fid,ts,tablename
            StringBuilder builder = new StringBuilder();
            string idv = formFields.FirstOrDefault(f => f.FieldName == FapDbConstants.FAPCOLUMN_FIELD_Id).FieldValue.ToString();
            string fidv = formFields.FirstOrDefault(f => f.FieldName == FapDbConstants.FAPCOLUMN_FIELD_Fid).FieldValue.ToString();
            string tsv = formFields.FirstOrDefault(f => f.FieldName == FapDbConstants.FAPCOLUMN_FIELD_Ts).FieldValue.ToString();
            builder.AppendLine(CreateHiddenControl(FapDbConstants.FAPCOLUMN_FIELD_Id, idv));
            builder.AppendLine(CreateHiddenControl(FapDbConstants.FAPCOLUMN_FIELD_Fid, fidv));
            builder.AppendLine(CreateHiddenControl(FapDbConstants.FAPCOLUMN_FIELD_Ts, tsv));
            //builder.AppendLine(CreateHiddenControl(FapWebConstants.FORM_TABLENAME, _fapTable.TableName));
            return builder.ToString();
        }
        /// <summary>
        /// 渲染form内容
        /// </summary>
        /// <returns></returns>
        protected abstract string RenderFormContent();
        protected string RenderHtmlElements()
        {
            StringBuilder formHtml = new StringBuilder();
            if (!_fapColumns.Any())
            {
                return "未发现元数据";
            }
            formHtml.AppendLine("<form class=\"form-horizontal\" method=\"post\" id=\"##formid##\" role=\"form\">");
            formHtml.AppendLine(RenderBaseControl());
            formHtml.AppendLine(RenderFormContent());
            //formToken 用于防止重复提交
            string avoidRepeatToken = UUIDUtils.Fid;
            formHtml.AppendLine(CreateHiddenControl(FapWebConstants.AVOID_REPEAT_TOKEN, avoidRepeatToken));
            //XSRF/CSRF 防护
            IAntiforgery antiforgery = ActivatorUtilities.GetServiceOrCreateInstance<IAntiforgery>(_serviceProvider);
            var context = _serviceProvider.GetService<IHttpContextAccessor>();
            IHtmlContent antiforeryHtml = antiforgery.GetHtml(context.HttpContext);
            var writer = new System.IO.StringWriter();
            antiforeryHtml.WriteTo(writer, HtmlEncoder.Default);
            formHtml.AppendLine(writer.ToString());
            //放验证信息
            formHtml.AppendLine("<div class=\"error\">				</div>");
            formHtml.AppendLine("</form>");
            formHtml.AppendLine("<div class=\"row\">");
            formHtml.AppendLine("<div class=\"col-xs-12 col-sm-6\" id=\"frm-result\"></div>");
            formHtml.AppendLine("</div>");
            formHtml.Replace("##formid##", $"frm-{FormId}");

            return formHtml.ToString();
        }
        protected string ToHtmlString()
        {
            return ToString();
        }
    }
    public enum FormStatus
    {
        Add = 1, Edit = 2, View = 3

    };
}
