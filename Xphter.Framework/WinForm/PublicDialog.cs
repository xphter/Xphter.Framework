using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Provides common dialog used in a WinForm application.
    /// </summary>
    public static class PublicDialog {
        private static String g_caption = Application.ProductName;
        /// <summary>
        /// Dialog caption, it is production name generally.
        /// </summary>
        public static String Caption {
            get {
                return g_caption;
            }
            set {
                if(!string.IsNullOrWhiteSpace(value)) {
                    g_caption = value;
                } else {
                    g_caption = Application.ProductName;
                }
            }
        }

        /// <summary>
        /// Show info message.
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowMessage(String format, params Object[] values) {
            ShowMessage(Form.ActiveForm, String.Format(format, values));
        }

        /// <summary>
        /// Show info message.
        /// </summary>
        /// <param name="owner">Target window.</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowMessage(IWin32Window owner, String format, params Object[] values) {
            ShowMessage(owner, String.Format(format, values));
        }

        /// <summary>
        /// Show info message.
        /// </summary>
        /// <param name="message">The info.</param>
        public static void ShowMessage(String message) {
            ShowMessage(Form.ActiveForm, message);
        }

        /// <summary>
        /// Show info message.
        /// </summary>
        /// <param name="owner">Target window.</param>
        /// <param name="message">The info.</param>
        public static void ShowMessage(IWin32Window owner, String message) {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowWarning(String format, params Object[] values) {
            ShowWarning(Form.ActiveForm, String.Format(format, values));
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowWarning(IWin32Window owner, String format, params Object[] values) {
            ShowWarning(owner, String.Format(format, values));
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public static void ShowWarning(String message) {
            ShowWarning(Form.ActiveForm, message);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="message">��Ϣ</param>
        public static void ShowWarning(IWin32Window owner, String message) {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowError(String format, params Object[] values) {
            ShowError(Form.ActiveForm, String.Format(format, values));
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowError(IWin32Window owner, String format, params Object[] values) {
            ShowError(owner, String.Format(format, values));
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public static void ShowError(String message) {
            ShowError(Form.ActiveForm, message);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="message">��Ϣ</param>
        public static void ShowError(IWin32Window owner, String message) {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// ��ʾȷ�ϣ�����ΪĬ�ϰ�ť
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(String message) {
            return ShowConfirm(Form.ActiveForm, message, true);
        }

        /// <summary>
        /// ��ʾȷ�ϣ�����ΪĬ�ϰ�ť
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="message">��Ϣ</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(IWin32Window owner, String message) {
            return ShowConfirm(owner, message, true);
        }

        /// <summary>
        /// ��ʾȷ�ϣ�����ΪĬ�ϰ�ť
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(String format, params Object[] values) {
            return ShowConfirm(Form.ActiveForm, String.Format(format, values), true);
        }

        /// <summary>
        /// ��ʾȷ�ϣ�����ΪĬ�ϰ�ť
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(IWin32Window owner, String format, params Object[] values) {
            return ShowConfirm(owner, String.Format(format, values), true);
        }

        /// <summary>
        /// ��ʾȷ��
        /// </summary>
        /// <param name="format">��Ϣ��ʽ</param>
        /// <param name="noIsDefault">�����Ƿ�ΪĬ�ϰ�ť</param>
        /// <param name="values">��Ϣ</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(String format, bool noIsDefault, params Object[] values) {
            return ShowConfirm(Form.ActiveForm, String.Format(format, values), noIsDefault);
        }

        /// <summary>
        /// ��ʾȷ��
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="noIsDefault">�����Ƿ�ΪĬ�ϰ�ť</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(IWin32Window owner, bool noIsDefault, String format, params Object[] values) {
            return ShowConfirm(owner, String.Format(format, values), noIsDefault);
        }

        /// <summary>
        /// ��ʾȷ��
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <param name="noIsDefault">�����Ƿ�ΪĬ�ϰ�ť</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(String message, bool noIsDefault) {
            return ShowConfirm(Form.ActiveForm, message, noIsDefault);
        }

        /// <summary>
        /// ��ʾȷ��
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="message">��Ϣ</param>
        /// <param name="noIsDefault">�����Ƿ�ΪĬ�ϰ�ť</param>
        /// <returns>�Ƿ��û�ѡ���ˡ��ǡ�</returns>
        public static bool ShowConfirm(IWin32Window owner, String message, bool noIsDefault) {
            MessageBoxDefaultButton defaultButton;
            if(noIsDefault) {
                defaultButton = MessageBoxDefaultButton.Button2;
            } else {
                defaultButton = MessageBoxDefaultButton.Button1;
            }
            if(MessageBox.Show(owner, message, Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, defaultButton) == DialogResult.Yes) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(String filter) {
            return ShowOpenFile(Form.ActiveForm, null, filter, true);
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(IWin32Window owner, String filter) {
            return ShowOpenFile(owner, null, filter, true);
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="originFile">��ʼѡ����ļ�</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(String originFile, String filter) {
            return ShowOpenFile(Form.ActiveForm, originFile, filter, true);
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="originFile">��ʼѡ����ļ�</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(IWin32Window owner, String originFile, String filter) {
            return ShowOpenFile(owner, originFile, filter, true);
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="root">��ʼѡ����ļ����ʼ�ļ���</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <param name="isFile">�棬root��ʾ��ʼѡ����ļ����٣�root��ʾ��ʼ�ļ���</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(String root, String filter, bool isFile) {
            return ShowOpenFile(Form.ActiveForm, root, filter, isFile);
        }

        /// <summary>
        /// ��ʾ���ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="root">��ʼѡ����ļ����ʼ�ļ���</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <param name="isFile">�棬root��ʾ��ʼѡ����ļ����٣�root��ʾ��ʼ�ļ���</param>
        /// <returns>�򿪵��ļ��ľ���·����NULL��ʾδ���ļ�</returns>
        public static String ShowOpenFile(IWin32Window owner, String root, String filter, bool isFile) {
            OpenFileDialog dlg = new OpenFileDialog();
            if(root != null && (root = root.Trim()).Length > 0) {
                if(isFile) {
                    dlg.FileName = root;
                } else {
                    dlg.InitialDirectory = root;
                }
            }
            if(filter != null) {
                dlg.Filter = filter;
            }

            if(dlg.ShowDialog(owner) == DialogResult.OK) {
                return dlg.FileName;
            } else {
                return null;
            }
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(String filter) {
            return ShowSaveFile(Form.ActiveForm, null, filter, true);
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(IWin32Window owner, String filter) {
            return ShowSaveFile(owner, null, filter, true);
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="originFile">��ʼѡ����ļ�</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(String originFile, String filter) {
            return ShowSaveFile(Form.ActiveForm, originFile, filter, true);
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="originFile">��ʼѡ����ļ�</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(IWin32Window owner, String originFile, String filter) {
            return ShowSaveFile(owner, originFile, filter, true);
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="root">��ʼѡ����ļ����ʼ�ļ���</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <param name="isFile">�棬root��ʾ��ʼѡ����ļ����٣�root��ʾ��ʼ�ļ���</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(String root, String filter, bool isFile) {
            return ShowSaveFile(Form.ActiveForm, root, filter, isFile);
        }

        /// <summary>
        /// ��ʾ�����ļ�����δѡ���ļ��򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="root">��ʼѡ����ļ����ʼ�ļ���</param>
        /// <param name="filter">�ļ����͹�����</param>
        /// <param name="isFile">�棬root��ʾ��ʼѡ����ļ����٣�root��ʾ��ʼ�ļ���</param>
        /// <returns>���浽���ļ��ľ���·����NULL��ʾδָ���ļ�</returns>
        public static String ShowSaveFile(IWin32Window owner, String root, String filter, bool isFile) {
            SaveFileDialog o = new SaveFileDialog();
            if(root != null && (root = root.Trim()).Length > 0) {
                if(isFile) {
                    o.FileName = root;
                } else {
                    o.InitialDirectory = root;
                }
            }
            if(filter != null) {
                o.Filter = filter;
            }

            if(o.ShowDialog(owner) == DialogResult.OK) {
                return o.FileName;
            } else {
                return null;
            }
        }

        /// <summary>
        /// ��ʾѡ����ɫ����δѡ����ɫ�򷵻����õĳ�ʼ��ɫ
        /// </summary>
        /// <param name="origin">��ʼ��ɫ</param>
        /// <returns>ѡ�����ɫ</returns>
        public static Color ShowSelectColor(Color origin) {
            return ShowSelectColor(Form.ActiveForm, origin);
        }

        /// <summary>
        /// ��ʾѡ����ɫ����δѡ����ɫ�򷵻����õĳ�ʼ��ɫ
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="origin">��ʼ��ɫ</param>
        /// <returns>ѡ�����ɫ</returns>
        public static Color ShowSelectColor(IWin32Window owner, Color origin) {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = origin;
            dlg.AnyColor = true;
            if(dlg.ShowDialog(owner) == DialogResult.OK) {
                return dlg.Color;
            } else {
                return origin;
            }
        }

        /// <summary>
        /// ��ʾѡ��Ŀ¼����δѡ���κ�Ŀ¼�򷵻�NULL
        /// </summary>
        /// <param name="root">��ʼĿ¼</param>
        /// <returns></returns>
        public static String ShowSelectDirectory(String root) {
            return ShowSelectDirectory(Form.ActiveForm, root);
        }

        /// <summary>
        /// ��ʾѡ��Ŀ¼����δѡ���κ�Ŀ¼�򷵻�NULL
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="root">��ʼĿ¼</param>
        /// <returns></returns>
        public static String ShowSelectDirectory(IWin32Window owner, String root) {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = root;
            dlg.ShowNewFolderButton = true;
            if(dlg.ShowDialog(owner) == DialogResult.OK) {
                return dlg.SelectedPath;
            } else {
                return null;
            }
        }

        /// <summary>
        /// ��ʾѡ�����壬��δѡ���κ������򷵻س�ʼ���塣
        /// </summary>
        /// <param name="origin">��ʼ����</param>
        /// <returns>ѡ�������</returns>
        public static Font ShowSelectFont(Font origin) {
            return ShowSelectFont(Form.ActiveForm, origin);
        }

        /// <summary>
        /// ��ʾѡ�����壬��δѡ���κ������򷵻س�ʼ���塣
        /// </summary>
        /// <param name="owner">Ŀ�괰��</param>
        /// <param name="origin">��ʼ����</param>
        /// <returns>ѡ�������</returns>
        public static Font ShowSelectFont(IWin32Window owner, Font origin) {
            FontDialog dlg = new FontDialog();
            dlg.Font = origin;
            if(dlg.ShowDialog(owner) == DialogResult.OK) {
                return dlg.Font;
            } else {
                return origin;
            }
        }
    }
}
