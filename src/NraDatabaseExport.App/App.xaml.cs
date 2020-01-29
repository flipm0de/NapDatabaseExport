using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using NraDatabaseExport.App.Options;
using NraDatabaseExport.App.ViewModels;

namespace NraDatabaseExport.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IConfigurationRoot _configuration;

		/// <inheritdoc/>
		protected override void OnStartup(StartupEventArgs e)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			_configuration = builder.Build();

			var serviceCollection = new ServiceCollection();

			ConfigureServices(serviceCollection);

			ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

			MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();

			mainWindow.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			IConfigurationSection exportOptionsSection = _configuration.GetSection("Export");

			services.Configure<ExportOptions>(exportOptionsSection);

			services.AddTransient<IDialogService, DialogService>();

			services.AddTransient<AppViewModel>();

			services.AddTransient<MainWindow>();
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(
				$"An unhandled exception has occurred: {e.Exception.Message}",
				"Error",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			e.Handled = true;
		}
	}
}
