using System;

namespace BanditCamp.Logs
{
	public class Log
	{
		public Log(DateTime time, LogStatus status, LogType type, string text)
		{
			if(status == LogStatus.None)
				return;

			Time = DateTime.Now.Subtract(time).ToString(@"mm\:ss\.ff");
			Status = status;
			Type = type.ToString().ToUpper();
			Text = text;
		}

		public string Time { get; } = "";
		public LogStatus Status { get; }
		public string Type { get; } = "";
		public string Text { get; } = "";
	}
}