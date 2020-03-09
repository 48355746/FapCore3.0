using Dapper;
using Fap.AspNetCore.Model;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls.DataForm
{
    public class XEditableForm : HtmlString
    {
        private bool _enabled = true;
        //显示几列
        private int _cols = 1;
        private XEditableFormModel _formModel = XEditableFormModel.Inline;
        private XEditableSaveModel _saveModel = XEditableSaveModel.All;
        public string TableName { get; private set; }
        private List<FapColumn> _fapColumns = new List<FapColumn>();
        private List<XEditableField> _xEditableFields = new List<XEditableField>();
       
        //pk的值
        private string _pkValue;
        //编辑URL
        private string _editUrl;
        private IDbContext _dbContext;
        private IFapApplicationContext _applicationContext;
        private IRbacService _rbacService;
        private IMultiLangService _multiLangService;
        public XEditableForm(IFapApplicationContext applicationContext, IDbContext dataAccessor,  IMultiLangService multiLangService,IRbacService rbacService) : base("")
        {
            _dbContext = dataAccessor;
            _applicationContext = applicationContext;
            _multiLangService = multiLangService;
            _rbacService = rbacService;
        }

        private dynamic FormData { get; set; }
        /// <summary>
        /// 设置是否可编辑,默认true
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public XEditableForm SetEditEnabled(bool enabled)
        {
            _enabled = enabled;
            return this;
        }
        public XEditableForm SetTwoColumns()
        {
            _cols = 2;
            return this;
        }
        /// <summary>
        /// 设置编辑模式，inline or  popup。默认inline
        /// </summary>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public XEditableForm SetFormModel(XEditableFormModel formModel)
        {
            _formModel = formModel;
            return this;
        }
        /// <summary>
        /// 设置保存方式，整表or字段，默认整表
        /// </summary>
        /// <param name="saveModel"></param>
        /// <returns></returns>
        public XEditableForm SetSaveModel(XEditableSaveModel saveModel)
        {
            _saveModel = saveModel;
            if (saveModel == XEditableSaveModel.Single)
            {
                _editUrl = "/Api/Core/SaveSingleField/";
            }
            else
            {
                _editUrl = "/Api/Core/Persistence/";
            }
            return this;
        }
        /// <summary>
        /// 设置编辑url，默认公共地址
        /// </summary>
        /// <param name="editUrl"></param>
        /// <returns></returns>
        public XEditableForm SetEditUrl(string editUrl)
        {
            _editUrl = editUrl;
            return this;
        }

        public XEditableForm SetFapClumns(IEnumerable<FapColumn> columns)
        {
            //不显示默认字段,隐藏字段,自定义字段，附件,
            _fapColumns = columns.Where(f => f.IsDefaultCol == 0 && f.IsCustomColumn == 0 && f.CtrlType != FapColumn.CTRL_TYPE_FILE && f.CtrlType != FapColumn.CTRL_TYPE_IMAGE && f.ShowAble == 1).ToList<FapColumn>();
            if (_fapColumns.Any())
            {
                if (TableName.IsMissing())
                {
                    TableName = _fapColumns.First().TableName;
                }
                bool existRolePower = false;
                //当前角色
                IEnumerable<FapRoleColumn> roleColumn = _rbacService.GetRoleColumnList(_applicationContext.CurrentRoleUid);
                if (roleColumn!=null&&roleColumn.Any())
                {
                    ////当前表的角色字段
                    //var currRoleColumns = roleColumn.Where(t => t.TableUid == TableName);
                    //if (currRoleColumns != null && currRoleColumns.Any())
                    //{
                    //    existRolePower = true;
                    //}
                }
                foreach (var column in _fapColumns)
                {
                    string datakey = column.ColName;
                    var fv = FormData.Get(datakey);
                    string strValue = (fv == null ? "" : fv.ToString());
                    if (existRolePower)
                    {
                        if (column.IsDefaultCol == 0 && roleColumn.Count(c => c.ColumnUid == column.Fid) < 1)
                        {
                            continue;
                        }
                    }
                    _xEditableFields.Add(new XEditableField(_multiLangService) { CurrFapColumn = column, FieldValue = strValue, EntityData = FormData });

                }
            }
            return this;
        }
        public XEditableForm SetQueryOption(QuerySet qs)
        {
            TableName = qs.TableName;
            DynamicParameters parameters = new DynamicParameters();
            qs.Parameters.ForEach(q => parameters.Add(q.ParamKey, q.ParamValue));
            FormData = _dbContext.QueryFirstOrDefault(qs.ToString(), parameters);

            _pkValue = FormData.Get("Fid");
            var queryColList = qs.QueryCols.Split(',');
            IEnumerable<FapColumn> fapColumns =_dbContext.Columns(qs.TableName).Where(c=> queryColList.Contains(c.ColName));
            if (fapColumns.Any())
            {
                SetFapClumns(fapColumns);
            }
            return this;
           
        }
        public override string ToString()
        {
            var script = new StringBuilder();

            // Start script
            script.AppendLine("<script type=\"text/javascript\">");
            JavaScriptCompressor compressor = new JavaScriptCompressor();
            compressor.Encoding = Encoding.UTF8;
            script.Append(compressor.Compress(RenderJavascript()));
            script.AppendLine("</script>");
            return script.ToString() + RenderHtmlElements();
        }

        private string RenderHtmlElements()
        {
            StringBuilder sb = new StringBuilder();
            //每列显示的个数
            int i = (_xEditableFields.Count + 1) / _cols;
            string colClass = "";
            if (_cols == 2)
            {
                colClass = "col-sm-6";
            }
            sb.AppendLine("<div class=\"col-xs-12 " + colClass + "\">");
            sb.AppendLine("	<div class=\"profile-user-info profile-user-info-striped\">");
            int count = 0;
            foreach (var xfield in _xEditableFields)
            {
                count++;
                sb.AppendLine(xfield.ToString());
                if (count == i)
                {
                    sb.AppendLine("</div>");
                    sb.AppendLine(" </div>");
                    sb.AppendLine("<div class=\"col-xs-12 " + colClass + "\">");
                    sb.AppendLine("	<div class=\"profile-user-info profile-user-info-striped\">");
                }
            }
            sb.AppendLine(" </div>");
            sb.AppendLine(" </div>");

            return sb.ToString();
        }

        private string RenderJavascript()
        {
            var script = new StringBuilder();
            script.AppendLine("var formdata={};");
            script.AppendLine("formdata.Fid='" + _pkValue + "';");
            script.AppendLine("$(function(){");
            if (_formModel == XEditableFormModel.Inline)
            {
                script.AppendLine(@"$.fn.editable.defaults.mode = 'inline';");
            }
            else
            {
                script.AppendLine(@"$.fn.editable.defaults.mode = 'popup';");
            }
            script.AppendLine("$.fn.editableform.loading = \"<div class='editableform-loading'><i class='ace-icon fa fa-spinner fa-spin fa-2x light-blue'></i></div>\";");
            script.AppendLine("$.fn.editableform.buttons = '<button type=\"submit\" class=\"btn btn-info editable-submit\"><i class=\"ace-icon fa fa-check\"></i></button>'+");
            script.AppendLine("                           '<button type=\"button\" class=\"btn editable-cancel\"><i class=\"ace-icon fa fa-times\"></i></button>';  ");

            foreach (var xfield in _xEditableFields)
            {
                var column = xfield.CurrFapColumn;
                //script.AppendLine(SetFormDataJson(column, xfield.FieldValue));
                if (column.EditAble == 1)
                {
                    script.AppendLine("$('#" + column.ColName + "').editable({");
                    script.AppendLine(" name:'" + column.ColName + "',");
                    script.AppendLine(GetOption(column, xfield.FieldValue));
                    if (column.NullAble == 0)
                    {
                        script.AppendLine(" validate: function(value) { if($.trim(value) == '') return '此项是必须的！';  },");
                    }
                    if (_saveModel == XEditableSaveModel.Single)
                    {
                        script.AppendLine(" url: basePath+'" + _editUrl + "',  ");
                        script.AppendLine("pk: '" + _pkValue + "',    ");
                        string strPv = "params.value=params.value";
                        //参照返回的是一个对象
                        if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            strPv = "params.value=params.value.code";
                        }
                        script.AppendLine(@"params: function(params) {
                                            //originally params contain pk, name and value
                                            params.tablename = '" + TableName + @"';" + strPv + @"
                                            return params;},");


                        //title始终放最后
                        script.AppendLine("title: '" +_multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.FapColumn, $"{column.TableName}_{column.ColName}") + "',");
                    }

                    //成功返回
                    //script.AppendLine("success: function(response, newValue) {formdata."+column.ColName+"= }");

                    script.AppendLine("});");
                }
            }
            //销毁
            script.AppendLine(@"$(document).one('ajaxloadstart.page', function(e) {
					    //in ajax mode, remove remaining elements before leaving page
					    try {
						    $('.editable').editable('destroy');
					    } catch(e) {}
					    $('[class*=select2]').remove();
				    });");
            if (!_enabled)
            {
                script.AppendLine(" $('.profile-user-info.profile-user-info-striped .editable').editable('disable');");
            }

            script.AppendLine("})");
            if (_saveModel == XEditableSaveModel.All)
            {
                script.AppendLine("var saveXEditFormData=function(){");
                script.AppendLine(@"
                                    var i = 0;
                                    for(f in formdata)
                                    {
                                        if(f=='Fid')
                                        {
                                            continue;
                                        }
                                        i++;
                                    }
                                    if(i==0)
                                    {
                                        bootbox.alert('没检测到你数据的改变，不需要保存');
                                        return;
                                    }
                ");
                script.AppendLine("formdata.TableName='" + TableName + "';formdata.oper='edit'");
                script.AppendLine("$.post(basePath+'" + _editUrl + "',formdata,function(result){if(result==true){bootbox.alert('更新成功')}else{bootbox.alert('更新失败')}});");
                script.AppendLine("};");
            }
            return script.ToString();
        }
        //private string SetFormDataJson(FapColumn column, string value)
        //{            
        //    string strJson = string.Empty;
        //    if (column.ColType == FapColumn.COL_TYPE_INT || column.ColType == FapColumn.COL_TYPE_LONG || column.ColType == FapColumn.COL_TYPE_DOUBLE || column.ColType == FapColumn.COL_TYPE_BOOL)
        //    {
        //        strJson = "formdata." + column.ColName + "=" + value;
        //    }
        //    else
        //    {

        //        strJson = "formdata." + column.ColName + "='" + value + "'";
        //    }
        //    return strJson;
        //}
        private string GetOption(FapColumn column, string fieldValue)
        {
            StringBuilder sbOption = new StringBuilder();
            if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX || column.CtrlType == FapColumn.CTRL_TYPE_RADIO)
            {
                IEnumerable<FapDict> dicList =_dbContext.Dictionarys(column.ComboxSource);
                string strSource = string.Join(",", dicList.Select(d => "{id:'" + d.Code.RemoveSpace() + "',text:'" + d.Name.RemoveSpace().Replace("'","") + "'}"));
                sbOption.AppendLine("type:'select2',");
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("source:[" + strSource + "],");
                sbOption.AppendLine("inputclass:'select2',");
                sbOption.AppendLine(@"select2: { width: 200,  allowClear: true,multiple: false,placeholder: '请选择'},");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_CHECKBOXLIST)
            {
                IEnumerable<FapDict> dicList =_dbContext.Dictionarys(column.ComboxSource);
                string strSource = string.Join(",", dicList.Select(d => "{id:'" + d.Code.RemoveSpace() + "',text:'" + d.Name.RemoveSpace().Replace("'", "") + "'"));
                sbOption.AppendLine("type:'checklist',");
                sbOption.AppendLine("value:[" + fieldValue.RemoveSpace() + "],");
                sbOption.AppendLine("inputclass:'ace ace-checkbox-2',");
                sbOption.AppendLine("separator:',',");
                sbOption.AppendLine("source:[" + strSource + "],");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
            {
                sbOption.AppendLine("type:'select2',");
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("source:[{id:1,text:'是'},{id:0,text:'否'}],");
                sbOption.AppendLine("inputclass:'select2',");
                sbOption.AppendLine(@"select2: { width: 100,  allowClear: false,multiple: false,placeholder: '请选择'},");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_MEMO)
            {
                //由于会存在回车换行情况，所以在html中赋值
                //sbOption.AppendLine("value:'" + StringUtil.ClearSpaceReturn(fieldValue) + "',");
                sbOption.AppendLine("type:'textarea',");
                sbOption.AppendLine("rows: 10,");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_DATE || column.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
            {
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("type:'combodate',");
                string strFormat = "YYYY-MM-DD";
                string strTemplate = "YYYY-MM-DD";
                string viewTemplate = "YYYY-MM-DD";
                if (column.CtrlType == FapColumn.CTRL_TYPE_DATETIME)
                {
                    viewTemplate = strFormat = "YYYY-MM-DD HH:mm";
                    strTemplate = "YYYY-MM-DD   H : mm";
                }
                sbOption.AppendLine("format: '" + strFormat + "',viewformat: '" + viewTemplate + "',template: '" + strTemplate + "',   ");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue.format('" + viewTemplate + "') },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_DOUBLE || column.CtrlType == FapColumn.CTRL_TYPE_INT || column.CtrlType == FapColumn.CTRL_TYPE_MONEY)
            {
                sbOption.AppendLine("value:" + (fieldValue.IsMissing() ? "0.0" : fieldValue) + ",");
                sbOption.AppendLine("type:'spinner',");
                int decimals = 0;
                if (column.CtrlType != FapColumn.CTRL_TYPE_INT)
                {
                    decimals = column.ColPrecision;
                }
                sbOption.AppendLine("spinner:{decimals: " + decimals + "},");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX)
            {
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("type:'wysiwyg',");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                var valueMC = FormData.Get(column.ColName + "MC");
                string strValue = "{code:'" + fieldValue.RemoveSpace() + "',name:'" + (valueMC == null ? "" : valueMC.ToString()) + "'}";
                sbOption.AppendLine("value:" + strValue.RemoveSpace() + ",");
                sbOption.AppendLine("type:'reference',");
                //refCode存放组件引用
                string refOption = "tablename: '" + column.TableName + "', colname: '" + column.ColName + "',multi: false,reftype: '" + column.RefType + "',refid: " + column.Id + ",component: '" + column.RefCode.ToStringOrEmpty() + "'";
                sbOption.AppendLine("reference: { " + refOption + " },");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue.code },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_EMAIL)
            {
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("type:'email',");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_PASSWORD)
            {
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("type:'password',");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            else
            {
                sbOption.AppendLine("value:'" + fieldValue.RemoveSpace() + "',");
                sbOption.AppendLine("type:'text',");
                sbOption.AppendLine("success: function(response, newValue) {formdata." + column.ColName + "=newValue },");
            }
            return sbOption.ToString();
        }

        public string ToHtmlString()
        {
            return ToString();
        }
    }
    public enum XEditableFormModel
    {
        Popup, Inline
    }
    public enum XEditableSaveModel
    {
        Single, All
    }
}
