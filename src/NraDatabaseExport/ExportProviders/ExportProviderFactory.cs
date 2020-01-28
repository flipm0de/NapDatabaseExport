using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents a factory of export providers.
	/// </summary>
	public static class ExportProviderFactory
	{
		/// <summary>
		/// Gets the list of the available database providers.
		/// </summary>
		/// <returns>the list of available database providers</returns>
		public static ExportProviderListItem[] ListProviders()
		{
			IEnumerable<ExportProviderType> dbProviderTypes = Enum.GetValues(typeof(ExportProviderType)).Cast<ExportProviderType>();

			ExportProviderListItem[] listItems = dbProviderTypes.Select(x => CreateListItem(x)).ToArray();

			return listItems;
		}

		/// <summary>
		/// Creates an instance of a specified database provider.
		/// </summary>
		/// <param name="providerType">the database provider type</param>
		/// <returns>the database provider instance</returns>
		public static IExportProvider CreateProvider(ExportProviderType providerType)
		{
			Type type = GetProviderType(providerType);

			var provider = (IExportProvider)Activator.CreateInstance(type);

			return provider;
		}

		private static ExportProviderListItem CreateListItem(ExportProviderType providerType)
		{
			Type type = GetProviderType(providerType);

			ExportProviderAttribute attribute = type.GetCustomAttribute<ExportProviderAttribute>();

			if (attribute is null)
			{
				throw new ExportProviderException($"Export provider type `{type}` is missing a `{typeof(ExportProviderAttribute)}` attribute.");
			}

			var listItem = new ExportProviderListItem(providerType, type, attribute.DisplayName);

			return listItem;
		}

		private static Type GetProviderType(ExportProviderType type)
		{
			switch (type)
			{
#if SUPPORT_CSV
				case ExportProviderType.Csv:
					{
						return typeof(CsvExportProvider);
					}
				case ExportProviderType.CommaCsv:
					{
						return typeof(CommaCsvExportProvider);
					}
				case ExportProviderType.SemicolonCsv:
					{
						return typeof(SemicolonCsvExportProvider);
					}
#endif
#if SUPPORT_JSON
				case ExportProviderType.Json:
					{
						return typeof(JsonExportProvider);
					}
#endif
				default:
					{
						throw new NotSupportedException($"Export provider `{type}` is not supported in this context.");
					}
			};
		}
	}
}
