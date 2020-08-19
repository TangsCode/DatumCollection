using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Selectors
{
    public class SelectorAttribute : Attribute
    {        
        public SelectorType Type { get; set; }
         
        public string Key { get; set; }

        public string Path { get; set; }
    }

    public enum SelectorType
    {
        XPath,
        Html,
        Json
    }
}
