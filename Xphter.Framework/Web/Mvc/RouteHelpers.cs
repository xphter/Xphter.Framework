using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Xphter.Framework.Web.Mvc {
    internal static class RouteHelpers {
        public static IEnumerable<string> GetMvcNamespaces(RouteBase route) {
            Route castRoute = route as Route;
            if(castRoute != null && castRoute.DataTokens != null) {
                return castRoute.DataTokens["Namespaces"] as IEnumerable<string>;
            }

            return null;
        }

        public static RouteValueDictionary ToRouteViewDictionary(object arguments) {
            return arguments != null ? new RouteValueDictionary(arguments) : null;
        }
    }
}
