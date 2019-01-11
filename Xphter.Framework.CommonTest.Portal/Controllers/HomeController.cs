using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Xphter.Framework.CommonTest.Portal.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return this.View();
        }

        public ActionResult CreateFile() {
            string file = this.Server.MapPath("~/Cache/test.txt");
            try {
                System.IO.File.WriteAllText(file, "test file", Encoding.UTF8);
                return this.Content("SUCCESS");
            } catch(Exception ex) {
                return this.Content("Error: ", ex.Message);
            }
        }

        public ActionResult DeleteFile() {
            string file = this.Server.MapPath("~/Cache/test.txt");
            try {
                System.IO.File.Delete(file);
                return this.Content("SUCCESS");
            } catch(Exception ex) {
                return this.Content("Error: ", ex.Message);
            }
        }

        public ActionResult CreateCache() {
            string key = "testfile";
            string file = this.Server.MapPath("~/Data/test.txt");
            try {
                System.IO.File.WriteAllText(file, "test file", Encoding.UTF8);
                this.HttpContext.Cache.Insert(key, file, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, (k, v, r) => {
                    string path = (string) v;
                    System.IO.File.Delete(path);
                });
                return this.Content("SUCCESS");
            } catch(Exception ex) {
                return this.Content("Error: ", ex.Message);
            }
        }

        public ActionResult DeleteCache() {
            string key = "testfile";
            try {
                this.HttpContext.Cache.Remove(key);
                return this.Content("SUCCESS");
            } catch(Exception ex) {
                return this.Content("Error: ", ex.Message);
            }
        }
    }
}
