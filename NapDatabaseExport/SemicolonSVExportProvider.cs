namespace NapDatabaseExport
{
    public class SemicolonSVExportProvider : SVExportProviderBase
    {
        public override string ExportType
        {
            get { return "CSV (Semicolon delimited)"; }
        }

        public override string DefaultFileExtension
        {
            get { return ".csv"; }
        }

        protected override string Delimiter
        {
            get { return ";"; }
        }
    }
}
