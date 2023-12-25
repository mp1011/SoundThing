using System.Text.RegularExpressions;

namespace SoundThing.Extensions
{
    static class StringExtensions
    {
        public static string AddSpacesAtCapitals(this string s)
         => Regex.Replace(s, @"(?<=\p{Ll})(?=\p{Lu})", " ");
    }
}
