using DatumCollection.Infrastructure.Data;
using DatumCollection.Infrastructure.Selectors;
using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.Entities
{
    [Schema("Channel")]
    public class Channel : SystemBase
    {
        [Column(Name = "ChannelEnum", Type = "int")]
        public int ChannelEnum { get; set; }

        [Column(Name = "ChannelCode")]
        public string ChannelCode { get; set; }

        [Column(Name = "ChannelName")]
        public string ChannelName { get; set; }

        [Column(Name = "PriceXPath")]
        public string PriceXPath { get; set; }

        [Column(Name = "PostageXPath")]
        public string PostageXPath { get; set; }

        [Column(Name = "PreferentialXPath")]
        public string PreferentialXPath { get; set; }

        [Column(Name = "CouponXPath")]
        public string CouponXPath { get; set; }

        [Column(Name = "ScreenshotXPath")]
        public string ScreenshotXPath { get; set; }

        [Column(Name = "ImageUrlXPath")]
        public string ImageUrlXPath { get; set; }

        [Column(Name = "CloseXPath")]
        public string CloseXPath { get; set; }
    }
}
