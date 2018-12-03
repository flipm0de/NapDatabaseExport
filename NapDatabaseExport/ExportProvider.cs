namespace NapDatabaseExport
{
    public abstract class ExportProvider
    {
        public abstract string ExportType { get; }

        public abstract string DefaultFileExtension { get; }

        public abstract void StartExport (string filePath);

        public abstract void WriteColumnNames (string [] columns);

        public abstract void WriteRow (object [] values);

        public abstract void FinishExport ();

        public static ExportProvider [] GetAll ()
        {
            return new ExportProvider []
                {
                    new SemicolonSVExportProvider (),
                    new CommaSVExportProvider ()
                };
        }
    }
}
