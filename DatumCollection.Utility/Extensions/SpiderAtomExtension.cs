using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatumCollection.Utility.Extensions
{
    public static class SpiderAtomExtension
    {
        public static IEnumerable<SpiderAtom> StatusOk(this IEnumerable<SpiderAtom> atoms)
        {
            return atoms.Where(a => a.SpiderStatus == SpiderStatus.OK); 
        }
    }
}
