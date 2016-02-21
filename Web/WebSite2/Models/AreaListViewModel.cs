using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class AreaListViewModel : BaseMenuViewModel
    {
        private List<AreaContract> _areaContracts = new List<AreaContract>();
        public List<AreaContract> AreaContracts
        {
            get
            {
                return _areaContracts;
            }
            set
            {
                _areaContracts = value ?? new List<AreaContract>();
            }
        }
    }
}