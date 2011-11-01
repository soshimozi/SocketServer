using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using log4net;

namespace SocketService
{
    static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String [] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            //var servicesToRun = new ServiceBase[] { new SocketService() };

            //if (args.Length > 0)
            //{
            //    if (args[0].ToLower() == "/gui")
            //    {
            //        var form = new ServerControlForm();
            //        Application.Run(form);
            //    }
            //}
            //else
            //{
            //    ServiceBase.Run(servicesToRun);
            //}

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
                using (var service = new SocketService())
                {
                    if (args.Length > 0 && args[0].Equals("RunAsService"))
                    {
                        service.RunAsService = true;
                    }
                    else
                    {
                       service.RunAsService = false;
                    }

                    //ConfigureUnhandledExceptionHandling();

                    service.Run();
                }
            }
        }
    }
}
