using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Xphter.Framework.Web.Mvc.Html {
    /// <summary>
    /// Existing for MVC3.
    /// </summary>
    public static class HtmlExtensions {
        public static MvcHtmlString ClientIdFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) {
            return MvcHtmlString.Create(helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
        }

        public static MvcHtmlString ClientNameFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) {
            return MvcHtmlString.Create(helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
        }
    }
}
