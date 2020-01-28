using System;
using System.Globalization;
using System.Windows.Data;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a converter that inverts <see cref="bool"/> value.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		#region IValueConverter Members

		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(bool))
			{
				throw new InvalidOperationException($"The target type must be a boolean but `{targetType}` provided instead.");
			}

			if (value is null)
			{
				return null;
			}

			if (value is bool b)
			{
				return !b;
			}

			throw new InvalidOperationException($"The value is must be boolean but `{value?.GetType()}` provided instead.");
		}

		/// <inheritdoc/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
