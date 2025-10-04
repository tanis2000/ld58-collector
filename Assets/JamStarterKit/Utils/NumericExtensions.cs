using System.Globalization;

namespace GameBase.Utils
{
    public static class NumericExtensions
    {
        public static string AsScore(this int value)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return value.ToString("#,0", nfi);
        }
    }
}