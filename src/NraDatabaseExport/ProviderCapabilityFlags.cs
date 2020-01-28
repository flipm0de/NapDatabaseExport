using System;

namespace NraDatabaseExport
{
	/// <summary>
	/// Represents an enumeration of provider capability flags.
	/// </summary>
	[Flags]
	public enum ProviderCapabilityFlags
	{
		/// <summary>
		/// Not supported
		/// </summary>
		NotSupported = 0,

		/// <summary>
		/// Supported
		/// </summary>
		Supported = 1,

		/// <summary>
		/// Required
		/// </summary>
		Required = 2,

		/// <summary>
		/// Supported and required
		/// </summary>
		SupportedAndRequired = Supported | Required,
	}
}
