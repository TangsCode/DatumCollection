using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Utility.HtmlParser
{
    public interface IParser
    {
        dynamic SelectSingle(dynamic text);

        IEnumerable<dynamic> Select(dynamic text);
    }
}
