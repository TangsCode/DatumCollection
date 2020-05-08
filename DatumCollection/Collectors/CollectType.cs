using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Collectors
{
    /// <summary>
    /// 采集类型
    /// </summary>
    public enum CollectType
    {
        /// <summary>
        /// 按商品采集
        /// 根据Schedule配置的所有商品采集
        /// </summary>
        Goods = 0,
        /// <summary>
        /// 按店铺采集
        /// 根据配置的店铺Url采集店铺下所有SKU商品信息
        /// </summary>
        Shop = 1,
        
    }
}
