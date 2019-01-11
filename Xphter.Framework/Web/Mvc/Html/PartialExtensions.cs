using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Xphter.Framework.Web.Mvc.Html {
    public static class PartialExtensions {
        static PartialExtensions() {
            g_mvcResources = typeof(HtmlHelper).Assembly.GetType("System.Web.Mvc.Properties.MvcResources");
        }

        private static Type g_mvcResources;

        /// <summary>
        /// Identical copy from MVC4 source code: HtmlHelper.FindPartialView.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="viewContext"></param>
        /// <param name="partialViewName"></param>
        /// <param name="viewEngineCollection"></param>
        /// <returns></returns>
        private static IView FindPartialView(HtmlHelper helper, ViewContext viewContext, string partialViewName, ViewEngineCollection viewEngineCollection) {
            ViewEngineResult result = viewEngineCollection.FindPartialView(viewContext, partialViewName);
            if(result.View != null) {
                return result.View;
            }

            StringBuilder locationsText = new StringBuilder();
            foreach(string location in result.SearchedLocations) {
                locationsText.AppendLine();
                locationsText.Append(location);
            }

            throw new InvalidOperationException(String.Format(
                CultureInfo.CurrentCulture,
                (string) g_mvcResources.GetProperty("Common_PartialViewNotFound", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null),
                partialViewName,
                locationsText));
        }

        /// <summary>
        /// Overload HtmlHelper.RenderPartialInternal in MVC4 source code.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="partialViewName"></param>
        /// <param name="controllerName"></param>
        /// <param name="areaName"></param>
        /// <param name="moduleName"></param>
        /// <param name="viewData"></param>
        /// <param name="model"></param>
        /// <param name="writer"></param>
        /// <param name="viewEngineCollection"></param>
        internal static void RenderPartialInternal(HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, ViewDataDictionary viewData, object model, TextWriter writer, ViewEngineCollection viewEngineCollection) {
            if(String.IsNullOrEmpty(partialViewName)) {
                throw new ArgumentException(
                    (string) g_mvcResources.GetProperty("Common_NullOrEmpty", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null),
                    "partialViewName");
            }

            ViewDataDictionary newViewData = null;

            if(model == null) {
                if(viewData == null) {
                    newViewData = new ViewDataDictionary(helper.ViewData);
                } else {
                    newViewData = new ViewDataDictionary(viewData);
                }
            } else {
                if(viewData == null) {
                    newViewData = new ViewDataDictionary(model);
                } else {
                    newViewData = new ViewDataDictionary(viewData) {
                        Model = model
                    };
                }
            }

            ViewContext newViewContext = new ViewContext(helper.ViewContext, helper.ViewContext.View, newViewData, helper.ViewContext.TempData, writer);

            /*
             * modify here: save and change and restore the controller name, area name and module name.
             */
            object controllerBackup = null;
            object areaBackup = null;
            if(!string.IsNullOrWhiteSpace(controllerName)) {
                if(helper.ViewContext.RouteData.Values.ContainsKey("controller")) {
                    controllerBackup = helper.ViewContext.RouteData.Values["controller"];
                }
                newViewContext.RouteData.Values["controller"] = controllerName;
            }
            if(!string.IsNullOrWhiteSpace(areaName) || !string.IsNullOrWhiteSpace(moduleName)) {
                if(helper.ViewContext.RouteData.DataTokens.ContainsKey("area")) {
                    areaBackup = helper.ViewContext.RouteData.DataTokens["area"];
                }
                newViewContext.RouteData.DataTokens["area"] = AreaHelpers.FormatMvcAreaName(areaName, moduleName);
            }

            IView view = FindPartialView(helper, newViewContext, partialViewName, viewEngineCollection);
            view.Render(newViewContext, writer);

            /*
             * modify here: restore the controller name, area name and module name.
             */
            if(controllerBackup != null) {
                helper.ViewContext.RouteData.Values["controller"] = controllerBackup;
            }
            if(areaBackup != null) {
                helper.ViewContext.RouteData.DataTokens["area"] = areaBackup;
            }
        }

        #region Partial

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName) {
            return helper.Partial(partialViewName);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, ViewDataDictionary viewData) {
            return helper.Partial(partialViewName, viewData);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, object model) {
            return helper.Partial(partialViewName, model);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, object model, ViewDataDictionary viewData) {
            return helper.Partial(partialViewName, model, viewData);
        }

        #endregion

        #region Partial: Controller

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName) {
            return ModularPartial(helper, partialViewName, controllerName, null, null, null, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, ViewDataDictionary viewData) {
            return ModularPartial(helper, partialViewName, controllerName, null, null, null, viewData);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, object model) {
            return ModularPartial(helper, partialViewName, controllerName, null, null, model, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, object model, ViewDataDictionary viewData) {
            return ModularPartial(helper, partialViewName, controllerName, null, null, model, viewData);
        }

        #endregion

        #region Partial: Controller and Area

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, null, null, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, ViewDataDictionary viewData) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, null, null, viewData);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, object model) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, null, model, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, object model, ViewDataDictionary viewData) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, null, model, viewData);
        }

        #endregion

        #region Partial: Controller and Area and Module

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, moduleName, null, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, ViewDataDictionary viewData) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, moduleName, null, viewData);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, object model) {
            return ModularPartial(helper, partialViewName, controllerName, areaName, moduleName, model, null);
        }

        public static MvcHtmlString ModularPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, object model, ViewDataDictionary viewData) {
            using(StringWriter writer = new StringWriter(CultureInfo.CurrentCulture)) {
                RenderPartialInternal(helper, partialViewName, controllerName, areaName, moduleName, viewData, model, writer, ViewEngines.Engines);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        #endregion

        #region Partial: Exresssion

        public static MvcHtmlString ModularPartial<TController>(this HtmlHelper helper, string partialViewName) where TController : ControllerBase {
            return ModularPartial<TController>(helper, partialViewName, null, null);
        }

        public static MvcHtmlString ModularPartial<TController>(this HtmlHelper helper, string partialViewName, ViewDataDictionary viewData) where TController : ControllerBase {
            return ModularPartial<TController>(helper, partialViewName, null, viewData);
        }

        public static MvcHtmlString ModularPartial<TController>(this HtmlHelper helper, string partialViewName, object model) where TController : ControllerBase {
            return ModularPartial<TController>(helper, partialViewName, model, null);
        }

        public static MvcHtmlString ModularPartial<TController>(this HtmlHelper helper, string partialViewName, object model, ViewDataDictionary viewData) where TController : ControllerBase {
            string controllerName, areaName, moduleName;
            controllerName = UrlHelperExtensions.GetControllerName<TController>();
            UrlHelperExtensions.DetermineAreaAndModule<TController>(helper.RouteCollection, out areaName, out moduleName);
            return ModularPartial(helper, partialViewName, controllerName, areaName, moduleName, model, viewData);
        }

        #endregion

        #region RenderPartial

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName) {
            helper.RenderPartial(partialViewName, null, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, ViewDataDictionary viewData) {
            helper.RenderPartial(partialViewName, null, viewData);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, Object model) {
            helper.RenderPartial(partialViewName, model, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, Object model, ViewDataDictionary viewData) {
            helper.RenderPartial(partialViewName, model, viewData);
        }

        #endregion

        #region RenderPartial: Controller

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName) {
            ModularRenderPartial(helper, partialViewName, controllerName, null, null, null, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, ViewDataDictionary viewData) {
            ModularRenderPartial(helper, partialViewName, controllerName, null, null, null, viewData);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, Object model) {
            ModularRenderPartial(helper, partialViewName, controllerName, null, null, model, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, Object model, ViewDataDictionary viewData) {
            ModularRenderPartial(helper, partialViewName, controllerName, null, null, model, viewData);
        }

        #endregion

        #region RenderPartial: Controller and Area

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, null, null, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, ViewDataDictionary viewData) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, null, null, viewData);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, Object model) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, null, model, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, Object model, ViewDataDictionary viewData) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, null, model, viewData);
        }

        #endregion

        #region RenderPartial: Controller and Area and Module

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, moduleName, null, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, ViewDataDictionary viewData) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, moduleName, null, viewData);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, Object model) {
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, moduleName, model, null);
        }

        public static void ModularRenderPartial(this HtmlHelper helper, string partialViewName, string controllerName, string areaName, string moduleName, Object model, ViewDataDictionary viewData) {
            RenderPartialInternal(helper, partialViewName, controllerName, areaName, moduleName, viewData, model, helper.ViewContext.Writer, ViewEngines.Engines);
        }

        #endregion

        #region RenderPartial: Expression

        public static void ModularRenderPartial<TController>(this HtmlHelper helper, string partialViewName) where TController : ControllerBase {
            ModularRenderPartial<TController>(helper, partialViewName, null, null);
        }

        public static void ModularRenderPartial<TController>(this HtmlHelper helper, string partialViewName, ViewDataDictionary viewData) where TController : ControllerBase {
            ModularRenderPartial<TController>(helper, partialViewName, null, viewData);
        }

        public static void ModularRenderPartial<TController>(this HtmlHelper helper, string partialViewName, Object model) where TController : ControllerBase {
            ModularRenderPartial<TController>(helper, partialViewName, model, null);
        }

        public static void ModularRenderPartial<TController>(this HtmlHelper helper, string partialViewName, Object model, ViewDataDictionary viewData) where TController : ControllerBase {
            string controllerName, areaName, moduleName;
            controllerName = UrlHelperExtensions.GetControllerName<TController>();
            UrlHelperExtensions.DetermineAreaAndModule<TController>(helper.RouteCollection, out areaName, out moduleName);
            ModularRenderPartial(helper, partialViewName, controllerName, areaName, moduleName, model, viewData);
        }

        #endregion
    }
}
