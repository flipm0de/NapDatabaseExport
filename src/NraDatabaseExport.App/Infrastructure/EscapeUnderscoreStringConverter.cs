using System;
using System.Globalization;
using System.Windows.Data;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a converter that escapes underscores in a <see cref="string"/> value.
	/// </summary>
	[ValueConversion(typeof(string), typeof(object))]
	public class EscapeUnderscoreStringConverter : IValueConverter
	{
		#region IValueConverter Members

		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null)
			{
				return null;
			}

			if (value is string s)
			{
				return s.Replace("_", "__");
			}

			throw new InvalidOperationException($"The value is must be string but `{value?.GetType()}` provided instead.");
		}

		/// <inheritdoc/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
