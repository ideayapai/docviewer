using System.Collections.Generic;
using Services.Contracts;

namespace WebAPI2.Models
{
    public class DisplayViewModel
    {
   
        private DocumentObject _document;

        public DocumentObject Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// 当前空间
        /// </summary>
        private SpaceObject _currentSpace;
        public SpaceObject CurrentSpace
        {
            get
            {
                return _currentSpace ?? (_currentSpace = new SpaceObject());
            }
            set
            {
                if (value != null)
                {
                    _currentSpace = value;
                }
            }
        }

        private List<SpaceObject> _parentSpaces = new List<SpaceObject>();
        public List<SpaceObject> ParentSpaces
        {
            get
            {
                return _parentSpaces;
            }
            set
            {
                _parentSpaces = value ?? new List<SpaceObject>();
            }
        }
    }
}