
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Http;

namespace Fap.Core.Rbac
{
    /// <summary>
    /// 表示该接口的实现类是用户上下文贮存器。
    /// </summary>
    public interface IFapSessionStorage
    {
        /// <summary>
        /// 根Url
        /// </summary>
        string BaseURL { get; }
        /// <summary>
        /// 客户IP地址
        /// </summary>
        string ClientIPAddress { get; }
        /// <summary>
        /// 当前语言
        /// </summary>
        MultiLanguageEnum Language { get; }
        /// <summary>
        /// 员工Fid
        /// </summary>
        string EmpUid { get; }
        /// <summary>
        /// 员工名称
        /// </summary>
        string EmpName { get; }
        /// <summary>
        /// 是否为开发者账号
        /// </summary>
        bool IsDeveloper { get; }
        /// <summary>
        /// 用户Fid
        /// </summary>
        string UserUid { get; }
        /// <summary>
        /// 用户类型
        /// </summary>
        string UserType { get; }
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// 在线用户Uid
        /// </summary>
        string OnlineUserUid { get;  }
        /// <summary>
        /// 部门Fid
        /// </summary>
        string DeptUid { get; }
        /// <summary>
        /// 部门编码
        /// </summary>
        string DeptCode { get;}
        /// <summary>
        /// 部门名称
        /// </summary>
        string DeptName { get; }
        /// <summary>
        /// 组织
        /// </summary>
        string OrgUid { get; }
        /// <summary>
        /// 集团
        /// </summary>
        string GroupUid { get; }
        
        /// <summary>
        /// 清空session
        /// </summary>
        void Clear();
        /// <summary>
        /// 从session中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetData<T>(string key);
        /// <summary>
        /// 移除session中的特定键
        /// </summary>
        /// <param name="key"></param>
        void RemoveData(string key);
        /// <summary>
        /// 设置session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetData<T>(string key, T data);
        /// <summary>
        /// 权限账号信息
        /// </summary>
        IFapAcSession AcSession { get;  }
        /// <summary>
        /// 获取令牌（防止CSRF）
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GetAndStoreTokens(string tableName);
        /// <summary>
        /// 封装有关单个HTTP请求的所有特定于HTTP的信息。
        /// </summary>
        HttpContext Context { get; set; }
    }
}
