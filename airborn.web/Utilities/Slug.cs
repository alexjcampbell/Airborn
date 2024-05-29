using System;
using System.Text.RegularExpressions;

namespace Airborn.web
{

    public static class TextUtilities
    {
        public static string ToUrlSlug(string value)
        {
            //First to lower case
            value = value.ToLowerInvariant();

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or \_
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
    }
}