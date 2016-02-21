using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class UserListViewModel : BaseMenuViewModel
    {
        private List<UserContract> _userContracts = new List<UserContract>();
        public List<UserContract> UserContracts
        {
            get
            {
                return _userContracts;
            }
            set
            {
                _userContracts = value ?? new List<UserContract>();
            }
        }
    }
}