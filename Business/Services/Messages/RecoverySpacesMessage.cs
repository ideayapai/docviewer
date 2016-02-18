using System;
using System.Collections.Generic;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class RecoverSpacesMessage: MessageBase
    {
        public List<SpaceObject> Contents { get; set; }
    }
}
