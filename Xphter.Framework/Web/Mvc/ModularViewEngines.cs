using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Xphter.Framework.Web.Mvc {
    public static class ModularViewEngines {
        public static void Initialize() {
            ViewEngineCollection engines = ViewEngines.Engines;
            engines.Clear();
            engines.Add(new ModularWebFormViewEngine());
            engines.Add(new ModularRazorViewEngine());

            ModuleAreaRegistration.RegisterAllModuleAreas();
            ModuleRegistration.RegisterAllModules();
            AreaRegistration.RegisterAllAreas();
        }
    }
}
