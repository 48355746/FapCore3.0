using System.Collections.Generic;

namespace Fap.Hcm.Service.Payroll
{
    public interface IPayrollService
    {
        IEnumerable<CaseItem> GetPayCaseItem(string caseUid);
        void AddPayItem(string caseUid, string[] payItems);
        long CreatePayCase(string caseUid);
        void InitEmployeeToPayCase(PayCase payCase, string empWhere);
        void InitPayrollData(PayrollInitDataViewModel payrollInitData);
        void UsePayPending(PayToDo payToDo);
        IList<string> PayrollCalculate(string formulaCaseUid);
        void PayrollOff(string caseUid);
        void PayrollOffCancel(string caseUid);
        PayGapEmployee PayGapAnalysis(string recordUid);
        void PayrollOffNotice(string caseUid);
    }
}