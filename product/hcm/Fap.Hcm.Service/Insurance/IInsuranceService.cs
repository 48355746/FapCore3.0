using Fap.Hcm.Service.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Insurance
{
    public interface IInsuranceService
    {
        void UseInsPending(InsToDo insToDo);
    }
}
