using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Xphter.Framework;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;
using Xphter.Framework.WinForm;

namespace dbcg {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();

            Image image = Properties.Resources.LoadingMax;
            this.m_pictureLoading = new PictureBox();
            this.m_pictureLoading.Width = image.Width;
            this.m_pictureLoading.Height = image.Height;
            this.m_pictureLoading.SizeMode = PictureBoxSizeMode.Zoom;
            this.m_pictureLoading.Image = image;
            this.m_pictureLoading.Visible = false;
            this.Controls.Add(this.m_pictureLoading);
        }

        private bool m_isGenerating;

        private PictureBox m_pictureLoading;

        private bool ValidateInput() {
            if(string.IsNullOrWhiteSpace(this.m_textDataSource.Text)) {
                PublicDialog.ShowWarning("Please input data source address.");
                return false;
            }
            if(string.IsNullOrWhiteSpace(this.m_textDatabase.Text)) {
                PublicDialog.ShowWarning("Please input database name.");
                return false;
            }
            if(!this.m_checkIsIntegratedSecurity.Checked) {
                if(string.IsNullOrWhiteSpace(this.m_textUserID.Text)) {
                    PublicDialog.ShowWarning("Please input user ID.");
                    return false;
                }
            }
            if(string.IsNullOrWhiteSpace(this.m_textEntityNamesapce.Text)) {
                PublicDialog.ShowWarning("Please input entity namespace.");
                return false;
            }
            if(string.IsNullOrWhiteSpace(this.m_textAccessorNamespace.Text)) {
                PublicDialog.ShowWarning("Please input accessor namespace.");
                return false;
            }

            return true;
        }

        private void m_checkIsIntegratedSecurity_CheckedChanged(object sender, EventArgs e) {
            this.m_textUserID.Enabled = this.m_textPassword.Enabled = !this.m_checkIsIntegratedSecurity.Checked;
        }

        private void m_buttonOK_Click(object sender, EventArgs e) {
            if(this.ValidateInput()) {
                this.m_pictureLoading.Left = (this.Width - this.m_pictureLoading.Width) / 2;
                this.m_pictureLoading.Top = (this.Height - this.m_pictureLoading.Height) / 2;

                this.tableLayoutPanel.Enabled = false;
                this.m_pictureLoading.Visible = true;
                this.m_pictureLoading.BringToFront();

                IDbCodeMembersProvider membersProvider = (IDbCodeMembersProvider) this.m_comboBoxMembersProvider.SelectedValue;
                new Thread(delegate() {
                    this.m_isGenerating = true;

                    try {
                        new DbCodeGenerator(new SqlServerCodeProvider(), membersProvider, CodeDomProvider.CreateProvider("CSharp")).GenerateCode(new DbDatabaseEntity(new DbSourceEntity(this.m_textDataSource.Text, new DbCredential(this.m_checkIsIntegratedSecurity.Checked, this.m_textUserID.Text, this.m_textPassword.Text), new SqlServerDatabaseEntityProvider()), this.m_textDatabase.Text, new SqlServerDataEntityProvider()), this.m_checkIsCascading.Checked, this.m_textEntityNamesapce.Text, this.m_textAccessorNamespace.Text, Path.Combine(Application.StartupPath, "GeneratedFiles"));
                        this.Invoke(new Action(delegate() {
                            PublicDialog.ShowMessage("Successfully!");
                        }));
                    } catch(Exception ex) {
                        this.Invoke(new Action(delegate() {
                            PublicDialog.ShowError(ex.Message);
                        }));
                    } finally {
                        this.Invoke(new Action(delegate() {
                            this.tableLayoutPanel.Enabled = true;
                            this.m_pictureLoading.Visible = false;
                        }));

                        this.m_isGenerating = false;
                    }
                }).Start();
            }
        }

        private void m_buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FormMain_Load(object sender, EventArgs e) {
            Assembly assembly = null;
            ICollection<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());
            foreach(string path in Directory.EnumerateFiles(Application.StartupPath, "*.dll", SearchOption.TopDirectoryOnly)) {
                try {
                    assembly = Assembly.LoadFrom(path);
                } catch {
                }

                if(assembly != null) {
                    assemblies.Add(assembly);
                }
            }

            ObjectManager<IDbCodeMembersProvider>.Instance.Register(assemblies);
            DropdownUtility.Fill(this.m_comboBoxMembersProvider, ObjectManager<IDbCodeMembersProvider>.Instance.Objects.Select((item) => new DropdownItem(item.Name, item)), true);
            if(ObjectManager<IDbCodeMembersProvider>.Instance.Objects.Any()) {
                this.m_comboBoxMembersProvider.SelectedValue = ObjectManager<IDbCodeMembersProvider>.Instance.Objects.First();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = this.m_isGenerating;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e) {
            Properties.Settings.Default.Save();
        }
    }
}
