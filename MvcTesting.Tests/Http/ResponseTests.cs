using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Http
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public void ActionResult_WhenLastResultNull_Throws()
        {
            var response = new Response { LastResult = null };

            var e = Assert.Throws<Exception>(() => response.ActionResult());

            e.Message.Should().Be("Expected ActionResult, but got no result from global filter CaptureResultFilter");
        }

        [Test]
        public void ActionResultOf_WhenIncorrectType_Throws()
        {
            var response = new Response { LastResult = new FileContentResult(new byte[0], "text/plain") };

            var e = Assert.Throws<Exception>(() => response.ActionResultOf<ContentResult>());

            e.Message.Should().Be("Expected ContentResult, but got FileContentResult");
        }
    }
}
