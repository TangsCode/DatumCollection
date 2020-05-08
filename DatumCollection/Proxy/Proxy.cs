using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Proxy
{
    /// <summary>
    /// 代理类
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// 代理协议
        /// </summary>
        public ProxyProtocol Protocol { get; set; } 
        
        /// <summary>
        /// 主机IP
        /// </summary>
        public string IP { get; set; }
        
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEffective { get; set; }
    }

    /// <summary>
    /// 代理协议
    /// </summary>
    public enum ProxyProtocol
    {
        HTTP,
        SSL
    }
}
