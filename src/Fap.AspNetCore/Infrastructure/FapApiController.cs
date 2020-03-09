using Fap.AspNetCore.Serivce;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Query;
using Fap.AspNetCore.Model;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// webapi基类
    /// </summary>
    [Authorize]
    //[ApiController]  //ApiController使用独立管道不支持  application/x-www-form-urlencoded
    public class FapApiController : ControllerBase
    {
        /// <summary>
        /// 编码UTF-8
        /// </summary>
        protected Encoding ENCODE_UTF8 = Encoding.GetEncoding("UTF-8");
        protected IDbContext _dbContext => _serviceProvider.GetService<IDbContext>();
        protected IFapPlatformDomain _platformDomain => _serviceProvider.GetService<IFapPlatformDomain>();
        protected IFapConfigService _configService => _serviceProvider.GetService<IFapConfigService>();
        protected IMultiLangService _multiLangService => _serviceProvider.GetService<IMultiLangService>();
        protected IFapApplicationContext _applicationContext => _serviceProvider.GetService<IFapApplicationContext>();
        protected ILoggerFactory _loggerFactory => _serviceProvider.GetService<ILoggerFactory>();
        protected IRbacService _rbacService => _serviceProvider.GetService<IRbacService>();

        public IServiceProvider _serviceProvider { get; set; }
        public IGridFormService _gridFormService => _serviceProvider.GetService<IGridFormService>();
        public FapApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
