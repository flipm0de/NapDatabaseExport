using System;
using NraDatabaseExport.App.Infrastructure;
using NraDatabaseExport.ExportProviders;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents the view model for an export provider.
	/// </summary>
	public class ExportProviderViewModel : ViewModelBase
	{
		private string _defaultExtension;

		/// <summary>
		/// Gets the type of the export provider.
		/// </summary>
		public ExportProviderType Type { get; }

		/// <summary>
		/// Gets the display name of the database provider.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Gets or sets the default file extension.
		/// </summary>
		/// <remarks>
		/// The extension should be prefixed with a dot as it is appended to the file name.
		/// </remarks>
		public string DefaultExtension
		{
			get => _defaultExtension;
			set => SetProperty(ref _defaultExtension, value);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="ExportProviderViewModel"/> class with a specified name.
		/// </summary>
		/// <param name="type">the type of the database provider</param>
		/// <param name="displayName">the display name of the database provider</param>
		public ExportProviderViewModel(ExportProviderType type, string displayName)
		{
			Type = type;
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
		}
	}
}
