using Fap.Core.Rbac.Model;
using System;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 会话业务实体
    /// </summary>
    [Serializable]
    public class FapAcSession : IFapAcSession
    {
        private readonly FapUser _fapUser;
        private  FapRole _fapRole;
        private readonly Employee _employee;
        private readonly FapOnlineUser _onlineUser;
        public FapAcSession(FapUser account, Employee employee, FapOnlineUser onlineUser,FapRole role, MultiLanguageEnum language = MultiLanguageEnum.ZhCn)
        {
            if (account == null)
            {
                //Identity = null;
                throw new ArgumentNullException("account");
            }
            else
            {
                //Identity = new FapIdentity(account.UserName);
            }
            _fapRole = role;
            _fapUser = account;
            _employee = employee;
            _onlineUser = onlineUser;
            _language = language;
        }

        
        /// <summary>
        /// 当前用户
        /// </summary>
        public Fap.Core.Rbac.Model.FapUser Account => _fapUser;
        

        /// <summary>
        /// 在线用户，含有角色
        /// </summary>
        public FapOnlineUser OnlineUser => _onlineUser;


        private MultiLanguageEnum _language = MultiLanguageEnum.ZhCn;
        /// <summary>
        /// 当前语种
        /// </summary>
        public MultiLanguageEnum Language => _language;
        


        /// <summary>
        /// 员工信息
        /// </summary>      

        public Employee Employee => _employee;

        public FapRole Role { get => _fapRole; set => _fapRole = value; }
    }
}