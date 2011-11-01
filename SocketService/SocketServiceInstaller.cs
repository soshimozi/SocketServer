using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace SocketService
{
    [RunInstaller(true)]
    public class SocketServiceInstaller : Installer
    {
        public SocketServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            ////Create Instance of EventLogInstaller
            var myEventLogInstaller = new EventLogInstaller
                                          {Source = "Blazing Sockets Messaging Server", Log = "Application"};

            //// Set the Source of Event Log, to be created.

            //// Set the Log that source is created in

            //set the process privileges
            processInstaller.Account = ServiceAccount.User;

            serviceInstaller.DisplayName = "Blazing Sockets Messaging Server";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "Blazing Sockets Messaging Server";


            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
            Installers.Add(myEventLogInstaller);
        }
    }
}