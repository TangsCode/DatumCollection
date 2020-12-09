using DatumCollection.Data.Attributes;
using DatumCollection.Infrastructure.Selectors;
using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.Entities
{
    [Schema("Channel")]
    public class Channel : SystemBase, ISpiderConfig
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

        public async Task<IEnumerable<SelectorAttribute>> GetAllSelectors()
        {
            var selectors = new List<SelectorAttribute>();

            try
            {
                var props = this.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.Name.ToLower().Contains(SelectorType.XPath.ToString().ToLower()))
                    {
                        selectors.Add(new SelectorAttribute
                        {
                            Type = SelectorType.XPath,
                            Key = prop.Name.Replace(SelectorType.XPath.ToString(), ""),
                            Path = prop.GetValue(this)?.ToString()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                
            }

            return selectors;
        }

        public Task<SelectorAttribute> GetTargetSelector()
        {
            var selector = new SelectorAttribute
            {
                Type = SelectorType.XPath,
                Key = "Price",
                Path = PriceXPath
            };
            return Task.FromResult(selector);
        }
    }
}
