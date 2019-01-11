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
    public abstract class ModuleRegistration {
        private static void RegisterAllModules(RouteCollection routes, object state) {
            foreach(ModuleRegistration registration in TypeUtility.LoadInstances<ModuleRegistration>(BuildManager.GetReferencedAssemblies().Cast<Assembly>())) {
                registration.RegisterModule(registration.CreateContext(routes, state));
            }
        }

        public static void RegisterAllModules() {
            RegisterAllModules(null);
        }

        public static void RegisterAllModules(object state) {
            RegisterAllModules(RouteTable.Routes, state);
        }

        public ModuleRegistrationContext CreateContext(RouteCollection routes, object state) {
            ModuleRegistrationContext context = new ModuleRegistrationContext(this.ModuleName, routes, state);

            string thisNamespace = this.GetType().Namespace;
            if(thisNamespace != null) {
                context.Namespaces.Add(thisNamespace + ".*");
            }

            return context;
        }

        public abstract string ModuleName {
            get;
        }

        public abstract void RegisterModule(ModuleRegistrationContext context);
    }
}
