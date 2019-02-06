using Microsoft.AspNetCore.Http;

namespace MvcTesting.StubApp.Controllers
{
    public class ViewRequestModel
    {
        public string           Method;
        public IQueryCollection Query;
    }
}
