using SgmlCore;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace BanditCamp
{
	public class NetClient : WebClient
	{
		public Uri Uri { get; private set; }

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			var response = base.GetWebResponse(request);
			Uri = response.ResponseUri;
			return response;
		}

		public XmlDocument DownloadXml(Uri uri)
		{
			var xml = new XmlDocument();
			using(var reader = new SgmlReader())
			{
				var html = CleanHtml(DownloadString(uri));
				reader.InputStream = new StringReader(html);
				reader.DocType = "HTML";
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.CaseFolding = CaseFolding.ToLower;
				xml.Load(reader);
			}
			return xml;
		}

		private string CleanHtml(string value)
		{
			//Removes HTML comments that may corrupt XML reading.
			return Regex.Replace(value, @"<!.*?>", "");
		}
	}
}