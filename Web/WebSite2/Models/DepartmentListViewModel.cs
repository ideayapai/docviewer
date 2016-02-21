using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class DepartmentListViewModel : BaseMenuViewModel
    {
        private List<DepartmentContract> _departmentContracts = new List<DepartmentContract>();
        public List<DepartmentContract> DepartmentContracts
        {
            get
            {
                return _departmentContracts;
            }
            set
            {
                _departmentContracts = value ?? new List<DepartmentContract>();
            }
        }
    }
}