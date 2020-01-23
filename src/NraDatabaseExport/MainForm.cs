using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NraDatabaseExport.DbProviders;
using NraDatabaseExport.ExportProviders;

namespace NraDatabaseExport
{
	public partial class MainForm : Form
	{
		private static readonly IDbProvider[] _dbProviders
			= new IDbProvider[]
			{
#if SUPPORT_MYSQL
				new MySqlDbProvider(),
#endif
#if SUPPORT_SQLITE
				new SqliteDbProvider(),
#endif
#if SUPPORT_SQLSERVER
				new SqlServerDbProvider(),
#endif
#if SUPPORT_MSACCESS
				new MSAccessDbProvider(),
#endif
#if SUPPORT_ODBC
				new OdbcDbProvider(),
#endif
#if SUPPORT_FIREBIRD
				new FirebirdDbProvider(),
#endif
			};

		private static readonly ExportProviderBase[] _exportProviders
			= new ExportProviderBase[]
			{
				new CommaCsvExportProvider(),
				new SemicolonCsvExportProvider(),
			};

		private bool _settingCheckBoxes;

		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			IDbProvider[] databaseProviders = _dbProviders;
			cboDatabaseType.DisplayMember = nameof(DbProviderBase.Name);
			cboDatabaseType.DataSource = databaseProviders;

			ExportProviderBase[] dataExporters = _exportProviders;
			cboExportType.DisplayMember = nameof(ExportProviderBase.Name);
			cboExportType.DataSource = dataExporters;

			txtExportFolder.Text = Directory.GetCurrentDirectory();

			SetDatabaseEnabled(false);
			SetTablesEnabled(false);
			SetExportEnabled(false);
		}

		private void cboDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var currentProvider = (DbProviderBase)cboDatabaseType.SelectedItem;

			lblDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;
			txtDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;
			btnDatabaseFile.Enabled = currentProvider.UsesDatabaseFile;

			lblServer.Enabled = currentProvider.UsesServer;
			txtServer.Enabled = currentProvider.UsesServer;

			lblPort.Visible = currentProvider.UsesPort;
			nudPort.Visible = currentProvider.UsesPort;
			if (currentProvider.DefaultPort > 0)
			{
				nudPort.Value = currentProvider.DefaultPort;
			}

			lblUser.Enabled = currentProvider.UsesUserName;
			txtUser.Enabled = currentProvider.UsesUserName;

			lblPassword.Enabled = currentProvider.UsesPassword;
			txtPassword.Enabled = currentProvider.UsesPassword;

			SetDatabaseEnabled(currentProvider.UsesDatabaseName);

			if (currentProvider.UsesDatabaseFile)
			{
				txtDatabaseFile.Focus();
			}
			else if (currentProvider.UsesServer)
			{
				txtServer.Focus();
			}
			else if (currentProvider.UsesPort)
			{
				nudPort.Focus();
			}
			else if (currentProvider.UsesUserName)
			{
				txtUser.Focus();
			}
			else if (currentProvider.UsesPassword)
			{
				txtPassword.Focus();
			}
			else if (currentProvider.UsesDatabaseName)
			{
				txtDatabaseFile.Focus();
			}
		}

		private void btnDatabaseFile_Click(object sender, EventArgs e)
		{
			ofdDatabaseFile.FileName = txtDatabaseFile.Text;
			if (ofdDatabaseFile.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			txtDatabaseFile.Text = ofdDatabaseFile.FileName;
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			SetDatabaseEnabled(false);
			SetTablesEnabled(false);
			SetExportEnabled(false);

			var currentProvider = (DbProviderBase)cboDatabaseType.SelectedItem;

			if (currentProvider.UsesDatabaseFile)
			{
				if (string.IsNullOrWhiteSpace(txtDatabaseFile.Text) || !File.Exists(txtDatabaseFile.Text))
				{
					MessageBox.Show(
						"Моля, въведете коректен файл за база данни.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					txtDatabaseFile.Focus();

					return;
				}
			}

			if (currentProvider.UsesServer)
			{
				if (string.IsNullOrWhiteSpace(txtServer.Text))
				{
					MessageBox.Show(
						"Моля, въведете сървър.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					txtServer.Focus();

					return;
				}
			}

			if (currentProvider.UsesPort)
			{
				if (nudPort.Value <= 0)
				{
					MessageBox.Show(
						"Моля, въведете коректен порт.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					nudPort.Focus();

					return;
				}
			}

			if (currentProvider.RequiresUserName)
			{
				if (string.IsNullOrWhiteSpace(txtUser.Text))
				{
					MessageBox.Show(
						"Моля, въведете потребител.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					txtUser.Focus();

					return;
				}
			}

			if (currentProvider.RequiresPassword)
			{
				if (string.IsNullOrWhiteSpace(txtPassword.Text))
				{
					MessageBox.Show(
						"Моля, въведете парола.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					txtPassword.Focus();

					return;
				}
			}

			currentProvider.DatabaseFileName = txtDatabaseFile.Text;
			currentProvider.ServerName = txtServer.Text;
			currentProvider.Port = (int)nudPort.Value;
			currentProvider.UserName = txtUser.Text;
			currentProvider.Password = txtPassword.Text;

			try
			{
				currentProvider.CreateConnection();

				if (currentProvider.UsesDatabaseName)
				{
					cboDatabase.DataSource = currentProvider.GetDatabaseNames();

					SetDatabaseEnabled(true);
				}
				else
				{
					LoadTables();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Грешка при свързване със сървъра: {ex}",
					"Внимание!",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private void cboDatabase_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTablesEnabled(false);
			SetExportEnabled(false);

			var currentProvider = (DbProviderBase)cboDatabaseType.SelectedItem;

			currentProvider.DatabaseName = (string)cboDatabase.SelectedValue;

			LoadTables();
		}

		private void LoadTables()
		{
			var currentProvider = (DbProviderBase)cboDatabaseType.SelectedItem;

			try
			{
				chlTables.DataSource = currentProvider.GetTableNames();
				SetTablesEnabled(true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Грешка при свързване със сървъра: {ex}",
					"Внимание!",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
		{
			if (chkSelectAll.CheckState == CheckState.Indeterminate || _settingCheckBoxes)
			{
				return;
			}

			try
			{
				_settingCheckBoxes = true;

				for (int i = 0; i < chlTables.Items.Count; i++)
				{
					chlTables.SetItemChecked(i, chkSelectAll.Checked);
				}
			}
			finally
			{
				_settingCheckBoxes = false;
			}
		}

		private void chlTables_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			SetExportEnabled(chlTables.CheckedItems.Count > 0 || e.NewValue == CheckState.Checked);

			if (_settingCheckBoxes)
			{
				return;
			}

			try
			{
				_settingCheckBoxes = true;

				var checkedItemsCount = chlTables.CheckedItems.Count;

				if ((checkedItemsCount == 0 || checkedItemsCount == 1) && e.NewValue == CheckState.Unchecked)
				{
					chkSelectAll.CheckState = CheckState.Unchecked;
				}
				else if ((checkedItemsCount == chlTables.Items.Count || checkedItemsCount == chlTables.Items.Count - 1) && e.NewValue == CheckState.Checked)
				{
					chkSelectAll.CheckState = CheckState.Checked;
				}
				else
				{
					chkSelectAll.CheckState = CheckState.Indeterminate;
				}
			}
			finally
			{
				_settingCheckBoxes = false;
			}
		}

		private void btnExportFolder_Click(object sender, EventArgs e)
		{
			fbdExportFolder.SelectedPath = txtExportFolder.Text;

			if (fbdExportFolder.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			txtExportFolder.Text = fbdExportFolder.SelectedPath;
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				var exportFolder = txtExportFolder.Text;

				if (!Directory.Exists(exportFolder))
				{
					MessageBox.Show(
						$@"Папка ""{exportFolder}"" не може да бъде открита.",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);

					return;
				}

				var exportProvider = (ExportProviderBase)cboExportType.SelectedItem;
				var databaseProvider = (DbProviderBase)cboDatabaseType.SelectedItem;

				string tableName = string.Empty;
				int tableRow = 0;

				try
				{
					foreach (string tableNames in chlTables.CheckedItems)
					{
						tableName = tableNames.Trim();
						tableRow = 0;

						string fileName = Path.Combine(exportFolder,
							tableName + exportProvider.DefaultFileExtension);

						exportProvider.BeginWrite(fileName);

						using (IDataReader reader = databaseProvider.ExecuteTableReader(tableName))
						{
							tableRow++;

							if (!reader.Read())
							{
								exportProvider.EndWrite();
								continue;
							}

							var columns = new string[reader.FieldCount];

							for (int j = 0; j < reader.FieldCount; j++)
							{
								columns[j] = reader.GetName(j);
							}

							exportProvider.WriteHeaderRow(columns);

							var row = new object[reader.FieldCount];

							do
							{
								for (int j = 0; j < reader.FieldCount; j++)
								{
									try
									{
										row[j] = reader.GetValue(j);
									}
									catch (Exception ex)
									{
										Debug.WriteLine($"Грешка при експорт на таблица {tableName}, ред {tableRow}, колона {columns[j]}: {ex}");
									}
								}

								exportProvider.WriteDataRow(row);
							} while (reader.Read());
						}

						exportProvider.EndWrite();
					}

					MessageBox.Show(
						"Експортът на данни приключи успешно",
						"Успех",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(
						$"Грешка при експорт на таблица {tableName}, ред {tableRow}: {ex}",
						"Внимание!",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void SetDatabaseEnabled(bool enabled)
		{
			lblDatabase.Enabled = enabled;
			cboDatabase.Enabled = enabled;
		}

		private void SetTablesEnabled(bool enabled)
		{
			grpTables.Enabled = enabled;
		}

		private void SetExportEnabled(bool enabled)
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
