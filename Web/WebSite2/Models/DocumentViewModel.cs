using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services.Context;
using Services.Contracts;

namespace WebSite2.Models
{
    public class DocumentViewModel
    {
        public DocumentObject DocumentObject { get; set; }
        private string UserId;
        public DocumentViewModel(DocumentObject documentObject, string userId)
        {
            DocumentObject = documentObject;
            UserId = userId;
        }

        public bool CanEdit
        {
            get
            {
                return DocumentObject.CreateUserId == UserId;
            }
        }

    }
}