using Fap.Core.Infrastructure.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Fap.Core.Infrastructure
{
    public interface ISurveyService
    {
        bool ExportSurveyStat(string fileName, string surveyUid);
        bool ExportSurveyUserDataStat(string fileName, string surveyUid, string category);
        JObject GetProject(Survey survey);
        JArray GetReportFilter(string surveyUid);
        JObject GetResponseList(string surveyUid);
        JArray GetSurveyQuestion(Survey survey);
        JObject GetSurveyReportResult(string surveyUid, int sortIndex);
        JObject GetSurveyResponse(string responseUid, string surveyUid, string status, int sortIndex);
        void PreviewSurvey(Survey survey);
        bool PublishSurvey(SurFilter surFilter);
        IEnumerable<SurReportFilter> SaveReportFilters(string surveyUid, string filters);
        bool SaveSelfCollectionSurvey(JObject jobj);
        JObject SurReportFilterToJSON(SurReportFilter filter);
    }
}