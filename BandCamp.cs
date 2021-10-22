using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BanditCamp
{
	public static class BandCamp
	{
		public static string AlbumsUri => "/music";

		public static IEnumerable<string> AlbumUris
		{
			get
			{
				yield return "album";
				yield return "track";
			}
		}

		public static bool IsBandCampUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.ToString().ToLower().Contains("bandcamp.com");
		}

		public static XmlNode GetScript(XmlNode xml)
		{
			return xml.SelectSingleNode("//*[local-name()='script' and @data-tralbum]");
		}

		public static string GetArtist(XmlNode xml)
		{
			return GetValue(JToken.Parse(xml.Attributes["data-embed"].InnerText), "artist");
		}

		public static IEnumerable<string> GetAlbumUris(XmlNode xml)
		{
			return xml.SelectNodes("//*[local-name()='li' and contains(@class, 'music-grid-item')]/*[local-name()='a']/@href").Cast<XmlNode>().Select(n => n.InnerText);
		}

		public static uint GetYear(XmlNode xml)
		{
			return (uint)DateTime.Parse(GetValue(JToken.Parse(xml.Attributes["data-tralbum"].InnerText), "current.release_date")).Year;
		}

		public static string GetAlbum(XmlNode xml)
		{
			return GetValue(JToken.Parse(xml.Attributes["data-embed"].InnerText), "album_title", "title");
		}

		public static JArray GetTracks(XmlNode xml) => JToken.Parse(xml.Attributes["data-tralbum"].InnerText).Value<JArray>("trackinfo");

		public static string GetTitle(JToken token) => GetValue(token, "title");

		public static string GetTrackUri(JToken token) => GetValue(token, "file.mp3-128");

		private static string GetValue(JToken token, params string[] keys)
		{
			foreach(var key in keys)
			{
				var value = token.SelectToken(key)?.Value<string>();
				if(!string.IsNullOrWhiteSpace(value))
					return value;
			}
			return "";
		}

		public static string GetArtUri(XmlDocument xml)
		{
			return xml.SelectSingleNode("//*[local-name()='div' and contains(@id, 'albumArt')]//*[local-name()='img']/@src").InnerText;
		}
	}
}