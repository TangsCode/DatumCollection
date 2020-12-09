﻿using DatumCollection.Data.Attributes;
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
    [Schema("ElectronicCommerceWebsite")]
    public class ElectronicCommerceWebsiteSpider : SystemBase, ISpider
    {
        [Column(Name = "FK_SpiderItem_ID", Type = "uniqueidentifier")]
        public Guid FK_SpiderItem_ID { get; set; }

        [JoinTable("FK_SpiderItem_ID")]
        public SpiderSource SpiderSource { get; set; }

        [Column(Name = "Price", Type = "decimal", Precision = 10, Scale = 2)]
        public decimal Price { get; set; }

        [Column(Name = "ScreenshotPath")]
        public string ScreenshotPath { get; set; }

        [Column(Name = "TaxFee", Type = "decimal", Precision = 10, Scale = 2)]
        public decimal TaxFee { get; set; }

        [Column(Name = "Postage")]
        public string Postage { get; set; }

        [Column(Name = "Preferential")]
        public string Preferential { get; set; }

        [Column(Name = "Coupon")]
        public string Coupon { get; set; }

        [Column(Name = "ImageText")]
        public string ImageText { get; set; }        
        
    }
}
