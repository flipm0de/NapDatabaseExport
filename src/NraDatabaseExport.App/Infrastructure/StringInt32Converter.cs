using System;
using System.Globalization;
using System.Windows.Data;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a converter between <see cref="string"/> and <see cref="int"/> values.
	/// </summary>
	[ValueConversion(typeof(string), typeof(int))]
	public class StringInt32Converter : IValueConverter
	{
		#region IValueConverter Members

		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null)
			{
				return null;
			}

			if (targetType == typeof(string))
			{
				try
				{
					return System.Convert.ToString(value, culture);
				}
				catch
				{
					// TODO:
					return null;
				}
			}

			// TODO:
			return null;
		}

		/// <inheritdoc/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null)
			{
				return null;
			}

			if (targetType == typeof(int) || targetType == typeof(int?))
			{
				try
				{
					return System.Convert.ToInt32(value, culture);
				}
				catch
				{
					// TODO:
					return null;
				}
			}

			// TODO:
			return null;
		}

		#endregion
	}
}
