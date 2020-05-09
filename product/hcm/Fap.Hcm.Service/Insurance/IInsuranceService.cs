using Fap.Core.Infrastructure.Model;
using Fap.Hcm.Service.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Insurance
{
    public interface IInsuranceService
    {
        void UseInsPending(InsToDo insToDo);
        IEnumerable<CaseItem> GetInsItems(string caseUid);
        void AddInsItems(string caseUid, string[] insItems);
        long CreateInsCase(string caseUid);
        void InitEmployeeToInsCase(InsCase insCase, string empWhere);
        void InitInsuranceData(InitDataViewModel initData);
        void InsuranceOff(string caseUid);
        void InsuranceOffCancel(string caseUid);
        GapEmployee InsGapAnalysis(string recordUid);
    }
}
