using System;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a single item in the result of listing available database providers.
	/// </summary>
	public class DbProviderListItem
	{
		/// <summary>
		/// Gets the type of the database provider.
		/// </summary>
		public DbProviderType ProviderType { get; }

		/// <summary>
		/// Gets the .NET type of the database provider.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Gets the display name of the database provider.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Initialize a new instance of the <see cref="DbProviderListItem"/> class with a specified name.
		/// </summary>
		/// <param name="providerType">the type of the database provider</param>
		/// <param name="type">the .NET type of the database provider</param>
		/// <param name="displayName">the display name of the database provider</param>
		public DbProviderListItem(DbProviderType providerType, Type type, string displayName)
		{
			ProviderType = providerType;
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
