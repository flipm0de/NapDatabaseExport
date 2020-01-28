using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace NraDatabaseExport.App.Slides
{
	/// <summary>
	/// Represents a user control for configuring database.
	/// </summary>
	public partial class ConfigureDatabaseSlide : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigureDatabaseSlide"/> class.
		/// </summary>
		public ConfigureDatabaseSlide()
		{
			InitializeComponent();
		}

		private void Port_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("\\d+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
