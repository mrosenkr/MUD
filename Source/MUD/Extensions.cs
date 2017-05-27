using System;

namespace MUD
{
    public static class Extensions
    {
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsEqual(this string s, string r)
        {
            return s.Equals(r, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
