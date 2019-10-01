using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MvcTesting.StubApp.Controllers
{
    public class HomeController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RawForm()
        {
            var model = new RawFormModel();

            model.Url = Request.GetDisplayUrl();
            model.HttpMethod = Request.Method;
            model.ContentType = Request.ContentType;

            var requestInput = Request.Body;
            using (var sr = new StreamReader(requestInput))
                model.Content = await sr.ReadToEndAsync();

            return View(model);
        }
    }
}