using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class TrashDocumentMessage: MessageBase
    {
        public DocumentObject Content { get; set; }
    }
}
