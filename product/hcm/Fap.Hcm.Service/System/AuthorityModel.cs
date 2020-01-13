using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.System
{
    public class AuthorityModel
    {
        public IEnumerable<FapUser> Users { get; set; }
        public IEnumerable<string> Menus { get; set; }
        public IEnumerable<string> Depts { get; set; }
        public IEnumerable<dynamic> Rpts { get; set; }
        public IEnumerable<FapRoleColumn> Columns { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Buttons { get; set; }
    }
}
