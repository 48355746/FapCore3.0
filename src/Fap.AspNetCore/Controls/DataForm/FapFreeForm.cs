using Dapper;
using Fap.AspNetCore.Controls.JqGrid;
using Fap.AspNetCore.Controls.JqGrid.Enums;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Utility;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
/* ==============================================================================
* 功能描述：自由表单  
* 创 建 者：wyf
* 创建日期：2017-04-24 18:48:06
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yahoo.Yui.Compressor;
using static System.String;
namespace Fap.AspNetCore.Controls.DataForm
{
    public class FapFreeForm : HtmlString
    {
        private string _id;
        public CfgFreeForm FFrm { get; private set; }
        private IDbContext _dbContext;
        private IFapApplicationContext _applicationContext;
        private IMultiLangService _multiLangService;
        private FormStatus _formStatus = FormStatus.Add;
        private FapTable _tb;
        private List<FapColumn> _fapColumns = new List<FapColumn>();
        private List<FapField> _fapFields = new List<FapField>();
        //存在模板上的字段
        private List<FapField> _existTemplateFields = new List<FapField>();
        //模板上的子表
        private Dictionary<string, FapTable> _childTableList = new Dictionary<string, FapTable>();
        //设置自定义默认值
        private Dictionary<string, string> _cutomDefault = new Dictionary<string, string>();
        private dynamic FormData { get; set; }
        private string _formTemplate = string.Empty;
        /// <summary>
        /// 表单Fid的值
        /// </summary>
        private string FidValue { get; set; }
        private ILoggerFactory _loggerFactory;
        private ILogger<FapFreeForm> _logger;

        public FapFreeForm(IDbContext dataAccessor, ILoggerFactory loggerFactory,  IFapApplicationContext applicationContext, IMultiLangService multiLangService, string id, FormStatus frmStatus = FormStatus.Add) : base("")
        {
            _id = id;
            _formStatus = frmStatus;
            _dbContext = dataAccessor;
            _applicationContext = applicationContext;
            _multiLangService = multiLangService;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<FapFreeForm>();
        }
        /// <FapFreeForm>
        /// 这个要写在前面，否则不起作用
        /// </summary>
        /// <param name="dicCustomDefaultData"></param>
        /// <returns></returns>
        public FapFreeForm SetCustomDefaultData(Dictionary<string, string> dicCustomDefaultData)
        {
            _cutomDefault = dicCustomDefaultData;
            return this;
        }
        public FapFreeForm SetSubTableListDefualtData(IEnumerable<SubTableDefaultValue> subTableListDefualtData)
        {
            _subTableListDefaultData = subTableListDefualtData;
            return this;
        }
        /// <summary>
        /// 设置表单状态
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public FapFreeForm SetFormStatus(FormStatus fs)
        {
            _formStatus = fs;
            return this;
        }
        /// <summary>
        /// 设置表单模板
        /// </summary>
        /// <param name="formTemplate"></param>
        /// <returns></returns>
        public FapFreeForm SetFromTemplate(string formTemplate)
        {
            _formTemplate = formTemplate;
            return this;
        }
        private QuerySet _querySet;
        private bool _gridReadonly;
        //子表默认值设置
        private IEnumerable<SubTableDefaultValue> _subTableListDefaultData;

        public FapFreeForm SetQueryOption(QuerySet qs)
        {
            _querySet = qs;
            _tb =_dbContext.Table(qs.TableName);
            DynamicParameters parameters = new DynamicParameters();
            qs.Parameters.ForEach(q => parameters.Add(q.ParamKey, q.ParamValue));
            var frmData = _dbContext.QueryFirstOrDefault(qs.ToString(), parameters);
            var queryColList= qs.QueryCols.Split(',');
            IEnumerable<FapColumn> fapColumns =_dbContext.Columns(qs.TableName).Where(c=>queryColList.Contains(c.ColName));
            if (frmData!=null)
            {
                FormData =(frmData as IDictionary<string,object>).ToFapDynamicObject(qs.TableName);
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
            FidValue = FormData.Get("Fid");
            if (fapColumns.Any())
            {
                SetFapClumns(fapColumns.ToList());
                //如果id为jqgriddataform的时候 formid改为frm-表名
                if (_id.Equals("jqgriddataform", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    _id = fapColumns.First().TableName;
                }
            }
            //获取自由表单设置
            GetFreeFromSet(qs.TableName);
            return this;
      
        }
        /// <summary>
        /// 设置列，包括权限部分
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private void SetFapClumns(List<FapColumn> columns)
        {
            _fapColumns = columns;
            if (_fapColumns.Any())
            {

                foreach (var col in _fapColumns)
                {
                    //参照MC为自定义列
                    if (col.IsCustomColumn == 1)
                    {
                        continue;
                    }
                    string key =  col.ColName;
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
                    _fapFields.Add(new FapField(_dbContext,_multiLangService) { CurrentColumn = col, FieldName = col.ColName, FieldComment = col.ColComment, ReadOnly = col.EditAble == 0 ? true : false, FieldValue = (fv == null ? "" : fv), FieldMCValue = (fvc == null ? "" : fvc) });

                }
            }
        }

        /// <summary>
        /// 获取自由表单设置
        /// </summary>
        /// <param name="billTable"></param>
        private void GetFreeFromSet(string billTable)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("Table", billTable);

            if (_formTemplate.IsPresent())
            {
                FFrm = _dbContext.Get<CfgFreeForm>(_formTemplate);
            }
            else
            {
                FFrm = _dbContext.QueryFirstOrDefaultWhere<CfgFreeForm>("BillTable=@Table and Enabled=1", param);
            }
            if (FFrm == null)
                return;
            //得到模板中用到的字段
            List<string> existCols = new List<string>();
            //子表集合 
            List<string> childTables = new List<string>();
            string vPattern = FapPlatformConstants.VariablePattern;
            Regex reg = new Regex(vPattern);
            //匹配字段
            MatchCollection matchs = reg.Matches(FFrm.FFContent);
            if (matchs.Any())
            {
                foreach (var mtch in matchs)
                {
                    int length = mtch.ToString().Length - 3;
                    string colComment = mtch.ToString().Substring(2, length);
                    existCols.Add(colComment);
                }
            }
            string cPattern = FapPlatformConstants.CollectionPattern;
            reg = new Regex(cPattern);
            //匹配子表
            matchs = reg.Matches(FFrm.FFContent);
            if (matchs.Any())
            {
                var childTableList =_dbContext.Tables(t => t.ExtTable == _tb.TableName);
                foreach (var mtch in matchs)
                {
                    string childTableLabel = mtch.ToString().TrimStart(new char[] { '{', '{' }).TrimEnd(new char[] { '}', '}' });
                    var childTable = childTableList.FirstOrDefault(t => t.TableComment.Equals(childTableLabel));
                    if (childTable != null && !IsNullOrWhiteSpace(childTable.TableName))
                    {
                        if (!_childTableList.ContainsKey(mtch.ToString()))
                        {
                            _childTableList.Add(mtch.ToString(), childTable);
                        }
                        else
                        {
                            //替换第一个为空，始终保留一个
                            FFrm.FFContent = reg.Replace(FFrm.FFContent, "", 1, 0);
                            _logger.LogError("自由表单包含两个一样的子表设置");
                        }
                    }
                }
            }
            //模板中存在的列
            _existTemplateFields = _fapFields.Where(f => existCols.Contains(f.FieldComment)).ToList();
        }
        /// <summary>
        /// 设置子表是否只读
        /// </summary>
        /// <param name="gridReadonly"></param>
        internal void SetGridReadonly(bool gridReadonly)
        {
            this._gridReadonly = gridReadonly;
        }

        public override string ToString()
        {
            if (FFrm == null || FFrm.FFContent.IsMissing())
            {
                return "未找到为本单据设计的自由表单，请先进行设计！";
            }
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
                JavaScriptCompressor compressor = new JavaScriptCompressor();
                compressor.Encoding = Encoding.UTF8;
                script.Append(compressor.Compress(RenderJavascript()));
                script.AppendLine("</script>");

                // Insert grid id where needed (in columns)
                script.Replace("##formid##", _id);
                // Return script + required elements
                return script.ToString() + RenderHtmlElements();
            }

        }

        private string RenderHtmlElements()
        {
            StringBuilder formHtml = new StringBuilder();
            if (!_fapColumns.Any())
            {
                return "未发现元数据";
            }
            if (_existTemplateFields.Count < 1)
            {
                return FFrm.FFContent;
            }

            formHtml.AppendFormat("<form class=\"form-horizontal\" id=\"frm-{0}\" role=\"form\">", _id).AppendLine();

            //Id,Fid,TableName,Ts这三列要隐藏
            string idv = _fapFields.FirstOrDefault(f => f.FieldName == "Id").FieldValue.ToString();
            string fidv = _fapFields.FirstOrDefault(f => f.FieldName == "Fid").FieldValue.ToString();
            string tsv = _fapFields.FirstOrDefault(f => f.FieldName == "Ts").FieldValue.ToString();
            formHtml.AppendLine(CreateHiddenControl("Id", idv));
            formHtml.AppendLine(CreateHiddenControl("Fid", fidv));
            formHtml.AppendLine(CreateHiddenControl("Ts", tsv));
            formHtml.AppendLine(CreateHiddenControl(FapWebConstants.FORM_TABLENAME, _tb.TableName));
            //formToken 用于防止重复提交
            string avoidRepeatToken = UUIDUtils.Fid;
            formHtml.AppendLine(CreateHiddenControl(FapWebConstants.AVOID_REPEAT_TOKEN, avoidRepeatToken));
            //保存的时候校验此值 
            _applicationContext.Session.SetString($"{_tb.TableName.ToLower()}{FapWebConstants.AVOID_REPEAT_TOKEN}", avoidRepeatToken);
            string ffContent = FFrm.FFContent;
            foreach (var field in _existTemplateFields)
            {
                ffContent = ffContent.Replace("${" + field.FieldComment + "}", field.BuilderFreeForm(_id, _formStatus));
            }
            foreach (var table in _childTableList)
            {
                string primaryKey =_dbContext.Columns(table.Value.TableName).FirstOrDefault(f => f.RefTable == _tb.TableName)?.ColName ?? "";
                if (IsNullOrWhiteSpace(primaryKey))
                {
                    _logger.LogError($"表{table.Value.TableName}未设置和主表的关联字段，请设置关联字段参照主表");
                    continue;
                }
                Grid grid = new Grid(_dbContext, _loggerFactory, _applicationContext, _multiLangService, $"grid-{table.Value.TableName}");
                QuerySet qs = new QuerySet();
                qs.TableName = table.Value.TableName;

                var ffParams = _querySet.Parameters;
                string newUid = string.Empty;
                if (ffParams != null && ffParams.Any())
                {
                    if (ffParams.Exists(p=>p.ParamKey=="Id"))
                    {

                        if (ffParams.First(p=>p.ParamKey=="Id").ParamValue.ToString() == "0")
                        {
                            qs.GlobalWhere = $"{primaryKey}='{fidv}'";
                            newUid = fidv;
                        }
                        else
                        {
                            qs.GlobalWhere = $"{primaryKey}=(select fid from {_tb.TableName} where Id={ffParams.First(p => p.ParamKey == "Id").ParamValue})";
                        }
                    }
                    if (ffParams.Exists(p => p.ParamKey == "Fid"))
                    {
                        qs.GlobalWhere = $"{primaryKey}='{ffParams.First(p => p.ParamKey == "Fid").ParamValue.ToString()}'";
                        newUid = ffParams.First(p => p.ParamKey == "Fid").ParamValue.ToString().ToString();
                    }
                }
                var subColumnList =_dbContext.Columns(table.Value.TableName);
                if (subColumnList != null && subColumnList.Any())
                {
                    //元数据默认值
                    foreach (var subCol in subColumnList)
                    {
                        if (subCol.ColDefault.IsPresent())
                        {
                            qs.AddDefaultValue(subCol.ColName, subCol.ColDefault);
                        }
                    }
                }
                if (_subTableListDefaultData != null && _subTableListDefaultData.Any())
                {
                    //自定义默认值
                    var subDefaultData = _subTableListDefaultData.First(t => t.TableName == table.Value.TableName)?.Data;
                    if (subDefaultData != null)
                    {
                        foreach (var sd in subDefaultData)
                        {
                            qs.AddDefaultValue(sd.Key, sd.Value);
                        }
                    }
                }
                qs.QueryCols = "*";
                grid.SetQueryOption(qs);
                grid.SetPostData(new PostData { QuerySet = qs, HasOperCol = false });
                if (_formStatus != FormStatus.View && !_gridReadonly)
                {
                    grid.SetEditRowModel(EditRowModel.Inline);

                    grid.SetPager($"pager-{table.Value.TableName}");
                }
                grid.SetEditUrl("clientArray");
                grid.SetEditRowModel(EditRowModel.Inline);
                grid.SetColMenu(false);
                grid.SetShrinkToFit(false);
                grid.SetInsideElement("girdcontainer");
                ffContent = ffContent.Replace(table.Key, grid.ToString());
                //ffContent = ffContent.Replace("{{" + table.TableComment + "}}", $"<iframe width='100%' height='400' frameborder='0' scrolling='auto' src='/PublicCtrl/InlineGridPage?tn={table.TableName}&fid={newUid}&primaryKey={primaryKey}'></iframe>");
            }
            formHtml.AppendLine("<div  class=\"wordpage\">");
            formHtml.AppendLine(ffContent);
            //增加存储的子表
            if (_childTableList.Any() && _formStatus != FormStatus.View && !_gridReadonly)
            {
                formHtml.Append($"<label id='lblchildgrid'  hidden='hidden'>{string.Join(',', _childTableList.Values.Select(t => t.TableName))}</label >");
            }
            formHtml.AppendLine("<div>");
            formHtml.AppendLine("<div class=\"error\">				</div>");
            formHtml.AppendLine("</form>");
            formHtml.AppendLine("<div class=\"row\">");
            formHtml.AppendLine("<div class=\"col-xs-12 col-sm-6\" id=\"frm-result\"></div>");
            formHtml.AppendLine("</div>");
            //当单独设置dataform的时候 生成这个层，如果是jqgrid弹出的form就不用，因为jqgrid中已经生成
            if (!_id.Equals("jqgriddataform", System.StringComparison.CurrentCultureIgnoreCase))
            {
                formHtml.AppendLine(" <div class=\"row\"> <div id=\"fapFormContent-" + _id + "\" class=\"col-lg-12\">");
                //formDialogDiv.AppendLine("    <iframe width=\"100%\" height=\"100%\" frameborder=\"0\" style=\"border:none 0;\" allowtransparency=\"true\" id=\"_DialogFrame_Dataform\" ></iframe>");
                formHtml.AppendLine(" </div>  </div>");
            }

            return formHtml.ToString();
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

            //参照弹出事件、上传附件、数值控件
            foreach (FapField field in _existTemplateFields)
            {
                FapColumn column = field.CurrentColumn;
                if (column.ColName == "Id" || column.ColName == "Fid" || column.ShowAble == 0)
                {
                    continue;
                }
                #region 日期
                else if (column.CtrlType == FapColumn.CTRL_TYPE_DATE)
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
                        maxDate= DateTimeUtils.DateFormat(DateTime.Now.AddDays((double)column.MaxValue));
                    }
                    if (dateFormat == "yyyy-mm-dd")
                    {
                        script.AppendLine(" $(\"#frm-" + _id + " #" + column.ColName + "\").scroller('destroy').scroller($.extend({preset:'date', minDate:moment('" + minDate + "').toDate(),maxDate:moment('" + maxDate + "').toDate()},{ theme: 'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                    else
                    {
                        script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + "\").datePicker({  followOffset: [0, 24],onselect:function(date){ var formatDate = DatePicker.formatDate(date, '" + dateFormat + "');this.shell.val(formatDate).change();  return false;},minDate:'" + minDate + "',maxDate:'" + maxDate + "', altFormat:'" + dateFormat + "',showMode:" + model + " }).next().on(ace.click_event, function () {$(this).prev().focus(); });");

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
                        script.AppendLine(" $(\"#frm-" + _id + " #" + column.ColName + "\").scroller('destroy').scroller($.extend({ preset:'datetime', minDate:moment('" + startDate + "').toDate(), maxDate:moment('" + endDate + "').toDate() , stepMinute: 5  }, { theme:'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                    else if (format.EqualsWithIgnoreCase("HH:mm"))
                    {
                        format = "time";
                        //script.AppendLine("opt.time = {preset : 'time'};");
                        script.AppendLine(" $(\"#frm-" + _id + " #" + column.ColName + "\").scroller('destroy').scroller($.extend({preset:'time'}, { theme: 'android-ics light', mode: 'scroller', display:'modal', lang: 'zh' }));");
                    }
                }
                #endregion

                #region 参照
                else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && column.EditAble == 1)
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
                    script.AppendLine("$(\"#frm-" + _id + " #" + _id + column.ColName + "\"+\"MC\").next().on(ace.click_event, function(){");
                    //script.AppendLine("//不可编辑字段不能弹出");
                    script.AppendLine(" if($(this).prev().attr(\"disabled\")==\"disabled\"){return;}");
                    //扩展参考值，参照参数用
                    script.AppendLine("var extra=[];");

                    //针对某些参照要用表单上的控件数据
                    if (column.RefCondition.IsPresent())
                    {
                        //DeptUid='${DeptUid}',@后面为表单上的控件
                        string fieldName = "";
                        string pattern = FapPlatformConstants.VariablePattern;// @"\$\{\S+?\}";
                        Regex regex = new Regex(pattern);
                        //Regex regex = new Regex(@"\@(\w+[.]?[-]?[?]?\w+)[\s\t\n]*", RegexOptions.IgnoreCase);
                        var mat = regex.Matches(column.RefCondition);
                        foreach (Match item in mat)
                        {
                            int length = item.ToString().Length - 3;
                            fieldName = item.ToString().Substring(2, length);
                            //fieldName = item.Groups[1].ToString();

                            FapColumn col = _fapColumns.Find(f => f.ColName.EqualsWithIgnoreCase(fieldName));
                            if (col != null)
                            {
                                script.AppendLine("var conv=$('#" + fieldName + "').val();if(conv==''){bootbox.alert('【" + _multiLangService.GetLangColumnComent(col) + "】为空，请先设置。');return;}");
                                script.AppendLine("extra.push('" + fieldName + "='+conv)");
                            }
                        }
                    }
                    script.AppendLine("loadRefMessageBox('" + dispalyName + "','" + _id + "'," + column.Id + ",'" + column.ColName + "','" + refUrl + "',extra)");
                    script.AppendLine("});");
                    script.AppendLine("$(\"#frm-" + _id + " #" + _id + column.ColName + "\"+\"MC\").on(ace.click_event,function(e){$(this).next().trigger(ace.click_event);e.preventDefault();})");
                }
                #endregion

                #region 附件

                else if (column.CtrlType == FapColumn.CTRL_TYPE_FILE)
                {
                    script.AppendLine("$(\"#frm-" + _id + " #file" + _id + column.ColName + "\").on(ace.click_event, function () {");
                    string tempFid =UUIDUtils.Fid;
                    if (field.FieldValue.ToString().IsMissing())
                    {
                        script.AppendLine("alert('数据导入的时候没有生成附件值，不能上传附件！')");
                    }
                    else
                    {
                        script.AppendLine("loadFileMessageBox('" + tempFid + "','" + _id + "',initFile" + _id + tempFid + ");");
                    }
                    script.AppendLine("})");
                    string allowExt = string.Empty;
                    if (column.FileSuffix.IsPresent())
                    {
                        string[] suffix = column.FileSuffix.Split(',');
                        if (suffix.Any())
                        {
                            allowExt = string.Join(",", suffix.Select(s => "'" + s + "'").ToList());
                        }
                    }
                    //建立初始化附件控件js函数
                    script.AppendLine("var initFile" + _id + tempFid + "=function(){");
                    script.AppendLine("$(\"#" + _id + tempFid + "-FILE\").fileinput({");
                    script.AppendLine("language: 'zh',");
                    script.AppendLine("uploadUrl:\"" + _applicationContext.BaseUrl+ "/Api/Core/uploadfile/" + field.FieldValue + "\",");
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
                    //IEnumerable<FapAttachment> attList = null;
                    //预览
                    //if (_formStatus == FormStatus.Edit)
                    //{
                    //    string bid = field.FieldValue.ToString();
                    //    DynamicParameters parameters = new DynamicParameters();
                    //    parameters.Add("Bid", bid);
                    //    attList = _dbContext.QueryWhere<FapAttachment>("Bid=@Bid", parameters);
                    //    if (attList.Any())
                    //    {
                    //        StringBuilder initPre = new StringBuilder();
                    //        StringBuilder initPreC = new StringBuilder();
                    //        initPre.AppendLine("initialPreview: [");
                    //        initPreC.AppendLine("initialPreviewConfig: [");
                    //        attList.ToList().ForEach(a =>
                    //        {
                    //            initPre.AppendLine(" \"<img style='height:160px' src='http://" + _session.BaseURL + "/Common/Home/AttachmentImg/" + a.Fid + "'>\",");
                    //            initPreC.AppendLine("{caption: \"" + a.FileName + "\", width: \"120px\", url: \"http://" + _session + "/Api/Core/deletefile\", key: \"" + a.Fid + "\"},");
                    //        });
                    //        initPreC.AppendLine("],");
                    //        initPre.AppendLine(" ],");
                    //        script.AppendLine(initPre.ToString());
                    //        script.AppendLine(initPreC.ToString());
                    //    }
                    //}
                    //else
                    //{
                    //    script.AppendLine("showPreview:false,");
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
                    }).on('fileuploaded', function (event, data, previewId, index) {  if(data.response.success==false){bootbox.alert('上传失败：'+data.response.msg);}else{loadFileList('" + _id + "', '" + column.ColName + "', '" + field.FieldValue.ToString() + "',1);                } });       ");
                    script.AppendLine("}");
                    //if (attList != null && attList.Count() > 0)
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
                        script.AppendLine("loadFileList('" + _id + "','" + column.ColName + "','" + bid + "',1);");
                    }
                }
                #endregion

                #region 图片头像
                else if (column.CtrlType == FapColumn.CTRL_TYPE_IMAGE && column.EditAble == 1)
                {
                    script.AppendLine("loadImageControl('avatar" + column.ColName + "')");
                }
                #endregion

                #region 富文本控件
                else if (column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
                {
                    script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + ".wysiwyg-editor\").ace_wysiwyg({" + @"
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
                    script.AppendLine(" $(\"#frm-" + _id + " input[name='" + column.ColName + "']\").TouchSpin({");
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
                    script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + "\").citypicker();");
                }

                #endregion

                #region 城市

                else if (column.CtrlType == FapColumn.CTRL_TYPE_CITY)
                {
                    //城市
                    script.AppendLine("$(\"#frm-" + _id + " #" + column.ColName + "\").cityselect();");
                }

                #endregion
                #region 评星级
                else if (column.CtrlType == FapColumn.CTRL_TYPE_STAR)
                {
                    if (!field.FieldValue.ToStringOrEmpty().IsPresent())
                    {
                        field.FieldValue = "0";
                    }
                    //评星级
                    script.AppendLine("if(!$(\"#frm-" + _id + " #" + column.ColName + "\").prop(\"disabled\")){ $(\"#frm-" + _id + " #rat-" + column.ColName + "\").raty({number: 5,score:" + field.FieldValue +
                    @", cancel: true,  'starType' : 'i',
                    'click': function(score,evt) {" +
                        "$(\"#frm-" + _id + " #" + column.ColName + "\").val(score);" +
                    @"},					
                    })}else{" + "$(\"#frm-" + _id + " #rat-" + column.ColName + "\").raty({number: 5,score:" + field.FieldValue +
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
                    script.AppendLine("$(\"#frm-" + _id + " #" + ctrmultiLang + "\").next().on(ace.click_event, function(){");
                    script.AppendLine(" document.addEventListener(\"mousedown\", onMultiLangPoverMouseDown, false);");
                    script.AppendLine("var fid=$(this).data(\"fid\");");
                    script.AppendLine("var X1 = $(\"#frm-" + _id + " #" + ctrmultiLang + "\").offset().top-55;var Y1 =$(\"#frm-" + _id + " #" + ctrmultiLang + "\").offset().left;");
                    script.AppendLine("var bg=$(\"#\"+fid).closest(\".modal-lg\");var top=X1;var left=Y1");
                    script.AppendLine("if(bg){ var bgo=bg.offset();   top=X1-bgo.top;left=Y1-bgo.left;}");
                    script.AppendLine("$(\"#\"+fid).css({\"position\": \"fixed\",\"display\":\"inline-grid\",\"top\":top+'px',\"left\":left+'px'});");
                    script.AppendLine("})");
                }
                #endregion
            }
            #region TextArea
            script.AppendLine(@"$('textarea.limited').inputlimiter({
					remText: '%n 字符剩余...',
					limitText: '最大字符数 : %n.'
				});");
            #endregion

            #region 表单校验

            //校验
            script.AppendLine("$('#frm-'+'" + _id + "').validate({");
            script.AppendLine("		errorElement: 'div',");
            script.AppendLine("		errorClass: 'help-block',");
            script.AppendLine("		focusInvalid: false,");
            script.AppendLine("		ignore: \"\",");
            script.AppendLine("		rules: {");
            foreach (FapColumn col in _fapColumns)
            {
                //非空可见
                if (col.NullAble == 0 && col.ShowAble == 1)
                {
                    if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                    {
                        script.AppendLine("                " + _id + col.ColName + "MC" + ": {");
                    }
                    else
                    {
                        script.AppendLine("             " + col.ColName + ": {");
                    }
                    script.AppendLine("				required: true,");
                    script.AppendLine("			},");
                }
                if (col.RemoteChkURL.IsPresent())
                {
                    string oriValue = FormData.Get(col.ColName);
                    script.AppendLine("             " + col.ColName + ": {");
                    script.AppendLine("				remote: '" +_applicationContext.BaseUrl + "/" + col.RemoteChkURL + "&fid=" + FidValue + "&orivalue=" + oriValue + "&currcol=" + col.ColName + "',");
                    script.AppendLine("			},");
                }
            }


            script.AppendLine("			},");

            script.AppendLine("		messages: {");

            foreach (FapColumn col in _fapColumns)
            {
                //非空可见
                if (col.NullAble == 0 && col.ShowAble == 1)
                {
                    if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                    {
                        script.AppendLine("                " + _id + col.ColName + "MC" + ": {");
                    }
                    else
                    {
                        script.AppendLine("                " + col.ColName + ": {");
                    }
                    script.AppendLine("				required: \"[" + col.ColComment + "]必须填写！\",");
                    script.AppendLine("			},");
                }
                if (col.RemoteChkURL.IsPresent())
                {
                    string msg = col.RemoteChkMsg;
                    if (msg.IsMissing())
                    {
                        msg = "[" + col.ColComment + "]此项值已经存在，请更换";
                    }
                    script.AppendLine("             " + col.ColName + ": {");
                    script.AppendLine("				remote: \"" + msg + "\",");
                    script.AppendLine("			},");
                }
            }


            script.AppendLine("		},");

            script.AppendLine("errorLabelContainer: $(\"#frm-" + _id + " div.error\"),");
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

            #endregion

            #region 表单联动脚本
            DynamicParameters pm = new DynamicParameters();
            pm.Add("TableName", _tb.TableName);
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
                            var paramCols = inject.ParamColumns.Split(',');
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
            string jsfilePath = Path.Combine(new string[] { Directory.GetCurrentDirectory(), "wwwroot", "Scripts", "FapFormPlugin", $"frm.plugin.{_tb.TableName}.js" });
            if (File.Exists(jsfilePath))
            {
                script.AppendLine(File.ReadAllText(jsfilePath, Encoding.UTF8));
            }

            #endregion

            script.AppendLine(" });");
            //存在子表时注入js
            if (_childTableList.Any() && _formStatus != FormStatus.View && !_gridReadonly)
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
            return script.ToString();
        }
        public string ToHtmlString()
        {
            return ToString();
        }
    }

}
