using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatumCollection.Utility.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// check if the string is null,empty or white spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNull(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// check if the string is not null,empty or white spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotNull(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// check if the string equals to another when ignoring case
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s,string value)
        {
            return s.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// contains all characters in the search string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string s,string search)
        {
            char[] chars = search.ToCharArray();
            int exclude = 0;
            foreach (char ch in chars)
            {
                if (!s.Contains(ch))
                {
                    exclude += 1;
                }
            }
            return (double)exclude / chars.Length <= 0.2 ? true : false;
        }
    }
}
