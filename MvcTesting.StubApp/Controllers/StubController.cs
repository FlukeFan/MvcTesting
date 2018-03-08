using Microsoft.AspNetCore.Mvc;
using MvcTesting.StubApp.Views.Stub;

namespace MvcTesting.StubApp.Controllers
{
    public class StubController : Controller
    {
        [HttpGet]
        public IActionResult SimpleForm()
        {
            var model = new SimpleFormModel
            {
                Id = 123,
                Text = "existing",
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SimpleForm(SimpleFormModel model)
        {
            return Content("Not implemented yet");
        }

        public IActionResult ViewRequest()
        {
            var model = new ViewRequestModel
            {
                Method = Request.Method,
            };

            return View(model);
        }
    }
}
