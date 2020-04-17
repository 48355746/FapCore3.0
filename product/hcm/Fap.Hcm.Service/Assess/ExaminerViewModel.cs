using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    public class ExaminerViewModel
    {
        public List<string> Objectives { get; } = new List<string>();
        public bool IsOrgDept { get; set; }
        public double DeptWeights { get; set; }
        public bool IsLeaderShip { get; set; }
        public double LeaderShipWeights { get; set; }
        public bool IsCustom { get; set; }
        public double CustomWeights { get; set; }
        public List<string> CustomExaminers { get; } = new List<string>();
    }
}
