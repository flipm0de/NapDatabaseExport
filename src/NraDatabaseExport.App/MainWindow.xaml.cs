using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.Options;
using NraDatabaseExport.App.ViewModels;

namespace NraDatabaseExport.App
{
	/// <summary>
	/// Represents the main window of the application.
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly AppViewModel _viewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow(AppViewModel viewModel)
		{
			_viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

			DataContext = _viewModel;

			InitializeComponent();
		}
	}
}
