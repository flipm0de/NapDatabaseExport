using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Options;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using NraDatabaseExport.App.Infrastructure;
using NraDatabaseExport.App.Options;
using NraDatabaseExport.DbProviders;
using NraDatabaseExport.ExportProviders;
using DbProviderFactory = NraDatabaseExport.DbProviders.DbProviderFactory;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents a view model for the application.
	/// </summary>
	public class AppViewModel : ViewModelBase
	{
		private readonly IDialogService _dialogService;
		private readonly IOptions<ExportOptions> _exportOptions;
		private bool _isBusy;
		private int _slideIndex;
		private DbProviderViewModel _selectedDbProvider;
		private string _databaseFileName;
		private string _serverName;
		private int? _port;
		private string _userName;
		private string _password;
		private DbDatabaseViewModel _selectedDatabase;
		private ExportProviderViewModel _selectedExportProvider;
		private string _exportPath;

		/// <summary>
		/// Gets or sets the flag indicating whether the model is busy or not.
		/// </summary>
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		/// <summary>
		/// Gets or sets the index of the current slide.
		/// </summary>
		public int SlideIndex
		{
			get => _slideIndex;
			set => SetProperty(ref _slideIndex, value);
		}

		#region "Welcome" Slide

		/// <summary>
		/// Gets the command to go to the "Configure Database" slide.
		/// </summary>
		public DelegateCommand GoToConfigureDatabaseCommand { get; }

		#endregion

		#region "Configure Database" Slide

		/// <summary>
		/// Gets the available database providers.
		/// </summary>
		public ObservableCollection<DbProviderViewModel> DbProviders { get; }

		/// <summary>
		/// Gets or sets the selected database provider.
		/// </summary>
		public DbProviderViewModel SelectedDbProvider
		{
			get => _selectedDbProvider;
			set
			{
				if (!SetProperty(ref _selectedDbProvider, value))
				{
					return;
				}

				NotifyPropertyChanged(nameof(UsesDatabaseFile));
				NotifyPropertyChanged(nameof(UsesServer));
				NotifyPropertyChanged(nameof(UsesPort));
				NotifyPropertyChanged(nameof(UsesUserName));
				NotifyPropertyChanged(nameof(RequiresUserName));
				NotifyPropertyChanged(nameof(UsesPassword));
				NotifyPropertyChanged(nameof(RequiresPassword));
				NotifyPropertyChanged(nameof(UsesDatabaseName));

				GoToSelectDatabaseCommand?.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets the flag indicating whether the database is file-based or not.
		/// </summary>
		public bool UsesDatabaseFile
			=> SelectedDbProvider?.UsesDatabaseFile ?? false;

		/// <summary>
		/// Gets or sets the database file name.
		/// </summary>
		public string DatabaseFileName
		{
			get => _databaseFileName;
			set => SetProperty(ref _databaseFileName, value);
		}

		/// <summary>
		/// Gets the flag indicating whether the database is server-based or not.
		/// </summary>
		public bool UsesServer
			=> SelectedDbProvider?.UsesServer ?? false;

		/// <summary>
		/// Gets the flag indicating whether the database requires specifying a server name or not.
		/// </summary>
		public bool RequiresServerName
			=> SelectedDbProvider?.RequiresServerName ?? false;

		/// <summary>
		/// Gets or sets the server name.
		/// </summary>
		public string? ServerName
		{
			get => _serverName;
			set => SetProperty(ref _serverName, value);
		}

		/// <summary>
		/// Gets the flag indicating whether the database server supports specifying a port or not.
		/// </summary>
		public bool UsesPort
			=> SelectedDbProvider?.UsesPort ?? false;

		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		public int? Port
		{
			get => _port;
			set => SetProperty(ref _port, value);
		}

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a user name or not.
		/// </summary>
		public bool UsesUserName
			=> SelectedDbProvider?.UsesUserName ?? false;

		/// <summary>
		/// Gets the flag indicating whether the database requires specifying a user name or not.
		/// </summary>
		public bool RequiresUserName
			=> SelectedDbProvider?.RequiresUserName ?? false;

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		public string? UserName
		{
			get => _userName;
			set
			{
				if (!SetProperty(ref _userName, value))
				{
					return;
				}

				GoToSelectDatabaseCommand?.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a password or not.
		/// </summary>
		public bool UsesPassword
			=> SelectedDbProvider?.UsesPassword ?? false;

		/// <summary>
		/// Gets the flag indicating whether the database requires specifying a password or not.
		/// </summary>
		public bool RequiresPassword
			=> SelectedDbProvider?.RequiresPassword ?? false;

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string? Password
		{
			get => _password;
			set
			{
				if (!SetProperty(ref _password, value))
				{
					return;
				}

				GoToSelectDatabaseCommand?.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets the command to select a database file.
		/// </summary>
		public DelegateCommand SelectDatabaseFileCommand { get; }

		/// <summary>
		/// Gets the command to go back to the "Welcome" slide.
		/// </summary>
		public DelegateCommand GoBackToWelcomeCommand { get; }

		/// <summary>
		/// Gets the command to go to the "Select Database" slide.
		/// </summary>
		public AsyncDelegateCommand GoToSelectDatabaseCommand { get; }

		#endregion

		#region "Select Database" Slide

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a database name or not.
		/// </summary>
		public bool UsesDatabaseName
			=> SelectedDbProvider?.UsesDatabaseName ?? false;

		/// <summary>
		/// Gets the available databases.
		/// </summary>
		public ObservableCollection<DbDatabaseViewModel> Databases { get; }

		/// <summary>
		/// Gets or sets the selected database.
		/// </summary>
		public DbDatabaseViewModel SelectedDatabase
		{
			get => _selectedDatabase;
			set
			{
				if (!SetProperty(ref _selectedDatabase, value))
				{
					return;
				}

				GoToSelectTablesCommand?.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets the command to go back to the "Configure Database" slide.
		/// </summary>
		public DelegateCommand GoBackToConfigureDatabaseCommand { get; }

		/// <summary>
		/// Gets the command to go to the "Select Tables" slide.
		/// </summary>
		public AsyncDelegateCommand GoToSelectTablesCommand { get; }

		#endregion

		#region "Select Tables" Slide

		/// <summary>
		/// Gets the available tables.
		/// </summary>
		public ObservableCollection<DbTableViewModel> Tables { get; }

		/// <summary>
		/// Gets the command of selecting all tables.
		/// </summary>
		public DelegateCommand SelectAllTablesCommand { get; }

		/// <summary>
		/// Gets the command of deselecting all tables.
		/// </summary>
		public DelegateCommand DeselectAllTablesCommand { get; }

		/// <summary>
		/// Gets the command of inverting table selection.
		/// </summary>
		public DelegateCommand InvertAllTablesCommand { get; }

		/// <summary>
		/// Gets the number of selected tables.
		/// </summary>
		public int SelectedTablesCount
			=> Tables.Count(x => x.IsSelected);

		/// <summary>
		/// Gets the command to go back to the "Select Database" slide.
		/// </summary>
		public DelegateCommand GoBackToSelectDatabaseCommand { get; }

		/// <summary>
		/// Gets the command to go to the "Configure Export" slide.
		/// </summary>
		public DelegateCommand GoToConfigureExportCommand { get; }

		#endregion

		#region "Configure Export" Slide

		/// <summary>
		/// Gets the available export providers.
		/// </summary>
		public ObservableCollection<ExportProviderViewModel> ExportProviders { get; }

		/// <summary>
		/// Gets or sets the selected export provider.
		/// </summary>
		public ExportProviderViewModel SelectedExportProvider
		{
			get => _selectedExportProvider;
			set
			{
				if (!SetProperty(ref _selectedExportProvider, value))
				{
					return;
				}

				GoToExportCommand.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets or sets the path to the export directory.
		/// </summary>
		public string ExportPath
		{
			get => _exportPath;
			set
			{
				if (!SetProperty(ref _exportPath, value))
				{
					return;
				}

				GoToExportCommand.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets the command to select an export directory.
		/// </summary>
		public DelegateCommand SelectExportPathCommand { get; }

		/// <summary>
		/// Gets the command to go to the "Select Tables" slide.
		/// </summary>
		public DelegateCommand GoBackToSelectTablesCommand { get; }

		/// <summary>
		/// Gets the command to go to the "Export" slide.
		/// </summary>
		public DelegateCommand GoToExportCommand { get; }

		#endregion

		#region "Review" Slide

		/// <summary>
		/// Gets the command to go to the "Configure Export" slide.
		/// </summary>
		public DelegateCommand GoBackToConfigureExportCommand { get; }

		/// <summary>
		/// Gets the command to export data.
		/// </summary>
		public AsyncDelegateCommand ExportCommand { get; }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewModel"/> class.
		/// </summary>
		public AppViewModel(IDialogService dialogService,
			IOptions<ExportOptions> exportOptions)
		{
			_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
			_exportOptions = exportOptions ?? throw new ArgumentNullException(nameof(exportOptions));

			#region "Welcome" Slide

			GoToConfigureDatabaseCommand = new DelegateCommand(GoToConfigureDatabase, CanGoToConfigureDatabase);

			#endregion

			#region "Configure Database" Slide

			DbProviderListItem[] dbProviderListItems = DbProviderFactory.ListProviders();

			IEnumerable<DbProviderViewModel> dbProviders = dbProviderListItems
				.Select(CreateDbProviderViewModel);

			DbProviders = new ObservableCollection<DbProviderViewModel>(dbProviders);

			_selectedDbProvider = DbProviders.Count <= 0 ? null : DbProviders[0];

			SelectDatabaseFileCommand = new DelegateCommand(SelectDatabaseFile);
			GoBackToWelcomeCommand = new DelegateCommand(GoBackToWelcome);
			GoToSelectDatabaseCommand = new AsyncDelegateCommand(GoToSelectDatabase, CanGoToSelectDatabase);

			#endregion

			#region "Select Database" Slide

			Databases = new ObservableCollection<DbDatabaseViewModel>();

			Databases.CollectionChanged += Databases_CollectionChanged;

			_selectedDatabase = null;

			GoBackToConfigureDatabaseCommand = new DelegateCommand(GoBackToConfigureDatabase);
			GoToSelectTablesCommand = new AsyncDelegateCommand(GoToSelectTables, CanGoToSelectTables);

			#endregion

			#region "Select Tables" Slide

			Tables = new ObservableCollection<DbTableViewModel>();

			Tables.CollectionChanged += Tables_CollectionChanged;

			SelectAllTablesCommand = new DelegateCommand(SelectAllTables);
			DeselectAllTablesCommand = new DelegateCommand(DeselectAllTables);
			InvertAllTablesCommand = new DelegateCommand(InvertAllTables);
			GoBackToSelectDatabaseCommand = new DelegateCommand(GoBackToSelectDatabase);
			GoToConfigureExportCommand = new DelegateCommand(GoToConfigureExport, CanGoToConfigureExport);

			#endregion

			#region "Configure Export" Slide

			ExportProviderListItem[] exportProviderListItems = ExportProviderFactory.ListProviders();

			IEnumerable<ExportProviderViewModel> exportProviders = exportProviderListItems
				.Select(CreateExportProviderViewModel);

			ExportProviders = new ObservableCollection<ExportProviderViewModel>(exportProviders);

			_selectedExportProvider = ExportProviders.Count <= 0 ? null : ExportProviders[0];

			SelectExportPathCommand = new DelegateCommand(SelectExportPath);
			GoBackToSelectTablesCommand = new DelegateCommand(GoBackToSelectTables);
			GoToExportCommand = new DelegateCommand(GoToExport, CanGoToExport);

			#endregion

			#region "Export" Slide

			GoBackToConfigureExportCommand = new DelegateCommand(GoBackToConfigureExport);
			ExportCommand = new AsyncDelegateCommand(Export, CanExport);

			#endregion

			#region Apply Export Options

			if (exportOptions.Value.DbProviderType != null)
			{
				DbProviderViewModel dbProviderViewModel = DbProviders
					.FirstOrDefault(x => x.Type == exportOptions.Value.DbProviderType.Value);

				SelectedDbProvider = dbProviderViewModel;
			}

			ServerName = exportOptions.Value.ServerName;

			UserName = exportOptions.Value.UserName;

			Password = exportOptions.Value.Password;

			if (_exportOptions.Value.ExportProviderType != null)
			{
				ExportProviderViewModel exportProviderViewModel = ExportProviders
					.FirstOrDefault(x => x.Type == _exportOptions.Value.ExportProviderType.Value);

				SelectedExportProvider = exportProviderViewModel;
			}

			if (_exportOptions.Value.ExportPath != null)
			{
				ExportPath = _exportOptions.Value.ExportPath;
			}

			#endregion

			SlideIndex = (int)Slide.Welcome;
		}

		#region "Welcome" Slide Commands

		private void GoToConfigureDatabase(object parameter)
			=> SlideIndex = (int)Slide.ConfigureDatabase;

		private bool CanGoToConfigureDatabase(object parameter)
			=> true;

		#endregion

		#region "Configure Database" Slide Commands

		private void SelectDatabaseFile(object parameter)
		{
			var dialogSettings = new OpenFileDialogSettings
			{
				CheckFileExists = true,
				FileName = DatabaseFileName,
			};

			bool? result = _dialogService.ShowOpenFileDialog(this, dialogSettings);
			if (result != true)
			{
				return;
			}

			DatabaseFileName = dialogSettings.FileName;
		}

		private void GoBackToWelcome(object parameter)
			=> SlideIndex = (int)Slide.Welcome;

		private async Task GoToSelectDatabase(object parameter, CancellationToken cancellationToken)
		{
			IsBusy = true;

			try
			{
				await RefreshDatabases(cancellationToken);

				SlideIndex = (int)Slide.SelectDatabase;
			}
			catch (Exception ex)
			{
				await ShowError(ex, "Error listing databases");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private bool CanGoToSelectDatabase(object parameter)
		{
			if (SelectedDbProvider is null)
			{
				return false;
			}

			if (RequiresServerName && string.IsNullOrWhiteSpace(ServerName))
			{
				return false;
			}

			if (RequiresUserName && string.IsNullOrWhiteSpace(UserName))
			{
				return false;
			}

			if (RequiresPassword && string.IsNullOrWhiteSpace(Password))
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Select Database" Slide Commands

		private void Databases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (INotifyPropertyChanged item in e.NewItems)
						{
							if (item is null)
							{
								continue;
							}

							item.PropertyChanged += DatabasesItem_PropertyChanged;
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						foreach (INotifyPropertyChanged item in e.OldItems)
						{
							if (item is null)
							{
								continue;
							}

							item.PropertyChanged -= DatabasesItem_PropertyChanged;
						}
						break;
					}
			}
		}

		private void DatabasesItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			GoToSelectDatabaseCommand.NotifyCanExecuteChanged();
		}

		private void GoBackToConfigureDatabase(object parameter)
			=> SlideIndex = (int)Slide.ConfigureDatabase;

		private async Task GoToSelectTables(object parameter, CancellationToken cancellationToken)
		{
			IsBusy = true;

			try
			{
				await RefreshTables(cancellationToken);

				SlideIndex = (int)Slide.SelectTables;
			}
			catch (Exception ex)
			{
				await ShowError(ex, "Error listing tables");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private bool CanGoToSelectTables(object parameter)
		{
			if (!SelectedDbProvider.UsesDatabaseName)
			{
				return true;
			}

			if (SelectedDatabase is null)
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Select Tables" Slide Commands

		private void SelectAllTables(object parameter)
		{
			Tables.ToList().ForEach(x => x.IsSelected = true);
		}

		private void DeselectAllTables(object parameter)
		{
			Tables.ToList().ForEach(x => x.IsSelected = false);
		}

		private void InvertAllTables(object parameter)
		{
			Tables.ToList().ForEach(x => x.IsSelected = !x.IsSelected);
		}

		private void Tables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (INotifyPropertyChanged item in e.NewItems)
						{
							if (item is null)
							{
								continue;
							}

							item.PropertyChanged += TablesItem_PropertyChanged;
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						foreach (INotifyPropertyChanged item in e.OldItems)
						{
							if (item is null)
							{
								continue;
							}

							item.PropertyChanged -= TablesItem_PropertyChanged;
						}
						break;
					}
			}

			NotifyPropertyChanged(nameof(SelectedTablesCount));
		}

		private void TablesItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(nameof(SelectedTablesCount));

			GoToConfigureExportCommand.NotifyCanExecuteChanged();
		}

		private void GoBackToSelectDatabase(object parameter)
			=> SlideIndex = (int)Slide.SelectDatabase;

		private void GoToConfigureExport(object parameter)
		{
			SlideIndex = (int)Slide.ConfigureExport;
		}

		private bool CanGoToConfigureExport(object parameter)
		{
			if (!Tables.Any(x => x.IsSelected))
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Configure Export" Slide Commands

		private void SelectExportPath(object parameter)
		{
			var dialogSettings = new FolderBrowserDialogSettings
			{
				ShowNewFolderButton = true,
				SelectedPath = ExportPath,
			};

			bool? result = _dialogService.ShowFolderBrowserDialog(this, dialogSettings);
			if (result != true)
			{
				return;
			}

			ExportPath = dialogSettings.SelectedPath;
		}

		private void GoBackToSelectTables(object parameter)
			=> SlideIndex = (int)Slide.SelectTables;

		private void GoToExport(object parameter)
		{
			SlideIndex = (int)Slide.Export;
		}

		private bool CanGoToExport(object parameter)
		{
			if (SelectedExportProvider is null)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(ExportPath))
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Export" Slide Commands

		private void GoBackToConfigureExport(object parameter)
			=> SlideIndex = (int)Slide.ConfigureExport;

		private async Task Export(object parameter, CancellationToken cancellationToken)
		{
			IsBusy = true;

			try
			{
				await Export(cancellationToken);

				_dialogService.ShowMessageBox(this,
					$"The export has completed successfully.",
					"Completed",
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				await ShowError(ex, "Error exporting data");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private bool CanExport(object parameter)
		{
			if (Tables is null)
			{
				return false;
			}

			return true;
		}

		#endregion

		private DbProviderViewModel CreateDbProviderViewModel(DbProviderListItem provider)
		{
			DbProviderAttribute attribute = provider.Type.GetCustomAttribute<DbProviderAttribute>();

			if (attribute is null)
			{
				throw new DbProviderException($"Database provider type `{provider.ProviderType}` is missing a `{typeof(DbProviderAttribute)}` attribute.");
			}

			var viewModel = new DbProviderViewModel(provider.ProviderType, provider.DisplayName)
			{

				UsesDatabaseFile = attribute.FileCapability.HasFlag(ProviderCapabilityFlags.Supported),
				UsesServer = attribute.ServerCapability.HasFlag(ProviderCapabilityFlags.Supported),
				RequiresServerName = attribute.ServerCapability.HasFlag(ProviderCapabilityFlags.SupportedAndRequired),
				UsesPort = attribute.PortCapability.HasFlag(ProviderCapabilityFlags.Supported),
				UsesUserName = attribute.UserNameCapability.HasFlag(ProviderCapabilityFlags.Supported),
				RequiresUserName = attribute.UserNameCapability.HasFlag(ProviderCapabilityFlags.SupportedAndRequired),
				UsesPassword = attribute.PasswordCapability.HasFlag(ProviderCapabilityFlags.Supported),
				RequiresPassword = attribute.PasswordCapability.HasFlag(ProviderCapabilityFlags.SupportedAndRequired),
				UsesDatabaseName = attribute.DatabaseCapability.HasFlag(ProviderCapabilityFlags.Supported),
			};

			return viewModel;
		}

		private IExportProvider CreateExportProvider()
		{
			IExportProvider provider = ExportProviderFactory.CreateProvider(SelectedExportProvider.Type);

			return provider;
		}

		private ExportProviderViewModel CreateExportProviderViewModel(ExportProviderListItem provider)
		{
			ExportProviderAttribute attribute = provider.Type.GetCustomAttribute<ExportProviderAttribute>();

			if (attribute is null)
			{
				throw new DbProviderException($"Export provider type `{provider.ProviderType}` is missing a `{typeof(ExportProviderAttribute)}` attribute.");
			}

			var viewModel = new ExportProviderViewModel(provider.ProviderType, provider.DisplayName)
			{
				DefaultExtension = attribute.DefaultExtension,
			};

			return viewModel;
		}

		/// <summary>
		/// Refreshes the list of available databases.
		/// </summary>
		private async Task RefreshDatabases(CancellationToken cancellationToken)
		{
			using IDbProvider dbProvider = CreateDbProvider(null);

			using DbConnection connection = await dbProvider.OpenConnectionAsync(cancellationToken: cancellationToken);

			DbDatabaseListItem[] databaseListItems
				= await dbProvider.ListDatabasesAsync(connection, cancellationToken: cancellationToken);

			Databases.Clear();

			foreach (DbDatabaseListItem databaseListItem in databaseListItems)
			{
				var database = new DbDatabaseViewModel(databaseListItem.Name);

				Databases.Add(database);
			}

			if (Databases.Count <= 0)
			{
				SelectedDatabase = null;
			}
			else if (_exportOptions.Value.DatabaseName is null)
			{
				SelectedDatabase = Databases[0];
			}
			else
			{
				DbDatabaseViewModel database = Databases
					.FirstOrDefault(x => x.Name == _exportOptions.Value.DatabaseName);

				if (database is null)
				{
					SelectedDatabase = Databases[0];
				}
				else
				{
					SelectedDatabase = database;
				}
			}
		}

		/// <summary>
		/// Refreshes the list of available tables.
		/// </summary>
		private async Task RefreshTables(CancellationToken cancellationToken)
		{
			using IDbProvider dbProvider = CreateDbProvider(SelectedDatabase.Name);

			using DbConnection connection = await dbProvider.OpenConnectionAsync(cancellationToken: cancellationToken);

			DbTableListItem[] tableListItems
				= await dbProvider.ListTablesAsync(connection, cancellationToken: cancellationToken);

			Tables.Clear();

			foreach (DbTableListItem tableListItem in tableListItems)
			{
				var table = new DbTableViewModel(tableListItem.Name, tableListItem.OwnerName);

				Tables.Add(table);
			}

			if (!(_exportOptions.Value.Tables is null))
			{
				foreach (DbTableOptionListItem tableOptionListItem in _exportOptions.Value.Tables)
				{
					DbTableViewModel table = Tables
						.FirstOrDefault(x => x.Name == tableOptionListItem.Name
							&& x.OwnerName == tableOptionListItem.OwnerName);

					if (!(table is null))
					{
						table.IsSelected = true;
					}
				}
			}
		}

		private IDbProvider CreateDbProvider(string? databaseName)
		{
			IDbProvider provider = DbProviderFactory.CreateProvider(SelectedDbProvider.Type);

			provider.DatabaseFileName = DatabaseFileName;
			provider.ServerName = ServerName;
			provider.Port = Port;
			provider.UserName = UserName;
			provider.Password = Password;
			provider.DatabaseName = databaseName;

			return provider;
		}

		/// <summary>
		/// Exports the data from the selected database to the specified file with the specified format.
		/// </summary>
		private async Task Export(CancellationToken cancellationToken = default)
		{
			IsBusy = true;

			try
			{
				if (!Directory.Exists(ExportPath))
				{
					throw new DirectoryNotFoundException($@"Could not find directory ""{ExportPath}"" to export data to.");
				}

				Tables.ToList().ForEach(x => x.ExportStatus = null);

				foreach (DbTableViewModel table in Tables)
				{
					if (!table.IsSelected)
					{
						table.ExportStatus = DbTableExportStatus.NotApplicable;

						continue;
					}

					table.ExportStatus = DbTableExportStatus.Busy;
					table.ExportedRowsCount = 0;

					string tableName = table.Name;
					string? ownerName = table.OwnerName;
					int rowIndex = 0;
					int columnIndex = 0;
					string fileName = Path.Combine(ExportPath,
						tableName + SelectedExportProvider.DefaultExtension);

					try
					{
						using IDbProvider dbProvider = CreateDbProvider(SelectedDatabase.Name);

						using DbConnection connection = await dbProvider.OpenConnectionAsync(cancellationToken: cancellationToken);

						using DbDataReader reader = await dbProvider.ExecuteTableReaderAsync(connection, tableName, ownerName, cancellationToken: cancellationToken);

						using IExportProvider exportProvider = CreateExportProvider();

						await exportProvider.OpenWriteAsync(fileName, cancellationToken: cancellationToken);

						var columnNames = new string[reader.FieldCount];

						for (columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
						{
							columnNames[columnIndex] = reader.GetName(columnIndex);
						}

						await exportProvider.WriteHeaderRowAsync(columnNames, cancellationToken: cancellationToken);

						rowIndex++;

						var values = new object[reader.FieldCount];

						while (await reader.ReadAsync(cancellationToken: cancellationToken))
						{
							for (columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
							{
								values[columnIndex] = reader.GetValue(columnIndex);
							}

							await exportProvider.WriteDataRowAsync(values, cancellationToken: cancellationToken);

							table.ExportedRowsCount++;
						}

						table.ExportStatus = DbTableExportStatus.Ok;
					}
					catch (Exception ex)
					{
						table.ExportStatus = DbTableExportStatus.Error;

						throw new ApplicationException($"Could not export data from table {tableName} ({ownerName}), row {rowIndex}, column {columnIndex}.", ex);
					}
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		private Task ShowError(Exception ex, string title)
		{
			var dialogSettings = new MessageBoxSettings
			{
				Caption = title,
				MessageBoxText = $"An error has occurred: {ex.Message}",
				Button = MessageBoxButton.OK,
				Icon = MessageBoxImage.Warning,
			};

			_dialogService.ShowMessageBox(this, dialogSettings);

			return Task.CompletedTask;
		}

		#region Nested Type: Slide

		/// <summary>
		/// Represents an enumeration of available slides.
		/// </summary>
		private enum Slide
		{
			/// <summary>
			/// "Welcome" slide
			/// </summary>
			Welcome = 0,

			/// <summary>
			/// "Configure Database" slide
			/// </summary>
			ConfigureDatabase = 1,

			/// <summary>
			/// "Select Database" slide
			/// </summary>
			SelectDatabase = 2,

			/// <summary>
			/// "Select Tables" slide
			/// </summary>
			SelectTables = 3,

			/// <summary>
			/// "Configure Export" slide
			/// </summary>
			ConfigureExport = 4,

			/// <summary>
			/// "Export" slide
			/// </summary>
			Export = 5,
		}

		#endregion
	}
}
