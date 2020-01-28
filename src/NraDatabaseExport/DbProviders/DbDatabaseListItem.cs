using System;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a single item in the result of listing database available in a connection.
	/// </summary>
	public class DbDatabaseListItem
	{
		/// <summary>
		/// Gets the name of the database.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initialize a new instance of the <see cref="DbDatabaseListItem"/> class with a specified name.
		/// </summary>
		/// <param name="name">the name of the database</param>
		public DbDatabaseListItem(string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}
	}
}
