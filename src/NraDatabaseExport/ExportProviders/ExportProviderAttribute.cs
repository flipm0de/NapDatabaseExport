using System;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents a custom attribute that describes an export provider.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExportProviderAttribute : Attribute
	{
		/// <summary>
		/// Gets the database provider type.
		/// </summary>
		public ExportProviderType Type { get; }

		/// <summary>
		/// Gets the display name.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Gets the default file extension.
		/// </summary>
		/// <remarks>
		/// The extension should be prefixed with a dot as it is appended to the file name.
		/// </remarks>
		public string DefaultExtension { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportProviderAttribute"/> class with a specified display name.
		/// </summary>
		/// <param name="type">the database provider type</param>
		/// <param name="displayName">the display name</param>
		public ExportProviderAttribute(ExportProviderType type, string displayName)
		{
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
