using Services.Contracts;

namespace WebSite2.Models
{
    public class AreaViewModel : BaseMenuViewModel
    {
        public AreaViewModel()
        {

        }

        public AreaViewModel(AreaContract contract)
        {
            AreaContract = contract;
        }

        public AreaContract AreaContract
        {
            get;
            set;
        }
    }
}