using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xphter.Framework.Web.Mvc {
    public class ModuleRegistrationContext : AreaRegistrationContext {
        public ModuleRegistrationContext(string moduleName, RouteCollection routes)
            : this(moduleName, routes, null) {
        }

        public ModuleRegistrationContext(string moduleName, RouteCollection routes, object state)
            : base(moduleName, routes, state) {
            typeof(AreaRegistrationContext).GetProperty(((MemberExpression) ((Expression<Func<AreaRegistrationContext, string>>) ((obj) => obj.AreaName)).Body).Member.Name, BindingFlags.Instance | BindingFlags.Public).SetValue(this, AreaHelpers.FormatMvcAreaName(this.ModuleName = moduleName), null);
        }

        public string ModuleName {
            get;
            protected set;
        }
    }
}
