using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class UpdateSpaceMessage: MessageBase
    {
        public SpaceObject Content { get; set; }
    }
}
