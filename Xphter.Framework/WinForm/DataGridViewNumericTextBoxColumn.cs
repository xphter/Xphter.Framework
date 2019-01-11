using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Represents a DataGridViewNumericTextBoxColumn to display numeric value.
    /// </summary>
    public class DataGridViewNumericTextBoxColumn : DataGridViewColumn {
        public DataGridViewNumericTextBoxColumn()
            : base(new DataGridViewNumericTextBoxCell()) {
        }

        [Browsable(true)]
        [Description("The value when the spin box display a empty string.")]
        [DefaultValue(0)]
        public decimal EmptyValue {
            get {
                return this.NumericTextBoxCellTemplate.EmptyValue;
            }
            set {
                this.NumericTextBoxCellTemplate.EmptyValue = value;
            }
        }

        [Browsable(true)]
        [Description("Number of digits after decimal point.")]
        [DefaultValue(0)]
        public int DecimalPlaces {
            get {
                return this.NumericTextBoxCellTemplate.DecimalPlaces;
            }
            set {
                this.NumericTextBoxCellTemplate.DecimalPlaces = Math.Max(0, value);
            }
        }

        [Browsable(true)]
        [Description("Maximum value.")]
        [DefaultValue(100000000)]
        public decimal Maximum {
            get {
                return this.NumericTextBoxCellTemplate.Maximum;
            }
            set {
                this.NumericTextBoxCellTemplate.Maximum = value;
            }
        }

        [Browsable(true)]
        [Description("Minimum value.")]
        [DefaultValue(0)]
        public decimal Minimum {
            get {
                return this.NumericTextBoxCellTemplate.Minimum;
            }
            set {
                this.NumericTextBoxCellTemplate.Minimum = value;
            }
        }

        [Browsable(false)]
        public override DataGridViewCell CellTemplate {
            get {
                return base.CellTemplate;
            }
            set {
                if(value != null && !(value is DataGridViewNumericTextBoxCell)) {
                    throw new InvalidCastException("Numeric Column Cell Template must be a DataGridViewNumericTextBoxCell.");
                }

                base.CellTemplate = value;
            }
        }

        [Browsable(false)]
        public virtual DataGridViewNumericTextBoxCell NumericTextBoxCellTemplate {
            get {
                return (DataGridViewNumericTextBoxCell) this.CellTemplate;
            }
        }

        public override object Clone() {
            DataGridViewNumericTextBoxColumn column = (DataGridViewNumericTextBoxColumn) base.Clone();
            column.EmptyValue = this.EmptyValue;
            column.DecimalPlaces = this.DecimalPlaces;
            column.Maximum = this.Maximum;
            column.Minimum = this.Minimum;
            return column;
        }
    }

    /// <summary>
    /// Represents a DataGridViewNumricTextBoxCell which can display and edit a numeric value.
    /// </summary>
    public class DataGridViewNumericTextBoxCell : DataGridViewTextBoxCell {
        public DataGridViewNumericTextBoxCell() {
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
                return typeof(DataGridViewNumericTextBoxEditingControl);
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

            DataGridViewNumericTextBoxEditingControl control = (DataGridViewNumericTextBoxEditingControl) this.DataGridView.EditingControl;
            control.DecimalPlaces = this.DecimalPlaces;
            control.Maximum = this.Maximum;
            control.Minimum = this.Minimum;
            control.Value = Convert.ToDecimal(this.Value != null && !Convert.IsDBNull(this.Value) ? this.Value : this.DefaultNewRowValue);
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context) {
            if(value == null || Convert.IsDBNull(value)) {
                return string.Empty;
            }

            decimal digit = Convert.ToDecimal(value);
            if(digit == this.EmptyValue) {
                return string.Empty;
            } else {
                return digit.ToString("F" + Math.Min(this.DecimalPlaces, digit.GetValidDecimalPlaces()));
            }
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter) {
            string formattedValueString = (string) formattedValue;
            if(string.IsNullOrWhiteSpace(formattedValueString)) {
                return this.EmptyValue;
            } else {
                decimal value = 0M;
                decimal.TryParse(formattedValueString, out value);
                return Math.Round(Math.Max(this.Minimum, Math.Min(this.Maximum, value)), this.DecimalPlaces);
            }
        }

        public override object Clone() {
            DataGridViewNumericTextBoxCell cell = (DataGridViewNumericTextBoxCell) base.Clone();
            cell.DecimalPlaces = this.DecimalPlaces;
            cell.Maximum = this.Maximum;
            cell.Minimum = this.Minimum;
            return cell;
        }

        public override string ToString() {
            return string.Format("DataGridViewNumericTextBoxCell {{ ColumnIndex={0}, RowIndex={1} }}", this.ColumnIndex, this.RowIndex);
        }

        #endregion
    }

    /// <summary>
    /// Editing control used in DataGridViewNumericTextBoxCell.
    /// </summary>
    public class DataGridViewNumericTextBoxEditingControl : NumericTextBox, IDataGridViewEditingControl {
        public DataGridViewNumericTextBoxEditingControl()
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
                return this.Value.ToString("F" + this.Value.GetValidDecimalPlaces());
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

        protected override void OnValueChanged() {
            this.EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);

            base.OnValueChanged();
        }
    }
}
