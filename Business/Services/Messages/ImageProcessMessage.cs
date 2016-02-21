using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
     [Serializable]
    public class ImageProcessMessage : MessageBase
    {
        public DocumentObject Document { get; set; }
    }
}
