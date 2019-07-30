using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcTesting.StubApp.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public virtual IActionResult Index(IFormFile file)
        {
            if (file == null)
                return Content("null file");

            if (file.Length == 0)
                return Content("0 file");

            using (var fileStream = System.IO.File.OpenWrite(file.FileName))
            {
                file.CopyTo(fileStream);
            }

            return Redirect("/");
        }
    }
}
