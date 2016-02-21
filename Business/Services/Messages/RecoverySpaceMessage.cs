using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class RecoverySpaceMessage: MessageBase
    {
        public SpaceObject Content { get; set; }
    }
}
