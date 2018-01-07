using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

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

            model.Url = Request.GetDisplayUrl();
            model.HttpMethod = Request.Method;
            model.ContentType = Request.ContentType;

            var requestInput = Request.Body;
            using (var sr = new StreamReader(requestInput))
                model.Content = sr.ReadToEnd();

            return View(model);
        }
    }
}