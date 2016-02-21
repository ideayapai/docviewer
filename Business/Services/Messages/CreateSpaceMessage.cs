using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class CreateSpaceMessage: MessageBase
    {
        public SpaceObject Content { get; set; }
    }
}
