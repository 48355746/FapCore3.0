using Dapper;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Hcm.Service.Assess
{
    [Service]
    public class AssessService : IAssessService
    {
        private readonly IDbContext _dbContext;
        public AssessService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<TreeDataView> GetSchemeCategoryTree()
        {
            IEnumerable<dynamic> prmCategory = _dbContext.Query("select * from PerfProgramCategory");
            List<TreeDataView> oriList = prmCategory.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { group = "1" }, Text = t.Name, Icon = "icon-folder blue ace-icon fa fa-file-word-o" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "方案分类",
                Data = new { group = "0" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return tree;
        }
        public ResponseViewModel OperSchemeCategory(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.DeleteExec(nameof(PerfProgramCategory), "Fid=@Fid", new DynamicParameters(new { Fid = postData.Id }));
                vm.success = c > 0 ? true : false;
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                PerfProgramCategory ppc = new PerfProgramCategory
                {
                    Pid = postData.Id,
                    Name = postData.Text
                };
                _dbContext.Insert(ppc);
                vm.success = true;
                vm.data = ppc.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var prmCategory = _dbContext.Get<PerfProgramCategory>(postData.Id);
                prmCategory.Name = postData.Text;
                vm.success = _dbContext.Update(prmCategory);
            }
            else if (postData.Operation == TreeNodeOper.MOVE_NODE)
            {
                var prmCategory = _dbContext.Get<PerfProgramCategory>(postData.Id);
                prmCategory.Pid = postData.Parent;
                vm.success = _dbContext.Update(prmCategory);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }
    }
}
