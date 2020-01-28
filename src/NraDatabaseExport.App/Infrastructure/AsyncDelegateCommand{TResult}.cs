using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Represents a command that executes a delegate asynchronously.
	/// </summary>
	public class AsyncDelegateCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
	{
		private readonly Func<object, CancellationToken, Task<TResult>> _executeAction;
		private readonly Func<object, bool> _canExecuteAction;
		private readonly CancelAsyncCommand _cancelCommand;
		private NotifyTaskCompletion<TResult> _execution;

		/// <summary>
		/// Gets the cancellation command.
		/// </summary>
		public ICommand CancelCommand
			=> _cancelCommand;

		#region INotifyPropertyChanged Members

		/// <inheritdoc/>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region AsyncCommandBase Members

		/// <inheritdoc/>
		public override bool CanExecute(object parameter)
		{
			if (_execution?.IsCompleted ?? true)
			{
				if (_canExecuteAction is null)
				{
					return true;
				}

				return _canExecuteAction.Invoke(parameter);
			}

			return false;
		}

		/// <inheritdoc/>
		public override async Task ExecuteAsync(object parameter)
		{
			_cancelCommand.NotifyCommandStarting();

			Task<TResult> commandResult = _executeAction.Invoke(parameter, _cancelCommand.Token);

			_execution = new NotifyTaskCompletion<TResult>(commandResult);

			NotifyCanExecuteChanged();

			await _execution.Task;

			_cancelCommand.NotifyCommandFinished();

			NotifyCanExecuteChanged();
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncDelegateCommand{TResult}"/> class with a specified asynchronous command.
		/// </summary>
		/// <param name="executeAction">the delegate to use to execute the command</param>
		/// <param name="canExecuteAction">the delegate to use to check if the command can be executed</param>
		public AsyncDelegateCommand(Func<object, CancellationToken, Task<TResult>> executeAction, Func<object, bool> canExecuteAction = null)
		{
			_executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
			_canExecuteAction = canExecuteAction;

			_cancelCommand = new CancelAsyncCommand();
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

		#region Nested Type: CancelAsyncCommand

		private class CancelAsyncCommand : ICommand
		{
			private CancellationTokenSource _cts = new CancellationTokenSource();

			private bool _commandExecuting;

			public CancellationToken Token
				=> _cts.Token;

			public void NotifyCommandStarting()
			{
				_commandExecuting = true;

				if (!_cts.IsCancellationRequested)
				{
					return;
				}

				_cts = new CancellationTokenSource();

				NotifyCanExecuteChanged();
			}

			public void NotifyCommandFinished()
			{
				_commandExecuting = false;
				NotifyCanExecuteChanged();
			}

			#region ICommand Members

			/// <inheritdoc/>
			public event EventHandler CanExecuteChanged
			{
				add { CommandManager.RequerySuggested += value; }
				remove { CommandManager.RequerySuggested -= value; }
			}

			/// <inheritdoc/>
			public bool CanExecute(object parameter)
				=> _commandExecuting && !_cts.IsCancellationRequested;

			/// <inheritdoc/>
			public void Execute(object parameter)
			{
				_cts.Cancel();

				NotifyCanExecuteChanged();
			}

			#endregion

			/// <summary>
			/// Sends a notification that the permission to execute the command has changed.
			/// </summary>
			protected void NotifyCanExecuteChanged()
			{
				CommandManager.InvalidateRequerySuggested();
			}
		}

		#endregion
	}
}
