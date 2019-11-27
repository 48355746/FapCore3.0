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
        public MultiLangService(IFapPlatformDomain appDomain, IFapApplicationContext applicationContext)
        {
            _appDomain = appDomain;
            _applicationContext = applicationContext;
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

        /// <summary>
        /// 获取所有语种名称后缀
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllLanguageSuffix()
        {
            foreach (string name in Enum.GetNames(typeof(MultiLanguageEnum)))
            {
                if (name == "ZhCn")
                {
                   yield return "";
                }
                else
                {
                    yield return name;
                }
            }
        }

        /// <summary>
        /// 是否多语字段
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool IsMultiLanguageField(string fieldName)
        {
            foreach (string lang in GetAllLanguage())
            {
                if (fieldName.EndsWith("_" + lang))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取字段的多语字段名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetFieldMultiLangName(string fieldName)
        {
            if (CurrentLanguageName == "ZhCn")
            {
                return fieldName;
            }
            else
            {
                return fieldName + CurrentLanguageName;
            }
        }

        /// <summary>
        /// 得到资源多语名称
        /// </summary>
        /// <param name="resCode">资源编号</param>
        /// <param name="defaultName">默认值</param>
        /// <param name="multiLang">多语种优先级高</param>
        /// <returns></returns>
        public string GetResName(string resCode, string defaultName = "", string multiLang = "")
        {
            if (_appDomain.MultiLangSet.TryGetValueByCode(resCode, out FapResMultiLang resMultiLang))
            {
                return GetObjectPropertyValue<FapResMultiLang>(resMultiLang, "Res" + (multiLang == "" ? CurrentLanguageName : multiLang));
            }
            else
            {
                return defaultName;
            }
        }

        /// <summary>
        /// 得到资源多语名称
        /// </summary>
        /// <param name="resCode">资源编号</param>
        /// <param name="defaultName">默认值</param>
        /// <param name="multiLanguage">指定语种</param>
        /// <returns></returns>
        public string GetResNameByLang(string resCode, string defaultName = "", MultiLanguageEnum multiLanguage = MultiLanguageEnum.ZhCn)
        {
            FapResMultiLang resMultiLang;
            if (_appDomain.MultiLangSet.TryGetValueByCode(resCode, out resMultiLang))
            {
                string languageName = Enum.GetName(typeof(MultiLanguageEnum), multiLanguage);
                return GetObjectPropertyValue<FapResMultiLang>(resMultiLang, "Res" + languageName);
            }
            else
            {
                return defaultName;
            }
        }

        /// <summary>
        /// 获取所有资源多语
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapResMultiLang> GetAllResMultiLang()
        {
            return _appDomain.MultiLangSet.ToList();
        }

        private string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null) return string.Empty;
            object o = property.GetValue(t, null);
            if (o == null) return string.Empty;
            return o.ToString();
        }

        /// <summary>
        /// 初始化资源多语JS文件内容
        /// </summary>
        public void InitMultiLangResJS()
        {
            string jsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Scripts");

            IEnumerable<FapResMultiLang> resList = GetAllResMultiLang();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("//JS资源多语， 由系统自动生成， 请勿修改");
            builder.AppendLine(@"var MultiLangHelper = (function () {
	            var resLang = 'ZhCn';");
            builder.AppendLine("var resArray = {");
            if (resList != null && resList.Count() > 0)
            {
                foreach (var res in resList)
                {
                    builder.Append(res.ResCode.RemoveSpace()).Append(": {").AppendFormat("'ZhCn': '{0}', 'ZhTW': '{1}', 'En': '{2}', 'Ja': '{3}'", FormateName(res.ResZhCn), FormateName(res.ResZhTW), FormateName(res.ResEn), FormateName(res.ResJa)).AppendLine("}");
                    builder.Append(",");
                }
                builder.Remove(builder.Length - 1, 1);
                builder.AppendLine("");
            }

            builder.AppendLine("}");
            builder.AppendLine(@"var Helper = {};
                //初始化语种
                Helper.initLang = function(lang) {
		            resLang = lang;
	            };
	            //获得资源多语
                Helper.getResName = function (code, defaultName) {
                    return resArray[code][resLang] || defaultName;
                };
                return Helper;");
            builder.AppendLine("})();");
            if (Directory.Exists(jsPath))
            {
                using (FileStream fs = new FileStream(Path.Combine(jsPath, "MultiLang.js"), FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(builder.ToString());
                    }
                }
            }

        }
        /// <summary>
        /// 根据语种获取菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public string GetLangMenuName(FapMenu menu)
        {
            if (_applicationContext.Language == MultiLanguageEnum.En && menu.LangEn.IsPresent())
            {
                return menu.LangEn;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.Ja && menu.LangJa.IsPresent())
            {
                return menu.LangJa;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.ZhTW && menu.LangZhTW.IsPresent())
            {
                return menu.LangZhTW;
            }
            else
            {
                return menu.MenuName;
            }
        }
        /// <summary>
        /// 根据语种获取模块名称
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string GetLangModuleName(FapModule module)
        {
            if (_applicationContext.Language == MultiLanguageEnum.En && module.LangEn.IsPresent())
            {
                return module.LangEn;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.Ja && module.LangJa.IsPresent())
            {
                return module.LangJa;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.ZhTW && module.LangZhTW.IsPresent())
            {
                return module.LangZhTW;
            }
            else
            {
                return module.ModuleName;
            }
        }
        /// <summary>
        /// 根据语种获取表描述
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetLangTableComment(FapTable table)
        {
            if (_applicationContext.Language == MultiLanguageEnum.En && table.LangEn.IsPresent())
            {
                return table.LangEn;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.Ja && table.LangJa.IsPresent())
            {
                return table.LangJa;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.ZhTW && table.LangZhTW.IsPresent())
            {
                return table.LangZhTW;
            }
            else
            {
                return table.TableComment;
            }
        }
        /// <summary>
        /// 根据语种获取元数据名称
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetLangColumnComent(FapColumn column)
        {
            if (_applicationContext.Language == MultiLanguageEnum.En && column.LangEn.IsPresent())
            {
                return column.LangEn;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.Ja && column.LangJa.IsPresent())
            {
                return column.LangJa;
            }
            else if (_applicationContext.Language == MultiLanguageEnum.ZhTW && column.LangZhTW.IsPresent())
            {
                return column.LangZhTW;
            }
            else
            {
                return column.ColComment;
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
