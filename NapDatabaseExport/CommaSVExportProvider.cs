namespace NapDatabaseExport
{
    public class CommaSVExportProvider : SVExportProviderBase
    {
        public override string ExportType
        {
            get { return "CSV (Comma delimited)"; }
        }

        public override string DefaultFileExtension
        {
            get { return ".csv"; }
        }

        protected override string Delimiter
        {
            get { return ","; }
        }
    }
}
