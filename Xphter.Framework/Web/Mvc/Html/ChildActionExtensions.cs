using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Xphter.Framework.Web.Mvc.Html {
    public static class ChildActionExtensions {
        #region Action

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName) {
            return ModularAction(helper, actionName, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, object arguments) {
            return ModularAction(helper, actionName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, RouteValueDictionary arguments) {
            return helper.Action(actionName, arguments);
        }

        #endregion

        #region Action and Controller

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName) {
            return ModularAction(helper, actionName, controllerName, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, object arguments) {
            return ModularAction(helper, actionName, controllerName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, RouteValueDictionary arguments) {
            return helper.Action(actionName, controllerName, arguments);
        }

        #endregion

        #region Action and Controller and Area

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName) {
            return ModularAction(helper, actionName, controllerName, areaName, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, object arguments) {
            return ModularAction(helper, actionName, controllerName, areaName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, RouteValueDictionary arguments) {
            return helper.Action(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, AreaHelpers.GetModuleName(helper.ViewContext.RouteData), arguments));
        }

        #endregion

        #region Action and Controller and Area and Module

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName) {
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, object arguments) {
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, RouteValueDictionary arguments) {
            return helper.Action(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, arguments));
        }

        #endregion

        #region Expression

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression) where TController : ControllerBase {
            return ModularAction(helper, expression, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, object arguments) where TController : ControllerBase {
            return ModularAction<TController>(helper, expression, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, RouteValueDictionary arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression) where TController : ControllerBase {
            return ModularAction(helper, expression, (RouteValueDictionary) null);
        }

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, object arguments) where TController : ControllerBase {
            return ModularAction<TController>(helper, expression, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static MvcHtmlString ModularAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, RouteValueDictionary arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        #endregion

        #region Action

        public static void ModularRenderAction(this HtmlHelper helper, string actionName) {
            ModularRenderAction(helper, actionName, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, object arguments) {
            ModularRenderAction(helper, actionName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, RouteValueDictionary arguments) {
            helper.RenderAction(actionName, arguments);
        }

        #endregion

        #region Action and Controller

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName) {
            ModularRenderAction(helper, actionName, controllerName, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, object arguments) {
            ModularRenderAction(helper, actionName, controllerName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, RouteValueDictionary arguments) {
            helper.RenderAction(actionName, controllerName, arguments);
        }

        #endregion

        #region Action and Controller and Area

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName) {
            ModularRenderAction(helper, actionName, controllerName, areaName, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, object arguments) {
            ModularRenderAction(helper, actionName, controllerName, areaName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, RouteValueDictionary arguments) {
            helper.RenderAction(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, AreaHelpers.GetModuleName(helper.ViewContext.RouteData), arguments));
        }

        #endregion

        #region Action and Controller and Area and Module

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName) {
            ModularRenderAction(helper, actionName, controllerName, areaName, moduleName, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, object arguments) {
            ModularRenderAction(helper, actionName, controllerName, areaName, moduleName, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, RouteValueDictionary arguments) {
            helper.RenderAction(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, arguments));
        }

        #endregion

        #region Expression

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression) where TController : ControllerBase {
            ModularRenderAction(helper, expression, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, object arguments) where TController : ControllerBase {
            ModularRenderAction<TController>(helper, expression, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, RouteValueDictionary arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            ModularRenderAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression) where TController : ControllerBase {
            ModularRenderAction(helper, expression, (RouteValueDictionary) null);
        }

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, object arguments) where TController : ControllerBase {
            ModularRenderAction<TController>(helper, expression, RouteHelpers.ToRouteViewDictionary(arguments));
        }

        public static void ModularRenderAction<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, RouteValueDictionary arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            ModularRenderAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        #endregion
    }
}
