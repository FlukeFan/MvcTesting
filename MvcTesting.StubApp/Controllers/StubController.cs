using Microsoft.AspNetCore.Mvc;

namespace MvcTesting.StubApp.Controllers
{
    public class StubController : Controller
    {
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
