using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Represents a DataGridViewColumn to display datetime value.
    /// </summary>
    public class DataGridViewCalendarColumn : DataGridViewColumn {
        public DataGridViewCalendarColumn()
            : base(new DataGridViewCalendarCell()) {
        }

        /// <summary>
        /// Gets or set custom DateTime format.
        /// </summary>
        [Browsable(true)]
        [Description("Custom DateTime format")]
        public string CustomFormat {
            get {
                return ((DataGridViewCalendarCell) this.CellTemplate).CustomFormat;
            }
            set {
                ((DataGridViewCalendarCell) this.CellTemplate).CustomFormat = value;
            }
        }

        /// <summary>
        /// Gets or set DateTime format type.
        /// </summary>
        [Browsable(true)]
        [Description("DateTime format type")]
        public DateTimePickerFormat Format {
            get {
                return ((DataGridViewCalendarCell) this.CellTemplate).Format;
            }
            set {
                ((DataGridViewCalendarCell) this.CellTemplate).Format = value;
            }
        }

        public override DataGridViewCell CellTemplate {
            get {
                return base.CellTemplate;
            }
            set {
                if(value != null && !(value is DataGridViewCalendarCell)) {
                    throw new InvalidCastException("Calendar Column Cell Template must be a DataGridViewCalendarCell.");
                }

                base.CellTemplate = value;
            }
        }

        public override object Clone() {
            DataGridViewCalendarColumn column = (DataGridViewCalendarColumn) base.Clone();
            column.CustomFormat = this.CustomFormat;
            column.Format = this.Format;
            return column;
        }
    }

    /// <summary>
    /// Represents a DataGridViewCell which can display and edit a datetime.
    /// </summary>
    public class DataGridViewCalendarCell : DataGridViewTextBoxCell {
        /// <summary>
        /// Gets or set custom DateTime format.
        /// </summary>
        public string CustomFormat {
            get;
            set;
        }

        private DateTimePickerFormat m_format = DateTimePickerFormat.Long;
        /// <summary>
        /// Gets or set DateTime format type.
        /// </summary>
        public DateTimePickerFormat Format {
            get {
                return this.m_format;
            }
            set {
                this.m_format = value;
            }
        }

        #region Override DataGridViewCell Members

        public override Type ValueType {
            get {
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue {
            get {
                return DateTime.Now;
            }
        }

        public override Type EditType {
            get {
                return typeof(DataGridViewCalendarEditingControl);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            DataGridViewCalendarEditingControl control = (DataGridViewCalendarEditingControl) this.DataGridView.EditingControl;
            control.CustomFormat = this.CustomFormat;
            control.Format = this.Format;
            control.Value = (DateTime) (this.Value ?? this.DefaultNewRowValue);
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context) {
            if(value == null) {
                return string.Empty;
            }

            string formattedValue = string.Empty;
            DateTime time = (DateTime) value;
            switch(this.Format) {
                case DateTimePickerFormat.Custom:
                    formattedValue = time.ToString(this.CustomFormat);
                    break;
                case DateTimePickerFormat.Long:
                    formattedValue = time.ToString("G");
                    break;
                case DateTimePickerFormat.Short:
                    formattedValue = time.ToString("g");
                    break;
                default:
                    formattedValue = time.ToString();
                    break;
            }

            return formattedValue;
        }

        #endregion

        public override object Clone() {
            DataGridViewCalendarCell cell = (DataGridViewCalendarCell) base.Clone();
            cell.CustomFormat = this.CustomFormat;
            cell.Format = this.Format;
            return cell;
        }
    }

    /// <summary>
    /// Editing control used in DataGridViewCalendarCell.
    /// </summary>
    public class DataGridViewCalendarEditingControl : DateTimePicker, IDataGridViewEditingControl {
        #region IDataGridViewEditingControl Members

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
            if(!string.IsNullOrEmpty(dataGridViewCellStyle.Format)) {
                this.CustomFormat = dataGridViewCellStyle.Format;
                this.Format = DateTimePickerFormat.Custom;
            }
        }

        public DataGridView EditingControlDataGridView {
            get;
            set;
        }

        public object EditingControlFormattedValue {
            get {
                return this.Text;
            }
            set {
                if(value is string) {
                    DateTime time = DateTime.Now;
                    DateTime.TryParse((string) value, out time);
                    this.Value = time;
                }
            }
        }

        public int EditingControlRowIndex {
            get;
            set;
        }

        public bool EditingControlValueChanged {
            get;
            set;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) {
            switch(keyData) {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public Cursor EditingPanelCursor {
            get {
                return this.Cursor;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) {
            return this.EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll) {
        }

        public bool RepositionEditingControlOnValueChange {
            get {
                return false;
            }
        }

        #endregion

        protected override void OnValueChanged(EventArgs eventargs) {
            this.EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);

            base.OnValueChanged(eventargs);
        }
    }
}
