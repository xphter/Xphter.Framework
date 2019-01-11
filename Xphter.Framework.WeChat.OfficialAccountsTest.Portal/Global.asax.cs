using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Xphter.Framework.WeChat.OfficialAccounts;

namespace Xphter.Framework.WeChat.OfficialAccountsTest.Portal {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication {
        private const string SERVICE_CACHE_KEY = "IOfficialAccountService_CacheKey";

        public static IOfficialAccountService OfficialAccountService {
            get {
                return (IOfficialAccountService) HttpRuntime.Cache[SERVICE_CACHE_KEY];
            }
        }

        private void InitializeWeChatService() {
            DefaultWeChatMessageFactory messageFactory = new DefaultWeChatMessageFactory();

            DefaultWeChatMessageHandlerFactory handlerFactory = new DefaultWeChatMessageHandlerFactory();
            handlerFactory.RegisterHandlers(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray());

            IOfficialAccountService service = new OfficialAccountPlatform(
                "sIk2kkJ5Uop0wpn3vb8bbxre",
                "gh_4dc650e1f33e",
                "wx3bb5c391e663740c",
                "2fe7aa96e28b58540d528d01162cfe63",
                messageFactory,
                handlerFactory);
            //IOfficialAccountService service = new OfficialAccountPlatform(
            //    "sIk2kkJ5Uop0wpn3vb8bbxre",
            //    "gh_4dc650e1f33e",
            //    "wx8426e58b42376f1c",
            //    "66558422944689bc531bc9093d47b714",
            //    messageFactory,
            //    handlerFactory);
            HttpRuntime.Cache[SERVICE_CACHE_KEY] = service;
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            this.InitializeWeChatService();
        }
    }
}