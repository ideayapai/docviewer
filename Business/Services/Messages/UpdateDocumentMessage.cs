using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class UpdateDocumentMessage: MessageBase
    {
        public DocumentObject Content { get; set; }
    }
}
