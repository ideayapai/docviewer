using System.ServiceProcess;

namespace DocViewerWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new DocViewerService();
            ServiceBase.Run(service);

        }
    }
}
