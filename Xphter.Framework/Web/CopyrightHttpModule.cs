using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Xphter.Framework.Web {
    public class CopyrightHttpModule : IHttpModule {
        public static void Register() {
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(CopyrightHttpModule));
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e) {
            HttpApplication application = (HttpApplication) sender;
            HttpResponse response = application.Context.Response;
            if(HttpRuntime.UsingIntegratedPipeline) {
                response.Headers.Add("Author", "Du Peng");
                response.Headers.Add("Copyright", "xphter.com");
                response.Headers.Add("Powered-By", "XphteR Framework");
            }
        }

        #region IHttpModule Members

        public void Dispose() {
        }

        public void Init(HttpApplication context) {
            context.PreRequestHandlerExecute += OnPreSendRequestHeaders;
        }

        

        #endregion
    }
}
