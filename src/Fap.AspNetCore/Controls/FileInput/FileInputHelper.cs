using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Fap.Core.Extensions;
using System.Collections.Generic;

namespace Fap.AspNetCore.Controls
{
    public static class FileInputHelper
    {
        public static FileInput FileInput(this HtmlHelper helper, string id)
        {
            return new FileInput(id);
        }
    }
    public class FapFileTagHelper : TagHelper
    {
        public string Id { get; set; }
        /// <summary>
        /// 自动上传
        /// </summary>
        public bool AutoUpload { get; set; } = true;
        /// <summary>
        /// 允许文件扩展名(jpg,rar,gif...)
        /// </summary>
        public string FileExtensions { get; set; }
        /// <summary>
        /// 上传Url
        /// </summary>
        public string UploadUrl { get; set; }
        /// <summary>
        /// 附件扩展数据.例如：{fid:'111'}
        /// </summary>
        public string ExtraData { get; set; }
       
        /// <summary>
        /// 最大上传文件数量
        /// </summary>
        public int MaxFilecount { get; set; }
        /// <summary>
        /// 最大上传文件大小
        /// </summary>
        public int MaxFilesize { get; set; }
        /// <summary>
        /// 设置标题
        /// </summary>
        public bool ShowCaption { get; set; }
        public bool OverwriteInitial { get; set; }

        public FapFileTagHelper()
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "label";
            output.Content.Clear();
            string id = "fapfile";
            if(Id.IsPresent())
            {
                id = $"file-{Id}";
            }
            FileInput file= new FileInput(id);
            if(ShowCaption)
            {
                file.SetShowCaption(ShowCaption);
            }
            if(FileExtensions.IsPresent())
            {
                string[] exts= FileExtensions.Split(',');
                List<string> list = new List<string>();
                foreach (var item in exts)
                {
                    list.Add(item);
                }
                file.SetAllowedFileExtensions(list.ToJson());
            }
            if(UploadUrl.IsPresent())
            {
                file.SetUploadUrl(UploadUrl);
            }
            if(MaxFilecount>0)
            {
                file.SetMaxFileCount(MaxFilecount);
            }
            if(MaxFilesize>0)
            {
                file.SetMaxFileSize(MaxFilesize);
            }
            if(OverwriteInitial)
            {
                file.SetOverwriteInitial(OverwriteInitial);
            }
            if(ExtraData.IsPresent())
            {
                file.SetUploadExtraData(ExtraData);
            }
            if(AutoUpload)
            {
                file.AutoUpload = AutoUpload;
            }
            output.Content.AppendHtml(file.ToString());

        }

    }
}
