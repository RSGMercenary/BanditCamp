using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BanditCamp.Models
{
	public abstract class Model : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string caller = null)
		{
			if(Equals(field, value))
				return false;
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
			return true;
		}

		protected bool OnPropertyChanged([CallerMemberName] string caller = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
			return true;
		}
	}
}
