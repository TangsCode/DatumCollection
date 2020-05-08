using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DatumCollection.Utility.Helper
{
    public static class FileStreamHelper
    {
        public static string ReadAllText(this FileStream fs)
        {
            StringBuilder str = new StringBuilder();
            byte[] bytes = new byte[fs.Length];
            UTF8Encoding utf = new UTF8Encoding(true);
            while(fs.Read(bytes,0,bytes.Length) > 0)
            {
                str.Append(utf.GetString(bytes));
            }

            fs.Position = 0;
            return str.ToString();
        }

        public static void WriteAllText(this FileStream fs, string text)
        {
            byte[] codes = new UTF8Encoding(true).GetBytes(text);
            fs.Write(codes, 0, codes.Length);
        }
    }
}
