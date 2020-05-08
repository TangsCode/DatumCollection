using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;

namespace DatumCollection
{
    public static class NetworkHelper
    {
        public static string GetLocalIpV4()
        {
            string host = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            return addresses.Where(a => 
                a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault().ToString();
        }
    }
}
