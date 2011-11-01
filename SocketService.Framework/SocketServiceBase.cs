using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using log4net;

namespace SocketService.Framework
{
    public class SocketServiceBase : ServiceBase
    {
        protected static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public SocketServiceBase()
        {
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
            RunAsService = true;
        }

        public bool RunAsService { get; set; }

        /// <summary>
        /// Override in subclasses to start your application code.  The code will automatically
        /// be started as either a service or an application depending on the RunAsService property.
        /// </summary>
        public virtual void StartService() { }

        /// <summary>
        /// APSServiceBase default implementation for OnStart.  This will only be called if RunAsService is set to true.
        /// Implementers should not override this method.  Instead, they should put all service start code in StartService.  
        /// The APSServiceBase will handle starting and stopping appropriately depending on whether RunAsService is set or not.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            var thread = new Thread(StartService) { Name = "ServiceThread", IsBackground = false};
            thread.Start();
        }

        /// <summary>
        /// Override in subclasses to stop your application code.  This will only be called if RunAsService is true.
        /// </summary>
        public virtual void StopService() { }

        /// <summary>
        /// APSServiceBase default implementation for OnStop.  This will only be called if RunAsService is set to true.
        /// Implementers should not override this method.  Instead, they should put all service shutdown code in StopService.  
        /// The APSServiceBase will handle starting and stopping appropriately depending on whether RunAsService is set or not.
        /// </summary>
        protected override void OnStop()
        {
            StopService();
        }

        public void Run()
        {
            // Set working directory to application directory.
            // Otherwise Windows/System32 is used.
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // We must start the server appropriately depending on whether we are a service or not.
            if (RunAsService)
            {
                Run(this);
            }
            else
            {
                StartService();
            }
        }
    }
}
