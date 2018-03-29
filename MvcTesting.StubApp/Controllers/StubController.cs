using System;
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
            if (model.Text == "success")
                return Redirect("~/Stub/Success");
            else
                return View(model.SetError("Please enter 'success'"));
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        public IActionResult ViewRequest()
        {
            var model = new ViewRequestModel
            {
                Method = Request.Method,
            };

            return View(model);
        }

        public IActionResult SetCookies()
        {
            Response.Cookies.Append("a", "2");
            Response.Cookies.Append("b", "3");
            return Content("done");
        }

        public IActionResult GetCookies()
        {
            var text = "";

            foreach (var cookie in Request.Cookies)
                text += $"{cookie.Key}={cookie.Value};";

            text = text == "" ? "none" : text.TrimEnd(';');
            return Content(text);
        }

        public IActionResult Throw()
        {
            throw new Exception("thrown from stub");
        }
    }
}
