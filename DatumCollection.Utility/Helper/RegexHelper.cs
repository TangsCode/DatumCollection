using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DatumCollection.Utility.Helper
{
    public static class RegexHelper
    {
        /// <summary>
        /// IP正则表达式
        /// </summary>
        public static readonly Regex IpAddress = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
        /// <summary>
        /// 数字正则表达式
        /// </summary>
        public static readonly Regex Number = new Regex(@"\d+");
        /// <summary>
        /// 小数正则表达式
        /// </summary>
        public static readonly Regex Decimal = new Regex(@"\d+(\.\d+)?");
        /// <summary>
        /// URL正则表达式
        /// </summary>
        public static string Url = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

        /// <summary>
        /// Replace characters in string by using regex pattern to match
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string RegexReplace(this string source, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(source))
            {
                return regex.Replace(source, replacement);
            }
            return source;
        }

        public static string Match(this string source, string pattern)
        {
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(source))
            {
                return regex.Match(source).Value;
            }
            return null;
        }
    }
}
