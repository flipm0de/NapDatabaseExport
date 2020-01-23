using System;
using System.Windows.Forms;

namespace NraDatabaseExport
{
	/// <summary>
	/// Represents the entry class for the application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// This is the main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
