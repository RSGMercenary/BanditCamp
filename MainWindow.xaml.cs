using BanditCamp.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BanditCamp
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = BanditCampModelProvider.Load();
			EventManager.RegisterClassHandler(typeof(TextBoxBase), PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			BanditCampModelProvider.Save(DataContext as BanditCampModel);
			base.OnClosing(e);
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if(e.ClickCount == 3 && sender is TextBoxBase box)
				box.SelectAll();
		}
	}
}