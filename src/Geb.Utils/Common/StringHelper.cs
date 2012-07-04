using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
	public static class StringHelper
	{
		/// <summary>
		/// 把字符串中间连续的空格、制表符、换行符用空格替代
		/// </summary>
		/// <param name="newString"></param>
		/// <returns></returns>
        public static String ShrinkSpace(this String newString)
		{
			if (newString == null) return null;
			StringBuilder sb = new StringBuilder(newString.Length);
			Boolean tailWithSpace = false;
			foreach (Char c in newString)
			{
				if (Char.IsWhiteSpace(c))
				{
					if (tailWithSpace == false)
					{
						sb.Append(' ');
						tailWithSpace = true;
					}
				}
				else
				{
					sb.Append(c);
					tailWithSpace = false;
				}
			}

			return sb.ToString();
		}

        public static String Extract(this String txt, String ahead, String latter)
        {
            if (txt == null) return String.Empty;
            Int32 indexStart = ahead == null ? 0 : txt.IndexOf(ahead);
            if (indexStart < 0) return String.Empty;

            Int32 startLength = ahead == null ? 0 : ahead.Length;
            Int32 indexEnd = latter == null ? txt.Length : txt.IndexOf(latter, Math.Max(0, indexStart + startLength));

            if (indexEnd < indexStart) return String.Empty;
            else return txt.Substring(indexStart + startLength, indexEnd - indexStart - startLength);
        }
	}
}
