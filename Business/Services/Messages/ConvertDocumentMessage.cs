using System;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class ConvertDocumentMessage: MessageBase
    {
        public DocumentObject Document { get; set; }
    }
}
