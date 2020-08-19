using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatumCollection.Utility.HtmlParser
{
    public abstract class HtmlParser : IParser
    {
        public abstract bool HasAttribute { get; }

        public virtual IEnumerable<dynamic> Select(dynamic text)
        {
            if (text != null)
            {
                if (text is HtmlNode htmlNode)
                {
                    return Select(htmlNode);
                }

                HtmlDocument document = new HtmlDocument { OptionAutoCloseOnEnd = true };
                document.LoadHtml(text);
                return Select(document.DocumentNode);
            }

            return Enumerable.Empty<dynamic>();
        }

        public abstract IEnumerable<dynamic> Select(HtmlNode element);

        public virtual dynamic SelectSingle(dynamic text)
        {
            if (text != null)
            {
                if (text is string)
                {
                    HtmlDocument document = new HtmlDocument { OptionAutoCloseOnEnd = true };
                    document.LoadHtml(text);
                    return SelectSingle(document.DocumentNode);
                }

                SelectSingle(text as HtmlNode);
            }
            return null;
        }

        public abstract dynamic SelectSingle(HtmlNode node);
        
    }
}
