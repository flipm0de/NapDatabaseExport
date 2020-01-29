using System;
using NraDatabaseExport.App.Infrastructure;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents the view model for a database table.
	/// </summary>
	public class DbTableViewModel : ViewModelBase
	{
		private bool _isSelected;
		private DbTableExportStatus? _exportStatus;
		private int exportedRowsCount;

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the name of the owner (schema) of the table.
		/// </summary>
		public string? OwnerName { get; }

		/// <summary>
		/// Gets the display name.
		/// </summary>
		public string DisplayName
			=> OwnerName is null
				? Name
				: $"{OwnerName}.{Name}";

		/// <summary>
		/// Gets or sets the flag indicating whether the item is selected or not.
		/// </summary>
		public bool IsSelected
		{
			get => _isSelected;
			set => SetProperty(ref _isSelected, value);
		}

		/// <summary>
		/// Gets or sets the status of the export.
		/// </summary>
		public DbTableExportStatus? ExportStatus
		{
			get => _exportStatus;
			set => SetProperty(ref _exportStatus, value);
		}

		/// <summary>
		/// Gets or sets the number of exported rows.
		/// </summary>
		public int ExportedRowsCount
		{
			get => exportedRowsCount;
			set => SetProperty(ref exportedRowsCount, value);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="DbTableViewModel"/> class with a specified name.
		/// </summary>
		/// <param name="name">the name of the table</param>
		/// <param name="ownerName">the of the owner (schema) of the table</param>
		public DbTableViewModel(string name, string? ownerName = null)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			OwnerName = ownerName;
		}
	}
}
