using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Represents a TextBox used to input numbers only.
    /// </summary>
    public class NumericTextBox : TextBox {
        /// <summary>
        /// Indicates whether can updat Text property.
        /// </summary>
        private bool m_canUpdatText = true;

        private decimal m_minimum = 0M;
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("The minimum value")]
        public decimal Minimum {
            get {
                return m_minimum;
            }
            set {
                if(this.m_minimum != value) {
                    this.m_minimum = Math.Min(value, this.m_maximum);
                    this.m_maximum = Math.Max(value, this.m_maximum);
                    this.Value = Math.Max(this.m_minimum, Math.Min(this.m_maximum, this.m_value));
                }
            }
        }

        private decimal m_maximum = 100M;
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(100)]
        [Description("The maximum value")]
        public decimal Maximum {
            get {
                return m_maximum;
            }
            set {
                if(this.m_maximum != value) {
                    this.m_minimum = Math.Min(value, this.m_minimum);
                    this.m_maximum = Math.Max(value, this.m_minimum);
                    this.Value = Math.Max(this.m_minimum, Math.Min(this.m_maximum, this.m_value));
                }
            }
        }

        private decimal m_emptyValue = 0M;
        /// <summary>
        /// Gets or sets the default value when Text is empty.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("The maximum value")]
        public decimal EmptyValue {
            get {
                return m_emptyValue;
            }
            set {
                if(this.m_emptyValue != value) {
                    this.m_emptyValue = value;
                    if(this.m_canUpdatText) {
                        this.Text = this.m_value == value ? string.Empty : this.m_value.ToString("F" + this.m_value.GetValidDecimalPlaces());
                    }
                }
            }
        }

        private int m_decimalPlaces = 0;
        /// <summary>
        /// Gets or sets the number of decimal places.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("")]
        public int DecimalPlaces {
            get {
                return m_decimalPlaces;
            }
            set {
                if(this.m_decimalPlaces != value) {
                    this.m_decimalPlaces = value;
                    this.Value = Math.Round(this.m_value, this.m_decimalPlaces);
                }
            }
        }

        private decimal m_value = 0M;
        /// <summary>
        /// Gets or sets current value.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("The current value")]
        public decimal Value {
            get {
                return m_value;
            }
            set {
                if(this.m_value != (value = Math.Round(Math.Max(this.m_minimum, Math.Min(this.m_maximum, value)), this.m_decimalPlaces))) {
                    this.m_value = value;
                    if(this.m_canUpdatText) {
                        this.Text = value == this.EmptyValue ? string.Empty : value.ToString("F" + value.GetValidDecimalPlaces());
                    }
                    this.OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Occured when current value has changed.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Raises ValueChanged event.
        /// </summary>
        protected virtual void OnValueChanged() {
            if(this.ValueChanged != null) {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            switch(e.KeyChar) {
                case '\b':
                case '\r':
                case '\n':
                case '\t':
                case '+':
                case '-':
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    e.Handled = false;
                    break;
                default:
                    e.Handled = true;
                    break;
            }

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e) {
            this.m_canUpdatText = false;
            if(!string.IsNullOrWhiteSpace(this.Text)) {
                decimal value = 0;
                if(decimal.TryParse(this.Text, out value)) {
                    this.Value = value;
                }
            } else {
                this.Value = this.EmptyValue;
            }
            this.m_canUpdatText = true;

            base.OnTextChanged(e);
        }
    }
}
