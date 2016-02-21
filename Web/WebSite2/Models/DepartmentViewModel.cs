using Services.Contracts;

namespace WebSite2.Models
{
   

    public class DepartmentViewModel : BaseMenuViewModel
    {
        public DepartmentViewModel()
        {

        }

        public DepartmentViewModel(DepartmentContract contract)
        {
            DepartmentContract = contract;
        }

        public DepartmentContract DepartmentContract
        {
            get;
            set;
        }
    }
}