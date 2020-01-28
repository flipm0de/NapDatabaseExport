using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		/// <inheritdoc/>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Sets the property stored in a specified backing field to a specified value.
		/// </summary>
		/// <typeparam name="T">the type of the property</typeparam>
		/// <param name="field">the reference to the backing field</param>
		/// <param name="newValue">the new value</param>
		/// <param name="propertyName">the name of the property</param>
		/// <returns><see langword="true"/>, if the value is changed; otherwise - <see langword="false"/></returns>
		protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return false;
			}

			field = newValue;

			NotifyPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Sends a notification that a specified property has changed.
		/// </summary>
		/// <param name="propertyName">the name of the property</param>
		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName is null)
			{
				throw new ArgumentNullException(nameof(propertyName));
			}

			PropertyChangedEventHandler handler = PropertyChanged;

			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
