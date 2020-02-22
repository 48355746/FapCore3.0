using System.Collections.Generic;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;

namespace Fap.Core.MultiLanguage
{
    public interface IMultiLangService
    {
        MultiLanguageEnum CurrentLanguage { get; }
        string CurrentLanguageName { get; }
        IEnumerable<string> GetAllLanguage();
        string GetOrAndMultiLangValue(MultiLanguageOriginEnum qualifer, string langkey, string langValue);
        string GetMultiLangValue(MultiLanguageOriginEnum qualifer, string langkey);
        void CreateMultilanguageJsFile();
    }
}