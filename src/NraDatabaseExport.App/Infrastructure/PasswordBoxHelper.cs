using System.Windows;
using System.Windows.Controls;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Contains helper methods for the <see cref="PasswordBox"/> control.
	/// </summary>
	public static class PasswordBoxHelper
	{
		public static readonly DependencyProperty BoundPassword
			= DependencyProperty.RegisterAttached(nameof(BoundPassword), typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

		public static readonly DependencyProperty BindPassword
			= DependencyProperty.RegisterAttached(nameof(BindPassword), typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnBindPasswordChanged));

		private static readonly DependencyProperty UpdatingPassword
			= DependencyProperty.RegisterAttached(nameof(UpdatingPassword), typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false));

		private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is PasswordBox passwordBox))
			{
				return;
			}

			// only handle this event when the property is attached to a PasswordBox
			// and when the BindPassword attached property has been set to true
			if (!GetBindPassword(passwordBox))
			{
				return;
			}

			// avoid recursive updating by ignoring the box's changed event
			passwordBox.PasswordChanged -= HandlePasswordChanged;

			string newPassword = (string)e.NewValue;

			if (!GetUpdatingPassword(passwordBox))
			{
				passwordBox.Password = newPassword;
			}

			passwordBox.PasswordChanged += HandlePasswordChanged;
		}

		private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			// when the BindPassword attached property is set on a PasswordBox,
			// start listening to its PasswordChanged event

			if (!(dp is PasswordBox passwordBox))
			{
				return;
			}

			bool wasBound = (bool)(e.OldValue);
			bool needToBind = (bool)(e.NewValue);

			if (wasBound)
			{
				passwordBox.PasswordChanged -= HandlePasswordChanged;
			}

			if (needToBind)
			{
				passwordBox.PasswordChanged += HandlePasswordChanged;
			}
		}

		private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			var box = (PasswordBox)sender;

			// set a flag to indicate that we're updating the password
			SetUpdatingPassword(box, true);

			// push the new password into the BoundPassword property
			SetBoundPassword(box, box.Password);

			SetUpdatingPassword(box, false);
		}

		public static bool GetBindPassword(DependencyObject dp)
		{
			return (bool)dp.GetValue(BindPassword);
		}

		public static void SetBindPassword(DependencyObject dp, bool value)
		{
			dp.SetValue(BindPassword, value);
		}

		public static string GetBoundPassword(DependencyObject dp)
		{
			return (string)dp.GetValue(BoundPassword);
		}

		public static void SetBoundPassword(DependencyObject dp, string value)
		{
			dp.SetValue(BoundPassword, value);
		}

		private static bool GetUpdatingPassword(DependencyObject dp)
		{
			return (bool)dp.GetValue(UpdatingPassword);
		}

		private static void SetUpdatingPassword(DependencyObject dp, bool value)
		{
			dp.SetValue(UpdatingPassword, value);
		}
	}
}
