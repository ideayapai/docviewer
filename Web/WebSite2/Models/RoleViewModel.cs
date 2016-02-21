using Services.Contracts;

namespace WebSite2.Models
{
    public class RoleViewModel : BaseMenuViewModel
    {
        public RoleViewModel()
        {

        }

        public RoleViewModel(RoleContract contract)
        {
            RoleContract = contract;
        }

        public RoleContract RoleContract
        {
            get;
            set;
        }
    }
}