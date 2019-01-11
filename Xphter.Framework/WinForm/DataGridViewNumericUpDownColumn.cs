using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Represents a DataGridViewColumn to display numeric value.
    /// </summary>
    public class DataGridViewNumericUpDownColumn : DataGridViewColumn {
        public DataGridViewNumericUpDownColumn()
            : base(new DataGridViewNumericUpDownCell()) {
        }

        [Browsable(true)]
        [Description("The value when the spin box display a empty string.")]
        [DefaultValue(0)]
        public decimal EmptyValue {
            get {
                return this.NumericUpDownCellTemplate.EmptyValue;
            }
            set {
                this.NumericUpDownCellTemplate.EmptyValue = value;
            }
        }

        [Browsable(true)]
        [Description("Number of digits after decimal point.")]
        [DefaultValue(0)]
        public int DecimalPlaces {
            get {
                return this.NumericUpDownCellTemplate.DecimalPlaces;
            }
            set {
                this.NumericUpDownCellTemplate.DecimalPlaces = Math.Max(0, value);
            }
        }

        [Browsable(true)]
        [Description("The value to increment or decrement the spin box (also known as an up-down control) when the up or down buttons are clicked.")]
        [DefaultValue(0)]
        public decimal Increment {
            get {
                return this.NumericUpDownCellTemplate.Increment;
            }
            set {
                this.NumericUpDownCellTemplate.Increment = Math.Max(0, value);
            }
        }

        [Browsable(true)]
        [Description("Whether a thousands separator is displayed in the spin box (also known as an up-down control) when appropriate.")]
        [DefaultValue(false)]
        public bool ThousandsSeparator {
            get {
                return this.NumericUpDownCellTemplate.ThousandsSeparator;
            }
            set {
                this.NumericUpDownCellTemplate.ThousandsSeparator = value;
            }
        }

        [Browsable(true)]
        [Description("Whether the spin box (also known as an up-down control) should display the value it contains in hexadecimal format.")]
        [DefaultValue(0)]
        public bool Hexadecimal {
            get {
                return this.NumericUpDownCellTemplate.Hexadecimal;
            }
            set {
                this.NumericUpDownCellTemplate.Hexadecimal = value;
            }
        }

        [Browsable(true)]
        [Description("Maximum value.")]
        [DefaultValue(100)]
        public decimal Maximum {
            get {
                return this.NumericUpDownCellTemplate.Maximum;
            }
            set {
                this.NumericUpDownCellTemplate.Maximum = value;
            }
        }

        [Browsable(true)]
        [Description("Minimum value.")]
        [DefaultValue(0)]
        public decimal Minimum {
            get {
                return this.NumericUpDownCellTemplate.Minimum;
            }
            set {
                this.NumericUpDownCellTemplate.Minimum = value;
            }
        }

        [Browsable(false)]
        public override DataGridViewCell CellTemplate {
            get {
                return base.CellTemplate;
            }
            set {
                if(value != null && !(value is DataGridViewNumericUpDownCell)) {
                    throw new InvalidCastException("Numeric Column Cell Template must be a DataGridViewNumericCell.");
                }

                base.CellTemplate = value;
            }
        }

        [Browsable(false)]
        public virtual DataGridViewNumericUpDownCell NumericUpDownCellTemplate {
            get {
                return (DataGridViewNumericUpDownCell) this.CellTemplate;
            }
        }

        public override object Clone() {
            DataGridViewNumericUpDownColumn column = (DataGridViewNumericUpDownColumn) base.Clone();
            column.EmptyValue = this.EmptyValue;
            column.DecimalPlaces = this.DecimalPlaces;
            column.Increment = this.Increment;
            column.ThousandsSeparator = this.ThousandsSeparator;
            column.Hexadecimal = this.Hexadecimal;
            column.Maximum = this.Maximum;
            column.Minimum = this.Minimum;
            return column;
        }
    }

    /// <summary>
    /// Represents a DataGridViewCell which can display and edit a numeric value.
    /// </summary>
    public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell {
        public DataGridViewNumericUpDownCell() {
            this.Maximum = decimal.MaxValue;
        }

        /// <summary>
        /// Gets or sets the value when the spin box display a empty string.
        /// </summary>
        public decimal EmptyValue {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of decimal places.
        /// </summary>
        public int DecimalPlaces {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to increment or decrement the spin box (also known as an up-down control) when the up or down buttons are clicked.
        /// </summary>
        public decimal Increment {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a thousands separator is displayed in the spin box (also known as an up-down control) when appropriate.
        /// </summary>
        public bool ThousandsSeparator {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the spin box (also known as an up-down control) should display the value it contains in hexadecimal format.
        /// </summary>
        public bool Hexadecimal {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public decimal Maximum {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public decimal Minimum {
            get;
            set;
        }

        #region Override DataGridViewCell Members

        public override Type ValueType {
            get {
                return typeof(decimal);
            }
        }

        public override object DefaultNewRowValue {
            get {
                return 0M;
            }
        }

        public override Type EditType {
            get {
                return typeof(DataGridViewNumericUpDownEditingControl);
            }
        }

        public override bool KeyEntersEditMode(KeyEventArgs e) {
            string negativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;

            if(char.IsDigit((char) e.KeyCode) ||
                e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 ||
                !string.IsNullOrWhiteSpace(negativeSign) && (char) e.KeyCode == negativeSign[0] ||
                e.KeyCode == Keys.Subtract ||
                e.KeyCode == Keys.Decimal &&
                !e.Shift && !e.Control && !e.Alt) {
                return true;
            }

            return false;
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            DataGridViewNumericUpDownEditingControl control = (DataGridViewNumericUpDownEditingControl) this.DataGridView.EditingControl;
            control.DecimalPlaces = this.DecimalPlaces;
            control.Increment = this.Increment;
            control.ThousandsSeparator = this.ThousandsSeparator;
            control.Hexadecimal = this.Hexadecimal;
            control.Maximum = this.Maximum;
            control.Minimum = this.Minimum;
            control.Value = Convert.ToDecimal(this.Value != null && !Convert.IsDBNull(this.Value) ? this.Value : this.DefaultNewRowValue);
            control.Text = (string) initialFormattedValue ?? string.Empty;
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context) {
            if(value == null || Convert.IsDBNull(value)) {
                return string.Empty;
            }

            decimal digit = Convert.ToDecimal(value);
            if(digit == this.EmptyValue) {
                return string.Empty;
            } else {
                return digit.ToString(string.Format("{0}{1}", this.ThousandsSeparator ? 'N' : 'F', this.DecimalPlaces));
            }
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter) {
            string formattedValueString = (string) formattedValue;
            if(string.IsNullOrWhiteSpace(formattedValueString)) {
                return this.EmptyValue;
            } else {
                decimal value = 0M;
                decimal.TryParse(formattedValueString, out value);
                return value;
                //return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            }
        }

        public override object Clone() {
            DataGridViewNumericUpDownCell cell = (DataGridViewNumericUpDownCell) base.Clone();
            cell.DecimalPlaces = this.DecimalPlaces;
            cell.Increment = this.Increment;
            cell.ThousandsSeparator = this.ThousandsSeparator;
            cell.Hexadecimal = this.Hexadecimal;
            cell.Maximum = this.Maximum;
            cell.Minimum = this.Minimum;
            return cell;
        }

        public override string ToString() {
            return string.Format("DataGridViewNumericUpDownCell {{ ColumnIndex={0}, RowIndex={1} }}", this.ColumnIndex, this.RowIndex);
        }

        #endregion
    }

    /// <summary>
    /// Editing control used in DataGridViewNumericCell.
    /// </summary>
    public class DataGridViewNumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl {
        public DataGridViewNumericUpDownEditingControl()
            : base() {
            this.TabStop = false;
        }

        #region IDataGridViewEditingControl Members

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) {
            switch(dataGridViewCellStyle.Alignment) {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.BottomLeft:
                    this.TextAlign = HorizontalAlignment.Left;
                    break;
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    this.TextAlign = HorizontalAlignment.Center;
                    break;
                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.BottomRight:
                    this.TextAlign = HorizontalAlignment.Right;
                    break;
            }
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
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
                    decimal number = 0M;
                    decimal.TryParse((string) value, out number);
                    this.Value = number;
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
            return !dataGridViewWantsInputKey;
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

        protected override void OnKeyPress(KeyPressEventArgs e) {
            this.EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);

            base.OnKeyPress(e);
        }

        protected override void OnValueChanged(EventArgs eventargs) {
            this.EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);

            base.OnValueChanged(eventargs);
        }
    }
}
