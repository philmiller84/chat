using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Need4Chat.Shared
{
    public class Formats
    {
        static public CultureInfo c = CultureInfo.CreateSpecificCulture("en-US");

        static public string StandardDate(DateTime dt)
        {
            return dt.ToString("F", c);
        }
    }
}
