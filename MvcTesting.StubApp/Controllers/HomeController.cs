using System.IO;
using System.Web.Mvc;

namespace MvcTesting.StubApp.Controllers
{
    public class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        public ActionResult RawForm()
        {
            var model = new RawFormModel();

            model.Url = Request.RawUrl;
            model.HttpMethod = Request.HttpMethod;
            model.ContentType = Request.ContentType;

            var requestInput = Request.InputStream;
            requestInput.Position = 0;
            using (var sr = new StreamReader(requestInput))
                model.Content = sr.ReadToEnd();

            return View(model);
        }
    }
}