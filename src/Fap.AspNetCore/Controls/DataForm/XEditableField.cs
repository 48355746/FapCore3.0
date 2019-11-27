using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using System.Text;

namespace Fap.AspNetCore.Controls.DataForm
{
    /// <summary>
    /// 可编辑表单字段
    /// </summary>
    public class XEditableField
    {
        private IMultiLangService _multiLang;
        public XEditableField(IMultiLangService  multiLangService)
        {
            _multiLang = multiLangService;
        }
        /// <summary>
        /// 对应FapColumn
        /// </summary>
        public FapColumn CurrFapColumn { get; set; }
        public dynamic EntityData { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" <div class=\"profile-info-row\">");
            string require = string.Empty;
            if (CurrFapColumn.NullAble == 0)
            {
                require = "<span class=\"text-danger\">*</span>";
            }
            sb.AppendFormat("		<div class=\"profile-info-name\"> {0} {1}</div>", _multiLang.GetLangColumnComent(CurrFapColumn), require).AppendLine();
            sb.AppendLine("		<div class=\"profile-info-value\">");
            if (CurrFapColumn.EditAble == 1)
            {
                if (CurrFapColumn.CtrlType == FapColumn.CTRL_TYPE_MEMO)
                {
                    sb.AppendFormat("		 	<span class=\"editable\" id=\"{0}\">{1}</span>", CurrFapColumn.ColName, FieldValue).AppendLine();
                }
                else
                {
                    sb.AppendFormat("		 	<span class=\"editable\" id=\"{0}\"></span>", CurrFapColumn.ColName).AppendLine();

                }

            }
            else
            {
                if (CurrFapColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                {
                    string value = EntityData.Get(CurrFapColumn.TableName + "_" + CurrFapColumn.ColName + "MC");
                    if (value != null)
                    {
                        sb.AppendFormat("		 	<span class=\"editable\" >{0}</span>", value).AppendLine();
                    }
                    else
                    {
                        sb.AppendLine("		 	<span class=\"editable\" ></span>");
                    }
                }
                else
                {

                    sb.AppendFormat("		 	<span class=\"editable\" >{0}</span>", FieldValue).AppendLine();
                }
            }
            sb.AppendLine("		</div>");
            sb.AppendLine(" </div>");

            return sb.ToString();
        }
    }
}
