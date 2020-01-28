using System;
using NraDatabaseExport.App.Infrastructure;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents the view model for a database.
	/// </summary>
	public class DbDatabaseViewModel : ViewModelBase
	{
		private bool _isSelected;

		/// <summary>
		/// Gets the name of the database.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the flag indicating whether the item is selected or not.
		/// </summary>
		public bool IsSelected
		{
			get => _isSelected;
			set => SetProperty(ref _isSelected, value);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="DbDatabaseViewModel"/> class with a specified name.
		/// </summary>
		/// <param name="name">the name of the database</param>
		public DbDatabaseViewModel(string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}
	}
}
