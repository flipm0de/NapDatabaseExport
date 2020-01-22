using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NraDatabaseExport.ExportProviders
{
	public abstract class CsvExportProviderBase : ExportProviderBase
	{
		private StreamWriter _writer;

		protected abstract string Delimiter { get; }

		#region ExportProvider Members

		/// <inheritdoc/>
		public override void StartExport(string filePath)
		{
			_writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
		}

		/// <inheritdoc/>
		public override void WriteColumnNames(string[] columns)
		{
			if (columns is null)
			{
				throw new ArgumentNullException(nameof(columns));
			}

			_writer.WriteLine(string.Join(Delimiter, columns));
		}

		/// <inheritdoc/>
		public override void WriteRow(object[] values)
		{
			if (values is null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			_writer.WriteLine(
				string.Join(Delimiter,
					values.Select(v => v is null || v is DBNull
						? string.Empty
						: GetCellValue(
							v is byte[] bytes
								? "0x" + BitConverter.ToString(bytes).Replace("-", "")
								: v.ToString()
							)
					)
				)
			);
		}

		/// <inheritdoc/>
		public override void FinishExport()
		{
			_writer.Flush();
			_writer.Dispose();
			_writer = null;
		}

		#endregion

		private string GetCellValue(string data)
		{
			var useQuotes = false;

			if (data.Contains("\"") || data.Contains("\n") || data.Contains("\r"))
			{
				data = data.Replace("\"", "\"\"");
				useQuotes = true;
			}

			if (data.Contains(Delimiter))
			{
				useQuotes = true;
			}

			return useQuotes
				? $"\"{data}\""
				: data;
		}
	}
}
