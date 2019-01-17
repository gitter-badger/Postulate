using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Tests
{
	public static class XmlConfig
	{
		public static string GetConnectionString(string name)
		{
			return GetValue($"/configuration/connectionStrings/add[@name='{name}']", (nd) => nd.Attributes["connectionString"].Value);
		}

		public static string GetValue(string xpath, Func<XmlNode, string> selector)
		{
			string fileName = Assembly.GetExecutingAssembly().Location + ".config";
			return GetValue(fileName, xpath, selector);
		}

		public static string GetValue(string fileName, string xpath, Func<XmlNode, string> selector)
		{
			string path = fileName.Replace("~", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			var node = doc.SelectSingleNode(xpath);
			return selector.Invoke(node);
		}
	}
}