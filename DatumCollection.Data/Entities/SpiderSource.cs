using DatumCollection.Data.Attributes;
using DatumCollection.Infrastructure.Selectors;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.Utility.HtmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.Entities
{
    [Schema("SpiderItem")]
    public class SpiderSource : SystemBase
    {
        [Column(Name = "Url", Type = "nvarchar")]
        public string Url { get; set; }

        [Column(Name = "Method", Type = "varchar", Length = 50)]
        public string Method { get; set; }

        [Column(Name = "ContentType", Type = "nvarchar")]
        public string ContentType { get; set; }
    
        [Column(Name = "Encoding", Type = "nvarchar")]
        public string Encoding { get; set; }

        [Column(Name = "FK_Channel_ID", Type = "uniqueidentifier")]
        public Guid FK_Channel_ID { get; set; }

        [JoinTable("FK_Channel_ID")]
        public Channel Channel { get; set; }
        
    }

    public class SpiderItem<T>:SpiderSource, ISpiderItem where T : ISpider
    {
        public ISpiderConfig SpiderConfig { get { return Channel; } }

        public async Task<ISpider> Spider(SpiderAtom atom)
        {
            try
            {
                var result = System.Activator.CreateInstance<T>();
                var selectors = await SpiderConfig.GetAllSelectors();
                var props = typeof(T).GetProperties();
                
                foreach (var selector in selectors)
                {
                    if (string.IsNullOrEmpty(selector.Path)
                        || !props.Any(p => p.Name.ToLower() == selector.Key.ToLower()))
                    {
                        continue;
                    }
                    IParser parser = null; object o = null;
                    switch (selector.Type)
                    {
                        case SelectorType.XPath:
                            {
                                parser = new XPathParser(selector.Path);
                                o = parser.SelectSingle(atom.Response.Content);
                            }
                            break;
                        case SelectorType.Html:
                            break;
                        case SelectorType.Json:
                            break;
                        default:
                            break;
                    }
                    var prop = props.FirstOrDefault(p => p.Name.ToLower() == selector.Key.ToLower());
                    prop?.SetValue(result, Convert.ChangeType(o, prop.PropertyType));
                }

                var item = props.FirstOrDefault(p => p.Name == "FK_SpiderItem_ID");
                item?.SetValue(result, ((SpiderSource)atom.SpiderItem).ID);
                //var item = props.FirstOrDefault(p => p.PropertyType.IsAssignableFrom(typeof(SpiderSource)));                
                //item?.SetValue(result, atom.SpiderItem);                

                return result;
            }
            catch (Exception e)
            {
                throw;
            }




        }

    }
}
