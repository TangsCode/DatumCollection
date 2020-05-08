using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.HtmlParser
{
    interface IParser
    {
        dynamic SelectSingle(dynamic text);

        IEnumerable<dynamic> Select(dynamic text);
    }
}
