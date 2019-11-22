using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// webapi基类
    /// </summary>
    [Authorize]
    public class FapApiController: ControllerBase
    {
        /// <summary>
        /// 编码UTF-8
        /// </summary>
        protected Encoding ENCODE_UTF8 = Encoding.GetEncoding("UTF-8");
        protected IDbContext _dataAccessor;
        protected IFapPlatformDomain _appDomain;
        protected IFapConfigService _configService;
        protected IFapApplicationContext _applicationContext;
        protected ILoggerFactory _loggerFactory;
        protected IRbacService _rbacService;
        public FapApiController(IDbContext dataAccessor, IFapPlatformDomain platformDomain, IFapConfigService configService, IFapApplicationContext applicationContext, ILoggerFactory loggerFactory, IRbacService rbacService)
        {
            _dataAccessor = dataAccessor;
            _appDomain = platformDomain;
            _configService = configService;
            _applicationContext = applicationContext;
            _rbacService = rbacService;
            _loggerFactory = loggerFactory;
        }
        /// <summary>
        /// 视图模型校验
        /// </summary>
        /// <returns></returns>
        protected string ModelValide()
        {
            if (!ModelState.IsValid)
            {
                string errorMsg = string.Empty;
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errorMsg += error.ErrorMessage + "|";
                    }
                }

                return errorMsg;
            }
            return string.Empty;
        }
       
    }
}
