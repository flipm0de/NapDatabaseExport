using System.Threading.Tasks;
using System.Windows.Input;

namespace NraDatabaseExport.App.Infrastructure
{
	/// <summary>
	/// Provides a mechanism for executing commands asynchronously.
	/// </summary>
	public interface IAsyncCommand : ICommand
	{
		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">the data used by the command</param>
		/// <returns>the command execution task</returns>
		Task ExecuteAsync(object parameter);
	}
}
