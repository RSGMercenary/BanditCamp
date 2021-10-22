using BanditCamp.Logs;
using BanditCamp.Tracks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace BanditCamp.Models
{
	[DataContract]
	public class BanditCampModel : Model
	{
		#region Fields
		private readonly NetClient Client = new NetClient();
		private readonly BackgroundWorker Worker = new BackgroundWorker();
		private string url = "";
		private string genre = "";
		private string notes = "";
		private bool isEnabled = true;
		private bool downloadMp3 = true;
		private bool downloadJpg = true;
		private bool embeddedJpg = false;
		#endregion

		public BanditCampModel()
		{
			BandCampCommand = new Command(OpenBandCamp);
			DownloadCommand = new Command(DownloadFiles);
			Worker.DoWork += (sender, e) => Download();
		}

		private void OpenBandCamp() => Process.Start(new ProcessStartInfo { FileName = "https://www.bandcamp.com", UseShellExecute = true });

		private void DownloadFiles()
		{
			if(!IsEnabled)
				return;
			if(!BandCamp.IsBandCampUrl(Url))
				return;

			Worker.RunWorkerAsync();
		}

		private void Download()
		{
			IsEnabled = false;

			var time = DateTime.Now;
			var uri = new Uri(Url);
			var track = new Track(Genre);

			if(Logs.Count > 1)
				UpdateLogs(LogStatus.None, LogType.None, "", time);
			UpdateLogs(LogStatus.Info, LogType.Url, $"{Url}{AppendGenre()}", time);

			try
			{
				Url = "";
				Genre = "";

				if(!BandCamp.AlbumUris.Any(uri.AbsolutePath.ToLower().Contains))
					DownloadArtist(Client.DownloadXml(new Uri(uri, BandCamp.AlbumsUri)), track);
				else
					DownloadAlbum(Client.DownloadXml(uri), track);
			}
			catch(Exception exception)
			{
				UpdateLogs(LogStatus.Error, LogType.Error, exception.Message, time);
			}

			UpdateLogs(LogStatus.Info, LogType.Info, "Download Done", time);
			IsEnabled = true;
		}

		private string AppendGenre() => string.IsNullOrWhiteSpace(Genre) ? "" : $" | {Genre}";

		private void DownloadArtist(XmlDocument xml, Track track)
		{
			var time = DateTime.Now;
			var albumUris = BandCamp.GetAlbumUris(xml);

			track.Artist = xml.SelectSingleNode("//*[local-name()='meta' and @name='title']/@content").InnerText;

			var status = GetStatus(track.ArtistFolder());

			if(albumUris.Any())
			{
				var uri = Client.Uri;
				foreach(var albumUri in albumUris)
					DownloadAlbum(Client.DownloadXml(new Uri(uri, albumUri)), track);
			}
			else
				DownloadAlbum(xml, track);

			UpdateLogs(status, LogType.Artist, track.Artist, time);
		}

		private LogStatus GetStatus(string path) => Directory.Exists(path) ? LogStatus.Exists : LogStatus.Add;

		private void DownloadAlbum(XmlDocument xml, Track track)
		{
			var time = DateTime.Now;
			var script = BandCamp.GetScript(xml);

			track.Artist = BandCamp.GetArtist(script);
			track.Album = BandCamp.GetAlbum(script);
			track.Year = BandCamp.GetYear(script);
			track.Number = 0;

			var status = GetStatus(track.AlbumFolder());

			if(DownloadJpg || EmbeddedJpg)
				DownloadArt(BandCamp.GetArtUri(xml), track);

			if(DownloadMp3)
			{
				foreach(var trackJson in BandCamp.GetTracks(script))
				{
					track.Title = BandCamp.GetTitle(trackJson);
					track.Number += 1;
					DownloadTrack(BandCamp.GetTrackUri(trackJson), track);
				}
			}

			if(!DownloadJpg && EmbeddedJpg)
				DeleteArt(track);

			UpdateLogs(status, LogType.Album, track.Album, time);
		}

		private void DownloadArt(string uri, Track track) => DownloadFile(uri, track.ArtFile(), LogType.Jpg, track.Album);

		private void DeleteArt(Track track)
		{
			File.Delete(track.ArtFile());
			UpdateLogs(LogStatus.Remove, LogType.Jpg, track.Album, DateTime.Now);
		}

		private void DownloadTrack(string uri, Track track)
		{
			if(!DownloadFile(uri, track.TrackFile(), LogType.Mp3, track.Title))
				return;
			Tag.SetTrackTags(track, EmbeddedJpg);
		}

		private bool DownloadFile(string uri, string path, LogType type, string value)
		{
			var time = DateTime.Now;

			if(string.IsNullOrWhiteSpace(uri))
			{
				UpdateLogs(LogStatus.Missing, type, value, time);
				return false;
			}

			if(File.Exists(path))
			{
				UpdateLogs(LogStatus.Exists, type, value, time);
				return false;
			}

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			Client.DownloadFile(new Uri(uri), path);
			UpdateLogs(LogStatus.Add, type, value, time);
			return true;
		}

		private void UpdateLogs(LogStatus status, LogType type, string value, DateTime time)
		{
			Application.Current.Dispatcher.Invoke(() => Logs.Insert(0, new Log(time, status, type, value)));
		}

		#region Properties
		public string Url
		{
			get => url;
			set => OnPropertyChanged(ref url, value);
		}

		public string Genre
		{
			get => genre;
			set => OnPropertyChanged(ref genre, value);
		}

		public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();

		[DataMember]
		public string Notes
		{
			get => notes;
			set => OnPropertyChanged(ref notes, value);
		}

		[DataMember]
		public bool DownloadJpg
		{
			get => downloadJpg;
			set => OnPropertyChanged(ref downloadJpg, value);
		}

		[DataMember]
		public bool DownloadMp3
		{
			get => downloadMp3;
			set => OnPropertyChanged(ref downloadMp3, value);
		}

		[DataMember]
		public bool EmbeddedJpg
		{
			get => embeddedJpg;
			set => OnPropertyChanged(ref embeddedJpg, value);
		}

		public bool IsEnabled
		{
			get => isEnabled;
			set => OnPropertyChanged(ref isEnabled, value);
		}

		public ICommand DownloadCommand { get; }

		public ICommand BandCampCommand { get; }
		#endregion
	}
}