using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class SpaceViewModelCollection: List<SpaceViewModel>
    {
        private List<SpaceObject> _spaces;
        public List<SpaceObject> Spaces
        {
            get
            {
                if (_spaces == null)
                {
                    _spaces = new List<SpaceObject>();
                }
                return _spaces;
            }
            set { _spaces = value; }
        }


        public SpaceViewModelCollection(List<SpaceObject> spaces, string userId)
        {
            _spaces = spaces;

            foreach (var space in spaces)
            {
                Add(new SpaceViewModel(space, userId));
            }
        }
    }
}