using System;
using Services.Contracts;

namespace WebSite2.Models
{
    [Serializable]
    public class SpaceViewModel : BaseMenuViewModel
    {
        public SpaceObject SpaceObject { get; set; }
    
        public string UserId { get; set; }

        
        public SpaceViewModel(SpaceObject documentObject, string userId)
        {
            SpaceObject = documentObject;
            UserId = userId;
        }

        public bool CanEdit
        {
            get
            {
                return SpaceObject.CreateUserId == UserId;
            }
        }
    }
}