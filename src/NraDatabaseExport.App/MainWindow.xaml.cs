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
		private readonly IOptions<ExportOptions> _exportOptions;
		private readonly AppViewModel _viewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow(IOptions<ExportOptions> exportOptions,
			AppViewModel viewModel)
		{
			_exportOptions = exportOptions ?? throw new ArgumentNullException(nameof(exportOptions));
			_viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

			DataContext = _viewModel;

			InitializeComponent();
		}

		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (_exportOptions.Value.DbProviderType != null)
			{
				DbProviderViewModel dbProviderViewModel = _viewModel.DbProviders
					.FirstOrDefault(x => x.Type == _exportOptions.Value.DbProviderType.Value);

				_viewModel.SelectedDbProvider = dbProviderViewModel;
			}

			_viewModel.ServerName = _exportOptions.Value.ServerName;

			_viewModel.UserName = _exportOptions.Value.UserName;

			_viewModel.Password = _exportOptions.Value.Password;

			if (_exportOptions.Value.ExportProviderType != null)
			{
				ExportProviderViewModel exportProviderViewModel = _viewModel.ExportProviders
					.FirstOrDefault(x => x.Type == _exportOptions.Value.ExportProviderType.Value);

				_viewModel.SelectedExportProvider = exportProviderViewModel;
			}
		}
	}
}
