using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fap.Core.MultiLanguage
{
    /// <summary>
    /// 多语帮助者
    /// </summary>
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class MultiLangService : IMultiLangService
    {
        private IFapPlatformDomain _appDomain;
        private IFapApplicationContext _applicationContext;
        private IDbContext _dbContext;
        public MultiLangService(IFapPlatformDomain appDomain, IFapApplicationContext applicationContext, IDbContext dbContext)
        {
            _appDomain = appDomain;
            _applicationContext = applicationContext;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 当前语种ZhHK繁体， En英语， Ja日语， 默认是“”简体中文
        /// </summary>
        public MultiLanguageEnum CurrentLanguage => _applicationContext.Language;


        /// <summary>
        /// 当前语种名称
        /// </summary>
        public string CurrentLanguageName => Enum.GetName(typeof(MultiLanguageEnum), CurrentLanguage);


        /// <summary>
        /// 获取所有语种
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllLanguage()
        {
            foreach (int lang in Enum.GetValues(typeof(MultiLanguageEnum)))
            {
                string name = Enum.GetName(typeof(MultiLanguageEnum), lang);//获取名称
                yield return name;
            }

        }
        public IEnumerable<FapColumn> GetMultiLanguageColumns(string tableName)
        {
            var fapColumns = _dbContext.Columns(tableName);
            foreach (var column in fapColumns)
            {
                string label= GetMultiLangValue(MultiLanguageOriginEnum.FapColumn, $"{column.TableName}_{column.ColName}");
                if (label.IsPresent()) {
                    column.ColComment = label;
                }
                else
                {
                    column.ColComment = column.ColComment;
                }
            }
            return fapColumns;
        }

        /// <summary>
        /// 获取所有资源多语
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapMultiLanguage> GetAllResMultiLang()
        {
            return _appDomain.MultiLangSet;
        }
        public string GetOrAndMultiLangValue(MultiLanguageOriginEnum qualifer, string langkey, string langValue)
        {
            if (_appDomain.MultiLangSet.TryGetValue(qualifer.ToString(), langkey, out FapMultiLanguage language))
            {
                string v = GetLangValue(language);
                if (v.IsMissing())
                {
                    return language.LangValue;
                }
                return v;
            }
            else
            {
                _dbContext.Insert(new FapMultiLanguage { Qualifier = qualifer.ToString(), LangKey = langkey, LangValue = langValue, LangValueZhCn = langValue });
                _appDomain.MultiLangSet.Refresh();
                return langValue;
            }
        }
        public string GetMultiLangValue(MultiLanguageOriginEnum qualifer, string langkey)
        {
            if (_appDomain.MultiLangSet.TryGetValue(qualifer.ToString(), langkey, out FapMultiLanguage language))
            {
                string v = GetLangValue(language);
                if (v.IsMissing())
                {
                    return language.LangValue;
                }
                return v;
            }
            return string.Empty;
        }
        private string GetLangValue(FapMultiLanguage language) => CurrentLanguage switch
        {
            MultiLanguageEnum.ZhCn => language.LangValueZhCn,
            MultiLanguageEnum.En => language.LangValueEn,
            MultiLanguageEnum.Ja => language.LangValueJa,
            MultiLanguageEnum.ZhTW => language.LangValueZhTW,
            _ => language.LangValue
        };


        /// <summary>
        /// 初始化资源多语JS文件内容
        /// </summary>
        public void CreateMultilanguageJsFile()
        {
            string jsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Scripts");
            _appDomain.MultiLangSet.TryGetValue(MultiLanguageOriginEnum.Javascript.ToString(), out IEnumerable<FapMultiLanguage> resList);
            string jsTemplate = @"
//JS资源多语， 由系统自动生成， 请勿修改
var MultiLangHelper = (function () {
    var resLang = 'ZhCn';
    var resArray = { 
##keyvalues##
    };
    var Helper = {};
    //初始化语种
    Helper.initLang = function (lang) {
        resLang = lang;
    };
    //获得资源多语
    Helper.getResName = function (code, defaultName) {
        if (resArray[code] === undefined) {
            $.post(basePath +'/Core/Api/MultiLanguage',{ langKey: code, langValue: defaultName }, function (rv) {

            });
            return defaultName;
        }
        return resArray[code][resLang] || defaultName;
    };

    return Helper;
})();";
            StringBuilder builder = new StringBuilder();
            foreach (var res in resList)
            {
                builder.Append("      ").AppendLine($"'{res.LangKey.Trim()}': {{ 'ZhCn': '{FormateName(res.LangValueZhCn)}', 'ZhTW': '{FormateName(res.LangValueZhTW)}', 'En': '{FormateName(res.LangValueEn)}', 'Ja': '{FormateName(res.LangValueJa)}' }},");
            }
            jsTemplate = jsTemplate.Replace("##keyvalues##", builder.ToString().TrimEnd('\n').TrimEnd('\r').TrimEnd(','));
            if (Directory.Exists(jsPath))
            {
                File.WriteAllText(Path.Combine(jsPath, "MultiLanguage.js"), jsTemplate, Encoding.UTF8);
            }

        }

        private string FormateName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            int lengthh = value.Length;
            StringBuilder filtered = new StringBuilder(lengthh);
            char prevChar = '\u0000';
            char c;
            for (int i = 0; i < lengthh; i++)
            {
                c = value[i];
                if (c == '"')
                {
                    filtered.Append("\\\"");
                }
                else if (c == '\'')
                {
                    filtered.Append("\\'");
                }
                else if (c == '\\')
                {
                    filtered.Append("\\\\");
                }
                else if (c == '\t')
                {
                    filtered.Append("\\t");
                }
                else if (c == '\n')
                {
                    if (prevChar != '\r')
                    {
                        filtered.Append("\\n");
                    }
                }
                else if (c == '\r')
                {
                    filtered.Append("\\n");
                }
                else if (c == '\f')
                {
                    filtered.Append("\\f");
                }
                else if (c == '/')
                {
                    filtered.Append("\\/");
                }
                else
                {
                    filtered.Append(c);
                }
                prevChar = c;
            }

            value = filtered.ToString();

            return value;
        }
    }

}
