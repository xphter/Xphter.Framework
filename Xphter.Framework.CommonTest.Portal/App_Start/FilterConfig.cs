﻿using System.Web;
using System.Web.Mvc;

namespace Xphter.Framework.CommonTest.Portal {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}