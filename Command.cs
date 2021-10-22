using System;
using System.Windows.Input;

namespace BanditCamp
{
	public class Command : ICommand
	{
		private readonly Action Method;

		public Command(Action method) => Method = method;

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter) => Method.Invoke();

		public event EventHandler CanExecuteChanged;
	}
}