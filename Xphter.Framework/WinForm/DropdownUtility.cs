using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Presents a tool used to fill a drop-down list.
    /// </summary>
    public static class DropdownUtility {
        /// <summary>
        /// Fill a dropdown list.
        /// </summary>
        /// <typeparam name="T">The type of the dropdown list.</typeparam>
        /// <param name="list">The dropdown list.</param>
        /// <param name="items">The data source.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        private static void FillDropdown<T>(T list, IEnumerable<DropdownItem> items, bool firstIsEmpty) {
            if(list == null) {
                throw new ArgumentException("Dropdown is null.", "list");
            }

            typeof(T).GetProperty("DisplayMember", BindingFlags.Public | BindingFlags.Instance).SetValue(list, DropdownItem.DisplayMember, null);
            typeof(T).GetProperty("ValueMember", BindingFlags.Public | BindingFlags.Instance).SetValue(list, DropdownItem.ValueMember, null);
            if(items == null) {
                items = Enumerable.Empty<DropdownItem>();
            }
            if(firstIsEmpty) {
                items = new DropdownItem[] { DropdownItem.Empty }.Concat(items);
            }
            typeof(T).GetProperty("DataSource", BindingFlags.Public | BindingFlags.Instance).SetValue(list, items.ToArray(), null);
        }

        /// <summary>
        /// Fill a dropdown list by the specified Enum type.
        /// </summary>
        /// <typeparam name="TList">The type of the dropdown list.</typeparam>
        /// <typeparam name="TEnum">The type of a Enum.</typeparam>
        /// <param name="list">The dropdown list.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        private static void FillDropdown<TList, TEnum>(TList list, bool firstIsEmpty) {
            Type enumType = typeof(TEnum);
            if(!enumType.IsEnum) {
                throw new ArgumentException("TEnum not represents a Enum type.", "TEnum");
            }

            ICollection<DropdownItem> items = new List<DropdownItem>();
            foreach(Enum value in Enum.GetValues(enumType)) {
                items.Add(new DropdownItem(EnumUtility.GetDescription(value), value));
            }
            FillDropdown<TList>(list, items, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified data.
        /// </summary>
        /// <param name="list">A ListControl.</param>
        /// <param name="items">The data used to fill <paramref name="list"/>.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill(ListControl list, IEnumerable<DropdownItem> items, bool firstIsEmpty) {
            FillDropdown<ListControl>(list, items, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified data.
        /// </summary>
        /// <param name="list">A DataGridViewComboBoxColumn.</param>
        /// <param name="items">The data used to fill <paramref name="list"/>.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill(DataGridViewComboBoxColumn list, IEnumerable<DropdownItem> items, bool firstIsEmpty) {
            FillDropdown<DataGridViewComboBoxColumn>(list, items, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified data.
        /// </summary>
        /// <param name="list">A DataGridViewComboBoxCell.</param>
        /// <param name="items">The data used to fill <paramref name="list"/>.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill(DataGridViewComboBoxCell list, IEnumerable<DropdownItem> items, bool firstIsEmpty) {
            FillDropdown<DataGridViewComboBoxCell>(list, items, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified data.
        /// </summary>
        /// <param name="list">A ToolStripComboBox.</param>
        /// <param name="items">The data used to fill <paramref name="list"/>.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill(ToolStripComboBox list, IEnumerable<DropdownItem> items, bool firstIsEmpty) {
            FillDropdown<ComboBox>(list.ComboBox, items, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified Enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of a Enum.</typeparam>
        /// <param name="list">A ListControl.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill<TEnum>(ListControl list, bool firstIsEmpty) where TEnum : struct {
            FillDropdown<ListControl, TEnum>(list, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified Enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of a Enum.</typeparam>
        /// <param name="list">A DataGridViewComboBoxColumn.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill<TEnum>(DataGridViewComboBoxColumn list, bool firstIsEmpty) where TEnum : struct {
            FillDropdown<DataGridViewComboBoxColumn, TEnum>(list, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified Enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of a Enum.</typeparam>
        /// <param name="list">A DataGridViewComboBoxCell.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill<TEnum>(DataGridViewComboBoxCell list, bool firstIsEmpty) where TEnum : struct {
            FillDropdown<DataGridViewComboBoxCell, TEnum>(list, firstIsEmpty);
        }

        /// <summary>
        /// Fill a list by the specified Enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of a Enum.</typeparam>
        /// <param name="list">A ToolStripComboBox.</param>
        /// <param name="firstIsEmpty">True: the first item should be empty.</param>
        public static void Fill<TEnum>(ToolStripComboBox list, bool firstIsEmpty) where TEnum : struct {
            FillDropdown<ComboBox, TEnum>(list.ComboBox, firstIsEmpty);
        }
    }

    /// <summary>
    /// The data item used to bind a drop-down list.
    /// </summary>
    public class DropdownItem {
        /// <summary>
        /// Initialize a new instance of DropDownItem class.
        /// </summary>
        /// <param name="display">Display.</param>
        /// <param name="value">Value.</param>
        public DropdownItem(string display, object value) {
            this.Display = display;
            this.Value = value;
        }

        /// <summary>
        /// The name of display member.
        /// </summary>
        internal static string DisplayMember = "Display";

        /// <summary>
        /// The name of value member.
        /// </summary>
        internal static string ValueMember = "Value";

        /// <summary>
        /// Gets display.
        /// </summary>
        public string Display {
            get;
            private set;
        }

        /// <summary>
        /// Gets value.
        /// </summary>
        public object Value {
            get;
            private set;
        }

        private static DropdownItem g_empty;
        /// <summary>
        /// Gets the empty DropdownItem。
        /// </summary>
        public static DropdownItem Empty {
            get {
                if(g_empty == null) {
                    g_empty = new DropdownItem(string.Empty, null);
                }
                return g_empty;
            }
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Display;
        }
    }
}
