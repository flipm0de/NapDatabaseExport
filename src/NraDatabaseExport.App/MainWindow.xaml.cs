using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using NraDatabaseExport.App.ViewModels;
using NraDatabaseExport.DbProviders;
using NraDatabaseExport.ExportProviders;

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
		public MainWindow()
		{
			_viewModel = new AppViewModel();

			DataContext = _viewModel;

			InitializeComponent();
		}

		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			string? dbProviderTypeName = ConfigurationManager.AppSettings["SelectedDbProviderType"];
			if (!string.IsNullOrWhiteSpace(dbProviderTypeName)
				&& Enum.TryParse(dbProviderTypeName, true, out DbProviderType dbProviderType))
			{
				DbProviderViewModel dbProviderViewModel = _viewModel.DbProviders.FirstOrDefault(x => x.Type == dbProviderType);

				_viewModel.SelectedDbProvider = dbProviderViewModel;
			}

			_viewModel.ServerName = ConfigurationManager.AppSettings["ServerName"];

			_viewModel.UserName = ConfigurationManager.AppSettings["UserName"];

			_viewModel.Password = ConfigurationManager.AppSettings["Password"];

			string? exportProviderTypeName = ConfigurationManager.AppSettings["SelectedExportProviderType"];
			if (!string.IsNullOrWhiteSpace(exportProviderTypeName)
				&& Enum.TryParse(exportProviderTypeName, true, out ExportProviderType exportProviderType))
			{
				ExportProviderViewModel exportProviderViewModel = _viewModel.ExportProviders.FirstOrDefault(x => x.Type == exportProviderType);

				_viewModel.SelectedExportProvider = exportProviderViewModel;
			}
		}
	}
}
