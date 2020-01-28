using System;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents a single item in the result of listing available export providers.
	/// </summary>
	public class ExportProviderListItem
	{
		/// <summary>
		/// Gets the type of the export provider.
		/// </summary>
		public ExportProviderType ProviderType { get; }

		/// <summary>
		/// Gets the .NET type of the datbase provider.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Gets the display name of the database provider.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Initialize a new instance of the <see cref="ExportProviderListItem"/> class with a specified name.
		/// </summary>
		/// <param name="providerType">the type of the database provider</param>
		/// <param name="type">the .NET type of the export provider</param>
		/// <param name="displayName">the display name of the database provider</param>
		public ExportProviderListItem(ExportProviderType providerType, Type type, string displayName)
		{
			ProviderType = providerType;
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
