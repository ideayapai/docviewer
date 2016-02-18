using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class DeleteDocumentMessage: MessageBase
    {
        public DocumentObject Content { get; set; }
    }
}
