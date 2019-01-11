using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Web.Mvc {
    public abstract class ModuleAreaRegistration {
        private static void RegisterAllModuleAreas(RouteCollection routes, object state) {
            foreach(ModuleAreaRegistration registration in TypeUtility.LoadInstances<ModuleAreaRegistration>(BuildManager.GetReferencedAssemblies().Cast<Assembly>())) {
                registration.RegisterModuleArea(registration.CreateContext(routes, state));
            }
        }

        public static void RegisterAllModuleAreas() {
            RegisterAllModuleAreas(null);
        }

        public static void RegisterAllModuleAreas(object state) {
            RegisterAllModuleAreas(RouteTable.Routes, state);
        }

        public ModuleAreaRegistrationContext CreateContext(RouteCollection routes, object state) {
            ModuleAreaRegistrationContext context = new ModuleAreaRegistrationContext(this.ModuleAreaName, this.ModuleName, routes, state);

            string thisNamespace = this.GetType().Namespace;
            if(thisNamespace != null) {
                context.Namespaces.Add(thisNamespace + ".*");
            }

            return context;
        }

        public abstract string ModuleName {
            get;
        }

        public abstract string ModuleAreaName {
            get;
        }

        public abstract void RegisterModuleArea(ModuleAreaRegistrationContext context);
    }
}
