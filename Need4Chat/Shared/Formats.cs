﻿using System;
using System.Globalization;

namespace Need4Chat.Shared
{
    public class Formats
    {
        public static CultureInfo c = CultureInfo.CreateSpecificCulture("en-US");

        public static string StandardDate(DateTime dt)
        {
            return dt.ToString("F", c);
        }
    }
}
