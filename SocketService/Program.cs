using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using log4net;

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

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "/gui")
                {
                    ServerControlForm form = new ServerControlForm();
                    Application.Run(form);
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			{ 
				new SocketService() 
			};
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
