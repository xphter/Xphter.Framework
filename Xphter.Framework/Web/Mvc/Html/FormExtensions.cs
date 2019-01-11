using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Xphter.Framework.Web.Mvc.Html {
    public static class FormExtensions {
        #region Action

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName) {
            return ModularBeginForm(helper, actionName, FormMethod.Post, (object) null, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, FormMethod method) {
            return ModularBeginForm(helper, actionName, method, (object) null, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, FormMethod method, object arguments) {
            return ModularBeginForm(helper, actionName, method, arguments, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, FormMethod method, object arguments, object htmlAttributes) {
            return helper.BeginForm(actionName, null, arguments, method, htmlAttributes);
        }

        #endregion

        #region Action and Controller

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName) {
            return ModularBeginForm(helper, actionName, controllerName, FormMethod.Post, (object) null, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, FormMethod method) {
            return ModularBeginForm(helper, actionName, controllerName, method, (object) null, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, FormMethod method, object arguments) {
            return ModularBeginForm(helper, actionName, controllerName, method, arguments, (object) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, FormMethod method, object arguments, object htmlAttributes) {
            return helper.BeginForm(actionName, controllerName, arguments, method, htmlAttributes);
        }

        #endregion

        #region Action and Controller and Area

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, FormMethod.Post, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, FormMethod method) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, method, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, FormMethod method, object arguments) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, method, arguments, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, FormMethod method, object arguments, object htmlAttributes) {
            return helper.BeginForm(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, AreaHelpers.GetModuleName(helper.ViewContext.RouteData), arguments), method, LinkExtensions.GetHtmlAttributes(htmlAttributes));
        }

        #endregion

        #region Action and Controller and Area and Module

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, moduleName, FormMethod.Post, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, FormMethod method) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, moduleName, method, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, FormMethod method, object arguments) {
            return ModularBeginForm(helper, actionName, controllerName, areaName, moduleName, method, arguments, (IDictionary<string, object>) null);
        }

        public static MvcForm ModularBeginForm(this HtmlHelper helper, string actionName, string controllerName, string areaName, string moduleName, FormMethod method, object arguments, object htmlAttributes) {
            return helper.BeginForm(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, arguments), method, LinkExtensions.GetHtmlAttributes(htmlAttributes));
        }

        #endregion

        #region Expression

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, FormMethod.Post, null, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, FormMethod method) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, method, null, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, FormMethod method, object arguments) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, method, arguments, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, FormMethod method, object arguments, object htmlAttributes) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularBeginForm(helper, actionName, controllerName, areaName, moduleName, method, arguments, htmlAttributes);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> expression) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, FormMethod.Post, null, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, FormMethod method) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, method, null, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, FormMethod method, object arguments) where TController : ControllerBase {
            return ModularBeginForm<TController>(helper, expression, method, arguments, null);
        }

        public static MvcForm ModularBeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, FormMethod method, object arguments, object htmlAttributes) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularBeginForm(helper, actionName, controllerName, areaName, moduleName, method, arguments, htmlAttributes);
        }

        #endregion
    }
}
