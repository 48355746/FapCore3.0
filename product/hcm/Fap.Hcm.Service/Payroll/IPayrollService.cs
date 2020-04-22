using System.Collections.Generic;

namespace Fap.Hcm.Service.Payroll
{
    public interface IPayrollService
    {
        IEnumerable<CaseItem> GetPayCaseItem(string caseUid);
        void AddPayItem(string caseUid, string[] payItems);
        long CreatePayCase(string caseUid);
        void InitEmployeeToPayCase(PayCase payCase, string empWhere);
        void InitPayrollData(InitDataViewModel payrollInitData);
        void UsePayPending(PayToDo payToDo);
        void PayrollOff(string caseUid);
        void PayrollOffCancel(string caseUid);
        GapEmployee PayGapAnalysis(string recordUid);
        void PayrollOffNotice(string caseUid);
        IEnumerable<MyPayroll> GetMyPayroll(string startYM, string endYM);
    }
}