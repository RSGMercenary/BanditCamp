using System.Globalization;
using System.Text.RegularExpressions;

namespace BanditCamp.Tracks
{
	public class Track
	{
		private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
		//Removes "(featuring ...)" suffixes from track names.
		private static readonly Regex Featuring = new Regex(@"\((?:feat|ft).*?\)", RegexOptions.IgnoreCase);

		private string artist = "";
		private string album = "";
		private string title = "";

		public string Artist { get => artist; set => artist = CleanName(value); }
		public string Album { get => album; set => album = CleanName(value); }
		public uint Year { get; set; } = 0;
		public string Title { get => title; set => title = CleanName(value); }
		public uint Number { get; set; } = 0;
		public string Genre { get; set; } = "";

		public Track() { }

		public Track(string genre)
		{
			Genre = genre;
		}

		private static string CleanName(string value)
		{
			value = Featuring.Replace(value, "");
			value = TextInfo.ToTitleCase(value);
			return value.Trim();
		}
	}
}