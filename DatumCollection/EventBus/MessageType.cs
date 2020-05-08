using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DatumCollection.EventBus
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        [Description("商品抓取")]
        GoodsSpider,
        [Description("门店抓取")]
        ShopSpider,
        [Description("释放资源")]
        DisposeResources,
    }
}
