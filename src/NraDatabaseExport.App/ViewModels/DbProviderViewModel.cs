using System;
using NraDatabaseExport.App.Infrastructure;
using NraDatabaseExport.DbProviders;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents the view model for a database provider.
	/// </summary>
	public class DbProviderViewModel : ViewModelBase
	{
		private bool _usesDatabaseFile;
		private bool _usesServer;
		private bool _usesPort;
		private bool _requiresServerName;
		private bool _usesUserName;
		private bool _requiresUserName;
		private bool _usesPassword;
		private bool _requiresPassword;
		private bool _usesDatabaseName;

		/// <summary>
		/// Gets the type of the database provider.
		/// </summary>
		public DbProviderType Type { get; }

		/// <summary>
		/// Gets the display name of the database provider.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Gets or sets the flag indicating whether the database is file-based or not.
		/// </summary>
		public bool UsesDatabaseFile
		{
			get => _usesDatabaseFile;
			set => SetProperty(ref _usesDatabaseFile, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database is server-based or not.
		/// </summary>
		public bool UsesServer
		{
			get => _usesServer;
			set => SetProperty(ref _usesServer, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database requires specifying a server name.
		/// </summary>
		public bool RequiresServerName
		{
			get => _requiresServerName;
			set => SetProperty(ref _requiresServerName, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database server supports specifying a port or not.
		/// </summary>
		public bool UsesPort
		{
			get => _usesPort;
			set => SetProperty(ref _usesPort, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database allows specifying a user name or not.
		/// </summary>
		public bool UsesUserName
		{
			get => _usesUserName;
			set => SetProperty(ref _usesUserName, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database requires specifying a user name or not.
		/// </summary>
		public bool RequiresUserName
		{
			get => _requiresUserName;
			set => SetProperty(ref _requiresUserName, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database allows specifying a password or not.
		/// </summary>
		public bool UsesPassword
		{
			get => _usesPassword;
			set => SetProperty(ref _usesPassword, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database requires specifying a password or not.
		/// </summary>
		public bool RequiresPassword
		{
			get => _requiresPassword;
			set => SetProperty(ref _requiresPassword, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating whether the database allows specifying a database name or not.
		/// </summary>
		public bool UsesDatabaseName
		{
			get => _usesDatabaseName;
			set => SetProperty(ref _usesDatabaseName, value);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="DbProviderViewModel"/> class with a specified name.
		/// </summary>
		/// <param name="type">the type of the database provider</param>
		/// <param name="displayName">the display name of the database provider</param>
		public DbProviderViewModel(DbProviderType type, string displayName)
		{
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
