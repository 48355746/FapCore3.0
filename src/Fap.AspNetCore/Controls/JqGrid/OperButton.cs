using Fap.Core.Extensions;

namespace Fap.AspNetCore.Controls.JqGrid
{
    public class OperButton
    {
        /// <summary>
        /// 按钮样式 
        /// btn-warning
        /// </summary>
        public string BtnClass { get; set; }
        /// <summary>
        /// 图标
        /// fa fa-flag
        /// </summary>
        public string IconClass { get; set; }
        /// <summary>
        /// 显示内容
        /// </summary>
        public string BtnContent { get; set; }
        /// <summary>
        /// Jscript函数
        /// </summary>
        public string BtnClickName { get; set; }
        /// <summary>
        /// 列索引1,2可以多个
        /// </summary>
        public string ArgumentsColumn { get; set; }

        public override string ToString()
        {
            string strFun = string.Empty;
            if (ArgumentsColumn.IsPresent())
            {
                foreach (string args in ArgumentsColumn.Split(','))
                {
                    //strFun += "'\"+rows[" + s + "]+\"',";

                    strFun += "'\"+rowObject."+args+"+\"',";
                }
            }
            else
            {
                //strFun = "'\"+rows[1]+\"'";
                strFun = "'\"+rowObject.Fid+\"'";
            }
            return "<button class=\\\"btn btn-xs " + BtnClass + "\\\" onclick=\\\"" + BtnClickName + "("+strFun.TrimEnd(',')+");\\\"><i class=\\\"ace-icon " + IconClass + " bigger-110\\\"></i>" + BtnContent + "	</button>";
        }

    }
}
