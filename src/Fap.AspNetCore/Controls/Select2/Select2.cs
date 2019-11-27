using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls
{
    /// <summary>
    /// Select2
    /// 引用select2.css，select2.js
    /// 获取和赋值
    /// alert("Selected value is: "+$("#e8").select2("val")); $("#e8").select2("val", "CA");
    /// 多选赋值：['','']
    /// $("#e13").val("CA").trigger("change");
    /// $("#e13").val(["AK","CO"]).trigger("change"); 
    /// </summary>
    public class Select2 : HtmlString
    {
        private string _id;
        private int _width;
        private bool _isMulti;
        //private bool _isClear;
        private string _selectOptions;
        private string _placeholder;
        //dataSource
        //private string _data;
        //event
        private string _changeEvent;
        private string _openEvent;
        private string _removedEvent;

        //默认值
        private string _defaultVal;

        private IDbContext _db;
        public Select2(IDbContext dataAccessor, string id):base("")
        {
            _id = id;
            _db = dataAccessor;
        }
        public Select2 SetDefaultValue(string defaultVal)
        {
            _defaultVal = defaultVal;
            return this;
        }
        /// <summary>
        /// 是否可多选
        /// </summary>
        /// <param name="isMulti"></param>
        /// <returns></returns>
        public Select2 SetMultiSelectAble(bool isMulti)
        {
            _isMulti = isMulti;
            return this;
        }
        /// <summary>
        /// 是否可清空,默认都清空
        /// </summary>
        /// <param name="isClear"></param>
        /// <returns></returns>
        //public Select2 SetClearAble(bool isClear)
        //{
        //    _isClear = isClear;
        //    return this;
        //}
        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public Select2 SetWidth(int width)
        {
            _width = width;
            return this;
        }
        public Select2 SetPlaceholder(string placeholder)
        {
            _placeholder = placeholder;
            return this;
        }
        /// <summary>
        /// 设置select2的数据源
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Select2 SetDataSource(List<Select2DataItem> list)
        {
            if (list != null && list.Any())
            {
                StringBuilder sbOption = new StringBuilder();
                foreach (var item in list)
                {
                    sbOption.Append("<option value=\"" + item.Id + "\">" + item.Text + "</option>");
                }
                _selectOptions = sbOption.ToString();
            }
            return this;
        }
        public Select2 SetSelect2Mode(Select2Model model)
        {
            string sql = string.Format("select {0} as Id,{1} as Name,#Grp# as Grp, #Data# as Data from {2}", model.IdField, model.NameField, model.TableName);
            if (model.GroupField.IsPresent())
            {
                sql = sql.Replace("#Grp#", model.GroupField);
            }
            else
            {
                sql = sql.Replace("#Grp#", "''");
            }

            if (model.DataField.IsPresent())
            {
                sql = sql.Replace("#Data#", model.DataField);
            }
            else
            {
                sql = sql.Replace("#Data#", "''");
            }

            if (model.Where.IsPresent())
            {
                sql += " where " + model.Where;
            }
            if (model.SortBy.IsPresent())
            {
                sql += " order by " + model.SortBy;
            }
            var list = _db.Query(sql);
            StringBuilder sbOption = new StringBuilder();
            sbOption.Append("  <option></option>");
            foreach (var item in list.GroupBy(f => f.Grp))
            {
                if (item.Key != "")
                {
                    sbOption.AppendLine("<optgroup label=\"" + item.Key + "\">");
                    foreach (var ci in item.ToList())
                    {
                        sbOption.Append("<option value=\"" + ci.Id + "\" data=\"" + ci.Data + "\">" + ci.Name + "</option>");
                    }

                    sbOption.AppendLine("</optgroup>");
                }
                else
                {
                    foreach (var ci in item.ToList())
                    {
                        sbOption.Append("<option value=\"" + ci.Id + "\" data=\"" + ci.Data + "\">" + ci.Name + "</option>");
                    }
                }

            }
            _selectOptions = sbOption.ToString();
            return this;
        }
        /// <summary>
        /// 值变化事件
        /// 参数e
        /// function(e) { log("change "+JSON.stringify({val:e.val, added:e.added, removed:e.removed})); 
        /// </summary>
        /// <param name="changeEvent"></param>
        /// <returns></returns>
        public Select2 SetChangeEvent(string changeEvent)
        {
            _changeEvent = changeEvent;
            return this;
        }
        /// <summary>
        /// 打开事件
        /// function() { log("open"); })
        /// </summary>
        /// <param name="openEvent"></param>
        /// <returns></returns>
        public Select2 SetOpenEvent(string openEvent)
        {
            _openEvent = openEvent;
            return this;
        }
        /// <summary>
        /// 移除事件
        ///  function(e) { log ("removed val="+ e.val+" choice="+ JSON.stringify(e.choice));}
        /// </summary>
        /// <param name="removedEvent"></param>
        /// <returns></returns>
        public Select2 SetRemovedEvent(string removedEvent)
        {
            _removedEvent = removedEvent;
            return this;
        }
        public override string ToString()
        {
            // Create javascript
            var script = new StringBuilder();
            // Start script
            script.AppendLine("<script type=\"text/javascript\">");
            JavaScriptCompressor compressor = new JavaScriptCompressor();
            compressor.Encoding = Encoding.UTF8;
            script.Append(compressor.Compress(RenderJavascript()));
            script.AppendLine("</script>");


            // Return script + required elements
            return script + RenderHtmlElements();
        }

        private string RenderHtmlElements()
        {
            string multi = "";
            if (_isMulti)
            {
                multi = "multiple";
            }
            string style = "width:300px";
            if (_width > 0)
            {
                style = "width:" + _width + "px";
            }
            return "<select " + multi + " name=\"" + _id + "\" id=\"" + _id + "\" style=\"" + style + "\"  class=\"select2-allowclear tag-input-style\">" + _selectOptions + "</select>";
            //ajax
            //<input type="hidden" class="bigdrop" id="e6" style="width:600px" value="3620194" />
        }

        private string RenderJavascript()
        {
            StringBuilder script = new StringBuilder();
            script.Append(" $(function () {").AppendLine();
            script.AppendLine("$('#" + _id + "').select2({");
            if (_placeholder.IsPresent())
            {
                script.AppendLine("placeholder: \"" + _placeholder + "\",");
            }

            //if (_defaultVal.IsPresent())
            //{
            //    script.AppendFormat("value:{0},",_defaultVal).AppendLine();
            //    script.AppendFormat("triggerChange:true,").AppendLine();
            //}
            script.AppendLine("allowClear: true");
            script.AppendLine("})");
            if (_changeEvent.IsPresent())
            {
                script.AppendFormat(".on(\"change\", function(e){{{0}(e)}})", _changeEvent).AppendLine();
            }
            if (_openEvent.IsPresent())
            {
                script.AppendFormat(".on(\"select2-open\", function(){{{0}(e)}})", _openEvent).AppendLine();
            }
            if (_removedEvent.IsPresent())
            {
                script.AppendFormat(".on(\"select2-removed\", function(e){{{0}(e)}}", _removedEvent).AppendLine();
            }
            if (_defaultVal.IsPresent())
            {
                script.AppendLine("$('#" + _id + "').val('" + _defaultVal + "').trigger(\"change\") ;");
            }
            script.AppendLine(" });");
            return script.ToString();
        }
        public string ToHtmlString()
        {
            return ToString();
        }
    }
    /// <summary>
    /// select2的项
    /// </summary>
    public class Select2DataItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
    public class Select2Model
    {
        /// <summary>
        /// ID字段
        /// </summary>
        public string IdField { get; set; }
        /// <summary>
        /// Name字段
        /// </summary>
        public string NameField { get; set; }
        /// <summary>
        /// Data字段
        /// </summary>
        public string DataField { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 分组字段,不必填
        /// </summary>
        public string GroupField { get; set; }
        /// <summary>
        /// 条件，不必填
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// 排序字段，不必填
        /// </summary>
        public string SortBy { get; set; }
    }
}
