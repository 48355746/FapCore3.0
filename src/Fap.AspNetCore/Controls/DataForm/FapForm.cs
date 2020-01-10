using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls.DataForm
{
    public class FapForm : HtmlString
    {
        private int _colCount = 2;
        private string _tableName;
        private FormStatus _formStatus = FormStatus.Add;
        private FapTable _tb;
        private IEnumerable<FapColumn> _fapColumns = new List<FapColumn>();
        private List<FapField> _fapFields = new List<FapField>();
        //设置自定义默认值
        private Dictionary<string, string> _cutomDefault = new Dictionary<string, string>();
        private FapDynamicObject FormData { get; set; }
        /// <summary>
        /// 表单Fid的值
        /// </summary>
        private string FidValue { get; set; }       
        //表单id
        public string Id { get; set; }
        
        //是否为单据
        private bool IsDocument = false;

        private IDbContext _dbContext;
        private IFapApplicationContext _applicationContext;
        private IMultiLangService _multiLangService;
        private IRbacService _rbacService;
        //子表默认值集合
        private IEnumerable<SubTableDefaultValue> _subTableListDefaultData;
        private IServiceProvider _serviceProvider;
        /// <summary>
        /// 如果id为jqgriddataform的时候 formid改为frm-表名
        /// </summary>
        /// <param name="id"></param>
        public FapForm(IServiceProvider serviceProvider, string id, FormStatus formStatus = FormStatus.Add) : base("")
        {
            _dbContext = serviceProvider.GetService<IDbContext>();
            _formStatus = formStatus;
            _multiLangService = serviceProvider.GetService<IMultiLangService>();
            _rbacService = serviceProvider.GetService<IRbacService>(); ;
            _applicationContext = serviceProvider.GetService<IFapApplicationContext>();
            _serviceProvider = serviceProvider;
            this.Id = id;
        }
        public FapForm SetColCount(int colCount)
        {
            _colCount = colCount;
            return this;
        }
        /// <summary>
        /// 设置表单状态
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public FapForm SetFormStatus(FormStatus fs)
        {
            _formStatus = fs;
            return this;
        }
        /// <summary>
        /// 这个要写在前面，否则不起作用
        /// </summary>
        /// <param name="dicCustomDefaultData"></param>
        /// <returns></returns>
        public FapForm SetCustomDefaultData(Dictionary<string, string> dicCustomDefaultData)
        {
            _cutomDefault = dicCustomDefaultData;
            return this;
        }
        public FapForm SetSubTableListDefualtData(IEnumerable<SubTableDefaultValue> subTableListDefualtData)
        {
            _subTableListDefaultData = subTableListDefualtData;
            return this;
        }
        /// <summary>
        /// 设置列，包括权限部分
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public FapForm SetFapClumns(QuerySet querySet)
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

                foreach (var col in _fapColumns)
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
                    FapField frmField = new FapField(_dbContext, _multiLangService) { FormData = FormData, CurrentColumn = col, FieldGroup = col.ColGroup.IsMissing() ? "默认分组" : col.ColGroup, FieldValue = (fv == null ? "" : fv), FieldMCValue = (fvc == null ? "" : fvc) };
                    #region 权限（只读可编辑）判断
                    if (readOnlyCols.Any() &&readOnlyCols.Contains(col.ColName,new Fap.Core.Utility.FapStringEqualityComparer()))
                    {
                        frmField.ReadOnly = true;
                    }
                  
                    #endregion
                    _fapFields.Add(frmField);

                }
            }
            return this;
        }
        public FapForm SetQueryOption(QuerySet qs)
        {
            _tableName = qs.TableName;
            _tb = _dbContext.Table(qs.TableName);
            DynamicParameters parameters = new DynamicParameters();
            qs.Parameters.ForEach(q => parameters.Add(q.ParamKey, q.ParamValue));
            var frmData = _dbContext.QueryFirstOrDefault(qs.ToString(), parameters, true);
            _fapColumns = _dbContext.Columns(qs.TableName);
            if (!qs.QueryCols.EqualsWithIgnoreCase("*"))
            {
                var queryColList = qs.QueryCols.ToLower().SplitComma();
                _fapColumns = _dbContext.Columns(qs.TableName).Where(c => queryColList.Contains(c.ColName.ToLower()));
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
                FormData = _dbContext.GetDefualtData(qs.TableName);
                _formStatus = FormStatus.Add;
            }
            if (_tb.TableFeature != null && _tb.TableFeature.Contains("BillFeature"))
            {
                IsDocument = true;
            }
            FidValue = FormData.Get("Fid").ToString();
            if (_fapColumns.Any())
            {
                SetFapClumns(qs);             
            }
            return this;

        }
        public override string ToString()
        {
            if (_formStatus == FormStatus.View)
            {
                return RenderHtmlElements();
            }
            else
            {
                // Create javascript
                var script = new StringBuilder();
                // Start script
                script.AppendLine("<script type=\"text/javascript\">");
                //压缩js
                JavaScriptCompressor compressor = new JavaScriptCompressor();
                compressor.Encoding = Encoding.UTF8;
                script.Append(compressor.Compress(RenderJavascript()));
                script.AppendLine("</script>");

                // Insert grid id where needed (in columns)
                script.Replace("##formid##", $"frm-{Id}");

                // Return script + required elements
                return script.ToString() + RenderHtmlElements();
            }

        }
        private string TableName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tableName))
                {
                    return _fapColumns.First().TableName;
                }
                return _tableName;
            }
        }
        private string RenderHtmlElements()
        {
            StringBuilder formHtml = new StringBuilder();

            //不能直接加载参照对话框，因为会重复添加，解决方案：用js去加载
            //参照弹出层
            //formHtml.AppendLine("<div id=\"dialog-reference-"+TableName+"\" class=\"hide\">");
            //formHtml.AppendLine("   <div class=\"row\">  <div id=\"refContent-" + TableName + "\" class=\"col-lg-12\">");
            ////formHtml.AppendLine("    <iframe width=\"100%\" height=\"100%\" frameborder=\"0\" style=\"border:none 0;\" allowtransparency=\"true\" id=\"_DialogFrame_Ref\" src=\"\"></iframe>");
            //formHtml.AppendLine("   </div></div>");
            //formHtml.AppendLine("</div>");
            if (!_fapColumns.Any())
            {
                return "未发现元数据";
            }
            //是否存在附件
            var existFile = _fapColumns.Any(f => f.CtrlType == FapColumn.CTRL_TYPE_FILE || f.CtrlType == FapColumn.CTRL_TYPE_IMAGE);
            if (existFile)
            {
                formHtml.AppendLine("<form class=\"form-horizontal\" enctype=\"multipart/form-data\" method=\"post\" id=\"##formid##\" role=\"form\">");

            }
            else
            {
                formHtml.AppendLine("<form class=\"form-horizontal\" method=\"post\" id=\"##formid##\" role=\"form\">");
            }
            var grpFields = _fapFields.GroupBy(f => f.FieldGroup);            
            
            foreach (var item in grpFields)
            {
                if (grpFields.Count() != 1 && item.Key != "默认分组")
                {
                    formHtml.AppendFormat("<h4 class=\"header smaller lighter blue\">{0}</h4>", item.Key).AppendLine();
                }
                int i = 0;
                foreach (var column in item.ToList())
                {
                    //Id,Fid,Ts这三列要隐藏
                    if (column.CurrentColumn.ColName == FapDbConstants.FAPCOLUMN_FIELD_Id || column.CurrentColumn.ColName == FapDbConstants.FAPCOLUMN_FIELD_Fid || column.CurrentColumn.ColName == FapDbConstants.FAPCOLUMN_FIELD_Ts)
                    {
                        formHtml.AppendLine(CreateHiddenControl(column.CurrentColumn.ColName, column.FieldValue.ToString()));
                        continue;
                    }
                    else if (!_cutomDefault.ContainsKey(column.CurrentColumn.ColName))
                    {
                        //表单显示排除自定义列和不可见列，参照列会在表单中处理
                        if (column.CurrentColumn.IsCustomColumn == 1 || (column.CurrentColumn.ShowAble == 0))
                            continue;
                    }
                    //自定义赋默认值的字段存在的时候且不显示，也要设置隐藏，例如：人员子集中的 EmpUid
                    if (_cutomDefault.ContainsKey(column.CurrentColumn.ColName) && column.CurrentColumn.ShowAble == 0)
                    {
                        formHtml.AppendLine(CreateHiddenControl(column.CurrentColumn.ColName, column.FieldValue.ToString()));
                        continue;
                    }

                    //单据特殊要处理
                    if (IsDocument)
                    {   //提交时间,当前审批人,审批时间,审批意见,单据状态,生效状态,生效时间
                        string[] billCols = { "SubmitTime", "CurrApprover", "ApprovalTime", "ApprovalComments", "BillStatus", "EffectiveState" };//, "EffectiveTime" };
                        if (billCols.Contains(column.CurrentColumn.ColName))
                        {
                            continue;
                        }
                    }                   
                    bool isColspan = IsColSpan(column.CurrentColumn);
                    //MEMO占一行，遇到提前换行，再生成一个group(当不该换行的时候遇到MEMO要加入换行，否则正常换行)
                    if (i % _colCount != 0 && isColspan)
                    {
                        formHtml.AppendLine("</div>");
                        i = 0;
                    }
                    if (i % _colCount == 0)
                    {
                        string style = _formStatus == FormStatus.View ? "row" : "form-group";

                        formHtml.AppendLine($"<div class=\"{style}\">");
                    }

                    formHtml.AppendLine(column.BuilderForm(Id, _colCount, isColspan, _formStatus));
                    i++;
                    //MEMO占一行，遇到提前换行
                    if (isColspan)
                    {
                        formHtml.AppendLine("</div>");
                        i = 0;
                        continue;
                    }
                    if (i % _colCount == 0)
                    {
                        formHtml.AppendLine("</div>");
                    }
                }
                //当出来的时候没有闭合div这里闭合
                if (i % _colCount != 0)
                {
                    formHtml.AppendLine("</div>");
                }

            }

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
            formHtml.Replace("##formid##", $"frm-{Id}");

            return formHtml.ToString();
        }
        /// <summary>
        /// 是否colspan占一行
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool IsColSpan(FapColumn column)
        {
            return column.CtrlType == FapColumn.CTRL_TYPE_MEMO || column.CtrlType == FapColumn.CTRL_TYPE_IMAGE || column.CtrlType == FapColumn.CTRL_TYPE_FILE || column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX || column.CtrlType == FapColumn.CTRL_TYPE_NATIVE;
        }
        private string CreateHiddenControl(string ctrlName, string fieldValue)
        {
            string ngModel = "formData." + ctrlName;
            return string.Format("<input type=\"text\" class=\"form-control hidden\" id=\"{0}\" name=\"{0}\" ng-model=\"{1}\"  value=\"{2}\"/>", ctrlName, ngModel, fieldValue);
        }
        private string RenderJavascript()
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
            foreach (FapField field in _fapFields)
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
                    if (column.MinValue.HasValue)
                    {
                        minDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MinValue));
                    }
                    if (column.MaxValue.HasValue)
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

                    if (column.MinValue.HasValue)
                    {
                        startDate = DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MinValue));
                    }
                    if (column.MaxValue.HasValue)
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
                else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE &&!field.ReadOnly)
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
                    string dispalyName = _multiLangService.GetLangColumnComent(column);
                    script.AppendLine("$(\"###formid## #" + column.ColName + "\"+\"MC\").next().on(ace.click_event, function(){");
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
                                script.AppendLine("var conv=$('#" + fieldName + "').val();if(conv==''){bootbox.alert('【" + _multiLangService.GetLangColumnComent(col) + "】为空，请先设置。');return;}");
                                script.AppendLine("extra.push('" + fieldName + "='+conv)");
                            }
                        }
                    }
                    script.AppendLine("loadRefMessageBox('" + dispalyName + "','##formid##','" + column.Fid + "','" + column.ColName + "','" + refUrl + "',extra)");
                    script.AppendLine("});");
                    script.AppendLine("$(\"###formid## #" + column.ColName + "\"+\"MC\").on(ace.click_event,function(e){$(this).next().trigger(ace.click_event);e.preventDefault();})");
                }
                #endregion

                #region 附件

                else if (column.CtrlType == FapColumn.CTRL_TYPE_FILE)
                {
                    script.AppendLine("$(\"###formid## #file" + Id + column.ColName + "\").on(ace.click_event, function () {");
                    string tempFid = UUIDUtils.Fid;
                    if (field.FieldValue.ToString().IsMissing())
                    {
                        script.AppendLine("alert('数据导入的时候没有生成附件值，不能上传附件！')");
                    }
                    else
                    {
                        script.AppendLine("loadFileMessageBox('" + tempFid + "','##formid##',initFile" + Id + tempFid + ");");
                    }
                    script.AppendLine("})");
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
                    script.AppendLine("var initFile" + Id + tempFid + "=function(){");
                    script.AppendLine("$(\"###formid##" + tempFid + "-FILE\").fileinput({");
                    script.AppendLine("language: 'zh',");
                    script.AppendLine("uploadUrl:\"" + _applicationContext.BaseUrl + "/Api/Core/uploadfile/" + field.FieldValue + "\",");
                    //script.AppendLine("deleteUrl:\"http://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  + "/Api/Core/deletefile\",");
                    if (allowExt.IsPresent())
                    {
                        script.AppendLine("allowedFileExtensions : [" + allowExt + "],");
                    }
                    else
                    {
                        script.AppendLine("fileType: \"any\",");
                    }
                    script.AppendLine("uploadExtraData:{fid:'" + field.FieldValue + "'},");
                    script.AppendLine("allowedPreviewTypes: ['image', 'text'],");
                    script.AppendLine(@"");
                    script.AppendLine("showUpload: true,");
                    script.AppendLine("showCaption: false,");
                    script.AppendLine("overwriteInitial: false,");
                    //List<FapAttachment> attList = null;
                    //预览
                    //if (_formStatus == FormStatus.Edit)
                    //{
                    //    string bid = field.FieldValue.ToString();
                    //    DynamicParameters parameters = new DynamicParameters();
                    //    parameters.Add("Bid", bid);
                    //    attList = _dbContext.QueryEntityByWhere<FapAttachment>("Bid=@Bid", parameters);
                    //    if (attList.Any())
                    //    {
                    //        StringBuilder initPre = new StringBuilder();
                    //        StringBuilder initPreC = new StringBuilder();
                    //        initPre.AppendLine("initialPreview: [");
                    //        initPreC.AppendLine("initialPreviewConfig: [");
                    //        attList.ForEach(a =>
                    //        {
                    //            initPre.AppendLine(" \"<img style='height:160px' src='" + _session.BaseURL + "/PublicCtrl/AttachmentImg/" + a.Fid + "'>\",");
                    //            initPreC.AppendLine("{caption: \"" + a.FileName + "\", width: \"120px\", url: \"" + _session.BaseURL + "/Api/Core/deletefile\", key: \"" + a.Fid + "\"},");
                    //        });
                    //        initPreC.AppendLine("],");
                    //        initPre.AppendLine(" ],");
                    //        script.AppendLine(initPre.ToString());
                    //        script.AppendLine(initPreC.ToString());
                    //    }
                    //}                  
                    script.AppendLine("slugCallback: function(filename) {");
                    script.AppendLine(" return filename.replace('(', '_').replace(']', '_');");
                    script.AppendLine("},");
                    //浏览按钮样式
                    script.AppendLine("browseClass: \"btn btn-primary\",");
                    //浏览按钮图标
                    script.AppendLine("previewFileIcon: \"<i class='glyphicon glyphicon-king'></i>\"");
                    script.AppendLine(@"}).on('fileloaded', function(event, file, previewId, index, reader) {
                       //$(this).fileinput('upload');
                       // alert(index);
                        var files =$(this).fileinput('getFileStack');
                        $(this).fileinput('uploadSingle',index,files,false);
                    }).on('fileuploaded', function (event, data, previewId, index) {  if(data.response.success==false){bootbox.alert('上传失败：'+data.response.msg);}else{loadFileList('" + Id + "', '" + column.ColName + "', '" + field.FieldValue.ToString() + "');                } });       ");
                    script.AppendLine("}");
                    //if (attList != null && attList.Count > 0)
                    //{
                    //    string child = "<div class='col-sm-2'></div> <div class='col-xs-10 col-sm-10'> ";
                    //    child += "<ul class='attachment-list pull-left list-unstyled'>";
                    //    foreach (var file in attList)
                    //    {

                    //        child += " <li>";
                    //        child += "     <a href='#' class='formctrl attached-file' data-filefid='" + file.Fid + "' data-rel='tooltip' title='" + file.FileName + "'>";
                    //        if (file.FileType.Contains("image"))
                    //        {
                    //            child += "    <i class='ace-icon fa fa-file-image-o bigger-110 purple'></i>";

                    //        }
                    //        else if (file.FileType.Contains("word"))
                    //        {
                    //            child += "  <i class='ace-icon fa fa-file-word-o bigger-110 blue'></i>";
                    //        }
                    //        else if (file.FileType.Contains("excel"))
                    //        {
                    //            child += "  <i class='ace-icon fa fa-file-excel-o bigger-110 green'></i>";
                    //        }
                    //        else
                    //        {
                    //            child += " <i class='ace-icon fa fa-file-o bigger-110 orange'></i>";
                    //        }
                    //        child += "  <span class='attached-name'>" + file.FileName + "</span>";
                    //        child += " </a>";
                    //        child += " <span class='action-buttons'>";
                    //        child += "      <a href='" + _session.BaseURL + "/PublicCtrl/DownloadFile/" + file.Fid + "'>";
                    //        child += "       <i class='ace-icon fa fa-download bigger-125 blue'></i>";
                    //        child += "     </a>";
                    //        child += "  <a href='#' data-filefid='" + file.Fid + "' class='formctrl deletefile'>";
                    //        child += "      <i class='ace-icon fa fa-trash-o bigger-125 red'></i>";
                    //        child += " </a>";
                    //        child += "</span></li>";

                    //    }
                    //    child += " </ul></div>";
                    //    //script.AppendLine("$(\"#file" + _id + column.ColName + "\").parent().append(\"<span class='badge badge-yellow'>" + attList.Count + "</span>\")");
                    //    script.AppendLine("$(\"#frm-" + _id + " #file" + _id + column.ColName + "\").parent().parent().parent().append(\"" + child + "\")");
                    //}
                    if (_formStatus == FormStatus.Edit)
                    {
                        string bid = field.FieldValue.ToString();
                        script.AppendLine("loadFileList('" + Id + "','" + column.ColName + "','" + bid + "');");
                    }
                }
                #endregion

                #region 图片头像
                else if (column.CtrlType == FapColumn.CTRL_TYPE_IMAGE && !field.ReadOnly)
                {
                    script.AppendLine("loadImageControl('avatar" + column.ColName + "')");
                }
                #endregion

                #region 富文本控件
                else if (column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
                {
                    script.AppendLine("$(\"###formid## #" + column.ColName + ".wysiwyg-editor\").ace_wysiwyg({" + @"
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
                    //script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + "\").html('" + StringUtil.TextToHtml(field.FieldValue.ToString()) + "');");
                }
                #endregion

                #region 数值控件
                else if (column.CtrlType == FapColumn.CTRL_TYPE_INT || column.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || column.CtrlType == FapColumn.CTRL_TYPE_MONEY)
                {
                    if (column.EditAble != 1)
                        continue;
                    int min = -1000000000;
                    int max = 1000000000;
                    if (column.MinValue.HasValue)
                    {
                        min = column.MinValue.Value;
                    }
                    if (column.MaxValue.HasValue)
                    {
                        max = column.MaxValue.Value;
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
                    string ctrmultiLang = column.ColName;
                    if (_multiLangService.CurrentLanguage != MultiLanguageEnum.ZhCn)
                    {
                        ctrmultiLang = ctrmultiLang + _multiLangService.CurrentLanguageName;
                    }
                    script.AppendLine("$(\"###formid## #" + ctrmultiLang + "\").next().on(ace.click_event, function(){");
                    script.AppendLine(" document.addEventListener(\"mousedown\", onMultiLangPoverMouseDown, false);");
                    script.AppendLine("var fid=$(this).data(\"fid\");");
                    script.AppendLine("var X1 = $(\"###formid## #" + ctrmultiLang + "\").offset().top-55;var Y1 =$(\"###formid## #" + ctrmultiLang + "\").offset().left;");
                    script.AppendLine("var bg=$(\"#\"+fid).closest(\".modal-lg\");var top=X1;var left=Y1");
                    script.AppendLine("if(bg){ var bgo=bg.offset();   top=X1-bgo.top;left=Y1-bgo.left;}");
                    script.AppendLine("$(\"#\"+fid).css({\"position\": \"fixed\",\"display\":\"inline-grid\",\"top\":top+'px',\"left\":left+'px'});");
                    script.AppendLine("})");
                }
                #endregion
            }
            #region 多语公共js
            if (_fapFields.Exists(f => f.CurrentColumn.IsMultiLang == 1))
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
            pm.Add("TableName", TableName);
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
                        script.AppendLine("$.post(basePath+'/Api/Core/frminjection'," + jsonData + ",function(result){");
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
            if (_tb.ScriptInjection.IsPresent())
            {
                script.AppendLine(_tb.ScriptInjection);
            }
            //js文件注入
            string jsfilePath = Path.Combine(new string[] { Directory.GetCurrentDirectory(), "wwwroot", "Scripts", "FapFormPlugin", $"frm.plugin.{TableName}.js" });
            if (File.Exists(jsfilePath))
            {
                script.AppendLine(File.ReadAllText(jsfilePath, Encoding.UTF8));
            }
            #endregion

            script.AppendLine(" });");
            return script.ToString();
        }
        public string ToHtmlString()
        {
            return ToString();
        }

    }
    public enum FormStatus
    {
        Add = 1, Edit = 2, View = 3

    };
}

