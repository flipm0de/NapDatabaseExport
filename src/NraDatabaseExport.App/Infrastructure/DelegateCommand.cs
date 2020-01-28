using System;
using System.Windows.Input;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a command that executes a delegate.
	/// </summary>
	public class DelegateCommand : ICommand
	{
		private readonly Action<object> _executeAction;
		private readonly Func<object, bool> _canExecuteAction;
		private bool _isExecuting;
		private DelegateCommand goBackToSelectDatabaseCommand;

		/// <summary>
		/// Gets the flag indicating whether the delegate is executing or not.
		/// </summary>
		public bool IsExecuting
		{
			get => _isExecuting;
			private set
			{
				_isExecuting = value;

				NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateCommand"/> class with a specified delegate to execute.
		/// </summary>
		/// <param name="executeAction">the delegate to use to execute the command</param>
		/// <param name="canExecuteAction">the delegate to use to check if the command can be executed</param>
		public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction = null)
		{
			_executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
			_canExecuteAction = canExecuteAction;
		}

		public DelegateCommand(DelegateCommand goBackToSelectDatabaseCommand)
		{
			this.goBackToSelectDatabaseCommand = goBackToSelectDatabaseCommand;
		}

		#region ICommand Members

		/// <inheritdoc/>
		public event EventHandler CanExecuteChanged;

		/// <inheritdoc/>
		public bool CanExecute(object parameter)
		{
			if (_isExecuting)
			{
				return false;
			}

			return _canExecuteAction?.Invoke(parameter) ?? true;
		}

		/// <inheritdoc/>
		public void Execute(object parameter)
		{
			_isExecuting = true;

			try
			{
				_executeAction.Invoke(parameter);
			}
			finally
			{
				_isExecuting = false;
			}
		}

		#endregion

		/// <summary>
		/// Sends a notification that the permission to execute the command has changed.
		/// </summary>
		public void NotifyCanExecuteChanged()
			=> CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}
