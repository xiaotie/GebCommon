using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public static class ConvertHelper
    {
        public static byte[] ConvertFromBase64String(String dataStr)
        {
            dataStr = dataStr.Replace("$", "/");
            dataStr = dataStr.Replace("#", "+");
            byte[] data = Convert.FromBase64String(dataStr);
            return data;
        }

        public static String ConvertToBase64String(byte[] data)
        {
            String dataStr = Convert.ToBase64String(data);
            dataStr = dataStr.Replace("/", "$");
            dataStr = dataStr.Replace("+", "#");
            return dataStr;
        }
    }
}
