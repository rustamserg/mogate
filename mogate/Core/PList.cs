using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace mogate
{
	public class PList : Dictionary<string, dynamic>
	{
		public PList ()
		{
		}

		public PList (Stream data)
		{
			Load (data);
		}

		public void Load (Stream data)
		{
			Clear();

			XmlDocument doc = new XmlDocument ();
			doc.Load(data);

			XmlNode plist = doc.SelectSingleNode("plist");
			XmlNode dict = plist.SelectSingleNode("dict");

			var dictElements = dict.ChildNodes;
			Parse(this, dictElements);
		}

		private void Parse(PList dict, XmlNodeList elements)
		{
			for (int i = 0; i < elements.Count; i += 2)
			{
				XmlNode key = elements.Item (i);
				XmlNode val = elements.Item (i + 1);

				dict[key.InnerText] = ParseValue(val);
			}
		}

		private List<dynamic> ParseArray(XmlNodeList elements)
		{
			List<dynamic> list = new List<dynamic>();
			/*foreach (XmlNode e in elements)
			{
				dynamic one = ParseValue(e);
				list.Add(one);
			}*/

			return list;
		}

		private dynamic ParseValue(XmlNode val)
		{
			switch (val.Name.ToString ()) {
			case "string":
				return val.InnerText;
			case "integer":
				return int.Parse (val.InnerText);
			case "real":
				return float.Parse (val.InnerText);
			case "true":
				return true;
			case "false":
				return false;
			case "dict":
				PList plist = new PList ();
				Parse (plist, val.ChildNodes);
				return plist;
			case "array":
				List<dynamic> list = ParseArray (val.ChildNodes);
				return list;
			default:
				throw new ArgumentException ("Unsupported");
			}
		}
	}
}

