using DatumCollection.Infrastructure.Web;
using DatumCollection.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DatumCollection.Infrastructure.Spider;

namespace DatumCollection.Core.Spider
{
    public class SpiderContext
    {
        public SpiderStatus SpiderStatus { get; set; }

        public SpiderTask Task { get; set; }        

        public IServiceProvider Services { get; }
        
        public HashSet<SpiderAtom> SpiderAtoms { get; set; }        

        public SpiderContext()
        {            
        }
    }

    public enum SpiderStatus
    {
        OK,        
        Error
    }
    
}
