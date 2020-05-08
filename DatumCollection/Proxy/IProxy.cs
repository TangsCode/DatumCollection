using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Proxy
{
    /// <summary>
    /// 代理接口
    /// 实现该接口的类定义代理连接的类型和配置
    /// </summary>
    interface IProxy
    {
        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        string GetProxy();
    }
}
