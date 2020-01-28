using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NraDatabaseExport.App.Infrastructure
{
	public class NotifyTaskCompletion : INotifyPropertyChanged
	{
		public Task Task { get; private set; }

		public TaskStatus Status
			=> Task.Status;

		public bool IsCompleted
			=> Task.IsCompleted;

		public bool IsNotCompleted
			=> !Task.IsCompleted;

		public bool IsSuccessfullyCompleted
			=> Task.Status == TaskStatus.RanToCompletion;

		public bool IsCancelled
			=> Task.IsCanceled;

		public bool IsFaulted
			=> Task.IsFaulted;

		public AggregateException Exception
			=> Task.Exception;

		public Exception InnerException
			=> Exception?.InnerException;

		public string ErrorMessage
			=> InnerException?.Message;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public NotifyTaskCompletion(Task task)
		{
			Task = task;

			if (!task.IsCompleted)
			{
				Task _ = WatchTaskAsync(task);
			}
		}

		private async Task WatchTaskAsync(Task task)
		{
			try
			{
				await task;
			}
			catch { }

			NotifyPropertyChanged(nameof(Status));
			NotifyPropertyChanged(nameof(IsCompleted));
			NotifyPropertyChanged(nameof(IsNotCompleted));

			if (task.IsCanceled)
			{
				NotifyPropertyChanged(nameof(IsCancelled));
			}
			else if (task.IsFaulted)
			{
				NotifyPropertyChanged(nameof(IsFaulted));
				NotifyPropertyChanged(nameof(Exception));
				NotifyPropertyChanged(nameof(InnerException));
				NotifyPropertyChanged(nameof(ErrorMessage));
			}
			else
			{
				NotifyPropertyChanged(nameof(IsSuccessfullyCompleted));
			}
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
