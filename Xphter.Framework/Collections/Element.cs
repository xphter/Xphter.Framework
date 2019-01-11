using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a element which has two components.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Element<T1, T2> {
        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element() {
        }

        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element(T1 component1, T2 component2) {
            this.Component1 = component1;
            this.Component2 = component2;
        }

        /// <summary>
        /// Gets or sets the component 1.
        /// </summary>
        public T1 Component1 {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the component 2.
        /// </summary>
        public T2 Component2 {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{{{0}, {1}}}", this.Component1, this.Component2);
        }
    }

    /// <summary>
    /// Represents a element which has three components.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class Element<T1, T2, T3> : Element<T1, T2> {
        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element() {
        }

        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element(T1 component1, T2 component2, T3 component3)
            : base(component1, component2) {
            this.Component3 = component3;
        }

        /// <summary>
        /// Gets or sets the component 3.
        /// </summary>
        public T3 Component3 {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{{{0}, {1}, {2}}}", this.Component1, this.Component2, this.Component3);
        }
    }

    /// <summary>
    /// Represents a element which has four components.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class Element<T1, T2, T3, T4> : Element<T1, T2, T3> {
        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element() {
        }

        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element(T1 component1, T2 component2, T3 component3, T4 component4)
            : base(component1, component2, component3) {
            this.Component4 = component4;
        }

        /// <summary>
        /// Gets or sets the component 4.
        /// </summary>
        public T4 Component4 {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{{{0}, {1}, {2}, {3}}}", this.Component1, this.Component2, this.Component3, this.Component4);
        }
    }

    /// <summary>
    /// Represents a element which has five components.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public class Element<T1, T2, T3, T4, T5> : Element<T1, T2, T3, T4> {
        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element() {
        }

        /// <summary>
        /// Initializes a new instance of Element class.
        /// </summary>
        public Element(T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
            : base(component1, component2, component3, component4) {
            this.Component5 = component5;
        }

        /// <summary>
        /// Gets or sets the component 5.
        /// </summary>
        public T5 Component5 {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{{{0}, {1}, {2}, {3}, {4}}}", this.Component1, this.Component2, this.Component3, this.Component4, this.Component5);
        }
    }
}
