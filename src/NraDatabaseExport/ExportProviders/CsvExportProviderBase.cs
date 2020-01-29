#if SUPPORT_CSV
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Provides a base class for export providers writing to a CSV file.
	/// </summary>
	public abstract class CsvExportProviderBase : ExportProviderBase
	{
		private StreamWriter _writer;
		private CsvWriter _csvWriter;

		/// <summary>
		/// Gets the string that delimits values on a row
		/// </summary>
		protected virtual string Delimiter { get; }

		/// <summary>
		/// Gets the quotation character.
		/// </summary>
		protected virtual char? Quote { get; }

		#region ExportProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask OpenWriteAsync(string filePath, CancellationToken cancellationToken = default)
		{
			StreamWriter writer = null;
			CsvWriter csvWriter = null;

			try
			{
				writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

				var csvConfiguration = new CsvConfiguration(Culture);

				if (!string.IsNullOrWhiteSpace(Delimiter))
				{
					csvConfiguration.Delimiter = Delimiter;
				}

				if (!(Quote is null))
				{
					csvConfiguration.Quote = Quote.Value;
				}

				csvWriter = new CsvWriter(writer, csvConfiguration);
			}
			catch
			{
				csvWriter?.Dispose();

				if (!(writer is null))
				{
					await writer.DisposeAsync();
				}

				throw;
			}

			_writer = writer;
			_csvWriter = csvWriter;
		}

		/// <inheritdoc/>
		public override async ValueTask WriteHeaderRowAsync(string[] columns, CancellationToken cancellationToken = default)
		{
			if (columns is null)
			{
				throw new ArgumentNullException(nameof(columns));
			}

			foreach (string column in columns)
			{
				_csvWriter.WriteField(column);
			}

			await _csvWriter.NextRecordAsync();
		}

		/// <inheritdoc/>
		public override async ValueTask WriteDataRowAsync(object[] values, CancellationToken cancellationToken = default)
		{
			if (values is null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			foreach (object value in values)
			{
				object valueToWrite;

				switch (value)
				{
					case byte[] bytes:
						{
							valueToWrite = "0x" + BitConverter.ToString(bytes).Replace("-", "");
							break;
						}
					default:
						{
							valueToWrite = value;
							break;
						}
				}

				_csvWriter.WriteField(valueToWrite);
			}

			await _csvWriter.NextRecordAsync();
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_csvWriter?.Flush();
				_csvWriter?.Dispose();
				_csvWriter = null;

				_writer?.Dispose();
				_writer = null;
			}

			base.Dispose(disposing);
		}

		#endregion
	}
}
#endif
