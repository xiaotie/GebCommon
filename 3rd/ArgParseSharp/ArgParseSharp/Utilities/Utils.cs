using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
	public static class Utils
	{
		public static string SafeFormatObject(this object o, string format, IFormatProvider provider, bool quoteCharsAndStrings) {
			string s;
			if (o is IFormattable) {
				s = (o as IFormattable)?.ToString(format, provider) ?? "NULL";
			}
			else if (quoteCharsAndStrings && o is string) {
				s = string.Format("\"{0}\"", o == null ? "NULL" : (o as string));
			}
			else if (quoteCharsAndStrings && o is char) {
				s = string.Format("'{0}'", o);
			}
			else {
				s = o?.ToString() ?? "NULL";
			}

			return s;
		}

		public static string SafeFormatObject(this object o, string format, bool quoteCharsAndStrings) {
			return SafeFormatObject(o, format, System.Globalization.CultureInfo.CurrentCulture, quoteCharsAndStrings);
		}

		public static string SafeFormatObject(this object o, bool quoteCharsAndStrings) {
			return SafeFormatObject(o, string.Empty, System.Globalization.CultureInfo.CurrentCulture, quoteCharsAndStrings);
		}

		public static string SafeFormatObject(this object o, string format) {
			return SafeFormatObject(o, format, System.Globalization.CultureInfo.CurrentCulture, false);
		}

		public static string SafeFormatObject(this object o) {
			return SafeFormatObject(o, string.Empty, System.Globalization.CultureInfo.CurrentCulture, false);
		}

		public static string SafeSubstring(this string s, int startIndex, int length=int.MaxValue) {
			length = Math.Min(length, s.Length - startIndex);
			return s.Substring(startIndex, length);
		}

		public static string JoinArray<T>(T[] ar, string delim, string format, IFormatProvider provider)
		{
			int lengthGuess = ar.Length * (1 + delim.Length) + 2;
			StringBuilder sb = new StringBuilder(lengthGuess);

			sb.Append("[");

			foreach (T o in ar)
			{
				sb.Append(SafeFormatObject(o, format, provider, true));
				sb.Append(delim);
			}

			if (ar.Length > 0)
				sb.Length -= delim.Length;

			sb.Append("]");

			return sb.ToString();
		}

		public static string JoinArray<T>(IList<T> ar, string delim, string format, IFormatProvider provider)
		{
			int lengthGuess = ar.Count * (1 + delim.Length) + 2;
			StringBuilder sb = new StringBuilder(lengthGuess);

			sb.Append("[");

			foreach (T o in ar)
			{
				sb.Append(SafeFormatObject(o, format, provider, true));
				sb.Append(delim);
			}

			if (ar.Count > 0)
				sb.Length -= delim.Length;

			sb.Append("]");

			return sb.ToString();
		}

		public static string JoinArray(IList ar, string delim, string format, IFormatProvider provider)
		{
			int lengthGuess = ar.Count * (1 + delim.Length) + 2;
			StringBuilder sb = new StringBuilder(lengthGuess);

			sb.Append("[");

			foreach (object o in ar)
			{
				sb.Append(SafeFormatObject(o, format, provider, true));
				sb.Append(delim);
			}

			if (ar.Count > 0)
				sb.Length -= delim.Length;

			sb.Append("]");

			return sb.ToString();
		}

		public static string JoinArray<T>(T[] ar, string delim, string format)
		{
			return JoinArray(ar, delim, format, System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray<T>(T[] ar, string delim)
		{
			return JoinArray(ar, delim, "", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray<T>(T[] ar)
		{
			return JoinArray(ar, ", ", "", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray<T>(IList<T> ar, string delim, string format)
		{
			return JoinArray(ar, delim, format, System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray<T>(IList<T> ar, string delim)
		{
			return JoinArray(ar, delim, "", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray<T>(IList<T> ar)
		{
			return JoinArray(ar, ", ", "", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray(IList ar, string delim, string format)
		{
			return JoinArray(ar, delim, format, System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray(IList ar, string delim)
		{
			return JoinArray(ar, delim, "", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string JoinArray(IList ar)
		{
			return JoinArray(ar, ", ", "", System.Globalization.CultureInfo.CurrentCulture);
		}
	}
}

