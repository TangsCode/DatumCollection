using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using DatumCollection.Utility.Extensions;

namespace DatumCollection.HtmlParser
{
    public class XPathParser : HtmlParser
    {
        private static readonly Regex AttributeXPathRegex = new Regex(@"[\w\s-]+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
        private readonly string _xpath;
        private readonly string _attrName;

        public XPathParser(string xpath)
        {
            _xpath = xpath;
            Match match = AttributeXPathRegex.Match(_xpath);
            if (match.Value.NotNull() && _xpath.EndsWith(match.Value))
            {
                _attrName = match.Value.Replace("@", "");
                _xpath = _xpath.Replace("/" + match.Value, "");
            }
        }

        public override bool HasAttribute => _attrName.NotNull();

        public override IEnumerable<dynamic> Select(HtmlNode element)
        {
            List<dynamic> result = new List<dynamic>();
            var nodes = element.SelectNodes(_xpath);
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (!HasAttribute)
                    {
                        result.Add(node);
                    }
                    else
                    {
                        var attr = node.Attributes[_attrName];
                        if (attr != null && attr.Value.NotNull())
                        {
                            result.Add(attr.Value.Trim());
                        }
                    }
                }
            }
            return result;
        }
         
        public override dynamic SelectSingle(HtmlNode element)
        {
            var node = element.SelectSingleNode(_xpath);
            if (node != null)
            {
                if (HasAttribute)
                {
                    var attr = node.Attributes[_attrName];
                    if (attr != null && attr.Value.NotNull())
                    {
                        return attr.Value.Trim();
                    }
                    return null;
                }

                return node;
            }
            return null;
        }
    }
}
