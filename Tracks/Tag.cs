using TagLib;

namespace BanditCamp.Tracks
{
	public static class Tag
	{
		public static void SetTrackTags(Track track, bool embed)
		{
			var tag = File.Create(track.TrackFile());
			tag.Tag.AlbumArtists = new[] { track.Artist };
			tag.Tag.Album = track.Album;
			tag.Tag.Year = track.Year;
			tag.Tag.Title = track.Title;
			tag.Tag.Track = track.Number;

			if(!string.IsNullOrWhiteSpace(track.Genre))
				tag.Tag.Genres = new[] { track.Genre };

			if(embed && System.IO.File.Exists(track.TrackFile()))
				tag.Tag.Pictures = new[] { new Picture(track.ArtFile()) };

			tag.Save();
		}
	}
}