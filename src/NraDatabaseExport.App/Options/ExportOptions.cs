using NraDatabaseExport.DbProviders;
using NraDatabaseExport.ExportProviders;

namespace NraDatabaseExport.App.Options
{
	/// <summary>
	/// Represents the export options.
	/// </summary>
	public class ExportOptions
	{
		/// <summary>
		/// Gets or sets the database provider type.
		/// </summary>
		public DbProviderType? DbProviderType { get; set; }

		/// <summary>
		/// Gets or sets the database file name.
		/// </summary>
		public string DatabaseFileName { get; set; }

		/// <summary>
		/// Gets or sets the server name.
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		public string DatabaseName { get; set; }

		/// <summary>
		/// Gets or sets the list of tables.
		/// </summary>
		public DbTableOptionListItem[] Tables { get; set; }

		/// <summary>
		/// Gets or sets the export provider type.
		/// </summary>
		public ExportProviderType? ExportProviderType { get; set; }

		/// <summary>
		/// Gets or sets the export path.
		/// </summary>
		public string ExportPath { get; set; }
	}
}
