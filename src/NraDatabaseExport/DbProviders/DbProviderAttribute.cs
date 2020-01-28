using System;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a custom attribute that describes a database provider.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DbProviderAttribute : Attribute
	{
		/// <summary>
		/// Gets the database provider type.
		/// </summary>
		public DbProviderType Type { get; }

		/// <summary>
		/// Gets the display name.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Gets or sets the capability for working with a database file.
		/// </summary>
		public ProviderCapabilityFlags FileCapability { get; set; }

		/// <summary>
		/// Gets or sets the capability for working with a database server.
		/// </summary>
		public ProviderCapabilityFlags ServerCapability { get; set; }

		/// <summary>
		/// Gets or sets the capability for specifying a database server port.
		/// </summary>
		public ProviderCapabilityFlags PortCapability { get; set; }

		/// <summary>
		/// Gets or sets the capability for specifying a user name.
		/// </summary>
		public ProviderCapabilityFlags UserNameCapability { get; set; }

		/// <summary>
		/// Gets or sets the capability for specifying a password.
		/// </summary>
		public ProviderCapabilityFlags PasswordCapability { get; set; }

		/// <summary>
		/// Gets or sets the capability for specifying a database.
		/// </summary>
		public ProviderCapabilityFlags DatabaseCapability { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DbProviderAttribute"/> class with a specified display name.
		/// </summary>
		/// <param name="type">the database provider type</param>
		/// <param name="displayName">the display name</param>
		public DbProviderAttribute(DbProviderType type, string displayName)
		{
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
