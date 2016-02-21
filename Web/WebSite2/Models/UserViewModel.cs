using Services.Contracts;

namespace WebSite2.Models
{
    public class UserViewModel : BaseMenuViewModel
    {
        public UserViewModel()
        {

        }

        public UserViewModel(UserContract contract)
        {
            UserContract = contract;
        }

        public UserContract UserContract
        {
            get;
            set;
        }
    }
}