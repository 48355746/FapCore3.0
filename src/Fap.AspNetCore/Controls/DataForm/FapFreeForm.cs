using Dapper;
using Fap.AspNetCore.Controls.JqGrid;
using Fap.AspNetCore.Controls.JqGrid.Enums;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
/* ==============================================================================
* 功能描述：自由表单  
* 创 建 者：wyf
* 创建日期：2017-04-24 18:48:06
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yahoo.Yui.Compressor;
using static System.String;
using Fap.Core.Exceptions;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Hosting;

namespace Fap.AspNetCore.Controls.DataForm
{
    internal class FapFreeForm : BaseForm
    {
        public CfgFreeForm FFrm { get; private set; }
        

        public FapFreeForm(IServiceProvider serviceProvider, string id, FormStatus formStatus = FormStatus.Add) : base(serviceProvider, id, formStatus)
        {
        }

        /// <summary>
        /// 获取自由表单设置
        /// </summary>
        private void GetFreeFromSet()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("Table", _fapTable.TableName);

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
                var childTableList = _dbContext.Tables(t => t.MainTable == _fapTable.TableName);
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
                            throw new FapException("自由表单包含两个一样的子表设置");
                        }
                    }
                }
            }
            //模板中存在的列
            _existTemplateFields = formFields.Where(f => existCols.Contains(f.FieldComment)).ToList();
        }   

        public override string ToString()
        {
            GetFreeFromSet();
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
                if (_env.IsDevelopment())
                {
                    script.Append(RenderJavascript());
                }
                else
                {
                    JavaScriptCompressor compressor = new JavaScriptCompressor();
                    compressor.Encoding = Encoding.UTF8;
                    script.Append(compressor.Compress(RenderJavascript()));
                }
                script.AppendLine("</script>");

                // Insert grid id where needed (in columns)
                script.Replace("##formid##", $"frm-{FormId}");
                // Return script + required elements
                return script.ToString() + RenderHtmlElements();
            }

        }
        protected override string RenderFormContent()
        {
            StringBuilder formHtml = new StringBuilder();
            string ffContent = FFrm.FFContent;
            foreach (var field in _existTemplateFields)
            {
                ffContent = ffContent.Replace("${" + field.FieldComment + "}", field.BuilderFreeForm(FormId, _formStatus));
            }
            foreach (var table in _childTableList)
            {
                string primaryKey = _dbContext.Columns(table.Value.TableName).FirstOrDefault(f => f.RefTable == _fapTable.TableName)?.ColName ?? "";
                Guard.Against.NullOrWhiteSpace(primaryKey, nameof(primaryKey));

                Grid grid = new Grid(_dbContext, _rbacService, _applicationContext, _multiLangService,_env, $"grid-{table.Value.TableName}");
                QuerySet qs = new QuerySet();
                qs.TableName = table.Value.TableName;
                qs.GlobalWhere = $"{primaryKey}='{FidValue}'";

                var subColumnList = _dbContext.Columns(table.Value.TableName);
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
                grid.SetPostData(new { QuerySet = qs, HasOperCol = false });
                if (_formStatus != FormStatus.View && !childGridReadOnly)
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
            if (_childTableList.Any() && _formStatus != FormStatus.View && !childGridReadOnly)
            {
                formHtml.Append($"<label id='lblchildgrid'  hidden='hidden'>{string.Join(',', _childTableList.Values.Select(t => t.TableName))}</label >");
            }
            formHtml.AppendLine("</div>");
            return formHtml.ToString();
        }


    }

}
