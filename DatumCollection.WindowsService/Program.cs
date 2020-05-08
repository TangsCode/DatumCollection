using System;
using System.ServiceProcess;
using DatumCollection;
using DatumCollection.Spiders;
using Serilog;
using Serilog.Events;
using DatumCollection.ConsoleApp.Service;
using System.Threading;

namespace DatumCollection.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartUp.CreateLogger();
            ServiceBase[] services = new ServiceBase[] { new SpiderService() };
            ServiceBase.Run(services);
        }
    }
}
