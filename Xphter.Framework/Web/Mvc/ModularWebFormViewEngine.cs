using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Xphter.Framework.Web.Mvc {
    internal class ModularWebFormViewEngine : ModularViewEngine {
        public ModularWebFormViewEngine()
            : this(null) {
        }

        public ModularWebFormViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator) {
            this.ModuleAreaViewLocationFormats = new[] {
                "~/Modules/{3}/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Modules/{3}/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Modules/{3}/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Modules/{3}/Areas/{2}/Views/Shared/{0}.ascx",
            };
            this.ModuleAreaMasterLocationFormats = new[] {
                "~/Modules/{3}/Areas/{2}/Views/{1}/{0}.master",
                "~/Modules/{3}/Areas/{2}/Views/Shared/{0}.master",
            };

            this.ModuleViewLocationFormats = new[] {
                "~/Modules/{2}/Views/{1}/{0}.aspx",
                "~/Modules/{2}/Views/{1}/{0}.ascx",
                "~/Modules/{2}/Views/Shared/{0}.aspx",
                "~/Modules/{2}/Views/Shared/{0}.ascx"
            };
            this.ModuleMasterLocationFormats = new[] {
                "~/Modules/{2}/Views/{1}/{0}.master",
                "~/Modules/{2}/Views/Shared/{0}.master"
            };

            this.AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.ascx",
            };
            this.AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.master",
                "~/Areas/{2}/Views/Shared/{0}.master",
            };

            this.ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx"
            };
            this.MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.master",
                "~/Views/Shared/{0}.master"
            };

            this.PartialViewLocationFormats = this.ViewLocationFormats;
            this.AreaPartialViewLocationFormats = this.AreaViewLocationFormats;
            this.ModulePartialViewLocationFormats = this.ModuleViewLocationFormats;
            this.ModuleAreaPartialViewLocationFormats = this.ModuleAreaViewLocationFormats;

            this.FileExtensions = new[] {
                "aspx",
                "ascx",
                "master",
            };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
            return new WebFormView(controllerContext, partialPath, null, ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath) {
            return new WebFormView(controllerContext, viewPath, masterPath, ViewPageActivator);
        }
    }
}
