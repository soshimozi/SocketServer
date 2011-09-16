using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using CertificateAuthorityServiceLibrary;
using System.ServiceModel.Description;

namespace CAServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(CertificateAuthorityService));

            host.Open();

            Console.WriteLine("Service is up and running with the following endpoints:");
            foreach (ServiceEndpoint se in host.Description.Endpoints)
                Console.WriteLine(se.Address.ToString());

            Console.ReadLine();

            host.Close();
        }
    }
}
