using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DatumCollection.Utility.Helper
{
    public static class IO
    {
        public static void ensureLocalDirectory(string directory)
        {
            if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
