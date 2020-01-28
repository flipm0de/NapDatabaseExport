using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a base class for asynchronous commands.
	/// </summary>
	public abstract class AsyncCommandBase : IAsyncCommand
	{
		#region ICommand Members

		/// <inheritdoc/>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		/// <inheritdoc/>
		public abstract bool CanExecute(object parameter);

		/// <inheritdoc/>
		public async void Execute(object parameter)
		{
			await ExecuteAsync(parameter);
		}

		#endregion

		#region IAsyncCommand Members

		/// <inheritdoc/>
		public abstract Task ExecuteAsync(object parameter);

		#endregion

		/// <summary>
		/// Sends a notification that the permission to execute the command has changed.
		/// </summary>
		public void NotifyCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}
