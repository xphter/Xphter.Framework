using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Xphter.Framework.Alibaba.ChinaAlibaba;

namespace Xphter.Framework.Alibaba.ChinaAlibabaTest.Portal {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication {
        public const string APP_KEY = "2988940";
        public const string APP_SECRET = "3tzdn2c9CU";

        private static IChinaAlibabaService g_chinaAlibabaService;
        public static IChinaAlibabaService ChinaAlibabaService {
            get {
                return g_chinaAlibabaService;
            }
        }

        private void InitializeOpen1688Service() {
            string refreshToken = null;
            DateTime? refreshTokenExpiredTime = null;
            string filePath = this.Server.MapPath("~/token.txt");
            string[] lines = File.Exists(filePath) ? File.ReadAllLines(filePath, Encoding.UTF8) : null;
            if(lines != null) {
                refreshToken = lines[0];
                refreshTokenExpiredTime = DateTime.Parse(lines[1]);
            }

            g_chinaAlibabaService = new ChinaAlibabaService(
                APP_KEY,
                APP_SECRET,
                refreshToken,
                refreshTokenExpiredTime,
                new DefaultChinaAlibabaApiFactory());
            g_chinaAlibabaService.AuthorizationChanged += (sender, args) => {
                File.WriteAllText(filePath, string.Format("{0}\r\n{1}", args.RefreshToken, args.RefreshTokenExpiredTime), Encoding.UTF8);
            };
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            this.InitializeOpen1688Service();
        }

        protected void Application_End() {
            g_chinaAlibabaService.Dispose();
        }
    }
}