using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class TrashViewModel : BaseMenuViewModel
    {

        private List<SpaceObject> _spaceObjects = new List<SpaceObject>();
        public List<SpaceObject> SpaceModels
        {
            get { return _spaceObjects; }
            set
            {
                if (value == null)
                {
                    _spaceObjects = new List<SpaceObject>();
                }
                else
                {
                    _spaceObjects = value;
                }
            }
        }

        public DocumentViewModelCollection DocumentModels { get; set; }

        public string PageCode { get; set; }
    }
}