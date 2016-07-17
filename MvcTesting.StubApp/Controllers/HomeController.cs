using System.Web.Mvc;

namespace MvcTesting.StubApp.Controllers
{
    public class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}