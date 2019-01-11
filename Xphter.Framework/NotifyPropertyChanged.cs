using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides a common implementation of INotifyPropertyChanged interface.
    /// </summary>
    [Serializable]
    public class NotifyPropertyChanged : INotifyPropertyChanged {
        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected internal virtual void OnPropertyChanged(string propertyName) {
            if(this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the changed property.</typeparam>
        /// <param name="expression">A lambda expression, such as () => this.PropertyName.</param>
        /// <exception cref="System.ArgumentException">The body of <paramref name="expression"/> must be a member expression.</exception>
        protected internal virtual void OnPropertyChanged<T>(Expression<Func<T>> expression) {
            if(expression != null) {
                MemberExpression member = expression.Body as MemberExpression;
                if(member == null) {
                    throw new ArgumentException("The body of this lambda expression must be a member expression.", "expression");
                }

                this.OnPropertyChanged(((MemberExpression) expression.Body).Member.Name);
            } else {
                this.OnPropertyChanged(null);
            }
        }

        #region INotifyPropertyChanged Members

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
