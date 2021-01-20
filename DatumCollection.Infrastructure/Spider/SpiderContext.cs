using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DatumCollection.Infrastructure.Spider;

namespace DatumCollection.Infrastructure.Spider
{
    public class SpiderContext
    {
        public SpiderTask Task { get; set; }        

        public IServiceProvider Services { get; }
        
        public HashSet<SpiderAtom> SpiderAtoms { get; set; }        

        public SpiderContext()
        {
            SpiderAtoms = new HashSet<SpiderAtom>();
            Task = new SpiderTask();
        }
    }

    public enum SpiderStatus
    {
        OK,        
        CollectError,
        ExtractError,
        StorageError,
    }
    
}
