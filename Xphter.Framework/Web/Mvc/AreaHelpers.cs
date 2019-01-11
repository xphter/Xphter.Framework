using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xphter.Framework.Web.Mvc {
    internal static class AreaHelpers {
        private const string MODULE_FORMAT = "Modules/{0}";
        private const string MODULE_AREA_FORMAT = "Modules/{1}/Areas/{0}";
        private static readonly Regex g_moduleRegex = new Regex(@"^Modules\/(?'moduleName'[\w\-\$\%]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex g_moduleAreaRegex = new Regex(@"^Modules\/(?'moduleName'[\w\-\$\%]+)\/Areas\/(?'areaName'[\w\-\$\%]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        internal static string GetMvcAreaName(RouteBase route) {
            IRouteWithArea routeWithArea = route as IRouteWithArea;
            if(routeWithArea != null) {
                return routeWithArea.Area;
            }

            Route castRoute = route as Route;
            if(castRoute != null && castRoute.DataTokens != null) {
                return castRoute.DataTokens["area"] as string;
            }

            return null;
        }

        internal static string GetMvcAreaName(RouteData routeData) {
            object area;
            if(routeData.DataTokens.TryGetValue("area", out area)) {
                return area as string;
            }

            return GetMvcAreaName(routeData.Route);
        }

        private static string GetModuleNameFromMvcAreaName(string mvcAreaName) {
            if(string.IsNullOrEmpty(mvcAreaName)) {
                return null;
            }

            string moduleName = null;
            Match match = g_moduleRegex.Match(mvcAreaName);
            if(match == null || !match.Success) {
                match = g_moduleAreaRegex.Match(mvcAreaName);
            }
            if(match != null && match.Success) {
                moduleName = match.Groups["moduleName"].Value;
            }

            return moduleName;
        }

        private static string GetAreaNameFromMvcAreaName(string mvcAreaName) {
            if(string.IsNullOrEmpty(mvcAreaName)) {
                return null;
            }

            Match match = g_moduleRegex.Match(mvcAreaName);
            if(match != null && match.Success) {
                return null;
            }

            string areaName = mvcAreaName;
            match = g_moduleAreaRegex.Match(mvcAreaName);
            if(match != null && match.Success) {
                areaName = match.Groups["areaName"].Value;
            }

            return areaName;
        }

        public static string GetAreaName(RouteBase route) {
            return GetAreaNameFromMvcAreaName(GetMvcAreaName(route));
        }

        public static string GetAreaName(RouteData routeData) {
            return GetAreaNameFromMvcAreaName(GetMvcAreaName(routeData));
        }

        public static string GetModuleName(RouteBase route) {
            return GetModuleNameFromMvcAreaName(GetMvcAreaName(route));
        }

        public static string GetModuleName(RouteData routeData) {
            return GetModuleNameFromMvcAreaName(GetMvcAreaName(routeData));
        }

        public static string FormatMvcAreaName(string moduleName) {
            return FormatMvcAreaName(null, moduleName);
        }

        public static string FormatMvcAreaName(string areaName, string moduleName) {
            string result = null;
            bool usingArea = !string.IsNullOrEmpty(areaName);
            bool usingModule = !string.IsNullOrEmpty(moduleName);

            if(usingArea && usingModule) {
                result = string.Format(MODULE_AREA_FORMAT, areaName, moduleName);
            } else if(usingArea && !usingModule) {
                result = areaName;
            } else if(!usingArea && usingModule) {
                result = string.Format(MODULE_FORMAT, moduleName);
            } else {
                result = null;
            }

            return result;
        }

        public static RouteValueDictionary GetRouteViewDictionary(string areaName, string moduleName) {
            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues["area"] = AreaHelpers.FormatMvcAreaName(areaName, moduleName);
            return routeValues;
        }

        public static RouteValueDictionary GetRouteViewDictionary(string areaName, string moduleName, object arguments) {
            RouteValueDictionary routeValues = arguments != null ? new RouteValueDictionary(arguments) : new RouteValueDictionary();
            routeValues["area"] = AreaHelpers.FormatMvcAreaName(areaName, moduleName);
            return routeValues;
        }

        public static RouteValueDictionary GetRouteViewDictionary(string areaName, string moduleName, IDictionary<string, object> dictionary) {
            RouteValueDictionary routeValues = dictionary != null ? new RouteValueDictionary(dictionary) : new RouteValueDictionary();
            routeValues["area"] = AreaHelpers.FormatMvcAreaName(areaName, moduleName);
            return routeValues;
        }

        public static RouteValueDictionary GetRouteViewDictionary(string areaName, string moduleName, RouteValueDictionary routeValues) {
            routeValues["area"] = AreaHelpers.FormatMvcAreaName(areaName, moduleName);
            return routeValues;
        }
    }
}
