using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Xphter.Framework.Web.Mvc.Html {
    public static class LinkExtensions {
        internal static IDictionary<string, object> GetHtmlAttributes(object htmlAttributes) {
            if(htmlAttributes == null) {
                return null;
            }

            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach(PropertyInfo property in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                if(!property.CanRead || property.GetIndexParameters().Length > 0) {
                    continue;
                }

                result[property.Name] = property.GetValue(htmlAttributes, null);
            }

            return result;
        }

        #region Action

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName) {
            return ModularActionLink(helper, linkText, actionName, (object) null, (object) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, object arguments) {
            return ModularActionLink(helper, linkText, actionName, arguments, (object) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, object arguments, object htmlAttributes) {
            return helper.ActionLink(linkText, actionName, arguments, htmlAttributes);
        }

        #endregion

        #region Action and Controller

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName) {
            return ModularActionLink(helper, linkText, actionName, controllerName, (object) null, (object) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object arguments) {
            return ModularActionLink(helper, linkText, actionName, controllerName, arguments, (object) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object arguments, object htmlAttributes) {
            return helper.ActionLink(linkText, actionName, controllerName, arguments, htmlAttributes);
        }

        #endregion

        #region Action and Controller and Area

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName) {
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName, object arguments) {
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, arguments, (IDictionary<string, object>) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName, object arguments, object htmlAttributes) {
            return helper.ActionLink(linkText, actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, AreaHelpers.GetModuleName(helper.ViewContext.RouteData), arguments), GetHtmlAttributes(htmlAttributes));
        }

        #endregion

        #region Action and Controller and Area and Module

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName, string moduleName) {
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, moduleName, (object) null, (IDictionary<string, object>) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName, string moduleName, object arguments) {
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, moduleName, arguments, (IDictionary<string, object>) null);
        }

        public static MvcHtmlString ModularActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName, string moduleName, object arguments, object htmlAttributes) {
            return helper.ActionLink(linkText, actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, arguments), GetHtmlAttributes(htmlAttributes));
        }

        #endregion

        #region Expression

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, string linkText) where TController : ControllerBase {
            return ModularActionLink<TController>(helper, expression, linkText, null, null);
        }

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, string linkText, object arguments) where TController : ControllerBase {
            return ModularActionLink<TController>(helper, expression, linkText, arguments, null);
        }

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Func<TController, object>> expression, string linkText, object arguments, object htmlAttributes) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, moduleName, arguments, htmlAttributes);
        }

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, string linkText) where TController : ControllerBase {
            return ModularActionLink<TController>(helper, expression, linkText, null, null);
        }

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, string linkText, object arguments) where TController : ControllerBase {
            return ModularActionLink<TController>(helper, expression, linkText, arguments, null);
        }

        public static MvcHtmlString ModularActionLink<TController>(this HtmlHelper helper, Expression<Action<TController>> expression, string linkText, object arguments, object htmlAttributes) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            UrlHelperExtensions.DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularActionLink(helper, linkText, actionName, controllerName, areaName, moduleName, arguments, htmlAttributes);
        }

        #endregion
    }
}
