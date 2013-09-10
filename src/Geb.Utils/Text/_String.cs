using System;
using System.Collections.Generic;
using System.Text;

namespace Orc.Util
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

        public static String Extract(this String txt, String start, String end)
        {
            if (txt == null) return null;
            Int32 startLength = start == null ? 0 : start.Length;
            Int32 indexStart = start == null ? 0 : txt.IndexOf(start);
            Int32 indexEnd = end == null ? txt.Length : txt.IndexOf(end, Math.Max(0, indexStart + startLength));

            if (indexStart < 0 || indexEnd < 0) return null;
            else return txt.Substring(indexStart + startLength, indexEnd - indexStart - startLength);
        }
	}
}
