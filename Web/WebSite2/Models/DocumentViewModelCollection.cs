using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services.Contracts;

namespace WebSite2.Models
{
    public class DocumentViewModelCollection: List<DocumentViewModel>
    {
        private List<DocumentObject> _documents;
        public List<DocumentObject> Documents
        {
            get
            {
                if (_documents == null)
                {
                    _documents = new List<DocumentObject>();
                }
                return _documents;
            }
            set { _documents = value; }
        }


        public DocumentViewModelCollection(List<DocumentObject> documents, string userId)
        {
            _documents = documents;

            foreach (var document in documents)
            {
                Add(new DocumentViewModel(document, userId));
            }
        }
    }
}