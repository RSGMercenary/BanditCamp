using System;
using System.Text.RegularExpressions;

namespace BanditCamp.Tracks
{
	public static class TrackExtensions
	{
		//Illegal Chars    \/:*?"<>|
		private static readonly Regex Dash = new Regex(@"\||\\|\/|:");
		private static readonly Regex Empty = new Regex(@"\*|\?");
		private static readonly Regex Quote = new Regex(@"""|<|>");

		public static string ArtistFolder(this Track track) => @$"C:\Users\{Environment.UserName}\Downloads\{ReplaceIllegalChars(track.Artist)}";

		public static string AlbumFolder(this Track track) => @$"{track.ArtistFolder()}\{ReplaceIllegalChars(track.Album)}";

		public static string ArtFile(this Track track) => @$"{track.AlbumFolder()}\{ReplaceIllegalChars(track.Album)}.jpg";

		public static string TrackFile(this Track track) => @$"{track.AlbumFolder()}\{ReplaceIllegalChars(track.Title)}.mp3";

		private static string ReplaceIllegalChars(string value)
		{
			value = Dash.Replace(value, "-");
			value = Empty.Replace(value, "");
			value = Quote.Replace(value, "'");
			return value;
		}
	}
}