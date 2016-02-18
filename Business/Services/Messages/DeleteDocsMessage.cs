using System;
using System.Collections.Generic;
using Messages;
using Services.Contracts;

namespace Services.Messages
{
    [Serializable]
    public class DeleteDocsMessage : MessageBase
    {
        public List<DocumentObject> Contents { get; set; }
    }

}
