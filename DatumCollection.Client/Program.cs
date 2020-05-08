using System;
using System.Diagnostics;
using System.Linq;
using DatumCollection;
using DatumCollection.Console;
using DatumCollection.Data;
using DatumCollection.Data.Entity;
using DatumCollection.Spiders;
using DatumCollection.Utility.Helper;

namespace DatumCollection.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            StartUp.ExecuteSpider<SpiderOne>();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses().Where(p =>
            p.ProcessName.ToLower() == "chromedriver");
            foreach (var process in processes)
            {                
                process.Kill();
            }
        }        

    }
}
