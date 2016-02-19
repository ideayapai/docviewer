using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class RoleListViewModel : BaseMenuViewModel
    {
        private List<RoleContract> _roleContracts = new List<RoleContract>();
        public List<RoleContract> RoleContracts
        {
            get
            {
                return _roleContracts;
            }
            set
            {
                _roleContracts = value ?? new List<RoleContract>();
            }
        }
    }
}