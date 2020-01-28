namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Provides an enumeration of database providers.
	/// </summary>
	public enum DbProviderType
	{
#if SUPPORT_MYSQL
		/// <summary>
		/// MySQL/MariaDB
		/// </summary>
		MySql,
#endif

#if SUPPORT_SQLITE
		/// <summary>
		/// SQLite
		/// </summary>
		Sqlite,
#endif

#if SUPPORT_SQLSERVER
		/// <summary>
		/// Microsoft SQL Server
		/// </summary>
		SqlServer,
#endif

#if SUPPORT_MSACCESS
		/// <summary>
		/// Microsoft Access
		/// </summary>
		MSAccess,
#endif

#if SUPPORT_ODBC
		/// <summary>
		/// ODBC
		/// </summary>
		Odbc,
#endif

#if SUPPORT_FIREBIRD
		/// <summary>
		/// Firebird Server
		/// </summary>
		Firebird,
#endif
	}
}
