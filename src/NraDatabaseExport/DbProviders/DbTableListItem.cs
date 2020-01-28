using System;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a single item in the result of listing tables in a database.
	/// </summary>
	public class DbTableListItem
	{
		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the name of the owner (schema) of the table.
		/// </summary>
		public string OwnerName { get; }

		/// <summary>
		/// Initialize a new instance of the <see cref="DbTableListItem"/> class with a specified name.
		/// </summary>
		/// <param name="name">the name of the table</param>
		/// <param name="ownerName">the of the owner (schema) of the table</param>
		public DbTableListItem(string name, string ownerName = null)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			OwnerName = ownerName;
		}
	}
}
