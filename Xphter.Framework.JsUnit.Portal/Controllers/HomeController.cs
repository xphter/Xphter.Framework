using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Xphter.Framework.JsUnit.Portal.Controllers {
    public class HomeController : Controller {
        private const string TEST_FOLDER = "Test";

        public ActionResult Index() {
            IEnumerable<string> files = Enumerable.Empty<string>();
            string folder = this.Server.MapPath(string.Format("~/{0}", TEST_FOLDER));
            if(Directory.Exists(folder)) {
                files = Directory.GetFiles(folder, "*.html", SearchOption.TopDirectoryOnly).Select((item) => this.Url.Content(string.Format("~/{0}/{1}", TEST_FOLDER, Path.GetFileName(item)))).OrderBy((item) => item);
            }

            return View(files);
        }
    }
}
