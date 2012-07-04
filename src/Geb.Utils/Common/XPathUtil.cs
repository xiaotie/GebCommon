using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Geb.Utils
{
	public delegate T XPathParseFunc<T>(XPathNavigator nav);

	public static class XPathUtil
	{
		public static String GetAttribute(XPathNavigator nav, String atbName)
		{
			String val = nav.GetAttribute(atbName, nav.GetNamespace(atbName));
			return val == null ? String.Empty : val;
		}

		public static String GetFirstChildValue(XPathNavigator nav, String xpath)
		{
			XPathNavigator find = nav.SelectSingleNode(xpath);
			return find == null ? String.Empty : find.Value;
		}

		public static List<String> GetChildValues(XPathNavigator nav, String xpath)
		{
			List<String> vlist = new List<string>();
			foreach (XPathNavigator n in nav.Select(xpath))
			{
				String v = n.Value;
				if (String.IsNullOrEmpty(v) == false) vlist.Add(v);
			}
			return vlist;
		}

		public static List<T> ParseAll<T>(XPathNavigator nav, String xpath, XPathParseFunc<T> perseFunc)
		{
			List<T> tList = new List<T>();

			foreach (XPathNavigator n in nav.Select(xpath))
			{
				T t = perseFunc(n);
				if (t != null) tList.Add(t);
			}

			return tList;
		}
	}
}
