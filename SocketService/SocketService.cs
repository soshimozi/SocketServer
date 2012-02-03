using System;
using System.Diagnostics;
using System.ServiceProcess;
using SocketServer.Core.Messaging;
using SocketServer.Net;

namespace SocketServer
{
    public partial class SocketService : SocketServiceBase
    {
        private readonly SocketManager _serverManager = new SocketManager();
        private readonly MessageServer _messageServer = new MessageServer();

        public SocketService()
        {
            InitializeComponent();
        }

        public override void StartService()
        {
            //InitializeCounters();

            _serverManager.StartServer();
            _messageServer.Start();
        }

        private static void InitializeCounters()
        {
            try
            {
                var counterDatas =
                    new CounterCreationDataCollection();

                // Create the counters and set their properties.
                var cdCounter1 =
                    new CounterCreationData();
                var cdCounter2 =
                    new CounterCreationData();

                cdCounter1.CounterName = "Total Requests Handled";
                cdCounter1.CounterHelp = "Total number of requests handled";
                cdCounter1.CounterType = PerformanceCounterType.NumberOfItems64;
                cdCounter2.CounterName = "Requests Per Secpmd";
                cdCounter2.CounterHelp = "Average number of requests per second.";
                cdCounter2.CounterType = PerformanceCounterType.RateOfCountsPerSecond64;

                // Add both counters to the collection.
                counterDatas.Add(cdCounter1);
                counterDatas.Add(cdCounter2);

                // Create the category and pass the collection to it.
                PerformanceCounterCategory.Create(
                    "Socket Service Data Stats", "Stats for the socket service.",
                    PerformanceCounterCategoryType.MultiInstance, counterDatas);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        public override void StopService()
        {
            _serverManager.StopServer();
            _messageServer.Stop();
        }
    }
}
