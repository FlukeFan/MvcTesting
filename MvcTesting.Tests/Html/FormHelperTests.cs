﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using MvcTesting.Html;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Html
{
    public static class ResponseTestHelpers
    {
        public static async Task<Response> ToAsync(this Response response)
        {
            return await Task.FromResult(response);
        }
    }

    [TestFixture]
    public class FormHelperTests
    {
        [Test]
        public async Task Scrape_ScrapeFormByIndex()
        {
            var html = @"
                <form><input type='text' name='Name' value='form0' /></form>
                <form><input type='text' name='Name' value='form1' /></form>
            ";

            var form = await new Response { Text = html }
                .ToAsync()
                .Form<FormModel>(1);

            form.GetText(m => m.Name).Should().Be("form1");
        }

        [Test]
        public async Task Scrape_SingleForm()
        {
            var html = @"
                <form><input type='text' name='Name' value='form0' /></form>
            ";

            var form = await new Response { Text = html }
                .ToAsync()
                .Form<FormModel>();

            form.GetText(m => m.Name).Should().Be("form0");
        }

        [Test]
        public void Scrape_MultipleFormsWithoutIndex_Throws()
        {
            var html = @"
                <form id='1'><input type='text' name='Name' value='form0' /></form>
                <form id='2'><input type='text' name='Name' value='form1' /></form>
            ";

            var response = new Response { Text = html }.ToAsync();
            var e = Assert.ThrowsAsync<Exception>(() => response.Form<FormModel>());

            e.Message.Should().Be("Multiple form elements found in document: <form id=\"1\">\n<form id=\"2\">");
        }

        [Test]
        public void Scrape_NoFormsFound_Throws()
        {
            var html = @"
                <input type='text' name='Name' value='form0' />
                <input type='text' name='Name' value='form1' />
            ";

            var response = new Response { Text = html }.ToAsync();
            var e = Assert.ThrowsAsync<Exception>(() => response.Form<FormModel>());

            e.Message.Should().Be("CSS selector 'form' did not match any elements in the document");
        }

        [Test]
        public void Scrape_IndexTooLarge_Throws()
        {
            var html = @"
                <form index='0'><input type='text' name='Name' value='form0' /></form>
            ";

            var response = new Response { Text = html }.ToAsync();
            var e = Assert.ThrowsAsync<Exception>(() => response.Form<FormModel>(1));

            e.Message.Should().Be("Index '1' is too large for collection with '1' forms: <form index=\"0\">");
        }

        [Test]
        public async Task Scrape_SpecifySelector()
        {
            var html = @"
                <form id='1'><input type='text' name='Name' value='form0' /></form>
                <form id='2'><input type='text' name='Name' value='form1' /></form>
            ";

            var form = await new Response { Text = html }
                .ToAsync()
                .Form<FormModel>("form[id=2]");

            form.GetText(m => m.Name).Should().Be("form1");
        }
    }
}
