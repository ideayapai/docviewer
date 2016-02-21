using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class DeleteSpaceMessage: MessageBase
    {
        public SpaceObject Content { get; set; }
    }
}
