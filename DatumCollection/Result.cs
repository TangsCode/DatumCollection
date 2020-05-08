using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection
{
    public class Result
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public dynamic Data { get; set; }
    }    


}
