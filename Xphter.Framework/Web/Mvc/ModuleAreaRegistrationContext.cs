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
    public class ModuleAreaRegistrationContext : ModuleRegistrationContext {
        public ModuleAreaRegistrationContext(string moduleAreaName, string moduleName, RouteCollection routes)
            : this(moduleAreaName, moduleName, routes, null) {
        }

        public ModuleAreaRegistrationContext(string moduleAreaName, string moduleName, RouteCollection routes, object state)
            : base(moduleName, routes, state) {
            typeof(AreaRegistrationContext).GetProperty(((MemberExpression) ((Expression<Func<AreaRegistrationContext, string>>) ((obj) => obj.AreaName)).Body).Member.Name, BindingFlags.Instance | BindingFlags.Public).SetValue(this, AreaHelpers.FormatMvcAreaName(this.ModuleAreaName = moduleAreaName, this.ModuleName = moduleName), null);
        }

        public string ModuleAreaName {
            get;
            protected set;
        }
    }
}
