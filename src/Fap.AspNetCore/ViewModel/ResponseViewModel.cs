namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 表示请求响应事务中的响应数据的模型。
    /// {"resultType":"success",id:"","msg":""}
    /// </summary>
    public class ResponseViewModel : IViewModel
    {       
        /// <summary>
        /// 一般只有在表格内编辑模式的时候才用赋此值
        /// 为处理表格内编辑模式（RowEditor）游离对象时返回Id
        /// </summary>
        public object id { get; set; }
        /// <summary>
        /// 响应成功状态
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 响应状态编码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 表示请求响应事务服务端返回的业务特定说明文字。
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object data { get; set; }
        
    }
    public class ResponseViewModelUtils
    {
        public static ResponseViewModel Sueecss(object data,string msg= "操作成功")
        {            
            ResponseViewModel rvm = new ResponseViewModel()
            {
                code=200,
                success = true,
                msg = msg,
                data = data,
            };
            return rvm;
        }
        public static ResponseViewModel Sueecss(string msg = "操作成功")
        {
            ResponseViewModel rvm = new ResponseViewModel()
            {
                code=200,
                success = true,
                msg = msg
            };
            return rvm;
        }
        public static ResponseViewModel NotFound(string msg = "数据未找到", int code = 404)
        {
            return new ResponseViewModel()
            {
                code = code,
                success = false,
                msg = msg,
                data = null
            };
        }
        public static ResponseViewModel Failure(string msg="失败", int code=500)
        {
            return new ResponseViewModel()
            {
                code=code,
                success = false,
                msg = msg,
                data = null
            };
        }
    }
}
