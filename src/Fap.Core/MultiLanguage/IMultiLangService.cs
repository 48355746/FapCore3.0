using System.Collections.Generic;
using Fap.Core.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;

namespace Fap.Core.MultiLanguage
{
    public interface IMultiLangService
    {
        MultiLanguageEnum CurrentLanguage { get; }
        string CurrentLanguageName { get; }

        IEnumerable<string> GetAllLanguage();
        IEnumerable<string> GetAllLanguageSuffix();
        IEnumerable<FapResMultiLang> GetAllResMultiLang();
        string GetFieldMultiLangName(string fieldName);
        string GetLangColumnComent(FapColumn column);
        string GetLangMenuName(FapMenu menu);
        string GetLangModuleName(FapModule module);
        string GetLangTableComment(FapTable table);
        string GetResName(string resCode, string defaultName = "", string multiLang = "");
        string GetResNameByLang(string resCode, string defaultName = "", MultiLanguageEnum multiLanguage = MultiLanguageEnum.ZhCn);
        void InitMultiLangResJS();
        bool IsMultiLanguageField(string fieldName);
    }
}