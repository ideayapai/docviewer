using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class CreateDocumentMessage: MessageBase
    {
        public DocumentObject Content { get; set; }
    }
}
