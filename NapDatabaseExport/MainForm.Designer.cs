namespace NapDatabaseExport
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblDatabaseType = new System.Windows.Forms.Label();
            this.cboDatabaseType = new System.Windows.Forms.ComboBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.grpTables = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.chlTables = new System.Windows.Forms.CheckedListBox();
            this.cboExportType = new System.Windows.Forms.ComboBox();
            this.lblExportType = new System.Windows.Forms.Label();
            this.btnExportFolder = new System.Windows.Forms.Button();
            this.txtExportFolder = new System.Windows.Forms.TextBox();
            this.lblExportFolder = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.fbdExportFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnDatabaseFile = new System.Windows.Forms.Button();
            this.txtDatabaseFile = new System.Windows.Forms.TextBox();
            this.lblDatabaseFile = new System.Windows.Forms.Label();
            this.ofdDatabaseFile = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.grpTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(10, 66);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(49, 13);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "Сървър:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(86, 63);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(145, 20);
            this.txtServer.TabIndex = 6;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(86, 89);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(136, 20);
            this.txtUser.TabIndex = 9;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(10, 92);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(70, 13);
            this.lblUser.TabIndex = 8;
            this.lblUser.Text = "Потребител:";
            // 
            // lblDatabaseType
            // 
            this.lblDatabaseType.AutoSize = true;
            this.lblDatabaseType.Location = new System.Drawing.Point(9, 13);
            this.lblDatabaseType.Name = "lblDatabaseType";
            this.lblDatabaseType.Size = new System.Drawing.Size(89, 13);
            this.lblDatabaseType.TabIndex = 0;
            this.lblDatabaseType.Text = "Тип база данни:";
            // 
            // cboDatabaseType
            // 
            this.cboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabaseType.FormattingEnabled = true;
            this.cboDatabaseType.Location = new System.Drawing.Point(104, 10);
            this.cboDatabaseType.Name = "cboDatabaseType";
            this.cboDatabaseType.Size = new System.Drawing.Size(202, 21);
            this.cboDatabaseType.TabIndex = 1;
            this.cboDatabaseType.SelectedIndexChanged += new System.EventHandler(this.cboDatabaseType_SelectedIndexChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(86, 115);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(136, 20);
            this.txtPassword.TabIndex = 11;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(10, 118);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(48, 13);
            this.lblPassword.TabIndex = 10;
            this.lblPassword.Text = "Парола:";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(237, 65);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(10, 13);
            this.lblPort.TabIndex = 22;
            this.lblPort.Text = ":";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(253, 63);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(53, 20);
            this.nudPort.TabIndex = 7;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(228, 89);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(79, 46);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Свързване";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(329, 13);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(68, 13);
            this.lblDatabase.TabIndex = 13;
            this.lblDatabase.Text = "База данни:";
            // 
            // cboDatabase
            // 
            this.cboDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Location = new System.Drawing.Point(405, 10);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(207, 21);
            this.cboDatabase.TabIndex = 14;
            this.cboDatabase.SelectedIndexChanged += new System.EventHandler(this.cboDatabase_SelectedIndexChanged);
            // 
            // grpTables
            // 
            this.grpTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTables.Controls.Add(this.chkSelectAll);
            this.grpTables.Controls.Add(this.chlTables);
            this.grpTables.Location = new System.Drawing.Point(332, 40);
            this.grpTables.Name = "grpTables";
            this.grpTables.Size = new System.Drawing.Size(281, 221);
            this.grpTables.TabIndex = 15;
            this.grpTables.TabStop = false;
            this.grpTables.Text = "Таблици";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 19);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(111, 17);
            this.chkSelectAll.TabIndex = 0;
            this.chkSelectAll.Text = "Избор на всички";
            this.chkSelectAll.ThreeState = true;
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // chlTables
            // 
            this.chlTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chlTables.FormattingEnabled = true;
            this.chlTables.Location = new System.Drawing.Point(6, 37);
            this.chlTables.Name = "chlTables";
            this.chlTables.Size = new System.Drawing.Size(269, 169);
            this.chlTables.TabIndex = 1;
            this.chlTables.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlTables_ItemCheck);
            // 
            // cboExportType
            // 
            this.cboExportType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboExportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExportType.FormattingEnabled = true;
            this.cboExportType.Location = new System.Drawing.Point(126, 267);
            this.cboExportType.Name = "cboExportType";
            this.cboExportType.Size = new System.Drawing.Size(486, 21);
            this.cboExportType.TabIndex = 17;
            // 
            // lblExportType
            // 
            this.lblExportType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblExportType.AutoSize = true;
            this.lblExportType.Location = new System.Drawing.Point(9, 270);
            this.lblExportType.Name = "lblExportType";
            this.lblExportType.Size = new System.Drawing.Size(111, 13);
            this.lblExportType.TabIndex = 16;
            this.lblExportType.Text = "Формат за експорт:";
            // 
            // btnExportFolder
            // 
            this.btnExportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportFolder.Location = new System.Drawing.Point(587, 296);
            this.btnExportFolder.Name = "btnExportFolder";
            this.btnExportFolder.Size = new System.Drawing.Size(25, 20);
            this.btnExportFolder.TabIndex = 20;
            this.btnExportFolder.Text = "...";
            this.btnExportFolder.UseVisualStyleBackColor = true;
            this.btnExportFolder.Click += new System.EventHandler(this.btnExportFolder_Click);
            // 
            // txtExportFolder
            // 
            this.txtExportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportFolder.Location = new System.Drawing.Point(126, 296);
            this.txtExportFolder.Name = "txtExportFolder";
            this.txtExportFolder.Size = new System.Drawing.Size(455, 20);
            this.txtExportFolder.TabIndex = 19;
            // 
            // lblExportFolder
            // 
            this.lblExportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblExportFolder.AutoSize = true;
            this.lblExportFolder.Location = new System.Drawing.Point(9, 299);
            this.lblExportFolder.Name = "lblExportFolder";
            this.lblExportFolder.Size = new System.Drawing.Size(101, 13);
            this.lblExportFolder.TabIndex = 18;
            this.lblExportFolder.Text = "Папка за експорт:";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(12, 322);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(600, 23);
            this.btnExport.TabIndex = 21;
            this.btnExport.Text = "ЕКСПОРТ";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnDatabaseFile
            // 
            this.btnDatabaseFile.Location = new System.Drawing.Point(281, 37);
            this.btnDatabaseFile.Name = "btnDatabaseFile";
            this.btnDatabaseFile.Size = new System.Drawing.Size(25, 20);
            this.btnDatabaseFile.TabIndex = 4;
            this.btnDatabaseFile.Text = "...";
            this.btnDatabaseFile.UseVisualStyleBackColor = true;
            this.btnDatabaseFile.Click += new System.EventHandler(this.btnDatabaseFile_Click);
            // 
            // txtDatabaseFile
            // 
            this.txtDatabaseFile.Location = new System.Drawing.Point(86, 37);
            this.txtDatabaseFile.Name = "txtDatabaseFile";
            this.txtDatabaseFile.Size = new System.Drawing.Size(189, 20);
            this.txtDatabaseFile.TabIndex = 3;
            // 
            // lblDatabaseFile
            // 
            this.lblDatabaseFile.AutoSize = true;
            this.lblDatabaseFile.Location = new System.Drawing.Point(9, 40);
            this.lblDatabaseFile.Name = "lblDatabaseFile";
            this.lblDatabaseFile.Size = new System.Drawing.Size(39, 13);
            this.lblDatabaseFile.TabIndex = 2;
            this.lblDatabaseFile.Text = "Файл:";
            // 
            // ofdDatabaseFile
            // 
            this.ofdDatabaseFile.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(624, 361);
            this.Controls.Add(this.btnDatabaseFile);
            this.Controls.Add(this.txtDatabaseFile);
            this.Controls.Add(this.lblDatabaseFile);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnExportFolder);
            this.Controls.Add(this.txtExportFolder);
            this.Controls.Add(this.lblExportFolder);
            this.Controls.Add(this.cboExportType);
            this.Controls.Add(this.lblExportType);
            this.Controls.Add(this.grpTables);
            this.Controls.Add(this.cboDatabase);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.cboDatabaseType);
            this.Controls.Add(this.lblDatabaseType);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.lblServer);
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Експорт на данни за НАП";
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.grpTables.ResumeLayout(false);
            this.grpTables.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cboDatabaseType;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.GroupBox grpTables;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.CheckedListBox chlTables;
        private System.Windows.Forms.ComboBox cboExportType;
        private System.Windows.Forms.Label lblExportType;
        private System.Windows.Forms.Button btnExportFolder;
        private System.Windows.Forms.TextBox txtExportFolder;
        private System.Windows.Forms.Label lblExportFolder;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.FolderBrowserDialog fbdExportFolder;
        private System.Windows.Forms.Button btnDatabaseFile;
        private System.Windows.Forms.TextBox txtDatabaseFile;
        private System.Windows.Forms.Label lblDatabaseFile;
        private System.Windows.Forms.OpenFileDialog ofdDatabaseFile;
    }
}

