using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NapDatabaseExport
{
    public abstract class SVExportProviderBase : ExportProvider
    {
        private StreamWriter writer;

        protected abstract string Delimiter { get; }

        public override void StartExport (string filePath)
        {
            writer = new StreamWriter (filePath, false, new UTF8Encoding (true));
        }

        public override void WriteColumnNames (string [] columns)
        {
            writer.WriteLine (string.Join (Delimiter, columns));
        }

        public override void WriteRow (object [] values)
        {
            writer.WriteLine (string.Join (Delimiter,
                values.Select (v => v != null && v != DBNull.Value
                                ? GetCellValue (v is byte[] ? "0x" + BitConverter.ToString((byte[]) v).Replace("-", "") : v.ToString ())
                                : string.Empty)));
        }

        private string GetCellValue (string data)
        {
            var useQuotes = false;
            if (data.Contains ("\"") || data.Contains("\n") || data.Contains("\r")) {
                data = data.Replace ("\"", "\"\"");
                useQuotes = true;
            }

            if (data.Contains (Delimiter))
                useQuotes = true;

            return useQuotes ? string.Format ("\"{0}\"", data) : data;
        }

        public override void FinishExport ()
        {
            writer.Flush ();
            writer.Dispose ();
            writer = null;
        }
    }
}