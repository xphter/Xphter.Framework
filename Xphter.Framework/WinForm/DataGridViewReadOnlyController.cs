using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Provides a tool used to control a read-only DataGridView.
    /// </summary>
    /// <typeparam name="T">The type of objects will show by the DataGridView.</typeparam>
    public class DataGridViewReadOnlyController<T> {
        /// <summary>
        /// Initialize a new instance of DataGridViewReadOnlyController.
        /// </summary>
        /// <param name="dataGridView">A DataGridView.</param>
        /// <exception cref="System.ArgumentException"><paramref name="dataGridView"/> is null.</exception>
        public DataGridViewReadOnlyController(DataGridView dataGridView)
            : this(dataGridView, true) {
        }

        /// <summary>
        /// Initialize a new instance of DataGridViewReadOnlyController.
        /// </summary>
        /// <param name="dataGridView">A DataGridView.</param>
        /// <param name="isAutomaticUpdateView">Whether automatic update this DataGridView when the underlying object have changed.</param>
        /// <exception cref="System.ArgumentException"><paramref name="dataGridView"/> is null.</exception>
        public DataGridViewReadOnlyController(DataGridView dataGridView, bool isAutomaticUpdateView) {
            if(dataGridView == null) {
                throw new ArgumentException("dataGridView is null", "dataGridView");
            }

            this.DataGridView = dataGridView;
            this.DataGridView.ReadOnly = true;

            this.IsAutomaticUpdateView = isAutomaticUpdateView && typeof(T).IsImplements<INotifyPropertyChanged>();
        }

        /// <summary>
        /// The displayed properties.
        /// </summary>
        protected ICollection<PropertyInfo> m_properties;

        /// <summary>
        /// Indicates whether now is filling a cell.
        /// </summary>
        protected bool m_isFillingCell;

        /// <summary>
        /// Gets a value to indicate whether automatic update this DataGridView when the underlying object have changed.
        /// The default value is true.
        /// </summary>
        public virtual bool IsAutomaticUpdateView {
            get;
            protected set;
        }

        /// <summary>
        /// Gets this associated DataGridView.
        /// </summary>
        public DataGridView DataGridView {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the selected object.
        /// </summary>
        public virtual T SelectedObject {
            get {
                return this.SelectedObjects.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the selected objects collection.
        /// </summary>
        public virtual IEnumerable<T> SelectedObjects {
            get {
                ICollection<T> result = new List<T>();

                if(this.DataGridView.SelectedRows.Count > 0) {
                    foreach(DataGridViewRow row in this.DataGridView.SelectedRows) {
                        if(!(row.Tag is T)) {
                            continue;
                        }

                        result.Add((T) row.Tag);
                    }
                } else if(this.DataGridView.SelectedCells.Count > 0) {
                    foreach(DataGridViewCell cell in this.DataGridView.SelectedCells) {
                        if(!(cell.OwningRow.Tag is T)) {
                            continue;
                        }

                        result.Add((T) cell.OwningRow.Tag);
                    }
                }

                return result.Distinct();
            }
        }

        /// <summary>
        /// Creates the default DataGridViewColumn for the specified property.
        /// </summary>
        /// <param name="property">A object property info.</param>
        /// <returns>The created default column</returns>
        protected virtual DataGridViewColumn CreateColumn(PropertyInfo property) {
            if(property.PropertyType.IsInherits<Image>()) {
                return new DataGridViewImageColumn {
                    ValuesAreIcons = false,
                };
            } else if(property.PropertyType.IsInherits<Icon>()) {
                return new DataGridViewImageColumn {
                    ValuesAreIcons = true,
                };
            } else {
                return new DataGridViewTextBoxColumn();
            }
        }

        /// <summary>
        /// Fill the specified row.
        /// </summary>
        /// <param name="info">The data.</param>
        /// <param name="row">The row.</param>
        protected void FillRow(T info, DataGridViewRow row) {
            DataGridViewCellCollection cells = row.Cells;
            DataGridViewControllerCellEventArgs<T> e = null;

            row.Tag = info;
            this.OnPreFillRow(new DataGridViewControllerRowEventArgs<T>(row, info));
            this.m_isFillingCell = true;
            DataGridViewCell cell = null;
            foreach(PropertyInfo property in this.m_properties) {
                this.OnCellFilling(e = new DataGridViewControllerCellEventArgs<T>(cell = cells[property.Name], info, property.Name, property.GetValue(info, null)));

                cell.Value = e.Value;
            }
            this.m_isFillingCell = false;
            this.OnPostFillRow(new DataGridViewControllerRowEventArgs<T>(row, info));

            if(this.IsAutomaticUpdateView) {
                ((INotifyPropertyChanged) info).PropertyChanged += new PropertyChangedEventHandler(this.OnObjectPropertyChanged);
            }
        }

        /// <summary>
        /// The method used to handle the PropertyChanged event of a data object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
            T info = (T) sender;
            string propertyName = e.PropertyName;

            if(string.IsNullOrWhiteSpace(propertyName)) {
                this.Update(info);
            } else {
                DataGridViewRow row = (from r in this.DataGridView.Rows.Cast<DataGridViewRow>()
                                       where object.Equals(r.Tag, info)
                                       select r).FirstOrDefault();
                PropertyInfo property = (from p in this.m_properties
                                         where p.Name.Equals(propertyName)
                                         select p).FirstOrDefault();
                if(row != null && property != null) {
                    this.m_isFillingCell = true;
                    DataGridViewCell cell = row.Cells[propertyName];
                    DataGridViewControllerCellEventArgs<T> cellEvent = new DataGridViewControllerCellEventArgs<T>(cell, info, propertyName, property.GetValue(info, null));
                    this.OnCellFilling(cellEvent);
                    cell.Value = cellEvent.Value;
                    this.m_isFillingCell = false;
                }
            }
        }

        /// <summary>
        /// Initialize this controller.
        /// </summary>
        public virtual void Initialize() {
            string description = null;
            BrowsableAttribute browsable = null;
            DataGridViewColumn column = null;
            DataGridViewControllerColumnEventArgs e = null;

            this.DataGridView.Columns.Clear();
            this.OnPreCreateColumns();

            this.m_properties = new List<PropertyInfo>();
            foreach(PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if(property.GetIndexParameters().Length > 0) {
                    continue;
                }
                browsable = (BrowsableAttribute) property.GetCustomAttributes(typeof(BrowsableAttribute), false).FirstOrDefault();
                if(browsable != null && !browsable.Browsable) {
                    continue;
                }
                description = ((DescriptionAttribute) Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute), true) ?? new DescriptionAttribute(property.Name)).Description;

                column = this.CreateColumn(property);
                column.Name = property.Name;
                column.HeaderText = description;
                e = new DataGridViewControllerColumnEventArgs(column, property.Name);
                this.OnColumnCreating(e);

                if(e.Column == null) {
                    e.Column = new DataGridViewTextBoxColumn();
                    e.Column.HeaderText = description;
                }
                e.Column.Name = property.Name;

                this.DataGridView.Columns.Add(e.Column);
                this.m_properties.Add(property);
            }

            this.OnPostCreateColumns();
        }

        /// <summary>
        /// Use the new data to fill this DataGridView.
        /// </summary>
        /// <param name="data">The data used to fill this DataGridView.</param>
        /// <exception cref="System.ArgumentException"><paramref name="data"/> is null.</exception>
        public virtual void Refresh(IEnumerable<T> data) {
            if(data == null) {
                throw new ArgumentException("data is null", "data");
            }

            this.Clear();

            DataGridViewRow row = null;
            DataGridViewRowCollection rows = this.DataGridView.Rows;
            this.OnPreCreateRows();
            foreach(T info in data) {
                if(info == null) {
                    continue;
                }

                row = rows[rows.Add()];
                this.FillRow(info, row);
            }
            this.OnPostCreateRows();

            this.DataGridView.ClearSelection();
            if(this.DataGridView.Rows.Count > 0) {
                this.DataGridView.Rows[0].Selected = true;
            }
        }

        /// <summary>
        /// Gets the object associated with a row in the specified position.
        /// </summary>
        /// <param name="index">The row position.</param>
        /// <returns>The associated object.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero or greater than or equals the row number.</exception>
        public virtual T GetObject(int index) {
            if(index < 0 || index >= this.DataGridView.Rows.Count) {
                throw new ArgumentException("index is less than zero or greater than or equals the row number.", "index");
            }

            return (T) this.DataGridView.Rows[index].Tag;
        }

        /// <summary>
        /// Gets associated objects from this DataGridView.
        /// </summary>
        /// <returns>The associated objects.</returns>
        public virtual IEnumerable<T> GetObjects() {
            ICollection<T> result = new List<T>(this.DataGridView.Rows.Count);

            foreach(DataGridViewRow row in this.DataGridView.Rows) {
                if(!(row.Tag is T)) {
                    continue;
                }

                result.Add((T) row.Tag);
            }

            return result;
        }

        /// <summary>
        /// Append the specified object to this DataGridView.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        public virtual void Add(T info) {
            if(info == null) {
                throw new ArgumentException("info is null", "info");
            }

            this.FillRow(info, this.DataGridView.Rows[this.DataGridView.Rows.Add()]);
        }

        /// <summary>
        /// Insert the specified object to this DataGridView.
        /// If <paramref name="index"/> greater than or equals the count of rows, then append this object to this DataGridView.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        /// <param name="index">The position to insert.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero.</exception>
        public virtual void Insert(T info, int index) {
            if(info == null) {
                throw new ArgumentException("info is null", "info");
            }
            if(index < 0) {
                throw new ArgumentException("index is less than zero.", "index");
            }

            DataGridViewRowCollection rows = this.DataGridView.Rows;
            if(rows.Count == 0) {
                index = 0;
            } else if(index >= rows.Count) {
                index = rows.Count;
            }
            rows.Insert(index);
            this.FillRow(info, rows[index]);
        }

        /// <summary>
        /// Update the row in the specifeid position with the specified object.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        /// <param name="index">The row position.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero or greater than or equals the row number.</exception>
        public virtual void Update(T info, int index) {
            if(info == null) {
                throw new ArgumentException("info is null", "info");
            }
            if(index < 0 || index >= this.DataGridView.Rows.Count) {
                throw new ArgumentException("index is less than zero or greater than or equals the row number.", "index");
            }

            DataGridViewRow row = this.DataGridView.Rows[index];
            if(this.IsAutomaticUpdateView && row.Tag is T) {
                ((INotifyPropertyChanged) row.Tag).PropertyChanged -= new PropertyChangedEventHandler(this.OnObjectPropertyChanged);
            }
            this.FillRow(info, row);
        }

        /// <summary>
        /// Update a row by the specified object.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        public virtual void Update(T info) {
            if(info == null) {
                throw new ArgumentException("info is null", "info");
            }

            DataGridViewRow row = (from r in this.DataGridView.Rows.Cast<DataGridViewRow>()
                                   where object.Equals(r.Tag, info)
                                   select r).FirstOrDefault();
            if(row != null) {
                this.Update(info, row.Index);
            }
        }

        /// <summary>
        /// Remove the row in the specified position.
        /// </summary>
        /// <param name="index">The row position.</param>
        /// <exception cref="System.ArgumentException"><paramref name="index"/> is less than zero or greater than or equals the row number.</exception>
        public virtual void Remove(int index) {
            if(index < 0 || index >= this.DataGridView.Rows.Count) {
                throw new ArgumentException("index is less than zero or greater than or equals the row number.", "index");
            }

            DataGridViewRow row = this.DataGridView.Rows[index];
            if(this.IsAutomaticUpdateView && row.Tag is T) {
                ((INotifyPropertyChanged) row.Tag).PropertyChanged -= new PropertyChangedEventHandler(this.OnObjectPropertyChanged);
            }
            this.DataGridView.Rows.RemoveAt(index);
        }

        /// <summary>
        /// Remove the row associated the specified object.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        public virtual void Remove(T info) {
            if(info == null) {
                throw new ArgumentException("info is null", "info");
            }

            DataGridViewRow row = (from r in this.DataGridView.Rows.Cast<DataGridViewRow>()
                                   where object.Equals(r.Tag, info)
                                   select r).FirstOrDefault();
            if(row != null) {
                this.DataGridView.Rows.RemoveAt(row.Index);
            }
        }

        /// <summary>
        /// Remove all rows in this DataGridView.
        /// </summary>
        public virtual void Clear() {
            if(this.IsAutomaticUpdateView) {
                foreach(DataGridViewRow row in this.DataGridView.Rows) {
                    if(row.Tag == null) {
                        continue;
                    }

                    ((INotifyPropertyChanged) row.Tag).PropertyChanged -= new PropertyChangedEventHandler(this.OnObjectPropertyChanged);
                }
            }

            this.DataGridView.Rows.Clear();
        }

        #region PreCreateColumns Event

        /// <summary>
        /// Occurs when before creating columns.
        /// </summary>
        public event EventHandler PreCreateColumns;

        /// <summary>
        /// Raises the PreCreateColumns event.
        /// </summary>
        protected virtual void OnPreCreateColumns() {
            if(this.PreCreateColumns != null) {
                this.PreCreateColumns(this, EventArgs.Empty);
            }
        }

        #endregion

        #region ColumnCreating Event

        /// <summary>
        /// Occurs when creating a column.
        /// </summary>
        public event DataGridViewControllerColumnEventHandler ColumnCreating;

        /// <summary>
        /// Raises the ColumnCreating event.
        /// </summary>
        /// <param name="e">A ColumnCreatingEventArgs object.</param>
        protected virtual void OnColumnCreating(DataGridViewControllerColumnEventArgs e) {
            if(this.ColumnCreating != null) {
                this.ColumnCreating(this, e);
            }
        }

        #endregion

        #region PostCreateColumns Event

        /// <summary>
        /// Occurs when after creating columns.
        /// </summary>
        public event EventHandler PostCreateColumns;

        /// <summary>
        /// Raises the PostCreateColumns event.
        /// </summary>
        protected virtual void OnPostCreateColumns() {
            if(this.PostCreateColumns != null) {
                this.PostCreateColumns(this, EventArgs.Empty);
            }
        }

        #endregion

        #region PreCreateRows Event

        /// <summary>
        /// Occurs when before creating rows.
        /// </summary>
        public event EventHandler PreCreateRows;

        /// <summary>
        /// Raises the PreCreateRows event.
        /// </summary>
        protected virtual void OnPreCreateRows() {
            if(this.PreCreateRows != null) {
                this.PreCreateRows(this, EventArgs.Empty);
            }
        }

        #endregion

        #region PreFillRow Event

        /// <summary>
        /// Occurs when before filling row.
        /// </summary>
        public event DataGridViewControllerRowEventHandler<T> PreFillRow;

        /// <summary>
        /// Raises the PreFillRow event.
        /// </summary>
        /// <param name="e">A DataGridViewControllerRowEventArgs object.</param>
        protected virtual void OnPreFillRow(DataGridViewControllerRowEventArgs<T> e) {
            if(this.PreFillRow != null) {
                this.PreFillRow(this, e);
            }
        }

        #endregion

        #region CellFilling Event

        /// <summary>
        /// Occurs when filling a cell.
        /// </summary>
        public event DataGridViewControllerCellEventHandler<T> CellFilling;

        /// <summary>
        /// Raises the CellFilling event.
        /// </summary>
        /// <param name="e">A DataGridViewControllerCellEventArgs object.</param>
        protected virtual void OnCellFilling(DataGridViewControllerCellEventArgs<T> e) {
            if(this.CellFilling != null) {
                this.CellFilling(this, e);
            }
        }

        #endregion

        #region PostFillRow Event

        /// <summary>
        /// Occurs when after filling row.
        /// </summary>
        public event DataGridViewControllerRowEventHandler<T> PostFillRow;

        /// <summary>
        /// Raises the PostFillRow event.
        /// </summary>
        /// <param name="e">A DataGridViewControllerRowEventArgs object.</param>
        protected virtual void OnPostFillRow(DataGridViewControllerRowEventArgs<T> e) {
            if(this.PostFillRow != null) {
                this.PostFillRow(this, e);
            }
        }

        #endregion

        #region PostCreateRows Event

        /// <summary>
        /// Occurs when after creating rows.
        /// </summary>
        public event EventHandler PostCreateRows;

        /// <summary>
        /// Raises the PostCreateRows event.
        /// </summary>
        protected virtual void OnPostCreateRows() {
            if(this.PostCreateRows != null) {
                this.PostCreateRows(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
