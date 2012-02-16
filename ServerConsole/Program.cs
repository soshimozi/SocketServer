using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer;
using log4net;
using log4net.Config;

namespace ServerConsole
{
    class Program : IMessageHandler
    {
        static readonly ILog logger = LogManager.GetLogger("ServerConsole");

        static void Main(string[] args)
        {
            // Set working directory to application directory.
            // Otherwise Windows/System32 is used.
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            try
            {

                XmlConfigurator.Configure();
                Console.WriteLine("Loaded app.config");
            }
            catch (Exception)
            {
                Console.WriteLine("Error: configuration file not found.");
                throw;
            }

            try
            {
                MessageDispatcher md = new MessageDispatcher();
                md.RegisterHandlers(new Program());
                ClientListener server = new ClientListener(3333, new SocketListener(), md);
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [MessageHandler]
        private void Process(TestMessage t)
        {
            logger.Debug("Test message received + " + t.Message);
        }


    }
}
