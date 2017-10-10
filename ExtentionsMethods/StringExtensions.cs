using System.Globalization;
using System.Text;

namespace Kundbilden
{
    static class StringExtensions
    {
         public static string UpperFirstLetter(this string str)
        {
            var splitStrings = str.Split(' ');
            var newString = new StringBuilder();

            var strText = "";

            foreach (var item in splitStrings)
            {
                if (item == "") continue;

                newString.Clear();
                var text = item.ToUpper();

                newString.Append(item.ToLower());
                newString[0] = text[0];

                strText += (splitStrings.Length > 1) ? newString + " " :  newString.ToString();

                
            }
            return strText;
        }


    }
}