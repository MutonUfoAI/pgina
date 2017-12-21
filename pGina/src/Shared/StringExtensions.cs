using System.Linq;

namespace pGina.Shared
{
    public static class StringExtensions
    {
        public static string EmptyStringIfNull(this string value)
        {
            if (value == null)
                return "";
            return value;
        }

        public static string[] EmptyStringArrayIfNull(this string[] value)
        {
            if (value == null)
            {
                return new string[] { };
            }
            else
            {
                return value;
            }
        }
    }
}
