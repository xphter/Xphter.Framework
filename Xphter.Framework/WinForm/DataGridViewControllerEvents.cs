using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Provides data for the ColumnCreating event.
    /// </summary>
    public class DataGridViewControllerColumnEventArgs {
        /// <summary>
        /// Initialize a new instace of DataGridViewControllerColumnEventArgs class.
        /// </summary>
        /// <param name="column">The created column.</param>        
        /// <param name="name">The property name.</param>        
        public DataGridViewControllerColumnEventArgs(DataGridViewColumn column, string name) {
            this.Name = name;
            this.Column = column;
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public DataGridViewColumn Column {
            get;
            set;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents the method will used to process the ColumnCreating event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataGridViewControllerColumnEventHandler(object sender, DataGridViewControllerColumnEventArgs e);

    /// <summary>
    /// Provides data for the CellFilling event.
    /// </summary>
    public class DataGridViewControllerCellEventArgs<T> {
        /// <summary>
        /// Initialize a new instace of DataGridViewControllerCellFillingEventArgs class.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="obj">The associated object.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The cell value.</param>
        public DataGridViewControllerCellEventArgs(DataGridViewCell cell, T obj, string name, object value) {
            this.Cell = cell;
            this.Object = obj;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets current cell.
        /// </summary>
        public DataGridViewCell Cell {
            get;
            set;
        }

        /// <summary>
        /// Gets the associated object with this row.
        /// </summary>
        public T Object {
            get;
            private set;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the cell value.
        /// </summary>
        public object Value {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the method will used to process the CellFilling event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataGridViewControllerCellEventHandler<T>(object sender, DataGridViewControllerCellEventArgs<T> e);

    /// <summary>
    /// Provides data for the PreFillRow and PostFillRow event.
    /// </summary>
    public class DataGridViewControllerRowEventArgs<T> {
        /// <summary>
        /// Initialize a new instace of DataGridViewControllerRowEventArgs class.
        /// </summary>
        /// <param name="obj">The associated object.</param>
        /// <param name="rowIndex">The row index.</param>
        public DataGridViewControllerRowEventArgs(DataGridViewRow row, T obj) {
            this.Row = row;
            this.Object = obj;
        }

        /// <summary>
        /// Gets current row.
        /// </summary>
        public DataGridViewRow Row {
            get;
            private set;
        }

        /// <summary>
        /// Gets the associated object with this row.
        /// </summary>
        public T Object {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents the method will used to process the PreFillRow and PostFillRow event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataGridViewControllerRowEventHandler<T>(object sender, DataGridViewControllerRowEventArgs<T> e);
}
