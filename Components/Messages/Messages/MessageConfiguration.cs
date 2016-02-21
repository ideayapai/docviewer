using System.Configuration;

namespace Messages
{
    public static class MessageConfiguration
    {
        public static string MessageAddress
        {
            get { return ConfigurationManager.AppSettings["msmq_address"]; }
        }
   
    }
}
