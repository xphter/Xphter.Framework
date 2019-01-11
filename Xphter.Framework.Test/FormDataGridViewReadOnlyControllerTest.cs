using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xphter.Framework.WinForm;

namespace Xphter.Framework.Test {
    public partial class FormDataGridViewReadOnlyControllerTest : Form {
        public FormDataGridViewReadOnlyControllerTest() {
            InitializeComponent();
        }

        private DataGridViewReadOnlyController<UserInfo> m_controller;

        private class UserInfo : NotifyPropertyChanged {
            public UserInfo() {
                this.Name = Guid.NewGuid().ToString();
                this.AddTime = DateTime.Now;
            }

            [Browsable(false)]
            [Description("标识")]
            public int ID {
                get;
                set;
            }

            private string m_name;
            [Description("用户名")]
            public string Name {
                get {
                    return this.m_name;
                }
                set {
                    this.m_name = value;
                    this.OnPropertyChanged(() => this.Name);
                }
            }

            private DateTime m_addTime;
            [Description("添加时间")]
            public DateTime AddTime {
                get {
                    return this.m_addTime;
                }
                set {
                    this.m_addTime = value;
                    this.OnPropertyChanged(() => this.AddTime);
                }
            }
        }

        private void FormDataGridViewReadOnlyControllerTest_Load(object sender, EventArgs e) {
            this.m_controller = new DataGridViewReadOnlyController<UserInfo>(this.dataGridViewData);
            this.m_controller.ColumnCreating += delegate(object sender2, DataGridViewControllerColumnEventArgs e2) {
                if(e2.Name == "AddTime") {
                    e2.Column.HeaderText = "创建日期";
                }
            };
            this.m_controller.CellFilling += delegate(object sender2, DataGridViewControllerCellEventArgs<UserInfo> e2) {
                if(e2.Name == "AddTime") {
                    e2.Value = ((DateTime) e2.Value).ToString("yyyy-MM-dd HH:mm:ss");
                }
            };
            this.m_controller.Initialize();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e) {
            this.m_controller.Refresh(new UserInfo[]{
                new UserInfo(),
                new UserInfo(),
                new UserInfo(),
                new UserInfo(),
                new UserInfo(),
            });
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e) {
            this.m_controller.Add(new UserInfo());
        }

        private void toolStripButtonInsert_Click(object sender, EventArgs e) {
            this.m_controller.Insert(new UserInfo(), 2);
        }

        private void toolStripButtonUpdate_Click(object sender, EventArgs e) {
            this.m_controller.Update(new UserInfo(), 2);
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e) {
            this.m_controller.Remove(2);
        }

        private void toolStripButtonUpdateDataSource_Click(object sender, EventArgs e) {
            UserInfo info = this.m_controller.GetObject(3);
            info.Name = Guid.NewGuid().ToString();
            info.AddTime = DateTime.Now;
        }
    }
}
