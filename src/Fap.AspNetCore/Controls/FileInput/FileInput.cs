using Microsoft.AspNetCore.Html;
using System.Text;
using System.Web;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls
{
    /// <summary>
    /// 文件上传控件
    /// js：~/Content/fileinput/fileinput.js,~/Content/fileinput/fileinput_locale_zh.js
    /// css:~/Content/css/fileinput.css
    /// </summary>
    public class FileInput : HtmlString
    {
        private string _id;
        //上传Url
        private string _uploadUrl;
        /// <summary>
        /// 上传Url
        /// </summary>
        /// <param name="uploadUrl"></param>
        /// <returns></returns>
        public FileInput SetUploadUrl(string uploadUrl)
        {
            _uploadUrl = uploadUrl;
            return this;
        }
        //多语 中文 zh
        private string _language="zh";
        /// <summary>
        /// 多语 中文 zh
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public FileInput SetLanguage(string language)
        {
            _language = language;
            return this;
        }
        private bool _autoUpload;
        //允许上传文件的扩展名，例如：["jpg", "png", "gif"]
        private string _allowedFileExtensions;
        /// <summary>
        /// 允许上传文件的扩展名，例如：["jpg", "png", "gif"]
        /// </summary>
        /// <param name="allowedFileExtensions"></param>
        /// <returns></returns>
        public FileInput SetAllowedFileExtensions(string allowedFileExtensions)
        {
            _allowedFileExtensions = allowedFileExtensions;
            return this;
        }
        //重新初始化，每次添加文件的时候
        private bool? _overwriteInitial;
        /// <summary>
        /// 重新初始化，每次添加文件的时候
        /// </summary>
        /// <param name="overwriteInitial"></param>
        /// <returns></returns>
        public FileInput SetOverwriteInitial(bool overwriteInitial)
        {
            _overwriteInitial = overwriteInitial;
            return this;
        }
        //文件上传大小
        private int? _maxFileSize;
        /// <summary>
        /// 文件上传大小
        /// </summary>
        /// <param name="maxFileSize"></param>
        /// <returns></returns>
        public FileInput SetMaxFileSize(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
            return this;
        }
        //上传文件个数
        private int? _maxFileCount;
        /// <summary>
        /// 上传文件个数
        /// </summary>
        /// <param name="maxFileCount"></param>
        /// <returns></returns>
        public FileInput SetMaxFileCount(int maxFileCount)
        {
            _maxFileCount = maxFileCount;
            return this;
        }
        //显示关闭
        private bool? _showClose;
        /// <summary>
        /// 显示关闭
        /// </summary>
        /// <param name="showClose"></param>
        /// <returns></returns>
        public FileInput SetShowClose(bool showClose)
        {
            _showClose = showClose;
            return this;
        }
        //显示文件标题，上传控件文本框
        private bool? _showCaption;
        /// <summary>
        /// 显示文件标题，上传控件文本框
        /// </summary>
        /// <param name="showCaption"></param>
        /// <returns></returns>
        public FileInput SetShowCaption(bool showCaption)
        {
            _showCaption = showCaption;
            return this;
        }
        //浏览按钮文字
        private string _browseLabel;
        public FileInput SetBrowseLabel(string browseLabel)
        {
            _browseLabel = browseLabel;
            return this;
        }
        //浏览按钮的样式，例如："btn btn-primary btn-block"
        private string _browseClass;
        /// <summary>
        /// 浏览按钮的样式，例如："btn btn-primary btn-block"
        /// </summary>
        /// <param name="browseClass"></param>
        /// <returns></returns>
        public FileInput SetBrowseClass(string browseClass)
        {
            _browseClass = browseClass;
            return this;
        }
        //浏览按钮图标，例如："<i class=\"glyphicon glyphicon-picture\"></i> "
        private string _browseIcon;
        /// <summary>
        /// 浏览按钮图标，例如："<i class=\"glyphicon glyphicon-picture\"></i> "
        /// </summary>
        /// <param name="browseIcon"></param>
        /// <returns></returns>
        public FileInput SetBrowseIcon(string browseIcon)
        {
            _browseIcon = browseIcon;
            return this;
        }
        //删除按钮文字
        private string _removeLabel;
        /// <summary>
        /// 删除按钮文字
        /// </summary>
        /// <param name="romoveLabel"></param>
        /// <returns></returns>
        public FileInput SetRemoceLabel(string romoveLabel)
        {
            _removeLabel = romoveLabel;
            return this;
        }
        //移除按钮样式，例如："btn btn-danger"
        private string _removeClass;
        /// <summary>
        /// 移除按钮样式，例如："btn btn-danger"
        /// </summary>
        /// <param name="removeClass"></param>
        /// <returns></returns>
        public FileInput SetRemoveClass(string removeClass)
        {
            _removeClass = removeClass;
            return this;
        }
        //移除按钮图标，例如："<i class=\"glyphicon glyphicon-trash\"></i> "
        private string _removeIcon;
        /// <summary>
        /// 移除按钮图标，例如："<i class=\"glyphicon glyphicon-trash\"></i> "
        /// </summary>
        /// <param name="removeIcon"></param>
        /// <returns></returns>
        public FileInput SetRemoveIcon(string removeIcon)
        {
            _removeIcon = removeIcon;
            return this;
        }
        //移除按钮文字
        private string _removeTitle;
        /// <summary>
        /// 移除按钮文字
        /// </summary>
        /// <param name="removeTitle"></param>
        /// <returns></returns>
        public FileInput SetRemoveTitle(string removeTitle)
        {
            _removeTitle = removeTitle;
            return this;
        }
        //上传按钮文字，例如: "上传",
        private string _uploadLabel;
        /// <summary>
        /// 上传按钮文字，例如: "上传",
        /// </summary>
        /// <param name="uploadLabel"></param>
        /// <returns></returns>
        public FileInput SetUploadLable(string uploadLabel)
        {
            _uploadLabel = uploadLabel;
            return this;
        }
        //上传按钮样式，例如: "btn btn-info"
        private string _uploadClass;
        /// <summary>
        /// 上传按钮样式，例如: "btn btn-info"
        /// </summary>
        /// <param name="uploadClass"></param>
        /// <returns></returns>
        public FileInput SetUploadClass(string uploadClass)
        {
            _uploadClass = uploadClass;
            return this;
        }
        //上传按钮图标，例如: "<i class=\"glyphicon glyphicon-upload\"></i> "
        private string _uploadIcon;
        /// <summary>
        /// 上传按钮图标，例如: "<i class=\"glyphicon glyphicon-upload\"></i> "
        /// </summary>
        /// <param name="uploadIcon"></param>
        /// <returns></returns>
        public FileInput SetUploadIcon(string uploadIcon)
        {
            _uploadIcon = uploadIcon;
            return this;
        }
        //显示错误信息容器id，例如：'#kv-avatar-errors'
        private string _elErrorContainer;
        /// <summary>
        /// 显示错误信息容器id，例如：'#kv-avatar-errors'
        /// </summary>
        /// <param name="elErrorContainer"></param>
        /// <returns></returns>
        public FileInput SetElErrowContainer(string elErrorContainer)
        {
            _elErrorContainer = elErrorContainer;
            return this;
        }
        //错误信息样式类，例如：'alert alert-block alert-danger'
        private string _msgErrorClass;
        /// <summary>
        /// 错误信息样式类，例如：'alert alert-block alert-danger'
        /// </summary>
        /// <param name="msgErrorClass"></param>
        /// <returns></returns>
        public FileInput SetMsgErrorClass(string msgErrorClass)
        {
            _msgErrorClass = msgErrorClass;
            return this;
        }
        //默认预览内容，例如：'<img src="/uploads/default_avatar_male.jpg" alt="Your Avatar" style="width:160px">'
        private string _defaultPreviewContent;
        /// <summary>
        /// 默认预览内容，例如：'<img src="/uploads/default_avatar_male.jpg" alt="Your Avatar" style="width:160px">'
        /// </summary>
        /// <param name="defaultPreviewContent"></param>
        /// <returns></returns>
        public FileInput SetDefaultPreviewContent(string defaultPreviewContent)
        {
            _defaultPreviewContent = defaultPreviewContent;
            return this;
        }
        //上传布局模板，例如：{main2: '{preview} ' +  btnCust + ' {remove} {browse}'}。btnCust为html内容。其他固定
        private string _layoutTemplates;
        /// <summary>
        /// 上传布局模板，例如：{main2: '{preview} ' +  btnCust + ' {remove} {browse}'}。btnCust为html内容。其他固定
        /// </summary>
        /// <param name="layoutTemplate"></param>
        /// <returns></returns>
        public FileInput SetLayoutTemplates(string layoutTemplate)
        {
            _layoutTemplates = layoutTemplate;
            return this;
        }
        //最大图片高度，需要引入 <script src="/js/plugins/canvas-to-blob.js"></script>
        private int? _maxImageHeight;
        /// <summary>
        /// 最大图片高度，需要引入 <script src="/js/plugins/canvas-to-blob.js"></script>
        /// </summary>
        /// <param name="maxImageHeight"></param>
        /// <returns></returns>
        public FileInput SetMaxImageHeight(int maxImageHeight)
        {
            _maxImageHeight = maxImageHeight;
            return this;
        }
        //最大图片宽度，需要引入 <script src="/js/plugins/canvas-to-blob.js"></script>
        private int? _maxImageWidth;
        /// <summary>
        /// 最大图片宽度，需要引入 <script src="/js/plugins/canvas-to-blob.js"></script>
        /// </summary>
        /// <param name="maxImageWidth"></param>
        /// <returns></returns>
        public FileInput SetMaxImageWidth(int maxImageWidth)
        {
            _maxImageWidth = maxImageWidth;
            return this;
        }
        //改变图片大小
        private bool? _resizeImage;
        /// <summary>
        /// 改变图片大小
        /// </summary>
        /// <param name="resizeImage"></param>
        /// <returns></returns>
        public FileInput SetResieImage(bool resizeImage)
        {
            _resizeImage = resizeImage;
            return this;
        }
        //初始化标题，_showCaption为true的时候起作用
        private string _initialCaption;
        /// <summary>
        /// 初始化标题，_showCaption为true的时候起作用
        /// </summary>
        /// <param name="initialCaption"></param>
        /// <returns></returns>
        public FileInput SetInitialCaption(string initialCaption)
        {
            _initialCaption = initialCaption;
            return this;
        }
        //初始化预览。例如： [
        //    '<img src="/images/moon.jpg" class="file-preview-image" alt="The Moon" title="The Moon">',
        //    '<img src="/images/earth.jpg" class="file-preview-image" alt="The Earth" title="The Earth">'
        //]
        private string _initialPreview;
        /// <summary>
        /// 初始化预览[
        ///    '<img src="/images/moon.jpg" class="file-preview-image" alt="The Moon" title="The Moon">',
        ///    '<img src="/images/earth.jpg" class="file-preview-image" alt="The Earth" title="The Earth">'
        ///]
        /// </summary>
        /// <param name="initialPreview"></param>
        /// <returns></returns>
        public FileInput SetInitialPreview(string initialPreview)
        {
            _initialPreview = initialPreview;
            return this;
        }
        //是否显示预览框
        private bool? _showPreview;
        /// <summary>
        /// 是否显示预览框，默认true
        /// </summary>
        /// <param name="showPreview"></param>
        /// <returns></returns>
        public FileInput SetShowPreview(bool showPreview)
        {
            _showPreview = showPreview;
            return this;
        }
        //向服务器传的附件数据，保存的时候用,json,例如：{kvId: '10'},
        private string _uploadExtraData;
        /// <summary>
        /// 自动上传
        /// </summary>
        public bool AutoUpload { get => _autoUpload; set => _autoUpload = value; }

        /// <summary>
        /// 向服务器传的附件数据，保存的时候用,json,例如：{kvId: '10'},
        /// </summary>
        /// <param name="uploadExtraData"></param>
        /// <returns></returns>
        public FileInput SetUploadExtraData(string uploadExtraData)
        {
            _uploadExtraData = uploadExtraData;
            return this;
        }

        public FileInput(string id):base("")
        {
            _id = id;
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
            return script + RenderHtml();

        }

        private string RenderJavascript()
        {
            StringBuilder script = new StringBuilder();
            script.Append(" $(function () {").AppendLine();

            script.Append("$(\"#" + _id + "\").fileinput({");
            if(!string.IsNullOrWhiteSpace(_language))
            {
                script.AppendFormat("language:'{0}',", _language).AppendLine();
            }
            if (_overwriteInitial.HasValue)
            {
                script.AppendFormat("overwriteInitial: {0},", _overwriteInitial.Value.ToString().ToLower()).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_uploadUrl))
            {
                script.AppendFormat("uploadUrl:'{0}',", _uploadUrl).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_allowedFileExtensions))
            {
                script.AppendFormat("allowedFileExtensions: {0},", _allowedFileExtensions).AppendLine();
            }
            if (_maxFileSize.HasValue)
            {
                script.AppendFormat("maxFileSize:{0},", _maxFileSize).AppendLine();
            }
            if (_maxFileCount.HasValue)
            {
                script.AppendFormat("maxFileCount:{0},", _maxFileCount).AppendLine();
            }
           
            if (_showCaption.HasValue)
            {
                script.AppendFormat("showCaption:{0},", _showCaption.Value.ToString().ToLower()).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_browseLabel))
            {
                script.AppendFormat("browseLabel: '{0}',", _browseLabel).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_browseIcon))
            {
                script.AppendFormat("browseIcon: '{0}',", _browseIcon).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_browseClass))
            {
                script.AppendFormat("browseClass: '{0}',", _browseClass).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_removeLabel))
            {
                script.AppendFormat("removeLabel: '{0}',", _removeLabel).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_browseIcon))
            {
                script.AppendFormat("browseIcon: '{0}',", _browseIcon).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_removeClass))
            {
                script.AppendFormat("removeClass: '{0}',", _removeClass).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(_removeIcon))
            {
                script.AppendFormat("removeIcon: '{0}',", _removeIcon).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_removeTitle))
            {
                script.AppendFormat("removeTitle: '{0}',", _removeTitle).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(_uploadLabel))
            {
                script.AppendFormat("uploadLabel: '{0}',", _uploadLabel).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(_uploadClass))
            {
                script.AppendFormat("uploadClass: '{0}',", _uploadClass).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_uploadIcon))
            {
                script.AppendFormat("uploadIcon: '{0}',", _uploadIcon).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(_elErrorContainer))
            {
                script.AppendFormat("elErrorContainer: '{0}',", _elErrorContainer).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_msgErrorClass))
            {
                script.AppendFormat("msgErrorClass: '{0}',", _msgErrorClass).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_defaultPreviewContent))
            {
                script.AppendFormat("defaultPreviewContent: '{0}',", _defaultPreviewContent).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_layoutTemplates))
            {
                script.AppendFormat("layoutTemplates: '{0}',", _layoutTemplates).AppendLine();
            }
            if (_maxImageHeight.HasValue)
            {
                script.AppendFormat("maxImageHeight: {0},", _maxImageHeight.Value).AppendLine();
            }
            if (_maxImageWidth.HasValue)
            {
                script.AppendFormat("maxImageWidth: {0},", _maxImageWidth.Value).AppendLine();
            }
            if (_resizeImage.HasValue)
            {
                script.AppendFormat("resizeImage: {0},", _resizeImage.Value).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_initialCaption))
            {
                script.AppendFormat("initialCaption: '{0}',", _initialCaption).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_initialPreview))
            {
                script.AppendFormat("initialPreview: {0},", _initialPreview).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_uploadExtraData))
            {
                script.AppendFormat("uploadExtraData: {0},", _uploadExtraData).AppendLine();
            }
            if (_showPreview.HasValue)
            {
                script.AppendFormat("showPreview: {0},", _showPreview.Value.ToString().ToLower()).AppendLine();
            }
            if (_showClose.HasValue)
            {
                script.AppendFormat("showClose:{0}", _showClose.Value.ToString().ToLower()).AppendLine();
            }
            else
            {
                script.AppendLine("showClose:true");
            }
            if (AutoUpload)
            {
                script.AppendLine(@"}).on('filebatchselected', function(event, files) {
                                              $(this).fileinput('upload');
                                                        })");
            }
            else
            {
                script.AppendLine("});");
            }
            script.AppendLine(" });");
            return script.ToString();
        }

        private string RenderHtml()
        {
            string multiple = "";
            if (_maxFileCount > 1)
            {
                multiple = "multiple";
            }
            return "<input id=\"" + _id + "\" type=\"file\" " + multiple + " class=\"file-loading\">";
        }
        public string ToHtmlString()
        {
            return ToString();
        }
    }
}
