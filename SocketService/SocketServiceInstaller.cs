using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;
using System.Diagnostics;

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
            var myEventLogInstaller = new EventLogInstaller();

            //// Set the Source of Event Log, to be created.
            myEventLogInstaller.Source = "Blazing Sockets Messaging Server";

            //// Set the Log that source is created in
            myEventLogInstaller.Log = "Application";

            //set the process privileges
            processInstaller.Account = ServiceAccount.User;

            serviceInstaller.DisplayName = "Blazing Sockets Messaging Server";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "Blazing Sockets Messaging Server";


            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
            this.Installers.Add(myEventLogInstaller);
        }
    }
}
