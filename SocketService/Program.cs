using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using log4net;
using System.Runtime.InteropServices;

namespace SocketServer
{
    static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SocketService service;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String [] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args.Length > 0 && args[0].Trim().ToLower() == "/i")
            {
                //Install service
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { "/i", Assembly.GetExecutingAssembly().Location });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
            else if (args.Length > 0 && args[0].Trim().ToLower() == "/u")
            {
                try
                {
                    //Uninstall service                 
                    ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
            else
            {
                service = new SocketService();

                if (args.Length > 0 && args[0].Equals("RunAsService"))
                {
                    service.RunAsService = true;
                }
                else
                {
                    service.RunAsService = false;
                    Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                }

                service.Run();

            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            service.Stop();
        }

    }
}
