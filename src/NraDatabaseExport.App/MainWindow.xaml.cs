using System.Windows;
using NraDatabaseExport.App.ViewModels;

namespace NraDatabaseExport.App
{
	/// <summary>
	/// Represents the main window of the application.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			var viewModel = new AppViewModel();

			DataContext = viewModel;
		}
	}
}
