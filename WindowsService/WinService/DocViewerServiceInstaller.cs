using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DocViewerWinService
{
    [RunInstaller(true)]
    public partial class DocViewerServiceInstaller : Installer
    {

        private string serviceName = "DocViewerService";
        private const string SERVICE_NAME_SWITCH = "ServiceName";
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller serviceProcessInstaller;


        public DocViewerServiceInstaller()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller = new ServiceInstaller {StartType = ServiceStartMode.Automatic};
            SetServiceName(serviceName);

            Installers.AddRange(new Installer[]
                                    {
                                        serviceProcessInstaller,
                                        serviceInstaller
                                    });
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            SetServiceNameFromContext();
            savedState[SERVICE_NAME_SWITCH] = serviceName;
            SetServiceName(serviceName);
            base.OnBeforeInstall(savedState);
        }

        private void SetServiceNameFromContext()
        {
            string name = GetContextParameter(SERVICE_NAME_SWITCH).Trim();
            if (!string.IsNullOrEmpty(name))
            {
                serviceName = name;
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            SetServiceNameFromContext();
            SetServiceName(serviceName);
            base.OnBeforeUninstall(savedState);
        }

        public string GetContextParameter(string key)
        {
            string sValue;
            try
            {
                sValue = Context.Parameters[key];
            }
            catch
            {
                sValue = "";
            }
            return sValue;
        }


        private void SetServiceName(string name)
        {
            serviceInstaller.DisplayName = name;
            serviceInstaller.ServiceName = name;
        }
    }
}
