using System.Collections.Generic;

namespace Fap.Hcm.Service.Payroll
{
    public interface IPayrollService
    {
        IEnumerable<PayCaseItem> GetPayCaseItem(string caseUid);
        void AddPayItem(string caseUid, string[] payItems);
        long GenericPayCase(string caseUid);
        void UsePayPending(PayToDo payToDo);
    }
}