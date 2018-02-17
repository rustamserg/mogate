using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Data.Xml.Dom;

// Copied from great arcticle
// http://www.codeproject.com/Tips/406235/A-Simple-PList-Parser-in-Csharp

namespace Elizabeth
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

            byte[] bytes = new byte[data.Length];
            data.Position = 0;
            data.Read(bytes, 0, (int)data.Length);
            string buf = Encoding.ASCII.GetString(bytes);

            XmlDocument doc = new XmlDocument ();
			doc.LoadXml(buf);

			IXmlNode plist = doc.SelectSingleNode("plist");
			IXmlNode dict = plist.SelectSingleNode("dict");

            var dictElements = dict.ChildNodes;
			Parse(this, dictElements);
		}

		private void Parse(PList dict, XmlNodeList elements)
		{
            uint i = 0;
            while (i < elements.Count)
            {
                if (elements.Item(i) is XmlElement)
                {
                    IXmlNode key = elements.Item(i);
                    IXmlNode val = elements.Item(i + 2);

                    dict[key.InnerText] = ParseValue(val);
                    i += 2;
                }
                ++i;
            }
		}
		
		private dynamic ParseValue(IXmlNode val)
		{
			switch (val.NodeName.ToString ()) {
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
				return new List<dynamic>();
			default:
				throw new ArgumentException ("Unsupported");
			}
		}
	}
}

