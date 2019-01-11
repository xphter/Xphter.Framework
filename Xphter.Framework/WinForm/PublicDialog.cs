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
        /// 显示警告
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowWarning(String format, params Object[] values) {
            ShowWarning(Form.ActiveForm, String.Format(format, values));
        }

        /// <summary>
        /// 显示警告
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowWarning(IWin32Window owner, String format, params Object[] values) {
            ShowWarning(owner, String.Format(format, values));
        }

        /// <summary>
        /// 显示警告
        /// </summary>
        /// <param name="message">信息</param>
        public static void ShowWarning(String message) {
            ShowWarning(Form.ActiveForm, message);
        }

        /// <summary>
        /// 显示警告
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="message">信息</param>
        public static void ShowWarning(IWin32Window owner, String message) {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowError(String format, params Object[] values) {
            ShowError(Form.ActiveForm, String.Format(format, values));
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        public static void ShowError(IWin32Window owner, String format, params Object[] values) {
            ShowError(owner, String.Format(format, values));
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="message">信息</param>
        public static void ShowError(String message) {
            ShowError(Form.ActiveForm, message);
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="message">信息</param>
        public static void ShowError(IWin32Window owner, String message) {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// 显示确认，“否”为默认按钮
        /// </summary>
        /// <param name="message">信息</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(String message) {
            return ShowConfirm(Form.ActiveForm, message, true);
        }

        /// <summary>
        /// 显示确认，“否”为默认按钮
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="message">信息</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(IWin32Window owner, String message) {
            return ShowConfirm(owner, message, true);
        }

        /// <summary>
        /// 显示确认，“否”为默认按钮
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(String format, params Object[] values) {
            return ShowConfirm(Form.ActiveForm, String.Format(format, values), true);
        }

        /// <summary>
        /// 显示确认，“否”为默认按钮
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(IWin32Window owner, String format, params Object[] values) {
            return ShowConfirm(owner, String.Format(format, values), true);
        }

        /// <summary>
        /// 显示确认
        /// </summary>
        /// <param name="format">信息格式</param>
        /// <param name="noIsDefault">“否”是否为默认按钮</param>
        /// <param name="values">信息</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(String format, bool noIsDefault, params Object[] values) {
            return ShowConfirm(Form.ActiveForm, String.Format(format, values), noIsDefault);
        }

        /// <summary>
        /// 显示确认
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="noIsDefault">“否”是否为默认按钮</param>
        /// <param name="format">String format.</param>
        /// <param name="values">Paramters used in string format.</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(IWin32Window owner, bool noIsDefault, String format, params Object[] values) {
            return ShowConfirm(owner, String.Format(format, values), noIsDefault);
        }

        /// <summary>
        /// 显示确认
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="noIsDefault">“否”是否为默认按钮</param>
        /// <returns>是否用户选择了“是”</returns>
        public static bool ShowConfirm(String message, bool noIsDefault) {
            return ShowConfirm(Form.ActiveForm, message, noIsDefault);
        }

        /// <summary>
        /// 显示确认
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="message">信息</param>
        /// <param name="noIsDefault">“否”是否为默认按钮</param>
        /// <returns>是否用户选择了“是”</returns>
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
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
        public static String ShowOpenFile(String filter) {
            return ShowOpenFile(Form.ActiveForm, null, filter, true);
        }

        /// <summary>
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
        public static String ShowOpenFile(IWin32Window owner, String filter) {
            return ShowOpenFile(owner, null, filter, true);
        }

        /// <summary>
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="originFile">初始选择的文件</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
        public static String ShowOpenFile(String originFile, String filter) {
            return ShowOpenFile(Form.ActiveForm, originFile, filter, true);
        }

        /// <summary>
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="originFile">初始选择的文件</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
        public static String ShowOpenFile(IWin32Window owner, String originFile, String filter) {
            return ShowOpenFile(owner, originFile, filter, true);
        }

        /// <summary>
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="root">初始选择的文件或初始文件夹</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <param name="isFile">真，root表示初始选择的文件；假，root表示初始文件夹</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
        public static String ShowOpenFile(String root, String filter, bool isFile) {
            return ShowOpenFile(Form.ActiveForm, root, filter, isFile);
        }

        /// <summary>
        /// 显示打开文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="root">初始选择的文件或初始文件夹</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <param name="isFile">真，root表示初始选择的文件；假，root表示初始文件夹</param>
        /// <returns>打开的文件的绝对路径，NULL表示未打开文件</returns>
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
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
        public static String ShowSaveFile(String filter) {
            return ShowSaveFile(Form.ActiveForm, null, filter, true);
        }

        /// <summary>
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
        public static String ShowSaveFile(IWin32Window owner, String filter) {
            return ShowSaveFile(owner, null, filter, true);
        }

        /// <summary>
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="originFile">初始选择的文件</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
        public static String ShowSaveFile(String originFile, String filter) {
            return ShowSaveFile(Form.ActiveForm, originFile, filter, true);
        }

        /// <summary>
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="originFile">初始选择的文件</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
        public static String ShowSaveFile(IWin32Window owner, String originFile, String filter) {
            return ShowSaveFile(owner, originFile, filter, true);
        }

        /// <summary>
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="root">初始选择的文件或初始文件夹</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <param name="isFile">真，root表示初始选择的文件；假，root表示初始文件夹</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
        public static String ShowSaveFile(String root, String filter, bool isFile) {
            return ShowSaveFile(Form.ActiveForm, root, filter, isFile);
        }

        /// <summary>
        /// 显示保存文件，若未选择文件则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="root">初始选择的文件或初始文件夹</param>
        /// <param name="filter">文件类型过滤器</param>
        /// <param name="isFile">真，root表示初始选择的文件；假，root表示初始文件夹</param>
        /// <returns>保存到的文件的绝对路径，NULL表示未指定文件</returns>
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
        /// 显示选择颜色，若未选择颜色则返回设置的初始颜色
        /// </summary>
        /// <param name="origin">初始颜色</param>
        /// <returns>选择的颜色</returns>
        public static Color ShowSelectColor(Color origin) {
            return ShowSelectColor(Form.ActiveForm, origin);
        }

        /// <summary>
        /// 显示选择颜色，若未选择颜色则返回设置的初始颜色
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="origin">初始颜色</param>
        /// <returns>选择的颜色</returns>
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
        /// 显示选择目录，若未选择任何目录则返回NULL
        /// </summary>
        /// <param name="root">初始目录</param>
        /// <returns></returns>
        public static String ShowSelectDirectory(String root) {
            return ShowSelectDirectory(Form.ActiveForm, root);
        }

        /// <summary>
        /// 显示选择目录，若未选择任何目录则返回NULL
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="root">初始目录</param>
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
        /// 显示选择字体，若未选择任何字体则返回初始字体。
        /// </summary>
        /// <param name="origin">初始字体</param>
        /// <returns>选择的字体</returns>
        public static Font ShowSelectFont(Font origin) {
            return ShowSelectFont(Form.ActiveForm, origin);
        }

        /// <summary>
        /// 显示选择字体，若未选择任何字体则返回初始字体。
        /// </summary>
        /// <param name="owner">目标窗口</param>
        /// <param name="origin">初始字体</param>
        /// <returns>选择的字体</returns>
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
