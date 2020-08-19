using DatumCollection.Data.Attributes;
using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.Entities
{
    /// <summary>
    /// 电商网站
    /// </summary>
    [Schema("GoodsData")]
    public class ElectronicCommerceWebsite : SystemBase, ISpider
    {
        [Column(Name = "Price", Type = "decimal", Precision = 10, Scale = 2)]
        public decimal Price { get; set; }

        [Column(Name = "ScreenshotPath")]
        public string ScreenshotPath { get; set; }

        [Column(Name = "TaxFee", Type = "decimal", Precision = 10, Scale = 2)]
        public decimal TaxFee { get; set; }

        [Column(Name = "PostageDesc")]
        public string PostageDesc { get; set; }

        [Column(Name = "PreferentialInfo")]
        public string PreferentialInfo { get; set; }

        [Column(Name = "CouponInfo")]
        public string CouponInfo { get; set; }

        [Column(Name = "ImageText")]
        public string ImageText { get; set; }
        
    }
}
