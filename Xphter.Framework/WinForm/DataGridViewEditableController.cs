using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Provides a tool used to control a editable DataGridView.
    /// </summary>
    /// <typeparam name="T">The type of objects will edit by the DataGridView.</typeparam>
    public class DataGridViewEditableController<T> : DataGridViewReadOnlyController<T> where T : new() {
        /// <summary>
        /// Initialize a new instance of DataGridViewEditableController.
        /// </summary>
        /// <param name="dataGridView">A DataGridView.</param>
        /// <exception cref="System.ArgumentException"><paramref name="dataGridView"/> is null.</exception>
        public DataGridViewEditableController(DataGridView dataGridView)
            : this(dataGridView, true, false) {
        }

        /// <summary>
        /// Initialize a new instance of DataGridViewEditableController.
        /// </summary>
        /// <param name="dataGridView">A DataGridView.</param>
        /// <param name="isAutomaticUpdateView">Whether automatic update this DataGridView when the underlying object have changed.</param>
        /// <param name="isAutomaticUpdateDataSource">Whether automatic update data source when the value of cell has changed.</param>
        /// <exception cref="System.ArgumentException"><paramref name="dataGridView"/> is null.</exception>
        public DataGridViewEditableController(DataGridView dataGridView, bool isAutomaticUpdateView, bool isAutomaticUpdateDataSource)
            : base(dataGridView, isAutomaticUpdateView) {
            this.DataGridView.ReadOnly = false;
            if(this.IsAutomaticUpdateDataSource = isAutomaticUpdateDataSource) {
                this.DataGridView.CellValueChanged += delegate(object sender, DataGridViewCellEventArgs e) {
                    if(e.RowIndex < 0 || e.ColumnIndex < 0 || this.m_isFillingCell) {
                        return;
                    }

                    DataGridViewRow row = this.DataGridView.Rows[e.RowIndex];
                    DataGridViewColumn column = this.DataGridView.Columns[e.ColumnIndex];
                    PropertyInfo property = (from p in this.m_properties
                                             where p.Name.Equals(column.Name)
                                             select p).FirstOrDefault();
                    if(row.Tag is T && property != null) {
                        T info = (T) row.Tag;
                        DataGridViewCell cell = row.Cells[property.Name];
                        DataGridViewControllerCellEventArgs<T> cellEvent = new DataGridViewControllerCellEventArgs<T>(cell, info, property.Name, cell.Value);
                        this.OnCellPicking(cellEvent);
                        property.SetValue(info, cellEvent.Value, null);
                    }
                };
            }
        }

        /// <summary>
        /// Gets a value to indicate whether automatic update data source when the value of cell has changed.
        /// The default value is false.
        /// </summary>
        public virtual bool IsAutomaticUpdateDataSource {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicate whether pick value from read-only cells.
        /// Default value is false.
        /// </summary>
        public bool IsPickReadOnlyValue {
            get;
            set;
        }

        /// <inheritdoc />
        protected override DataGridViewColumn CreateColumn(PropertyInfo property) {
            DataGridViewColumn column = null;

            if(property.PropertyType.IsBoolean()) {
                column = new DataGridViewCheckBoxColumn();
            } else if(property.PropertyType.IsString()) {
                column = new DataGridViewTextBoxColumn();
            } else if(property.PropertyType.IsNumber()) {
                column = new DataGridViewNumericTextBoxColumn();
            } else if(property.PropertyType.IsDateTime()) {
                column = new DataGridViewCalendarColumn();
            } else if(property.PropertyType.IsEnum) {
                column = new DataGridViewComboBoxColumn();
                typeof(DropdownUtility).GetMethod(
                    ((MethodCallExpression) (((Expression<Action>) (() => DropdownUtility.Fill<BindingFlags>((ListControl) null, false))).Body)).Method.Name,
                    new Type[] { typeof(DataGridViewComboBoxColumn), typeof(bool) }).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { column, false });
            } else if(property.PropertyType.IsInherits<Image>()) {
                column = new DataGridViewImageColumn {
                    ValuesAreIcons = false,
                };
            } else if(property.PropertyType.IsInherits<Icon>()) {
                column = new DataGridViewImageColumn {
                    ValuesAreIcons = true,
                };
            } else {
                column = new DataGridViewTextBoxColumn();
            }

            return column;
        }

        /// <summary>
        /// Initialize the associated object by the specified row.
        /// </summary>
        /// <param name="row">A DataGridViewRow.</param>
        /// <returns>The associated object.</returns>
        protected virtual T FromRow(DataGridViewRow row) {
            T info = (T) row.Tag;

            DataGridViewCell cell = null;
            DataGridViewCellCollection cells = row.Cells;
            DataGridViewControllerCellEventArgs<T> e = null;
            foreach(PropertyInfo property in this.m_properties) {
                if((cell = cells[property.Name]).ReadOnly && !this.IsPickReadOnlyValue) {
                    continue;
                }

                this.OnCellPicking(e = new DataGridViewControllerCellEventArgs<T>(cell, info, property.Name, cell.Value));
                property.SetValue(info, e.Value, null);
            }

            return info;
        }

        /// <summary>
        /// Gets the associated object with a row in the specified position.
        /// </summary>
        /// <param name="index">The row position.</param>
        /// <returns>The associated object.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero or greater than or equals the row number.</exception>
        public override T GetObject(int index) {
            if(index < 0 || index >= this.DataGridView.Rows.Count) {
                throw new ArgumentException("index is less than zero or greater than or equals the row number.", "index");
            }
            DataGridViewRow row = this.DataGridView.Rows[index];
            if(!(row.Tag is T)) {
                return default(T);
            }

            return this.FromRow(row);
        }

        /// <summary>
        /// Gets the associated objects from this DataGridView.
        /// </summary>
        /// <returns>The associated objects.</returns>
        public override IEnumerable<T> GetObjects() {
            ICollection<T> result = new List<T>(this.DataGridView.Rows.Count);

            foreach(DataGridViewRow row in this.DataGridView.Rows) {
                if(!(row.Tag is T)) {
                    continue;
                }

                result.Add(this.FromRow(row));
            }

            return result;
        }

        /// <summary>
        /// Append a new object to this DataGridView.
        /// </summary>
        /// <returns>The created object.</returns>
        public virtual T Add() {
            T info = new T();
            this.Add(info);
            return info;
        }

        /// <summary>
        /// Insert a new object to this DataGridView.
        /// If <paramref name="index"/> greater than or equals the count of rows, then append this object to this DataGridView.
        /// </summary>
        /// <param name="index">The position to insert.</param>
        /// <returns>The created object.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero.</exception>
        public virtual T Insert(int index) {
            if(index < 0) {
                throw new ArgumentException("index is less than zero.", "index");
            }

            T info = new T();
            this.Insert(info, index);
            return info;
        }

        #region CellPicking Event

        /// <summary>
        /// Occurs when picking up the value of a cell.
        /// </summary>
        public event DataGridViewControllerCellEventHandler<T> CellPicking;

        /// <summary>
        /// Raises the CellFilling event.
        /// </summary>
        /// <param name="e">A DataGridViewControllerCellEventArgs object.</param>
        protected virtual void OnCellPicking(DataGridViewControllerCellEventArgs<T> e) {
            if(this.CellPicking != null) {
                this.CellPicking(this, e);
            }
        }

        #endregion
    }
}