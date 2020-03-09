using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls.DataForm
{
    internal class FapForm : BaseForm
    {
        private int _colCount = 2;
    

        public FapForm(IServiceProvider serviceProvider, string id, FormStatus formStatus = FormStatus.Add) : base(serviceProvider, id, formStatus)
        {
        }      
        public FapForm SetColCount(int colCount)
        {
            _colCount = colCount;
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
                string js = RenderJavascript().Replace("##formid##", $"frm-{FormId}");
                script.Append(compressor.Compress(js));
                script.AppendLine("</script>");
                // Return script + required elements
                return script.ToString() + RenderHtmlElements();
            }

        }
        protected override string RenderFormContent()
        {
            StringBuilder formHtml = new StringBuilder();
            var grpFields = formFields.GroupBy(f => f.FieldGroup);
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

                    formHtml.AppendLine(column.BuilderForm(FormId, _colCount, isColspan, _formStatus));
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
            return formHtml.ToString();

        }
        /// <summary>
        /// 是否colspan占一行
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool IsColSpan(FapColumn column)
        {
            return column.CtrlType == FapColumn.CTRL_TYPE_MEMO
                || column.CtrlType == FapColumn.CTRL_TYPE_IMAGE
                || column.CtrlType == FapColumn.CTRL_TYPE_FILE
                || column.CtrlType == FapColumn.CTRL_TYPE_RICHTEXTBOX
                || column.CtrlType == FapColumn.CTRL_TYPE_NATIVE
                || column.CtrlType == FapColumn.CTRL_TYPE_RANGE;
        }
   
       
       

    }
 
}

