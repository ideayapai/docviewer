namespace Infrasturcture.Web
{
    public interface IFormsAuthentication
    {
        

        void SetAuthCookie(string userName, bool createPersistentCookie);

        void SignIn(string userName, bool createPersistentCookie);

        void SignOut();

        string DefaultUrl { get; }
    }
}
