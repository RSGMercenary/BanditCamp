using BanditCamp.Logs;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BanditCamp
{
	public class StatusColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (LogStatus)value;
			if(status == LogStatus.Error)
				return new SolidColorBrush(Colors.Red);
			if(status == LogStatus.Add)
				return new SolidColorBrush(Colors.ForestGreen);
			if(status == LogStatus.Remove)
				return new SolidColorBrush(Colors.Orange);
			if(status == LogStatus.Exists)
				return new SolidColorBrush(Colors.Yellow);
			if(status == LogStatus.Missing)
				return new SolidColorBrush(Colors.Gray);
			if(status == LogStatus.Info)
				return (SolidColorBrush)parameter;
			return new SolidColorBrush(Colors.White);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}