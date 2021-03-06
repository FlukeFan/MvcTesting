﻿using System.Linq;
using System.Net;
using FluentAssertions;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Http
{
    [TestFixture]
    public class RequestTests
    {
        [Test]
        public void Construct()
        {
            var request = new Request("/test/path");

            request.Url.Should().Be("/test/path");
            request.Query().Should().BeNull();
            request.Verb.Should().Be("GET");
            request.ExptectedResponse.Should().Be(HttpStatusCode.OK);
            request.Headers.Count.Should().Be(0);
            request.FormValues.Should().BeNull();
        }

        [Test]
        public void Construct_WithQuery()
        {
            var request = new Request("/test/path?p1=123&p2%26=234%2b");

            request.Query().Should().Be("p1=123&p2%26=234%2b");
        }

        [Test]
        public void Construct_WithEmptyQuery()
        {
            var request = new Request("/test/path?");

            request.Query().Should().Be("");
        }

        [Test]
        public void Construct_EmptyExpectedResponse()
        {
            var request = new Request("/test", "PUSH");

            request.ExptectedResponse.Should().BeNull();
        }

        [Test]
        public void AddFormValue()
        {
            var request = new Request("/test", "POST");

            request.AddFormValue("p1", "123");
            request.AddFormValue("p2", "234");

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("p1", "123"),
                new NameValue("p2", "234"),
            });
        }

        [Test]
        public void AddFormValue_Get()
        {
            var request = new Request("/test/path?a=b");

            request.AddFormValue("p1", "123");

            request.Query().Should().Be("p1=123");

            request.AddFormValue("p2&", "234+");

            request.Query().Should().Be("p1=123&p2%26=234%2b");
        }

        [Test]
        public void EmptyFormValues()
        {
            var request = new Request("/test/path?a=b");

            request.StartForm();

            request.FormValues.Count().Should().Be(0);
            request.Query().Should().Be("");
        }
    }
}
