using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace SocketService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String [] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var servicesToRun = new ServiceBase[] { new SocketService() };

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "/gui")
                {
                    var form = new ServerControlForm();
                    Application.Run(form);
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
