using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace DatumCollection
{
    public static class SpiderEnvironment
    {
        public const string DefaultSettings = "appsettings.json";

        public static readonly string ApplicationRootPath = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string SpiderDoucmentsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "spider");
         
        public static readonly bool IsServer2008;
        public static readonly string IpAddress;
        public static readonly string OsDescription;

        public static readonly int TotalMemory;

        private struct MemoryStatus
        {
            public uint DwLength { get; set; }
            public uint DwMemoryLoad { get; set; }
            public ulong DwTotalPhys { get; set; }//总物理内存大小
            public ulong DwAvailPhys { get; set; }//可用的物理内存大小
            public ulong DwTotalPageFile { get; set; }
            public ulong DwAvailPageFile { get; set; }//可用的页面文件大小
            public ulong DwTotalVirtual { get; set; }//返回调用进程的用户模式部分的全部可用虚拟地址空间
            public ulong DwAvailVirtual { get; set; }//返回调用进程的用户模式部分的实际自由可用的虚拟内存
        }

        static SpiderEnvironment()
        {
            var systemVersion = Environment.OSVersion.Version.ToString();
            IsServer2008 = systemVersion.StartsWith("6.0") || systemVersion.StartsWith("6.1");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var mStatus = new MemoryStatus();
                GlobalMemoryStatus(ref mStatus);
                TotalMemory = (int)(Convert.ToInt64(mStatus.DwTotalPhys) / 1024 / 1024);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var lines = File.ReadAllLines("/proc/meminfo");
                var infoDic = lines
                    .Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToList())
                    .ToDictionary(items => items[0], items => long.Parse(items[1]));
                TotalMemory = (int)(infoDic["MemTotal:"] / 1024);
            }

            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .First(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                            i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            var unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;
            IpAddress = unicastAddresses.First(a => 
            //a.IPv4Mask.ToString() == "255.255.255.0" &&
            a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Address.ToString();

            OsDescription = $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version}";
        }        

        internal static long GetFreeMemory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var mStatus = new MemoryStatus();
                GlobalMemoryStatus(ref mStatus);
                return Convert.ToInt64(mStatus.DwAvailPhys) / 1024 / 1024;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var lines = File.ReadAllLines("/proc/meminfo");
                var infoDic = lines
                    .Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToList())
                    .ToDictionary(items => items[0], items => long.Parse(items[1]));
                var free = infoDic["MemFree:"];
                var sReclaimable = infoDic["SReclaimable:"];
                return (free + sReclaimable) / 1024;
            }
            return 0;
        }

        static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public static int GetFreeCPU()
        {            
            return 100 - (int)cpuCounter.NextValue();
        }

        [DllImport("kernel32.dll",SetLastError = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatus(ref MemoryStatus lpBuffer);

        public static void RegisterEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

    }
}
