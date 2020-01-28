using System.Windows;
using System.Windows.Threading;

namespace NraDatabaseExport.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(
				$"An unhandled exception has occurred: {e.Exception.Message}",
				"Error",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			e.Handled = true;
		}
	}
}
