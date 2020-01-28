namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents an enumeration of the export statuses of a database table.
	/// </summary>
	public enum DbTableExportStatus
	{
		/// <summary>
		/// Not applicable
		/// </summary>
		/// <remarks>
		/// The table has not been selected for import.
		/// </remarks>
		NotApplicable,

		/// <summary>
		/// Busy
		/// </summary>
		/// <remarks>
		/// The table is being exported.
		/// </remarks>
		Busy,

		/// <summary>
		/// OK
		/// </summary>
		/// <remarks>
		/// The table has been exported successfully.
		/// </remarks>
		Ok,

		/// <summary>
		/// Error
		/// </summary>
		/// <remarks>
		/// An error has occurred during table export.
		/// </remarks>
		Error,
	}
}
