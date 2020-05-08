using DatumCollection.Spiders;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.ConsoleApp.Service
{
    public partial class SpiderService : ServiceBase
    {
        private Timer timer;
        public SpiderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("爬虫服务启动");
            StartUp.ExecuteSpider<SpiderOne>();
            //timer = new Timer(new TimerCallback(CheckScheduleConfig), null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));            
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("爬虫服务关闭");
            timer?.Dispose();
        }
    }
}
