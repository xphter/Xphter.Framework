using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Xphter.Framework.Web.Mvc {
    internal class ModularRazorViewEngine : ModularViewEngine {
        public ModularRazorViewEngine()
            : this(null) {
        }

        public ModularRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator) {
            this.ModuleAreaViewLocationFormats = new[] {
                "~/Modules/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Modules/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Modules/{3}/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            this.ModuleViewLocationFormats = new[] {
                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.vbhtml"
            };

            this.AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            this.ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };

            this.MasterLocationFormats = this.ViewLocationFormats;
            this.PartialViewLocationFormats = this.ViewLocationFormats;
            this.AreaMasterLocationFormats = this.AreaViewLocationFormats;
            this.AreaPartialViewLocationFormats = this.AreaViewLocationFormats;
            this.ModuleMasterLocationFormats = this.ModuleViewLocationFormats;
            this.ModulePartialViewLocationFormats = this.ModuleViewLocationFormats;
            this.ModuleAreaMasterLocationFormats = this.ModuleAreaViewLocationFormats;
            this.ModuleAreaPartialViewLocationFormats = this.ModuleAreaViewLocationFormats;

            FileExtensions = new[] {
                "cshtml",
                "vbhtml",
            };

            this.m_displayModeProviderProperty = typeof(RazorView).GetProperty(RAZOR_VIEW_DISPLAYMODEPROVIDER_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
            if(this.m_displayModeProviderProperty == null) {
                throw new InvalidOperationException(string.Format("Type {0} not contains {1} property.", typeof(RazorView).Name, RAZOR_VIEW_DISPLAYMODEPROVIDER_PROPERTY_NAME));
            }
        }

        internal static readonly string ViewStartFileName = "_ViewStart";
        private const string RAZOR_VIEW_DISPLAYMODEPROVIDER_PROPERTY_NAME = "DisplayModeProvider";
        private PropertyInfo m_displayModeProviderProperty;

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
            IView view = new RazorView(controllerContext, partialPath, layoutPath: null, runViewStartPages: false, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator);
            this.m_displayModeProviderProperty.SetValue(view, this.DisplayModeProvider, null);
            return view;
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath) {
            IView view = new RazorView(controllerContext, viewPath, layoutPath: masterPath, runViewStartPages: true, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator);
            this.m_displayModeProviderProperty.SetValue(view, this.DisplayModeProvider, null);
            return view;
        }
    }
}
