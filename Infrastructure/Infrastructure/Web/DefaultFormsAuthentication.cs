using System;
using System.Web;
using System.Web.Security;

namespace Infrasturcture.Web
{
    public class DefaultFormsAuthentication : IFormsAuthentication
    {
        public void SignOut()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
        }

        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);


        }

        public string DefaultUrl
        {
            get { return FormsAuthentication.DefaultUrl; }
        }
    }
}
