using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a factory of database providers.
	/// </summary>
	public static class DbProviderFactory
	{
		/// <summary>
		/// Gets the list of the available database providers.
		/// </summary>
		/// <returns>the list of available database providers</returns>
		public static DbProviderListItem[] ListProviders()
		{
			IEnumerable<DbProviderType> dbProviderTypes = Enum.GetValues(typeof(DbProviderType)).Cast<DbProviderType>();

			DbProviderListItem[] listItems = dbProviderTypes.Select(x => CreateListItem(x)).ToArray();

			return listItems;
		}

		/// <summary>
		/// Creates an instance of a specified database provider.
		/// </summary>
		/// <param name="type">the database provider type</param>
		/// <returns>the database provider instance</returns>
		public static IDbProvider CreateProvider(DbProviderType type)
		{
			Type providerType = GetProviderType(type);

			var provider = (IDbProvider)Activator.CreateInstance(providerType);

			return provider;
		}

		private static DbProviderListItem CreateListItem(DbProviderType providerType)
		{
			Type type = GetProviderType(providerType);

			DbProviderAttribute attribute = type.GetCustomAttribute<DbProviderAttribute>();

			if (attribute is null)
			{
				throw new DbProviderException($"Database provider type `{type}` is missing a `{typeof(DbProviderAttribute)}` attribute.");
			}

			var listItem = new DbProviderListItem(providerType, type, attribute.DisplayName);

			return listItem;
		}

		private static Type GetProviderType(DbProviderType type)
		{
			switch (type)
			{
#if SUPPORT_MYSQL
				case DbProviderType.MySql:
					{
						return typeof(MySqlDbProvider);
					}
#endif
#if SUPPORT_SQLITE
				case DbProviderType.Sqlite:
					{
						return typeof(SqliteDbProvider);
					}
#endif
#if SUPPORT_SQLSERVER
				case DbProviderType.SqlServer:
					{
						return typeof(SqlServerDbProvider);
					}
#endif
#if SUPPORT_MSACCESS
				case DbProviderType.MSAccess:
					{
						return typeof(MSAccessDbProvider);
					}
#endif
#if SUPPORT_ODBC
				case DbProviderType.Odbc:
					{
						return typeof(OdbcDbProvider);
					}
#endif
#if SUPPORT_FIREBIRD
				case DbProviderType.Firebird:
					{
						return typeof(FirebirdDbProvider);
					}
#endif
				default:
					{
						throw new NotSupportedException($"Database provider `{type}` is not supported in this context.");
					}
			};
		}
	}
}
