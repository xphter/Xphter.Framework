namespace dbcg {
  partial class FormMain {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.m_textPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.m_textAccessorNamespace = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.m_textEntityNamesapce = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.m_textDataSource = new System.Windows.Forms.TextBox();
            this.m_textUserID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_textDatabase = new System.Windows.Forms.TextBox();
            this.m_checkIsIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.m_checkIsCascading = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.m_comboBoxMembersProvider = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data &Source:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&User ID:";
            // 
            // m_textPassword
            // 
            this.m_textPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textPassword.Enabled = false;
            this.m_textPassword.Location = new System.Drawing.Point(137, 50);
            this.m_textPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.PasswordChar = '*';
            this.m_textPassword.Size = new System.Drawing.Size(386, 21);
            this.m_textPassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Password:";
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_buttonOK.Location = new System.Drawing.Point(103, 5);
            this.m_buttonOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.Size = new System.Drawing.Size(56, 18);
            this.m_buttonOK.TabIndex = 0;
            this.m_buttonOK.Text = "&OK";
            this.m_buttonOK.UseVisualStyleBackColor = true;
            this.m_buttonOK.Click += new System.EventHandler(this.m_buttonOK_Click);
            // 
            // m_buttonCancel
            // 
            this.m_buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonCancel.Location = new System.Drawing.Point(365, 5);
            this.m_buttonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.Size = new System.Drawing.Size(56, 18);
            this.m_buttonCancel.TabIndex = 1;
            this.m_buttonCancel.Text = "&Cancel";
            this.m_buttonCancel.UseVisualStyleBackColor = true;
            this.m_buttonCancel.Click += new System.EventHandler(this.m_buttonCancel_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 9);
            this.tableLayoutPanel.Controls.Add(this.m_textAccessorNamespace, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.m_textEntityNamesapce, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.m_textDataSource, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.m_textUserID, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.m_textPassword, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.m_textDatabase, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.m_checkIsIntegratedSecurity, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.m_checkIsCascading, 0, 8);
            this.tableLayoutPanel.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.m_comboBoxMembersProvider, 1, 6);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 10;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(525, 244);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // m_textAccessorNamespace
            // 
            this.m_textAccessorNamespace.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::dbcg.Properties.Settings.Default, "AccessorNamespace", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_textAccessorNamespace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textAccessorNamespace.Location = new System.Drawing.Point(137, 122);
            this.m_textAccessorNamespace.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textAccessorNamespace.Name = "m_textAccessorNamespace";
            this.m_textAccessorNamespace.Size = new System.Drawing.Size(386, 21);
            this.m_textAccessorNamespace.TabIndex = 11;
            this.m_textAccessorNamespace.Text = global::dbcg.Properties.Settings.Default.AccessorNamespace;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 128);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "&Accessor Namespace:";
            // 
            // m_textEntityNamesapce
            // 
            this.m_textEntityNamesapce.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::dbcg.Properties.Settings.Default, "EntityNamespace", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_textEntityNamesapce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textEntityNamesapce.Location = new System.Drawing.Point(137, 98);
            this.m_textEntityNamesapce.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textEntityNamesapce.Name = "m_textEntityNamesapce";
            this.m_textEntityNamesapce.Size = new System.Drawing.Size(386, 21);
            this.m_textEntityNamesapce.TabIndex = 9;
            this.m_textEntityNamesapce.Text = global::dbcg.Properties.Settings.Default.EntityNamespace;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 104);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "&Entity Namespace:";
            // 
            // m_textDataSource
            // 
            this.m_textDataSource.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::dbcg.Properties.Settings.Default, "DataSource", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_textDataSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textDataSource.Location = new System.Drawing.Point(137, 2);
            this.m_textDataSource.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textDataSource.Name = "m_textDataSource";
            this.m_textDataSource.Size = new System.Drawing.Size(386, 21);
            this.m_textDataSource.TabIndex = 1;
            this.m_textDataSource.Text = global::dbcg.Properties.Settings.Default.DataSource;
            // 
            // m_textUserID
            // 
            this.m_textUserID.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::dbcg.Properties.Settings.Default, "UserID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_textUserID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textUserID.Enabled = false;
            this.m_textUserID.Location = new System.Drawing.Point(137, 26);
            this.m_textUserID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textUserID.Name = "m_textUserID";
            this.m_textUserID.Size = new System.Drawing.Size(386, 21);
            this.m_textUserID.TabIndex = 3;
            this.m_textUserID.Text = global::dbcg.Properties.Settings.Default.UserID;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 80);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "&Database:";
            // 
            // m_textDatabase
            // 
            this.m_textDatabase.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::dbcg.Properties.Settings.Default, "InitialCatalog", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_textDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textDatabase.Location = new System.Drawing.Point(137, 74);
            this.m_textDatabase.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_textDatabase.Name = "m_textDatabase";
            this.m_textDatabase.Size = new System.Drawing.Size(386, 21);
            this.m_textDatabase.TabIndex = 7;
            this.m_textDatabase.Text = global::dbcg.Properties.Settings.Default.InitialCatalog;
            // 
            // m_checkIsIntegratedSecurity
            // 
            this.m_checkIsIntegratedSecurity.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_checkIsIntegratedSecurity.AutoSize = true;
            this.m_checkIsIntegratedSecurity.Checked = global::dbcg.Properties.Settings.Default.IntergredSecurity;
            this.m_checkIsIntegratedSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel.SetColumnSpan(this.m_checkIsIntegratedSecurity, 2);
            this.m_checkIsIntegratedSecurity.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::dbcg.Properties.Settings.Default, "IntergredSecurity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_checkIsIntegratedSecurity.Location = new System.Drawing.Point(184, 172);
            this.m_checkIsIntegratedSecurity.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_checkIsIntegratedSecurity.Name = "m_checkIsIntegratedSecurity";
            this.m_checkIsIntegratedSecurity.Size = new System.Drawing.Size(156, 16);
            this.m_checkIsIntegratedSecurity.TabIndex = 14;
            this.m_checkIsIntegratedSecurity.Text = "&Windows Authentication";
            this.m_checkIsIntegratedSecurity.UseVisualStyleBackColor = true;
            this.m_checkIsIntegratedSecurity.CheckedChanged += new System.EventHandler(this.m_checkIsIntegratedSecurity_CheckedChanged);
            // 
            // m_checkIsCascading
            // 
            this.m_checkIsCascading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_checkIsCascading.AutoSize = true;
            this.m_checkIsCascading.Checked = global::dbcg.Properties.Settings.Default.IsCascading;
            this.m_checkIsCascading.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel.SetColumnSpan(this.m_checkIsCascading, 2);
            this.m_checkIsCascading.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::dbcg.Properties.Settings.Default, "IsCascading", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_checkIsCascading.Location = new System.Drawing.Point(154, 196);
            this.m_checkIsCascading.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.m_checkIsCascading.Name = "m_checkIsCascading";
            this.m_checkIsCascading.Size = new System.Drawing.Size(216, 16);
            this.m_checkIsCascading.TabIndex = 15;
            this.m_checkIsCascading.Text = "&Generate Cascading Deletion Code";
            this.m_checkIsCascading.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 152);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "&Members Provider:";
            // 
            // m_comboBoxMembersProvider
            // 
            this.m_comboBoxMembersProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_comboBoxMembersProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxMembersProvider.FormattingEnabled = true;
            this.m_comboBoxMembersProvider.Location = new System.Drawing.Point(137, 146);
            this.m_comboBoxMembersProvider.Margin = new System.Windows.Forms.Padding(2);
            this.m_comboBoxMembersProvider.Name = "m_comboBoxMembersProvider";
            this.m_comboBoxMembersProvider.Size = new System.Drawing.Size(386, 20);
            this.m_comboBoxMembersProvider.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel.SetColumnSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.m_buttonOK, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_buttonCancel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 216);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(525, 28);
            this.tableLayoutPanel1.TabIndex = 16;
            // 
            // FormMain
            // 
            this.AcceptButton = this.m_buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_buttonCancel;
            this.ClientSize = new System.Drawing.Size(535, 254);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Code Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox m_textDataSource;
    private System.Windows.Forms.TextBox m_textUserID;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox m_textPassword;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button m_buttonOK;
    private System.Windows.Forms.Button m_buttonCancel;
    private System.Windows.Forms.CheckBox m_checkIsIntegratedSecurity;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox m_textDatabase;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox m_textEntityNamesapce;
    private System.Windows.Forms.TextBox m_textAccessorNamespace;
    private System.Windows.Forms.CheckBox m_checkIsCascading;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox m_comboBoxMembersProvider;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}

