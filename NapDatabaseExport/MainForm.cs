using System;
using System.IO;
using System.Windows.Forms;

namespace NapDatabaseExport
{
    public partial class MainForm : Form
    {
        public MainForm ()
        {
            InitializeComponent ();

            var databaseProviders = DatabaseProvider.GetAll ();
            cboDatabaseType.DisplayMember = nameof (DatabaseProvider.DatabaseTypeName);
            cboDatabaseType.DataSource = databaseProviders;

            var dataExporters = ExportProvider.GetAll ();
            cboExportType.DisplayMember = nameof (ExportProvider.ExportType);
            cboExportType.DataSource = dataExporters;

            txtExportFolder.Text = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);

            SetDatabaseEnabled (false);
            SetTablesEnabled (false);
            SetExportEnabled (false);
        }

        private void cboDatabaseType_SelectedIndexChanged (object sender, EventArgs e)
        {
            var currentProvider = (DatabaseProvider) cboDatabaseType.SelectedItem;

            lblDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;
            txtDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;
            btnDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;

            lblServer.Enabled = currentProvider.UsesServer;
            txtServer.Enabled = currentProvider.UsesServer;

            lblPort.Visible = currentProvider.UsesPort;
            nudPort.Visible = currentProvider.UsesPort;
            if (currentProvider.DefaultPort > 0)
                nudPort.Value = currentProvider.DefaultPort;

            lblUser.Enabled = currentProvider.UsesUser;
            txtUser.Enabled = currentProvider.UsesUser;

            lblPassword.Enabled = currentProvider.UsesPassword;
            txtPassword.Enabled = currentProvider.UsesPassword;

            SetDatabaseEnabled (currentProvider.UsesDatabase);

            if (currentProvider.UsesDatabaseFile)
                txtDatabaseFile.Focus ();
            else if (currentProvider.UsesServer)
                txtServer.Focus ();
            else if (currentProvider.UsesPort)
                nudPort.Focus ();
            else if (currentProvider.UsesUser)
                txtUser.Focus ();
            else if (currentProvider.UsesPassword)
                txtPassword.Focus ();
            else if (currentProvider.UsesDatabase)
                txtDatabaseFile.Focus ();
        }

        private void btnDatabaseFile_Click (object sender, EventArgs e)
        {
            ofdDatabaseFile.FileName = txtDatabaseFile.Text;
            if (ofdDatabaseFile.ShowDialog () != DialogResult.OK)
                return;

            txtDatabaseFile.Text = ofdDatabaseFile.FileName;
        }

        private void btnConnect_Click (object sender, EventArgs e)
        {
            SetDatabaseEnabled (false);
            SetTablesEnabled (false);
            SetExportEnabled (false);

            var currentProvider = (DatabaseProvider) cboDatabaseType.SelectedItem;

            if (currentProvider.UsesDatabaseFile) {
                if (string.IsNullOrWhiteSpace (txtDatabaseFile.Text) || !File.Exists (txtDatabaseFile.Text)) {
                    MessageBox.Show ("Моля, въведете коректен файл за база данни.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDatabaseFile.Focus ();
                    return;
                }
            }

            if (currentProvider.UsesServer) {
                if (string.IsNullOrWhiteSpace (txtServer.Text)) {
                    MessageBox.Show ("Моля, въведете сървър.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtServer.Focus ();
                    return;
                }
            }

            if (currentProvider.UsesPort) {
                if (nudPort.Value <= 0) {
                    MessageBox.Show ("Моля, въведете коректен порт.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    nudPort.Focus ();
                    return;
                }
            }

            if (currentProvider.UsesUser) {
                if (string.IsNullOrWhiteSpace (txtUser.Text)) {
                    MessageBox.Show ("Моля, въведете потребител.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUser.Focus ();
                    return;
                }
            }

            if (currentProvider.UsesPassword) {
                if (string.IsNullOrWhiteSpace (txtPassword.Text)) {
                    MessageBox.Show ("Моля, въведете парола.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus ();
                    return;
                }
            }

            currentProvider.DatabaseFile = txtDatabaseFile.Text;
            currentProvider.Server = txtServer.Text;
            currentProvider.Port = (uint) nudPort.Value;
            currentProvider.User = txtUser.Text;
            currentProvider.Password = txtPassword.Text;

            try {
                if (!currentProvider.TryConnect ()) {
                    MessageBox.Show ("Грешка при свързване със сървъра", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentProvider.UsesDatabase) {
                    cboDatabase.DataSource = currentProvider.GetDatabases ();
                    SetDatabaseEnabled (true);
                } else
                    LoadTables ();
            } catch (Exception ex) {
                MessageBox.Show ("Грешка при свързване със сървъра: " + ex, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cboDatabase_SelectedIndexChanged (object sender, EventArgs e)
        {
            SetTablesEnabled (false);
            SetExportEnabled (false);

            var currentProvider = (DatabaseProvider) cboDatabaseType.SelectedItem;

            currentProvider.Database = (string) cboDatabase.SelectedValue;

            LoadTables ();
        }

        private void LoadTables ()
        {
            var currentProvider = (DatabaseProvider) cboDatabaseType.SelectedItem;

            try {
                chlTables.DataSource = currentProvider.GetTables ();
                SetTablesEnabled (true);
            } catch (Exception ex) {
                MessageBox.Show ("Грешка при свързване със сървъра: " + ex, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool settingCheckBoxes;

        private void chkSelectAll_CheckedChanged (object sender, EventArgs e)
        {
            if (chkSelectAll.CheckState == CheckState.Indeterminate || settingCheckBoxes)
                return;

            try {
                settingCheckBoxes = true;
                for (int i = 0; i < chlTables.Items.Count; i++)
                    chlTables.SetItemChecked (i, chkSelectAll.Checked);
            } finally {
                settingCheckBoxes = false;
            }
        }

        private void chlTables_ItemCheck (object sender, ItemCheckEventArgs e)
        {
            SetExportEnabled (chlTables.CheckedItems.Count > 0 || e.NewValue == CheckState.Checked);

            if (settingCheckBoxes)
                return;

            try {
                settingCheckBoxes = true;
                var checkedItemsCount = chlTables.CheckedItems.Count;
                if ((checkedItemsCount == 0 || checkedItemsCount == 1) && e.NewValue == CheckState.Unchecked)
                    chkSelectAll.CheckState = CheckState.Unchecked;
                else if ((checkedItemsCount == chlTables.Items.Count || checkedItemsCount == chlTables.Items.Count - 1) && e.NewValue == CheckState.Checked)
                    chkSelectAll.CheckState = CheckState.Checked;
                else
                    chkSelectAll.CheckState = CheckState.Indeterminate;
            } finally {
                settingCheckBoxes = false;
            }
        }

        private void btnExportFolder_Click (object sender, EventArgs e)
        {
            fbdExportFolder.SelectedPath = txtExportFolder.Text;
            if (fbdExportFolder.ShowDialog () != DialogResult.OK)
                return;

            txtExportFolder.Text = fbdExportFolder.SelectedPath;
        }

        private void btnExport_Click (object sender, EventArgs e)
        {
            var exportFolder = txtExportFolder.Text;

            if (!Directory.Exists (exportFolder)) {
                MessageBox.Show ("Папка \"" + exportFolder + "\" не може да бъде открита.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var exportProvider = (ExportProvider) cboExportType.SelectedItem;
            var databaseProvider = (DatabaseProvider) cboDatabaseType.SelectedItem;

            try {
                foreach (string table in chlTables.CheckedItems) {
                    exportProvider.StartExport (Path.Combine (exportFolder, table + exportProvider.DefaultFileExtension));

                    using (var reader = databaseProvider.ExecuteReader ("SELECT * FROM " + table)) {
                        if (!reader.Read ())
                            continue;

                        var columns = new string [reader.FieldCount];
                        for (int j = 0; j < reader.FieldCount; j++)
                            columns [j] = reader.GetName (j);

                        exportProvider.WriteColumnNames (columns);

                        var row = new object [reader.FieldCount];
                        do {
                            for (int j = 0; j < reader.FieldCount; j++)
                                row [j] = reader.GetValue (j);

                            exportProvider.WriteRow (row);
                        } while (reader.Read ());
                    }

                    exportProvider.FinishExport ();
                }

                MessageBox.Show ("Експорта на данни приключи успешно", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show ("Грешка при експорт на данни: " + ex, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetDatabaseEnabled (bool enabled)
        {
            lblDatabase.Enabled = enabled;
            cboDatabase.Enabled = enabled;
        }

        private void SetTablesEnabled (bool enabled)
        {
            grpTables.Enabled = enabled;
        }

        private void SetExportEnabled (bool enabled)
        {
            lblExportType.Enabled = enabled;
            cboExportType.Enabled = enabled;
            lblExportFolder.Enabled = enabled;
            txtExportFolder.Enabled = enabled;
            btnExportFolder.Enabled = enabled;
            btnExport.Enabled = enabled;
        }
    }
}
