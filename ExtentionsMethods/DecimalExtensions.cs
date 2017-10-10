using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kundbilden.ExtentionsMethods
{
    public static class DecimalExtensions
    {
        public static string GetSwedishKr(this decimal number)
        {
            // Return the whole number if less than 0,01
            if (number < 0.01M) return number.ToString(CultureInfo.CreateSpecificCulture("sv-SE"));

            // Round down the number to nearest 2 decimals
            var nr = Math.Floor(number * 100) / 100; ;
            return nr.ToString("C", CultureInfo.CreateSpecificCulture("sv-SE"));
        }

        public static string GetProcent(this decimal number)
        {
            return $"{number: 0.00} %";
        }
    }
}
