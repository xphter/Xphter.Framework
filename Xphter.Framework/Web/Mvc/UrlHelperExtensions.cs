using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq.Expressions;
using System.Reflection;

namespace Xphter.Framework.Web.Mvc {
    public static class UrlHelperExtensions {
        internal static string GetActionName<TController>(LambdaExpression expression) where TController : ControllerBase {
            if(expression.Body.NodeType != ExpressionType.Call) {
                throw new ArgumentException("The expression body must be a method call expression.", "expression");
            }

            return GetActionName<TController>(((MethodCallExpression) expression.Body).Method.Name);
        }

        internal static string GetActionName<TController>(string actionName) where TController : ControllerBase {
            if(actionName.EndsWith("Async")) {
                string methodName = actionName.Substring(0, actionName.Length - "Async".Length);
                if(typeof(TController).GetMethod(methodName + "Completed", BindingFlags.Public | BindingFlags.Instance) != null) {
                    actionName = methodName;
                }
            }

            return actionName;
        }

        internal static string GetControllerName<TController>() where TController : ControllerBase {
            Type controllerType = typeof(TController);
            string controllerTypeName = controllerType.Name;
            if(!controllerTypeName.EndsWith("Controller")) {
                throw new ArgumentException("The name of controller type must be end of \"Controller\"");
            }

            return controllerTypeName.Substring(0, controllerTypeName.Length - "Controller".Length);
        }

        internal static void DetermineAreaAndModule<TController>(RouteCollection routes, out string areaName, out string moduleName) where TController : ControllerBase {
            areaName = moduleName = null;

            string controllerNamespace = typeof(TController).Namespace;
            if(string.IsNullOrEmpty(controllerNamespace)) {
                throw new ArgumentException("The controller type is not defines a namespace.");
            }

            foreach(RouteBase route in from r in routes
                                       let ns = RouteHelpers.GetMvcNamespaces(r)
                                       where ns != null && ns.Contains(controllerNamespace)
                                       select r) {
                if(!string.IsNullOrEmpty(areaName) && !string.IsNullOrEmpty(moduleName)) {
                    break;
                }

                if(string.IsNullOrEmpty(areaName)) {
                    areaName = AreaHelpers.GetAreaName(route);
                }
                if(string.IsNullOrEmpty(moduleName)) {
                    moduleName = AreaHelpers.GetModuleName(route);
                }
            }
        }

        internal static void DetermineParameters<TController>(LambdaExpression expression, RouteCollection routes, out string actionName, out string controllerName, out string areaName, out string moduleName) where TController : ControllerBase {
            actionName = controllerName = areaName = moduleName = null;
            actionName = GetActionName<TController>(expression);
            controllerName = GetControllerName<TController>();
            DetermineAreaAndModule<TController>(routes, out areaName, out moduleName);
        }

        #region Action

        public static string ModularAction(this UrlHelper helper, string actionName) {
            return ModularAction(helper, actionName, (object) null);
        }

        public static string ModularAction(this UrlHelper helper, string actionName, object arguments) {
            return helper.Action(actionName, arguments);
        }

        #endregion

        #region Action and Controller

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName) {
            return ModularAction(helper, actionName, controllerName, (object) null);
        }

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, object arguments) {
            return helper.Action(actionName, controllerName, arguments);
        }

        #endregion

        #region Action and Controller and Area

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, string areaName) {
            return ModularAction(helper, actionName, controllerName, areaName, (object) null);
        }

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, string areaName, object arguments) {
            return helper.Action(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, AreaHelpers.GetModuleName(helper.RequestContext.RouteData), arguments));
        }

        #endregion

        #region Action and Controller and Area and Module

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, string areaName, string moduleName) {
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, (object) null);
        }

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, string areaName, string moduleName, object arguments) {
            return helper.Action(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, arguments));
        }

        public static string ModularAction(this UrlHelper helper, string actionName, string controllerName, string areaName, string moduleName, IDictionary<string, object> dictionary) {
            return helper.Action(actionName, controllerName, AreaHelpers.GetRouteViewDictionary(areaName, moduleName, dictionary));
        }

        #endregion

        #region Expression

        public static string ModularAction<TController>(this UrlHelper helper, string actionName) where TController : ControllerBase {
            return ModularAction<TController>(helper, actionName, (object) null);
        }

        public static string ModularAction<TController>(this UrlHelper helper, string actionName, object arguments) where TController : ControllerBase {
            string areaName, moduleName;
            DetermineAreaAndModule<TController>(helper.RouteCollection, out areaName, out moduleName);
            return ModularAction(helper, GetActionName<TController>(actionName), GetControllerName<TController>(), areaName, moduleName, arguments);
        }

        public static string ModularAction<TController>(this UrlHelper helper, string actionName, IDictionary<string, object> dictionary) where TController : ControllerBase {
            string areaName, moduleName;
            DetermineAreaAndModule<TController>(helper.RouteCollection, out areaName, out moduleName);
            return ModularAction(helper, GetActionName<TController>(actionName), GetControllerName<TController>(), areaName, moduleName, dictionary);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Func<TController, object>> expression) where TController : ControllerBase {
            return ModularAction<TController>(helper, expression, null);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Func<TController, object>> expression, object arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Func<TController, object>> expression, IDictionary<string, object> dictionary) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, dictionary);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Action<TController>> expression) where TController : ControllerBase {
            return ModularAction<TController>(helper, expression, null);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Action<TController>> expression, object arguments) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, arguments);
        }

        public static string ModularAction<TController>(this UrlHelper helper, Expression<Action<TController>> expression, IDictionary<string, object> dictionary) where TController : ControllerBase {
            string actionName, controllerName, areaName, moduleName;
            DetermineParameters<TController>(expression, helper.RouteCollection, out actionName, out controllerName, out areaName, out moduleName);
            return ModularAction(helper, actionName, controllerName, areaName, moduleName, dictionary);
        }

        #endregion
    }
}
