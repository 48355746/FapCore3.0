using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using System.Collections.Generic;

namespace Fap.Hcm.Service.Assess
{
    public interface IAssessService
    {
        IEnumerable<TreeDataView> GetSchemeCategoryTree();
        ResponseViewModel OperSchemeCategory(TreePostData postData);
        void CreateExaminer(ExaminerViewModel examinerVM);
        void CopyScheme(string fid);
    }
}