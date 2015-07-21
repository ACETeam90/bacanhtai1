using System;
using System.Collections.Generic;
using System.Data;
using Novacode;
using System.IO;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Common
{
    public static class ConvertAttributeValue
    {
        public static string MultiChoiceValue(int intValue)
        {
            string value = string.Empty;
            switch (intValue)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
            }
            return value;
        }

        public static string MultiChoiceValue(string intValue)
        {
            string value = string.Empty;
            switch (intValue)
            {
                case "1":
                    return "A";
                case "2":
                    return "B";
                case "3":
                    return "C";
                case "4":
                    return "D";
                case "5":
                    return "E";
            }
            return value;
        }
    }
}