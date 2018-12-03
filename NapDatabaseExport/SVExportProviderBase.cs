using System;
using System.IO;
using System.Linq;

namespace NapDatabaseExport
{
    public abstract class SVExportProviderBase : ExportProvider
    {
        private StreamWriter writer;

        protected abstract string Delimiter { get; }

        public override void StartExport (string filePath)
        {
            writer = File.CreateText (filePath);
        }

        public override void WriteColumnNames (string [] columns)
        {
            writer.WriteLine (string.Join (Delimiter, columns));
        }

        public override void WriteRow (object [] values)
        {
            writer.WriteLine (string.Join (Delimiter,
                values.Select (v => v != null && v != DBNull.Value ? GetCellValue (v.ToString ()) : string.Empty)));
        }

        private string GetCellValue (string data)
        {
            var useQuotes = false;

            if (data.Contains ("\"")) {
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