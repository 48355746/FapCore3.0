using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Interceptor
{
    [Service]
    public class FapMenuDataInterceptor: DataInterceptorBase
    {
        public FapMenuDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
        }
        private FapMenu ToFapMenu(FapDynamicObject dynamicObject) => new FapMenu
        {
            Fid=dynamicObject.Get(nameof(FapMenu.Fid)).ToString(),
            MenuCode=dynamicObject.Get(nameof(FapMenu.MenuCode)).ToString(),
            MenuName=dynamicObject.Get(nameof(FapMenu.MenuName)).ToString()
            
        };
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            FapMenu menu = ToFapMenu(fapDynamicData);
            _dbContext.Insert(new FapMultiLanguage { Qualifier=MultiLanguageOriginEnum.Menu.ToString(),LangKey=menu.Fid,LangValue=menu.MenuName });
            RefreshCache();
        }

        private void RefreshCache()
        {
            _appDomain.MenuSet.Refresh();
            _appDomain.MultiLangSet.Refresh();
        }

        public override void BeforeDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            _dbContext.DeleteExec(nameof(FapMultiLanguage), "Qualifier=@Qualifier and LangKey=@LangKey", new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.Menu.ToString(), LangKey = fid }));
            RefreshCache();
        }
        public override void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            FapMenu menu = ToFapMenu(fapDynamicData);
            FapMenu oriMenu = _dbContext.Get<FapMenu>(menu.Fid);
            if (!menu.MenuName.Equals(oriMenu.MenuName))
            {
                string langkey = menu.Fid;
                string updateMultisql = $"Update {nameof(FapMultiLanguage)} set {nameof(FapMultiLanguage.LangValue)}=@LangValue where Qualifier=@Qualifier and LangKey=@LangKey";
                var param = new Dapper.DynamicParameters(new { Qualifier = MultiLanguageOriginEnum.Menu.ToString(), LangKey = langkey, LangValue = menu.MenuName });
                _dbContext.Execute(updateMultisql, param);
            }
            RefreshCache();
        }
    }
}
