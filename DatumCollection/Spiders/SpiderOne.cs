using DatumCollection.Utility.Extensions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatumCollection.Spiders
{
    /// <summary>
    /// the first generation of spider
    /// </summary>    
    public class SpiderOne : SpiderBase
    {
        public SpiderOne(SpiderParameters parameters) : base(parameters)
        {
            
        }

    }
}
